// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;
using Admin.NET.Core.Service;
using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Admin.NET.Web.Core;

public class JwtHandler : AppAuthorizeHandler
{
    private readonly SysCacheService _sysCacheService = App.GetRequiredService<SysCacheService>();
    private readonly SysConfigService _sysConfigService = App.GetRequiredService<SysConfigService>();
    private static readonly SysMenuService SysMenuService = App.GetRequiredService<SysMenuService>();

    /// <summary>
    /// Automatically refresh Token
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        var userId = context.User.FindFirst(ClaimConst.UserId)?.Value;
        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // 🛡️ Blacklist verification (including users and tokens)
        if (_sysCacheService.ExistKey($"{CacheConst.KeyBlacklist}{userId}") ||
            _sysCacheService.ExistKey($"blacklist:token:{token}"))
        {
            context.Fail();
            context.GetCurrentHttpContext().SignoutToSwagger();
            return;
        }

        var tokenExpire = await _sysConfigService.GetTokenExpire();
        var refreshTokenExpire = await _sysConfigService.GetRefreshTokenExpire();
        if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext(), tokenExpire, refreshTokenExpire))
        {
            await AuthorizeHandleAsync(context);
        }
        else
        {
            context.Fail(); // Authorization failed
            var currentHttpContext = context.GetCurrentHttpContext();
            if (currentHttpContext == null) return;

            // Skip failures due to SignatureAuthentication
            if (currentHttpContext.Items.ContainsKey(SignatureAuthenticationDefaults.AuthenticateFailMsgKey)) return;
            currentHttpContext.SignoutToSwagger();
        }
    }

    public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // Jwt Token validity has been automatically verified
        return await CheckAuthorizeAsync(httpContext);
    }

    /// <summary>
    /// Permission verification core logic
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static async Task<bool> CheckAuthorizeAsync(DefaultHttpContext httpContext)
    {
        // Login mode to determine PC and APP
        if (App.User.FindFirst(ClaimConst.LoginMode)?.Value == ((int)LoginModeEnum.APP).ToString())
            return true;

        // Exclude super pipe
        if (App.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString())
            return true;

        // Route name
        var routeName = httpContext.Request.Path.StartsWithSegments("/api")
            ? httpContext.Request.Path.Value![5..].Replace("/", ":")
            : httpContext.Request.Path.Value![1..].Replace("/", ":");

        // Get the set of button permissions that the user has
        var ownBtnPermList = await SysMenuService.GetOwnBtnPermList();
        if (ownBtnPermList.Exists(u => routeName.Equals(u, StringComparison.CurrentCultureIgnoreCase)))
            return true;

        // Get the permission set of all buttons in the system
        var allBtnPermList = await SysMenuService.GetAllBtnPermList();
        return allBtnPermList.TrueForAll(u => !routeName.Equals(u, StringComparison.CurrentCultureIgnoreCase));
    }
}