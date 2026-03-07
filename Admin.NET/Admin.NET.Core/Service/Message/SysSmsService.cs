// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20190711;

namespace Admin.NET.Core.Service;

/// <summary>
/// System SMS service 🧩
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(Order = 150)]
public class SysSmsService : IDynamicApiController, ITransient
{
    private readonly SMSOptions _smsOptions;
    private readonly SysCacheService _sysCacheService;

    public SysSmsService(IOptions<SMSOptions> smsOptions,
        SysCacheService sysCacheService)
    {
        _smsOptions = smsOptions.Value;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// Send SMS 📨
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="templateId">SMS template id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Send a text message")]
    public async Task SendSms([Required] string phoneNumber, string templateId = "0")
    {
        if (_smsOptions.Custom != null && _smsOptions.Custom.Enabled && !string.IsNullOrWhiteSpace(_smsOptions.Custom.ApiUrl))
        {
            await CustomSendSms(phoneNumber, templateId);
        }
        else if (!string.IsNullOrWhiteSpace(_smsOptions.Aliyun.AccessKeyId) && !string.IsNullOrWhiteSpace(_smsOptions.Aliyun.AccessKeySecret))
        {
            await AliyunSendSms(phoneNumber, templateId);
        }
        else
        {
            await TencentSendSms(phoneNumber, templateId);
        }
    }

    /// <summary>
    /// Verify SMS verification code
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Verify SMS verification code")]
    public bool VerifyCode(SmsVerifyCodeInput input)
    {
        var verifyCode = _sysCacheService.Get<string>($"{CacheConst.KeyPhoneVerCode}{input.Phone}");

        if (string.IsNullOrWhiteSpace(verifyCode)) throw Oops.Oh("The verification code does not exist or has expired, please retrieve it again!");

        if (verifyCode != input.Code) throw Oops.Oh("Verification code error!");

        return true;
    }

    /// <summary>
    /// Alibaba Cloud sends SMS 📨
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="templateId">SMS template id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Alibaba Cloud sends SMS")]
    public async Task AliyunSendSms([Required] string phoneNumber, string templateId = "0")
    {
        if (!phoneNumber.TryValidate(ValidationTypes.PhoneNumber).IsValid) throw Oops.Oh("Please fill in your mobile phone number correctly");

        // Generate random verification code
        var random = new Random();
        var verifyCode = random.Next(100000, 999999);

        var templateParam = new
        {
            code = verifyCode
        };

        var client = CreateAliyunClient();
        var template = _smsOptions.Aliyun.GetTemplate(templateId);
        var sendSmsRequest = new SendSmsRequest
        {
            PhoneNumbers = phoneNumber, // Mobile phone number to be sent, multiple separated by commas
            SignName = template.SignName, // SMS signature
            TemplateCode = template.TemplateCode, // SMS template
            TemplateParam = templateParam.ToJson(), // Variables in templates replace JSON strings
            OutId = YitIdHelper.NextId().ToString()
        };
        var sendSmsResponse = await client.SendSmsAsync(sendSmsRequest);
        if (sendSmsResponse.Body.Code == "OK" && sendSmsResponse.Body.Message == "OK")
        {
            // var bizId = sendSmsResponse.Body.BizId;
            _sysCacheService.Set($"{CacheConst.KeyPhoneVerCode}{phoneNumber}", verifyCode, TimeSpan.FromSeconds(_smsOptions.VerifyCodeExpireSeconds));
        }
        else
        {
            throw Oops.Oh($"Short messageSendFailure：{sendSmsResponse.Body.Code}-{sendSmsResponse.Body.Message}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Send SMS template
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="templateParam">SMS content</param>
    /// <param name="templateId">SMS template id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Send SMS template")]
    public async Task AliyunSendSmsTemplate([Required] string phoneNumber, [Required] dynamic templateParam, string templateId)
    {
        if (!phoneNumber.TryValidate(ValidationTypes.PhoneNumber).IsValid) throw Oops.Oh("Please fill in your mobile phone number correctly");

        if (string.IsNullOrWhiteSpace(templateParam.ToString())) throw Oops.Oh("SMS content cannot be empty");

        var client = CreateAliyunClient();
        var template = _smsOptions.Aliyun.GetTemplate(templateId);
        var sendSmsRequest = new SendSmsRequest
        {
            PhoneNumbers = phoneNumber, // Mobile phone number to be sent, multiple separated by commas
            SignName = template.SignName, // SMS signature
            TemplateCode = template.TemplateCode, // SMS template
            TemplateParam = templateParam.ToString(), // Variables in templates replace JSON strings
            OutId = YitIdHelper.NextId().ToString()
        };
        var sendSmsResponse = await client.SendSmsAsync(sendSmsRequest);
        if (sendSmsResponse.Body.Code == "OK" && sendSmsResponse.Body.Message == "OK")
        {
        }
        else
        {
            throw Oops.Oh($"Short messageSendFailure：{sendSmsResponse.Body.Code}-{sendSmsResponse.Body.Message}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Send SMS via Tencent Cloud 📨
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="templateId">SMS template id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Tencent Cloud sends SMS")]
    public async Task TencentSendSms([Required] string phoneNumber, string templateId = "0")
    {
        if (!phoneNumber.TryValidate(ValidationTypes.PhoneNumber).IsValid) throw Oops.Oh("Please fill in your mobile phone number correctly");

        // Generate random verification code
        var random = new Random();
        var verifyCode = random.Next(100000, 999999);

        // Instantiate the client object to request the product, clientProfile is optional
        var client = new SmsClient(CreateTencentClient(), "ap-guangzhou", new ClientProfile() { HttpProfile = new HttpProfile() { Endpoint = ("sms.tencentcloudapi.com") } });
        var template = _smsOptions.Tencentyun.GetTemplate(templateId);
        // Instantiate a request object. Each interface will correspond to a request object.
        var req = new TencentCloud.Sms.V20190711.Models.SendSmsRequest
        {
            PhoneNumberSet = new string[] { "+86" + phoneNumber.Trim(',') },
            SmsSdkAppid = _smsOptions.Tencentyun.SdkAppId,
            Sign = template.SignName,
            TemplateID = template.TemplateCode,
            TemplateParamSet = new string[] { verifyCode.ToString() }
        };

        // The returned resp is an instance of SendSmsResponse, corresponding to the request object
        TencentCloud.Sms.V20190711.Models.SendSmsResponse resp = client.SendSmsSync(req);

        if (resp.SendStatusSet[0].Code == "Ok" && resp.SendStatusSet[0].Message == "send success")
        {
            // var bizId = sendSmsResponse.Body.BizId;
            _sysCacheService.Set($"{CacheConst.KeyPhoneVerCode}{phoneNumber}", verifyCode, TimeSpan.FromSeconds(_smsOptions.VerifyCodeExpireSeconds));
        }
        else
        {
            throw Oops.Oh($"SMS sending failed: {resp.SendStatusSet[0].Code}-{resp.SendStatusSet[0].Message}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Alibaba Cloud SMS configuration
    /// </summary>
    /// <returns></returns>
    private AlibabaCloud.SDK.Dysmsapi20170525.Client CreateAliyunClient()
    {
        var config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = _smsOptions.Aliyun.AccessKeyId,
            AccessKeySecret = _smsOptions.Aliyun.AccessKeySecret,
            Endpoint = "dysmsapi.aliyuncs.com"
        };
        return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
    }

    /// <summary>
    /// Tencent Cloud SMS configuration
    /// </summary>
    /// <returns></returns>
    private Credential CreateTencentClient()
    {
        var cred = new Credential
        {
            SecretId = _smsOptions.Tencentyun.AccessKeyId,
            SecretKey = _smsOptions.Tencentyun.AccessKeySecret
        };
        return cred;
    }

    /// <summary>
    /// Customized SMS interface to send SMS 📨
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="templateId">SMS template id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Send SMS via custom SMS interface")]
    public async Task CustomSendSms([DataValidation(ValidationTypes.PhoneNumber)] string phoneNumber, string templateId = "0")
    {
        if (_smsOptions.Custom == null || !_smsOptions.Custom.Enabled)
            throw Oops.Oh("Custom SMS interface is not enabled");

        if (string.IsNullOrWhiteSpace(_smsOptions.Custom.ApiUrl))
            throw Oops.Oh("Custom SMS interface address is not configured");

        // Generate random verification code
        var verifyCode = Random.Shared.Next(100000, 999999);

        // Get template
        var template = _smsOptions.Custom.GetTemplate(templateId);
        if (template == null)
            throw Oops.Oh($"SMS template [{templateId}] does not exist");

        // Replace placeholders in template content
        var content = template.Content.Replace("{code}", verifyCode.ToString());

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            HttpResponseMessage response;

            //Replace URL placeholder
            var url = _smsOptions.Custom.ApiUrl
                .Replace("{templateId}", templateId)
                .Replace("{mobile}", phoneNumber)
                .Replace("{content}", Uri.EscapeDataString(content))
                .Replace("{code}", verifyCode.ToString());

            if (_smsOptions.Custom.Method.ToUpper() == "POST")
            {
                // replace placeholder
                var postData = _smsOptions.Custom.PostData?
                    .Replace("{templateId}", templateId)
                    .Replace("{mobile}", phoneNumber)
                    .Replace("{content}", content)
                    .Replace("{code}", verifyCode.ToString());
                HttpContent httpContent = new StringContent(postData ?? string.Empty, Encoding.UTF8, _smsOptions.Custom.ContentType ?? "application/x-www-form-urlencoded");
                response = await httpClient.PostAsync(url, httpContent);
            }
            else
            {
                // GET request
                response = await httpClient.GetAsync(url);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            // Determine whether sending is successful
            if (response.IsSuccessStatusCode && responseContent.Contains(_smsOptions.Custom.SuccessFlag))
            {
                if (_smsOptions.Custom.ApiUrl.Contains("{code}") || template.Content.Contains("{code}") || (_smsOptions.Custom.PostData?.Contains("{code}") == true))
                {
                    // If the template contains a verification code, add it to the cache
                    _sysCacheService.Set($"{CacheConst.KeyPhoneVerCode}{phoneNumber}", verifyCode, TimeSpan.FromSeconds(_smsOptions.VerifyCodeExpireSeconds));
                }
            }
            else
            {
                throw Oops.Oh($"SMS sending failed: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            throw Oops.Oh($"SMS sending exception: {ex.Message}");
        }
    }
}