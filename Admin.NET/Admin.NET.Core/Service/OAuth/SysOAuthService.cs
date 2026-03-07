// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Admin.NET.Core.Service;

/// <summary>
/// System OAuth service 🧩
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(Order = 498)]
public class SysOAuthService : IDynamicApiController, ITransient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SqlSugarRepository<SysWechatUser> _sysWechatUserRep;

    public SysOAuthService(IHttpContextAccessor httpContextAccessor,
        SqlSugarRepository<SysWechatUser> sysWechatUserRep)
    {
        _httpContextAccessor = httpContextAccessor;
        _sysWechatUserRep = sysWechatUserRep;
    }

    /// <summary>
    /// Third party login 🔖
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "SignIn"), HttpGet]
    [DisplayName("Third party login")]
    public virtual async Task<IActionResult> SignIn([FromQuery] string provider, [FromQuery] string redirectUrl)
    {
        if (string.IsNullOrWhiteSpace(provider) || !await _httpContextAccessor.HttpContext.IsProviderSupportedAsync(provider))
            throw Oops.Oh("Unsupported OAuth types");

        var request = _httpContextAccessor.HttpContext!.Request;
        var url = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}Callback?provider={provider}&redirectUrl={redirectUrl}";
        var properties = new AuthenticationProperties
        {
            RedirectUri = url,
            Items = { ["LoginProvider"] = provider }
        };
        return await Task.FromResult(new ChallengeResult(provider, properties));
    }

    /// <summary>
    /// Authorization callback 🔖
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "SignInCallback"), HttpGet]
    [DisplayName("Authorization callback")]
    public virtual async Task<IActionResult> SignInCallback([FromQuery] string provider = null, [FromQuery] string redirectUrl = "")
    {
        if (string.IsNullOrWhiteSpace(provider) || !await _httpContextAccessor.HttpContext.IsProviderSupportedAsync(provider))
            throw Oops.Oh("Unsupported OAuth types");

        var authenticateResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync(provider);
        if (!authenticateResult.Succeeded)
            throw Oops.Oh("Authorization failed");

        var openIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
        if (openIdClaim == null || string.IsNullOrWhiteSpace(openIdClaim.Value))
            throw Oops.Oh("Authorization failed");

        var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
        var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var mobilePhone = authenticateResult.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
        var dateOfBirth = authenticateResult.Principal.FindFirst(ClaimTypes.DateOfBirth)?.Value;
        var gender = authenticateResult.Principal.FindFirst(ClaimTypes.Gender)?.Value;
        var avatarUrl = "";

        var platformType = PlatformTypeEnum.WeChat Official Account;
        if (provider == "Gitee")
        {
            platformType = PlatformTypeEnum.Gitee;
            avatarUrl = authenticateResult.Principal.FindFirst(OAuthClaim.GiteeAvatarUrl)?.Value;
        }

        // If the account does not exist, create a new one
        var wechatUser = await _sysWechatUserRep.AsQueryable().Includes(u => u.SysUser).ClearFilter().FirstAsync(u => u.OpenId == openIdClaim.Value);
        if (wechatUser == null)
        {
            var userId = await App.GetRequiredService<SysUserService>().AddUser(new AddUserInput()
            {
                Account = name,
                RealName = name,
                NickName = name,
                Email = email,
                Avatar = avatarUrl,
                Phone = mobilePhone,
                OrgId = 1300000000101, // root organizational structure
                RoleIdList = new List<long> { 1300000000104 } // Only personal data role
            });

            await _sysWechatUserRep.InsertAsync(new SysWechatUser()
            {
                UserId = userId,
                OpenId = openIdClaim.Value,
                Avatar = avatarUrl,
                NickName = name,
                PlatformType = platformType
            });

            wechatUser = await _sysWechatUserRep.AsQueryable().Includes(u => u.SysUser).ClearFilter().FirstAsync(u => u.OpenId == openIdClaim.Value);
        }

        // Build Token
        var token = await App.GetRequiredService<SysAuthService>().CreateToken(wechatUser.SysUser);

        return new RedirectResult($"{redirectUrl}/#/login?token={token.AccessToken}");
    }
}