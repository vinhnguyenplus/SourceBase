// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// WeChat Mini Program Service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 240)]
public class SysWxOpenService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatUser> _sysWechatUserRep;
    private readonly SysConfigService _sysConfigService;
    private readonly WechatApiClient _wechatApiClient;
    private readonly SysFileService _sysFileService;
    private readonly WechatApiClientFactory _wechatApiClientFactory;

    public SysWxOpenService(SqlSugarRepository<SysWechatUser> sysWechatUserRep,
        SysConfigService sysConfigService,
        WechatApiClientFactory wechatApiClientFactory,
        SysFileService sysFileService)
    {
        _sysWechatUserRep = sysWechatUserRep;
        _sysConfigService = sysConfigService;
        _wechatApiClient = wechatApiClientFactory.CreateWxOpenClient();
        _sysFileService = sysFileService;
        _wechatApiClientFactory = wechatApiClientFactory;
    }

    /// <summary>
    /// Get WeChat user OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("Obtain WeChat user's OpenId")]
    public async Task<WxOpenIdOutput> GetWxOpenId([FromQuery] JsCode2SessionInput input)
    {
        var reqJsCode2Session = new SnsJsCode2SessionRequest()
        {
            JsCode = input.JsCode,
        };
        var resCode2Session = await _wechatApiClient.ExecuteSnsJsCode2SessionAsync(reqJsCode2Session);
        if (resCode2Session.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resCode2Session.ErrorMessage + " " + resCode2Session.ErrorCode);

        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == resCode2Session.OpenId);
        if (wxUser == null)
        {
            wxUser = new SysWechatUser
            {
                OpenId = resCode2Session.OpenId,
                UnionId = resCode2Session.UnionId,
                SessionKey = resCode2Session.SessionKey,
                PlatformType = PlatformTypeEnum.WeChatMiniProgram
            };
            wxUser = await _sysWechatUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return new WxOpenIdOutput
        {
            OpenId = resCode2Session.OpenId
        };
    }

    /// <summary>
    /// Get WeChat user phone number 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("ObtainWeChatUserTelephoneNumber")]
    public async Task<WxPhoneOutput> GetWxPhone([FromQuery] WxPhoneInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqUserPhoneNumber = new WxaBusinessGetUserPhoneNumberRequest()
        {
            Code = input.Code,
            AccessToken = accessToken,
        };
        var resUserPhoneNumber = await _wechatApiClient.ExecuteWxaBusinessGetUserPhoneNumberAsync(reqUserPhoneNumber);
        if (resUserPhoneNumber.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resUserPhoneNumber.ErrorMessage + " " + resUserPhoneNumber.ErrorCode);

        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
        {
            wxUser = new SysWechatUser
            {
                OpenId = input.OpenId,
                Mobile = resUserPhoneNumber.PhoneInfo?.PhoneNumber,
                PlatformType = PlatformTypeEnum.WeChatMiniProgram
            };
            wxUser = await _sysWechatUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            wxUser.Mobile = resUserPhoneNumber.PhoneInfo?.PhoneNumber;
            await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return new WxPhoneOutput
        {
            PhoneNumber = resUserPhoneNumber.PhoneInfo?.PhoneNumber
        };
    }

    /// <summary>
    /// WeChat applet login OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("WeChat applet login OpenId")]
    public async Task<dynamic> WxOpenIdLogin(WxOpenIdLoginInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(u => u.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("WeChat Mini Program login failed");

        var tokenExpire = await _sysConfigService.GetTokenExpire();
        return new
        {
            wxUser.Avatar,
            accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
            {
                { ClaimConst.UserId, wxUser.Id },
                { ClaimConst.RealName, wxUser.NickName },
                { ClaimConst.LoginMode, LoginModeEnum.APP },
            }, tokenExpire)
        };
    }

    /// <summary>
    /// Upload mini program avatar
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Upload mini program avatar")]
    public async Task<SysFile> UploadAvatar([FromForm] UploadAvatarInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(u => u.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("User upload failed not found");

        var res = await _sysFileService.UploadFile(new UploadFileInput { File = input.File, FileType = input.FileType }, "upload/wechatAvatar");
        wxUser.Avatar = res.Url;
        await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();

        return res;
    }

    /// <summary>
    /// Set mini program user nickname
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task SetNickName(SetNickNameInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(u => u.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("User information not found, setting failed");
        wxUser.NickName = input.NickName;
        await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        return;
    }

    /// <summary>
    /// Get mini program user information
    /// </summary>
    /// <param name="openid"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<dynamic> GetUserInfo(string openid)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(u => u.OpenId == openid);
        if (wxUser == null)
            throw Oops.Oh("Failed to obtain user information; user not found");
        return new { nickName = wxUser.NickName, avator = wxUser.Avatar };
    }

    /// <summary>
    /// Get subscription message template list 🔖
    /// </summary>
    [DisplayName("Get the list of subscription message templates")]
    public async Task<dynamic> GetMessageTemplateList()
    {
        var accessToken = await GetCgibinToken();
        var reqTemplate = new WxaApiNewTemplateGetTemplateRequest()
        {
            AccessToken = accessToken
        };
        var resTemplate = await _wechatApiClient.ExecuteWxaApiNewTemplateGetTemplateAsync(reqTemplate);
        if (resTemplate.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resTemplate.ErrorMessage + " " + resTemplate.ErrorCode);

        return resTemplate.TemplateList;
    }

    /// <summary>
    /// Send subscription message 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send subscription message")]
    public async Task<dynamic> SendSubscribeMessage(SendSubscribeMessageInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinMessageSubscribeSendRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId,
            ToUserOpenId = input.ToUserOpenId,
            Data = input.Data,
            MiniProgramState = input.MiniprogramState,
            Language = input.Language,
            MiniProgramPagePath = input.MiniProgramPagePath
        };
        var resMessage = await _wechatApiClient.ExecuteCgibinMessageSubscribeSendAsync(reqMessage);
        return resMessage;
    }

    /// <summary>
    /// Add subscription message template 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddSubscribeMessageTemplate"), HttpPost]
    [DisplayName("Add subscription message template")]
    public async Task<dynamic> AddSubscribeMessageTemplate(AddSubscribeMessageTemplateInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new WxaApiNewTemplateAddTemplateRequest()
        {
            AccessToken = accessToken,
            TemplateTitleId = input.TemplateTitleId,
            KeyworkIdList = input.KeyworkIdList,
            SceneDescription = input.SceneDescription
        };
        var resTemplate = await _wechatApiClient.ExecuteWxaApiNewTemplateAddTemplateAsync(reqMessage);
        return resTemplate;
    }

    /// <summary>
    /// Generate QR codes for mini programs with parameters (the total number of codes generated is limited to 100,000)
    /// </summary>
    /// <param name="input"> The mini program page path entered by scanning the QR code has a maximum length of 128 characters and cannot be empty; eg: pages / index ? id = AY000001 </param>
    /// <returns></returns>
    [DisplayName("Generate mini program QR code")]
    [ApiDescriptionSettings(Name = "GenerateQRImage")]
    public async Task<GenerateQRImageOutput> GenerateQRImageAsync(GenerateQRImageInput input)
    {
        GenerateQRImageOutput generateQRImageOutInput = new GenerateQRImageOutput();
        if (input.PagePath.IsNullOrEmpty())
        {
            generateQRImageOutInput.Message = $"Generation failed. Page path cannot be empty.";
            return generateQRImageOutInput;
        }

        if (input.ImageName.IsNullOrEmpty())
        {
            input.ImageName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        var accessToken = await GetCgibinToken();
        var request = new CgibinWxaappCreateWxaQrcodeRequest
        {
            AccessToken = accessToken,
            Path = input.PagePath,
            Width = input.Width
        };
        var response = await _wechatApiClient.ExecuteCgibinWxaappCreateWxaQrcodeAsync(request);

        if (response.IsSuccessful())
        {
            var QRImagePath = App.GetConfig<string>("Wechat:QRImagePath");
            var relativeImgPath = string.Empty;

            // Determine whether the path is an absolute path or a relative path
            var isPathRooted = Path.IsPathRooted(QRImagePath);
            if (!isPathRooted)
            {
                // relative path
                relativeImgPath = string.IsNullOrEmpty(QRImagePath) ? Path.Combine("upload", "QRImage") : QRImagePath;
                QRImagePath = Path.Combine(App.WebHostEnvironment.WebRootPath, relativeImgPath);
            }

            //Determine whether the file storage path exists
            if (!Directory.Exists(QRImagePath))
            {
                Directory.CreateDirectory(QRImagePath);
            }
            // Save QR code image data as a file
            var fileName = $"{input.ImageName.ToUpper()}.png";
            var filePath = Path.Combine(QRImagePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, response.GetRawBytes());

            generateQRImageOutInput.Success = true;
            generateQRImageOutInput.ImgPath = filePath;
            generateQRImageOutInput.RelativeImgPath = Path.Combine(relativeImgPath, fileName);
            generateQRImageOutInput.Message = "Generated successfully";
        }
        else
        {
            // Handle error conditions
            generateQRImageOutInput.Message = $"Generation failed Error code: {response.ErrorCode} Error description: {response.ErrorMessage}";
        }
        return generateQRImageOutInput;
    }

    /// <summary>
    /// Generate QR code (get unlimited mini program code)
    /// </summary>
    /// <param name="input">Add ginseng</param>
    /// <returns></returns>
    [DisplayName("Generate mini program QR code")]
    [ApiDescriptionSettings(Name = "GenerateQRImageUnlimit")]
    public async Task<GenerateQRImageOutput> GenerateQRImageUnlimitAsync(GenerateQRImageUnLimitInput input)
    {
        GenerateQRImageOutput generateQRImageOutInput = new GenerateQRImageOutput();
        if (input.PagePath.IsNullOrEmpty())
        {
            generateQRImageOutInput.Message = $"Generation failed, the page path cannot be empty";
            return generateQRImageOutInput;
        }

        if (input.Scene.Length > 32)
        {
            generateQRImageOutInput.Message = $"Generation failed, the length of the provided parameters exceeds the limit";
            return generateQRImageOutInput;
        }

        if (input.ImageName.IsNullOrEmpty())
        {
            input.ImageName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        var accessToken = await GetCgibinToken();
        var request = new WxaGetWxaCodeRequest
        {
            AccessToken = accessToken,
            Width = input.Width,
            PagePath = input.PagePath,
        };
        var response = await _wechatApiClient.ExecuteWxaGetWxaCodeAsync(request);

        if (response.IsSuccessful())
        {
            var QRImagePath = App.GetConfig<string>("Wechat:QRImagePath");
            var relativeImgPath = string.Empty;

            // Determine whether the path is an absolute path or a relative path
            var isPathRooted = Path.IsPathRooted(QRImagePath);
            if (!isPathRooted)
            {
                // relative path
                relativeImgPath = string.IsNullOrEmpty(QRImagePath) ? Path.Combine("upload", "QRImageUnLimit") : QRImagePath;
                QRImagePath = Path.Combine(App.WebHostEnvironment.WebRootPath, relativeImgPath);
            }

            //Determine whether the file storage path exists
            if (!Directory.Exists(QRImagePath))
            {
                Directory.CreateDirectory(QRImagePath);
            }
            // Save QR code image data as a file
            var fileName = $"{input.ImageName.ToUpper()}.png";
            var filePath = Path.Combine(QRImagePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, response.GetRawBytes());

            generateQRImageOutInput.Success = true;
            generateQRImageOutInput.ImgPath = filePath;
            generateQRImageOutInput.RelativeImgPath = Path.Combine(relativeImgPath, fileName);
            generateQRImageOutInput.Message = "Generated successfully";
        }
        else
        {
            // Handle error conditions
            generateQRImageOutInput.Message = $"Generation failed Error code: {response.ErrorCode} Error description: {response.ErrorMessage}";
        }
        return generateQRImageOutInput;
    }

    /// <summary>
    /// Get Access_token
    /// </summary>
    private async Task<string> GetCgibinToken()
    {
        return await _wechatApiClientFactory.TryGetWxOpenAccessTokenAsync();
    }
}