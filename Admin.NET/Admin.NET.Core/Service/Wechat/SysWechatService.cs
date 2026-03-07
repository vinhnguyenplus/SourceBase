// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// WeChat public account service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 230)]
public class SysWechatService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatUser> _sysWechatUserRep;
    private readonly SysConfigService _sysConfigService;
    private readonly WechatApiClientFactory _wechatApiClientFactory;
    private readonly WechatApiClient _wechatApiClient;

    public SysWechatService(SqlSugarRepository<SysWechatUser> sysWechatUserRep,
        SysConfigService sysConfigService,
        WechatApiClientFactory wechatApiClientFactory,
        SysCacheService sysCacheService)
    {
        _sysWechatUserRep = sysWechatUserRep;
        _sysConfigService = sysConfigService;
        _wechatApiClientFactory = wechatApiClientFactory;
        _wechatApiClient = wechatApiClientFactory.CreateWechatClient();
    }

    /// <summary>
    /// Generate web page authorization Url 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Generate web page authorization URL")]
    public string GenAuthUrl(GenAuthUrlInput input)
    {
        return _wechatApiClient.GenerateParameterizedUrlForConnectOAuth2Authorize(input.RedirectUrl, input.Scope, input.State);
    }

    /// <summary>
    /// Get WeChat user OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("Obtain WeChat user's OpenId")]
    public async Task<string> SnsOAuth2([FromQuery] WechatOAuth2Input input)
    {
        var reqOAuth2 = new SnsOAuth2AccessTokenRequest()
        {
            Code = input.Code,
        };
        var resOAuth2 = await _wechatApiClient.ExecuteSnsOAuth2AccessTokenAsync(reqOAuth2);
        if (resOAuth2.ErrorCode != (int)WechatReturnCodeEnum.Requestsuccess)
            throw Oops.Oh(resOAuth2.ErrorMessage + " " + resOAuth2.ErrorCode);

        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == resOAuth2.OpenId);
        if (wxUser == null)
        {
            var reqUserInfo = new SnsUserInfoRequest()
            {
                OpenId = resOAuth2.OpenId,
                AccessToken = resOAuth2.AccessToken,
            };
            var resUserInfo = await _wechatApiClient.ExecuteSnsUserInfoAsync(reqUserInfo);
            wxUser = resUserInfo.Adapt<SysWechatUser>();
            wxUser.Avatar = resUserInfo.HeadImageUrl;
            wxUser.NickName = resUserInfo.Nickname;
            wxUser.OpenId = resOAuth2.OpenId;
            wxUser.UnionId = resOAuth2.UnionId;
            wxUser.AccessToken = resOAuth2.AccessToken;
            wxUser.RefreshToken = resOAuth2.RefreshToken;
            wxUser = await _sysWechatUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            wxUser.AccessToken = resOAuth2.AccessToken;
            wxUser.RefreshToken = resOAuth2.RefreshToken;
            await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return resOAuth2.OpenId;
    }

    /// <summary>
    /// WeChat user login OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("WeChat user login OpenId")]
    public async Task<dynamic> OpenIdLogin(WechatUserLogin input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("WeChat user login OpenId error");

        var tokenExpire = await _sysConfigService.GetTokenExpire();
        return new
        {
            wxUser.Avatar,
            accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
            {
                { ClaimConst.UserId, wxUser.Id },
                { ClaimConst.NickName, wxUser.NickName },
                { ClaimConst.LoginMode, LoginModeEnum.APP },
            }, tokenExpire)
        };
    }

    /// <summary>
    /// Get configuration signature parameters (wx.config) 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get configuration signature parameters (wx.config)")]
    public async Task<dynamic> GenConfigPara(SignatureInput input)
    {
        string ticket = await _wechatApiClientFactory.TryGetWechatJsApiTicketAsync();
        return _wechatApiClient.GenerateParametersForJSSDKConfig(ticket, input.Url);
    }

    /// <summary>
    /// Get template list 🔖
    /// </summary>
    [DisplayName("Get template list")]
    public async Task<dynamic> GetMessageTemplateList()
    {
        var accessToken = await GetCgibinToken();
        var reqTemplate = new CgibinTemplateGetAllPrivateTemplateRequest()
        {
            AccessToken = accessToken
        };
        var resTemplate = await _wechatApiClient.ExecuteCgibinTemplateGetAllPrivateTemplateAsync(reqTemplate);
        if (resTemplate.ErrorCode != (int)WechatReturnCodeEnum.Requestsuccess)
            throw Oops.Oh(resTemplate.ErrorMessage + " " + resTemplate.ErrorCode);

        return resTemplate.TemplateList;
    }

    /// <summary>
    /// Send template message 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send template message")]
    public async Task<dynamic> SendTemplateMessage(MessageTemplateSendInput input)
    {
        var dataInfo = input.Data.ToDictionary(k => k.Key, k => k.Value);
        var messageData = new Dictionary<string, CgibinMessageTemplateSendRequest.Types.DataItem>();
        foreach (var item in dataInfo)
        {
            messageData.Add(item.Key, new CgibinMessageTemplateSendRequest.Types.DataItem() { Value = "" + item.Value.Value.ToString() + "" });
        }

        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinMessageTemplateSendRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId,
            ToUserOpenId = input.ToUserOpenId,
            Url = input.Url,
            MiniProgram = new CgibinMessageTemplateSendRequest.Types.MiniProgram
            {
                AppId = _wechatApiClientFactory._wechatOptions.WxOpenAppId,
                PagePath = input.MiniProgramPagePath,
            },
            Data = messageData
        };
        var resMessage = await _wechatApiClient.ExecuteCgibinMessageTemplateSendAsync(reqMessage);
        return resMessage;
    }

    /// <summary>
    /// Delete template 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteMessageTemplate"), HttpPost]
    [DisplayName("Delete template")]
    public async Task<dynamic> DeleteMessageTemplate(DeleteMessageTemplateInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinTemplateDeletePrivateTemplateRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId
        };
        var resTemplate = await _wechatApiClient.ExecuteCgibinTemplateDeletePrivateTemplateAsync(reqMessage);
        return resTemplate;
    }

    /// <summary>
    /// Get Access_token
    /// </summary>
    [NonAction]
    public async Task<string> GetCgibinToken()
    {
        return await _wechatApiClientFactory.TryGetWechatAccessTokenAsync();
    }
}