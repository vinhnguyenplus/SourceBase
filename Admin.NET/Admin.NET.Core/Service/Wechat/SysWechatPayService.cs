// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion.Logging.Extensions;
using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// WeChat payment service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 210)]
public class SysWechatPayService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatPay> _sysWechatPayRep;
    private readonly SqlSugarRepository<SysWechatRefund> _sysWechatRefundRep;

    private readonly WechatPayOptions _wechatPayOptions;
    private readonly PayCallBackOptions _payCallBackOptions;

    private readonly WechatTenpayClient _wechatTenpayClient;

    public SysWechatPayService(SqlSugarRepository<SysWechatPay> sysWechatPayUserRep,
        SqlSugarRepository<SysWechatRefund> sysWechatRefundRep,
        IOptions<WechatPayOptions> wechatPayOptions,
        IOptions<PayCallBackOptions> payCallBackOptions)
    {
        _sysWechatPayRep = sysWechatPayUserRep;
        _sysWechatRefundRep = sysWechatRefundRep;
        _wechatPayOptions = wechatPayOptions.Value;
        _payCallBackOptions = payCallBackOptions.Value;

        _wechatTenpayClient = CreateTenpayClient();
    }

    /// <summary>
    /// Initialize WeChat payment client
    /// </summary>
    /// <returns></returns>
    private WechatTenpayClient CreateTenpayClient()
    {
        var cerFilePath = Path.Combine(App.WebHostEnvironment.ContentRootPath, _wechatPayOptions.MerchantCertificatePrivateKey.Replace("/", Path.DirectorySeparatorChar.ToString()));

        var tenpayClientOptions = new WechatTenpayClientOptions()
        {
            MerchantId = _wechatPayOptions.MerchantId,
            MerchantV3Secret = _wechatPayOptions.MerchantV3Secret,
            MerchantCertificateSerialNumber = _wechatPayOptions.MerchantCertificateSerialNumber,
            MerchantCertificatePrivateKey = File.Exists(cerFilePath) ? File.ReadAllText(cerFilePath) : "",
            PlatformCertificateManager = new InMemoryCertificateManager()
        };
        return new WechatTenpayClient(tenpayClientOptions);
    }

    /// <summary>
    /// Query payment list by page 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<SysWechatPay>> Page(WechatPayPageInput input)
    {
        var query = _sysWechatPayRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.OutTradeNumber == input.Keyword || u.TransactionId == input.Keyword)
            .WhereIF(input.CreateTimeRange != null && input.CreateTimeRange.Count > 0 && input.CreateTimeRange[0].HasValue, x => x.CreateTime >= input.CreateTimeRange[0])
            .WhereIF(input.CreateTimeRange != null && input.CreateTimeRange.Count > 1 && input.CreateTimeRange[1].HasValue, x => x.CreateTime < ((DateTime)input.CreateTimeRange[1]).AddDays(1));
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Query refund information list
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [DisplayName("Get refund information list based on payment ID")]
    public async Task<List<SysWechatRefund>> ListRefund([FromBody] string id)
    {
        var query = _sysWechatRefundRep.AsQueryable()
            .Where(u => u.TransactionId == id);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Generate parameters required for JSAPI to call payment 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Generate JSAPI parameters required to initiate payment")]
    public WechatPayParaOutput GenerateParametersForJsapiPay(WechatPayParaInput input)
    {
        var data = _wechatTenpayClient.GenerateParametersForJsapiPayRequest(_wechatPayOptions.AppId, input.PrepayId);
        return new WechatPayParaOutput()
        {
            AppId = data["appId"],
            TimeStamp = data["timeStamp"],
            NonceStr = data["nonceStr"],
            Package = data["package"],
            SignType = data["signType"],
            PaySign = data["paySign"]
        };
    }

    /// <summary>
    /// Order via WeChat payment (direct connection to merchant) 🔖
    /// </summary>
    [DisplayName("Order with WeChat payment (direct connection to merchant)")]
    public async Task<WechatPayTransactionOutput> CreatePayTransaction([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayTransactionJsapiRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // Order number
            AppId = _wechatPayOptions.AppId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayTransactionJsapiRequest.Types.Amount() { Total = input.Total },
            Payer = new CreatePayTransactionJsapiRequest.Types.Payer() { OpenId = input.OpenId }
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayTransactionJsapiAsync(request);
        if (!response.IsSuccessful())
            throw Oops.Oh(response.ErrorMessage);

        var singInfo = this.GenerateParametersForJsapiPay(new WechatPayParaInput() { PrepayId = response.PrepayId });
        // Save order information
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            OpenId = input.OpenId,
            TransactionId = "",
            Tags = input.Tags,
            BusinessId = input.BusinessId,
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);

        return new WechatPayTransactionOutput()
        {
            PrepayId = response.PrepayId,
            OutTradeNumber = request.OutTradeNumber,
            SingInfo = singInfo
        };
    }

    /// <summary>
    /// WeChat Pay Order (Direct Merchant Connection) Native
    /// </summary>
    [DisplayName("WeChat Pay Order (Merchant Direct Connection) Native")]
    public async Task<dynamic> CreatePayTransactionNative([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayTransactionNativeRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // Order number
            AppId = _wechatPayOptions.AppId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayTransactionNativeRequest.Types.Amount() { Total = input.Total },
            //Payer = new CreatePayTransactionNativeRequest.Types.Payer() { OpenId = input.OpenId }
            Scene = new CreatePayTransactionNativeRequest.Types.Scene() { ClientIp = "127.0.0.1" }
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayTransactionNativeAsync(request);
        if (!response.IsSuccessful())
        {
            JsonConvert.SerializeObject(response).LogInformation();
            throw Oops.Oh(response.ErrorMessage);
        }
        // Save order information
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            //OpenId = input.OpenId,
            TransactionId = "",
            QrcodeContent = response.QrcodeUrl,
            Tags = input.Tags,
            BusinessId = input.BusinessId,
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);
        return new
        {
            request.OutTradeNumber,
            response.QrcodeUrl
        };
    }

    /// <summary>
    /// Order with WeChat payment (service provider model) 🔖
    /// </summary>
    [DisplayName("WeChat Pay Order (Service Provider Mode)")]
    public async Task<dynamic> CreatePayPartnerTransaction([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayPartnerTransactionJsapiRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // Order number
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount() { Total = input.Total },
            Payer = new CreatePayPartnerTransactionJsapiRequest.Types.Payer() { OpenId = input.OpenId }
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayPartnerTransactionJsapiAsync(request);
        if (!response.IsSuccessful())
            throw Oops.Oh(response.ErrorMessage);
        var singInfo = this.GenerateParametersForJsapiPay(new WechatPayParaInput() { PrepayId = response.PrepayId });
        // Save order information
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            OpenId = input.OpenId,
            TransactionId = ""
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);

        return new
        {
            response.PrepayId,
            request.OutTradeNumber,
            singInfo
        };
    }

    /// <summary>
    /// Get payment order details (local database) 🔖
    /// </summary>
    /// <param name="tradeId"></param>
    /// <returns></returns>
    [DisplayName("Get payment order details (local database)")]
    public async Task<SysWechatPay> GetPayInfo(string tradeId)
    {
        return await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == tradeId);
    }

    /// <summary>
    /// Get payment order details (WeChat interface) 🔖
    /// </summary>
    /// <param name="tradeId"></param>
    /// <returns></returns>
    [DisplayName("ObtainPay OrderDetails(WeChatInterface)")]
    public async Task<SysWechatPay> GetPayInfoFromWechat(string tradeId)
    {
        var request = new GetPayTransactionByOutTradeNumberRequest();
        request.OutTradeNumber = tradeId;
        var response = await _wechatTenpayClient.ExecuteGetPayTransactionByOutTradeNumberAsync(request);
        // Modify order payment status
        var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == response.OutTradeNumber
            && u.MerchantId == response.MerchantId);
        // If the status is inconsistent, update the record in the database
        if (wechatPay != null && wechatPay.TradeState != response.TradeState)
        {
            wechatPay.OpenId = response.Payer.OpenId;
            wechatPay.TransactionId = response.TransactionId; // Payment order number
            wechatPay.TradeType = response.TradeType; // transaction type
            wechatPay.TradeState = response.TradeState; // transaction status
            wechatPay.TradeStateDescription = response.TradeStateDescription; // Transaction status description
            wechatPay.BankType = response.BankType; // Payment bank type
            wechatPay.Total = response.Amount.Total; // Total order amount
            wechatPay.PayerTotal = response.Amount.PayerTotal; // User payment amount
            wechatPay.SuccessTime = response.SuccessTime.Value.DateTime; // Payment completion time
            await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();
        }
        wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Attachment = response.Attachment,
            Total = response.Amount.Total, // Total order amount
            TransactionId = response.TransactionId,
            TradeType = response.TradeType, // transaction type
            TradeState = response.TradeState, // transaction status
            TradeStateDescription = response.TradeStateDescription, // Transaction status description
            BankType = response.BankType, // Payment bank type
            PayerTotal = response.Amount.PayerTotal, // User payment amount
            SuccessTime = response.SuccessTime.Value.DateTime // Payment completion time
        };
        return wechatPay;
    }

    /// <summary>
    /// Refund request
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Refund request")]
    [HttpPost]
    public async Task<dynamic> CreateRefundDomestic([FromBody] WechatPayRefundDomesticInput input)
    {
        // refund/domestic/refunds
        var request = new CreateRefundDomesticRefundRequest()
        {
            Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
            {
                Refund = input.Refund,
                Total = input.Total,
                Currency = "CNY"
            },

            OutTradeNumber = input.TradeId,
            OutRefundNumber = "R" + DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // Order number
            NotifyUrl = _payCallBackOptions.WechatRefundUrl, // The WechatRefundUrl parameter should be used. If it is the same as the WechatPayUrl entrance, the parameters should also be set separately.
            Reason = input.Reason,
        };
        var response = await _wechatTenpayClient.ExecuteCreateRefundDomesticRefundAsync(request);
        if (string.IsNullOrEmpty(response.ErrorCode))
        {
            // Successfully, the refund order information should be saved here.
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == response.OutTradeNumber);
            // Save order information
            if (wechatPay != null)
            {
                var wechatRefund = new SysWechatRefund()
                {
                    WechatPayId = wechatPay.Id,
                    TransactionId = response.TransactionId,
                    Refund = input.Refund,
                    Reason = input.Reason,
                    OutRefundNumber = request.OutRefundNumber,
                    Channel = response.Channel,
                    UserReceivedAccount = response.UserReceivedAccount,
                };
                await _sysWechatRefundRep.InsertAsync(wechatRefund);
            }
        }
        else
        {
            throw Oops.Bah($"[{response.ErrorCode}]{response.ErrorMessage}");
        }
        return response;
    }

    /// <summary>
    /// Get refund order details (WeChat interface)
    /// </summary>
    /// <param name="refundId"></param>
    /// <returns></returns>
    [DisplayName("Get refund order details (WeChat API)")]
    public async Task<SysWechatRefund> GetRefundInfoFromWechat(string refundId)
    {
        var request = new GetRefundDomesticRefundByOutRefundNumberRequest();
        request.OutRefundNumber = refundId;
        var response = await _wechatTenpayClient.ExecuteGetRefundDomesticRefundByOutRefundNumberAsync(request);
        // Modify order payment status
        var wechatRefund = await _sysWechatRefundRep.GetFirstAsync(u => u.OutRefundNumber == refundId);
        // If the status is inconsistent, update the record in the database
        if (wechatRefund != null && wechatRefund.TradeState != response.Status)
        {
            wechatRefund.TransactionId = response.TransactionId; // Payment order number
            wechatRefund.TradeState = response.Status; // transaction status
            wechatRefund.SuccessTime = response.SuccessTime.Value.DateTime; // Payment completion time
            await _sysWechatRefundRep.AsUpdateable(wechatRefund).IgnoreColumns(true).ExecuteCommandAsync();
            // If there is a refund, please refresh the order status.
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.Id == wechatRefund.WechatPayId);
            if (wechatPay != null)
                await GetPayInfoFromWechat(wechatPay.OutTradeNumber);
        }
        wechatRefund = new SysWechatRefund()
        {
            TransactionId = response.TransactionId,
            Refund = response.Amount.Refund,
            OutRefundNumber = request.OutRefundNumber,
            Channel = response.Channel,
            UserReceivedAccount = response.UserReceivedAccount,
            TradeState = response.Status, // transaction status
            SuccessTime = response.SuccessTime.Value.DateTime, // Payment completion time
        };
        return wechatRefund;
    }

    /// <summary>
    /// WeChat payment successful callback (direct merchant connection)
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("WeChat Pay Successful Callback (Merchant Direct Connection)")]
    public async Task<WechatPayOutput> PayCallBack()
    {
        using var ms = new MemoryStream();
        await App.HttpContext.Request.Body.CopyToAsync(ms);
        var b = ms.ToArray();
        var callbackJson = Encoding.UTF8.GetString(b);

        var callbackModel = _wechatTenpayClient.DeserializeEvent(callbackJson);
        if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
        {
            try
            {
                var callbackPayResource = _wechatTenpayClient.DecryptEventResource<TransactionResource>(callbackModel);

                // Modify order payment status
                var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == callbackPayResource.OutTradeNumber
                    && u.MerchantId == callbackPayResource.MerchantId);
                if (wechatPay == null) return null;
                wechatPay.OpenId = callbackPayResource.Payer.OpenId; // Payer ID
                //wechatPay.MerchantId = callbackResource.MerchantId; // WeChat merchant number
                //wechatPay.OutTradeNumber = callbackResource.OutTradeNumber; // Merchant order number
                wechatPay.TransactionId = callbackPayResource.TransactionId; // Payment order number
                wechatPay.TradeType = callbackPayResource.TradeType; // transaction type
                wechatPay.TradeState = callbackPayResource.TradeState; // transaction status
                wechatPay.TradeStateDescription = callbackPayResource.TradeStateDescription; // Transaction status description
                wechatPay.BankType = callbackPayResource.BankType; // Payment bank type
                wechatPay.Total = callbackPayResource.Amount.Total; // Total order amount
                wechatPay.PayerTotal = callbackPayResource.Amount.PayerTotal; // User payment amount
                wechatPay.SuccessTime = callbackPayResource.SuccessTime.DateTime; // Payment completion time

                await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();

                return new WechatPayOutput()
                {
                    Total = wechatPay.Total,
                    Attachment = wechatPay.Attachment,
                    GoodsTag = wechatPay.GoodsTag
                };
            }
            catch (Exception ex)
            {
                "WeChat PayCallbacktimeError:".LogError(ex);
            }
        }
        else if ("REFUND.SUCCESS".Equals(callbackModel.EventType))
        {
            //Reference: https://pay.weixin.qq.com/docs/merchant/apis/jsapi-payment/refund-result-notice.html
            try
            {
                var callbackRefundResource = _wechatTenpayClient.DecryptEventResource<RefundResource>(callbackModel);
                // Modify order payment status
                var wechatRefund = await _sysWechatRefundRep.GetFirstAsync(u => u.OutRefundNumber == callbackRefundResource.OutRefundNumber);
                if (wechatRefund == null) return null;
                wechatRefund.TradeState = callbackRefundResource.RefundStatus; // transaction status
                wechatRefund.SuccessTime = callbackRefundResource.SuccessTime.Value.DateTime; // Payment completion time

                await _sysWechatRefundRep.AsUpdateable(wechatRefund).IgnoreColumns(true).ExecuteCommandAsync();
                // If there is a refund, please refresh the order status.
                await GetPayInfoFromWechat(callbackRefundResource.OutTradeNumber);
            }
            catch (Exception ex)
            {
                "Error during WeChat refund callback:".LogError(ex);
            }
        }
        else
        {
            callbackModel.EventType.LogInformation();
        }

        return null;
    }

    /// <summary>
    /// WeChat payment successful callback (service provider mode) 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("WeChat Pay Successful Callback (Service Provider Mode)")]
    public async Task PayPartnerCallBack()
    {
        using var ms = new MemoryStream();
        await App.HttpContext.Request.Body.CopyToAsync(ms);
        var b = ms.ToArray();
        var callbackJson = Encoding.UTF8.GetString(b);

        var callbackModel = _wechatTenpayClient.DeserializeEvent(callbackJson);
        if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
        {
            var callbackResource = _wechatTenpayClient.DecryptEventResource<PartnerTransactionResource>(callbackModel);

            // Modify order payment status
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == callbackResource.OutTradeNumber
                && u.MerchantId == callbackResource.MerchantId);
            if (wechatPay == null) return;
            //wechatPay.OpenId = callbackResource.Payer.OpenId; // Payer ID
            //wechatPay.MerchantId = callbackResource.MerchantId; // WeChat merchant number
            //wechatPay.OutTradeNumber = callbackResource.OutTradeNumber; // Merchant order number
            wechatPay.TransactionId = callbackResource.TransactionId; // Payment order number
            wechatPay.TradeType = callbackResource.TradeType; // transaction type
            wechatPay.TradeState = callbackResource.TradeState; // transaction status
            wechatPay.TradeStateDescription = callbackResource.TradeStateDescription; // Transaction status description
            wechatPay.BankType = callbackResource.BankType; // Payment bank type
            wechatPay.Total = callbackResource.Amount.Total; // Total order amount
            wechatPay.PayerTotal = callbackResource.Amount.PayerTotal; // User payment amount
            wechatPay.SuccessTime = callbackResource.SuccessTime.DateTime; // Payment completion time

            await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();
        }
    }
}