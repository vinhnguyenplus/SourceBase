// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using Microsoft.AspNetCore.Hosting;
using NewLife.Reflection;

namespace Admin.NET.Core.Service;

/// <summary>
/// Alipay payment service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 240)]
public class SysAlipayService : IDynamicApiController, ITransient
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SysConfigService _sysConfigService;
    private readonly List<IAopClient> _alipayClientList;
    private readonly IHttpContextAccessor _httpContext;
    private readonly AlipayOptions _option;
    private readonly ISqlSugarClient _db;

    public SysAlipayService(
        ISqlSugarClient db,
        IHttpContextAccessor httpContext,
        SysConfigService sysConfigService,
        IWebHostEnvironment webHostEnvironment,
        IOptions<AlipayOptions> alipayOptions)
    {
        _db = db;
        _httpContext = httpContext;
        _sysConfigService = sysConfigService;
        _option = alipayOptions.Value;
        _webHostEnvironment = webHostEnvironment;

        // Initialize Alipay client list
        _alipayClientList = [];
        foreach (var account in _option.AccountList) _alipayClientList.Add(_option.GetClient(account));
    }

    /// <summary>
    /// Get authorization information 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [NonUnify]
    [AllowAnonymous]
    [DisplayName("ObtainAuthorizationInformation")]
    [ApiDescriptionSettings(Name = "AuthInfo"), HttpGet]
    public ActionResult GetAuthInfo([FromQuery] AlipayAuthInfoInput input)
    {
        var type = input.UserId?.Split('-').FirstOrDefault().ToInt();
        var userId = input.UserId?.Split('-').LastOrDefault().ToLong();
        var account = _option.AccountList.FirstOrDefault();
        var alipayClient = _alipayClientList.First();

        // Current web interface address
        var currentUrl = $"{_option.AppAuthUrl}{_httpContext.HttpContext!.Request.Path}?userId={input.UserId}";
        if (string.IsNullOrEmpty(input.AuthCode))
        {
            // Reauthorize
            var url = $"{_option.AuthUrl}?app_id={account!.AppId}&scope=auth_user&redirect_uri={currentUrl}";
            return new RedirectResult(url);
        }

        // Assemble authorization request parameters
        AlipaySystemOauthTokenRequest request = new()
        {
            GrantType = AlipayConst.GrantType,
            Code = input.AuthCode
        };
        AlipaySystemOauthTokenResponse response = alipayClient.CertificateExecute(request);

        // Token exchange for user information
        AlipayUserInfoShareRequest infoShareRequest = new();
        AlipayUserInfoShareResponse info = alipayClient.CertificateExecute(infoShareRequest, response.AccessToken);

        // Record authorization information
        var entity = _db.Queryable<SysAlipayAuthInfo>().First(u =>
            (!string.IsNullOrWhiteSpace(u.UserId) && u.UserId == info.UserId) ||
            (!string.IsNullOrWhiteSpace(u.OpenId) && u.OpenId == info.OpenId)) ?? new();
        entity.Copy(info, excludes: [nameof(SysAlipayAuthInfo.Gender), nameof(SysAlipayAuthInfo.Age)]);
        entity.Age = int.Parse(info.Age);
        entity.Gender = info.Gender switch
        {
            "m" => GenderEnum.Male,
            "f" => GenderEnum.Female,
            _ => GenderEnum.Unknown
        };
        entity.AppId = account!.AppId;
        if (entity.Id <= 0) _db.Insertable(entity).ExecuteCommand();
        else _db.Updateable(entity).ExecuteCommand();

        // After execution, redirect to the specified interface
        //var authPageUrl = _sysConfigService.GetConfigValueByCode<string>(ConfigConst.AlipayAuthPageUrl + type).Result;
        //return new RedirectResult(authPageUrl);
        return new RedirectResult(_option.AppAuthUrl + "/index.html");
    }

    /// <summary>
    /// Payment callback 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Payment Callback")]
    [ApiDescriptionSettings(Name = "Notify"), HttpPost]
    public string Notify()
    {
        SortedDictionary<string, string> sorted = [];
        foreach (string key in _httpContext.HttpContext!.Request.Form.Keys)
            sorted.Add(key, _httpContext.HttpContext.Request.Form[key]);

        var account = _option.AccountList.FirstOrDefault();
        string alipayPublicKey = Path.Combine(_webHostEnvironment.ContentRootPath, account!.AlipayPublicCertPath!.Replace('/', '\\').TrimStart('\\'));
        bool signVerified = AlipaySignature.RSACertCheckV1(sorted, alipayPublicKey, "UTF-8", account.SignType); // Call SDK to verify signature
        if (!signVerified) throw Oops.Oh("TransactionFailure");

        // Update transaction history
        var outTradeNo = sorted.GetValueOrDefault("out_trade_no");
        var transaction = _db.Queryable<SysAlipayTransaction>().First(x => x.OutTradeNo == outTradeNo) ?? throw Oops.Oh("Transaction record does not exist");
        transaction.TradeNo = sorted.GetValueOrDefault("trade_no");
        transaction.TradeStatus = sorted.GetValueOrDefault("trade_status");
        transaction.FinishTime = sorted.ContainsKey("gmt_payment") ? DateTime.Parse(sorted.GetValueOrDefault("gmt_payment")) : null;
        transaction.BuyerLogonId = sorted.GetValueOrDefault("buyer_logon_id");
        transaction.BuyerUserId = sorted.GetValueOrDefault("buyer_user_id");
        transaction.SellerUserId = sorted.GetValueOrDefault("seller_id");
        transaction.Remark = sorted.GetValueOrDefault("remark");
        _db.Updateable(transaction).ExecuteCommand();

        return "success";
    }

    /// <summary>
    ///  Unified order collection and payment page interface 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Unified order placement and payment page interface")]
    [ApiDescriptionSettings(Name = "AlipayTradePagePay"), HttpPost]
    public string AlipayTradePagePay(AlipayTradePagePayInput input)
    {
        // Create transaction record, status is awaiting payment
        var transactionRecord = new SysAlipayTransaction
        {
            AppId = _option.AccountList.First().AppId,
            OutTradeNo = input.OutTradeNo,
            TotalAmount = input.TotalAmount.ToDecimal(),
            TradeStatus = "WAIT_PAY", // Waiting for payment
            CreateTime = DateTime.Now,
            Subject = input.Subject,
            Body = input.Body,
            Remark = "Waiting for user payment"
        };
        _db.Insertable(transactionRecord).ExecuteCommand();

        // Set the payment page request, assemble the business parameter model, and set the asynchronous notification receiving address
        AlipayTradeWapPayRequest request = new();
        request.SetBizModel(new AlipayTradeWapPayModel()
        {
            Subject = input.Subject,
            OutTradeNo = input.OutTradeNo,
            TotalAmount = input.TotalAmount,
            Body = input.Body,
            ProductCode = "QUICK_WAP_WAY",
            TimeExpire = input.TimeoutExpress
        });
        request.SetNotifyUrl(_option.NotifyUrl);

        var alipayClient = _alipayClientList.First();
        var response = alipayClient.SdkExecute(request);
        if (response.IsError) throw Oops.Oh(response.SubMsg);
        return $"{_option.ServerUrl}?{response.Body}";
    }

    /// <summary>
    ///  Transaction pre-creation 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Transaction pre-creation")]
    [ApiDescriptionSettings(Name = "AlipayPreCreate"), HttpPost]
    public string AlipayPreCreate(AlipayPreCreateInput input)
    {
        // Create transaction record, status is awaiting payment
        var transactionRecord = new SysAlipayTransaction
        {
            AppId = _option.AccountList.First().AppId,
            OutTradeNo = input.OutTradeNo,
            TotalAmount = input.TotalAmount.ToDecimal(),
            TradeStatus = "WAIT_PAY", // Waiting for payment
            CreateTime = DateTime.Now,
            Subject = input.Subject,
            Remark = "Waiting for user payment"
        };
        _db.Insertable(transactionRecord).ExecuteCommand();

        // Set the asynchronous notification receiving address and assemble the business parameter model
        AlipayTradePrecreateRequest request = new();
        request.SetNotifyUrl(_option.NotifyUrl);
        request.SetBizModel(new AlipayTradePrecreateModel()
        {
            Subject = input.Subject,
            OutTradeNo = input.OutTradeNo,
            TotalAmount = input.TotalAmount,
            TimeoutExpress = input.TimeoutExpress
        });

        var alipayClient = _alipayClientList.First();
        var response = alipayClient.CertificateExecute(request);
        if (response.IsError) throw Oops.Oh(response.SubMsg);
        return response.QrCode;
    }

    /// <summary>
    /// Single transfer to Alipay account
    ///  https://opendocs.alipay.com/open/62987723_alipay.fund.trans.uni.transfer
    /// </summary>
    [NonAction]
    public async Task<AlipayFundTransUniTransferResponse> Transfer(AlipayFundTransUniTransferInput input)
    {
        var account = _option.AccountList.FirstOrDefault(u => u.AppId == input.AppId) ?? throw Oops.Oh("Not yetFind MerchantAlipayAccount number");
        var alipayClient = _option.GetClient(account);

        // Construct request parameters to call the interface
        AlipayFundTransUniTransferRequest request = new();
        AlipayFundTransUniTransferModel model = new()
        {
            BizScene = AlipayConst.BizScene,
            ProductCode = AlipayConst.ProductCode,
            OutBizNo = input.OutBizNo, // Merchant order
            TransAmount = $"{input.TransAmount}:F2", // Total order amount
            OrderTitle = input.OrderTitle, // business title
            Remark = input.Remark, // Business notes
            PayeeInfo = new() // Payee information
            {
                CertType = input.CertType?.ToString(),
                CertNo = input.CertNo,
                Identity = input.Identity,
                Name = input.Name,
                IdentityType = input.IdentityType.ToString()
            },
            BusinessParams = input.PayerShowNameUseAlias ? "{\"payer_show_name_use_alias\":\"true\"}" : null
        };

        request.SetBizModel(model);
        var response = alipayClient.CertificateExecute(request);

        // Save transfer records
        await _db.Insertable(new SysAlipayTransaction
        {
            UserId = input.UserId,
            AppId = input.AppId,
            TradeNo = response.OrderId,
            OutTradeNo = input.OutBizNo,
            TotalAmount = response.Amount.ToDecimal(),
            TradeStatus = response.Code == "10000" ? "SUCCESS" : "FAILED",
            Subject = input.OrderTitle,
            ErrorInfo = response.SubMsg,
            Remark = input.Remark
        }).ExecuteCommandAsync();

        return response;
    }
}