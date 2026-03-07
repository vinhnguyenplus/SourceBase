// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion.SpecificationDocument;
using Lazy.Captcha.Core;
using NewLife.Reflection;

namespace Admin.NET.Core.Service;

/// <summary>
/// System login authorization service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 500)]
public class SysAuthService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SysMenuService _sysMenuService;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysUserService _sysUserService;
    private readonly SysTenantService _sysTenantService;
    private readonly SysSmsService _sysSmsService;
    private readonly SysLdapService _sysLdapService;
    private readonly ICaptcha _captcha;
    private readonly IEventPublisher _eventPublisher;
    private readonly SysCacheService _sysCacheService;

    public SysAuthService(
        SqlSugarRepository<SysUser> sysUserRep,
        IHttpContextAccessor httpContextAccessor,
        SysOnlineUserService sysOnlineUserService,
        SysConfigService sysConfigService,
        SysLdapService sysLdapService,
        IEventPublisher eventPublisher,
        SysSmsService sysSmsService,
        SysCacheService sysCacheService,
        SysMenuService sysMenuService,
        SysUserService sysUserService,
        SysTenantService sysTenantService,
        UserManager userManager,
        ICaptcha captcha)
    {
        _captcha = captcha;
        _sysUserRep = sysUserRep;
        _userManager = userManager;
        _sysSmsService = sysSmsService;
        _eventPublisher = eventPublisher;
        _sysUserService = sysUserService;
        _sysTenantService = sysTenantService;
        _sysMenuService = sysMenuService;
        _sysCacheService = sysCacheService;
        _sysConfigService = sysConfigService;
        _httpContextAccessor = httpContextAccessor;
        _sysOnlineUserService = sysOnlineUserService;
        _sysLdapService = sysLdapService;
    }

    /// <summary>
    /// Login with account and password 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <remarks>Username/password: superadmin/123456</remarks>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Account numberpasswordLogin")]
    public virtual async Task<LoginOutput> Login([Required] LoginInput input)
    {
        // Determine the number of incorrect passwords (cached for 30 minutes)
        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{input.Account}";
        var passwordErrorTimes = _sysCacheService.Get<int>(keyPasswordErrorTimes);
        var passwordMaxErrorTimes = await _sysConfigService.GetConfigValue<int>(ConfigConst.SysPasswordMaxErrorTimes);
        // If it is not configured or misconfigured to 0 or a negative number, the maximum number of default password errors is 5.
        if (passwordMaxErrorTimes < 1) passwordMaxErrorTimes = 5;
        if (passwordErrorTimes > passwordMaxErrorTimes) throw Oops.Oh(ErrorCodeEnum.D1027);

        // Determine whether the verification code is enabled and verify the verification code
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysCaptcha) && !_captcha.Validate(input.CodeId.ToString(), input.Code)) throw Oops.Oh(ErrorCodeEnum.D0008);

        // Get login tenant and user
        var (tenant, user) = await GetLoginUserAndTenant(input.TenantId, account: input.Account);

        // Is the account frozen?
        if (user.Status == StatusEnum.Disable) throw Oops.Oh(ErrorCodeEnum.D1017);

        // Whether to enable domain login verification
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysDomainLogin))
        {
            var userLdap = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserLdap>>().GetFirstAsync(u => u.UserId == user.Id && u.TenantId == tenant.Id);
            if (userLdap == null)
            {
                VerifyPassword(input.Password, keyPasswordErrorTimes, passwordErrorTimes, user);
            }
            else if (!await App.GetRequiredService<SysLdapService>().AuthAccount(tenant.Id, userLdap.Account, CryptogramUtil.Decrypt(input.Password)))
            {
                _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
                throw Oops.Oh(ErrorCodeEnum.D1000);
            }
        }
        else
            VerifyPassword(input.Password, keyPasswordErrorTimes, passwordErrorTimes, user);

        // If the login is successful, the number of incorrect passwords will be cleared.
        _sysCacheService.Remove(keyPasswordErrorTimes);

        return await CreateToken(user);
    }

    /// <summary>
    /// Get login tenant and user
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="account"></param>
    /// <param name="phone"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<(SysTenant tenant, SysUser user)> GetLoginUserAndTenant(long? tenantId, string account = null, string phone = null)
    {
        // Does the account exist?
        var user = await _sysUserRep.AsQueryable().Includes(u => u.SysOrg).ClearFilter()
            .WhereIF(tenantId > 0, u => (u.AccountType == AccountTypeEnum.SuperAdmin || u.TenantId == tenantId))
            .WhereIF(!string.IsNullOrWhiteSpace(account), u => u.Account.Equals(account))
            .WhereIF(!string.IsNullOrWhiteSpace(phone), u => u.Phone.Equals(phone)).FirstAsync();
        _ = user ?? throw Oops.Oh(ErrorCodeEnum.D1000);

        // Whether the tenant exists or is disabled
        var tenant = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().AsQueryable()
            .WhereIF(tenantId > 0, u => u.Id == tenantId).WhereIF(tenantId.ToLong() == 0, u => u.Id == user.TenantId).FirstAsync();
        if (tenant?.Status != StatusEnum.Enable) throw Oops.Oh(ErrorCodeEnum.Z1003);

        // If you are a super administrator, refer to the tenant selected for login to enter the system.
        if (tenantId > 0 && user.AccountType == AccountTypeEnum.SuperAdmin)
            user.TenantId = tenantId;

        return (tenant, user);
    }

    /// <summary>
    /// Verify user password
    /// </summary>
    /// <param name="password"></param>
    /// <param name="keyPasswordErrorTimes"></param>
    /// <param name="passwordErrorTimes"></param>
    /// <param name="user"></param>
    private void VerifyPassword(string password, string keyPasswordErrorTimes, int passwordErrorTimes, SysUser user)
    {
        try
        {
            // National secret SM2 decryption (front-end password transmission SM2 encrypted)
            password = CryptogramUtil.SM2Decrypt(password);
            if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
            {
                if (user.Password.Equals(MD5Encryption.Encrypt(password))) return;
            }
            else
            {
                if (CryptogramUtil.Decrypt(user.Password).Equals(password)) return;
            }
        }
        catch (Exception ex)
        {
            Log.Error("User password verification exception:", ex);
        }

        _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
        throw Oops.Oh(ErrorCodeEnum.D1000);
    }

    /// <summary>
    /// Verify lock screen password 🔖
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    [DisplayName("Verify lock screen password")]
    public virtual async Task<bool> UnLockScreen([Required, FromQuery] string password)
    {
        // Does the account exist?
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == _userManager.UserId);
        _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        var passwordErrorTimes = _sysCacheService.Get<int>(keyPasswordErrorTimes);

        // Whether to enable domain login verification
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysDomainLogin))
        {
            var userLdap = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserLdap>>().GetFirstAsync(u => u.UserId == user.Id && u.TenantId == user.TenantId);
            if (userLdap == null)
            {
                VerifyPassword(password, keyPasswordErrorTimes, passwordErrorTimes, user);
            }
            else if (!await _sysLdapService.AuthAccount(user.TenantId!.Value, userLdap.Account, CryptogramUtil.Decrypt(password)))
            {
                _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
                throw Oops.Oh(ErrorCodeEnum.D1000);
            }
        }
        else
            VerifyPassword(password, keyPasswordErrorTimes, passwordErrorTimes, user);

        return true;
    }

    /// <summary>
    /// Login with mobile phone number 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Login with phone number")]
    public virtual async Task<LoginOutput> LoginPhone([Required] LoginPhoneInput input)
    {
        // Verify SMS verification code
        _sysSmsService.VerifyCode(new SmsVerifyCodeInput { Phone = input.Phone, Code = input.Code });

        // Get login tenant and user
        var (_, user) = await GetLoginUserAndTenant(input.TenantId, phone: input.Phone);

        return await CreateToken(user);
    }

    /// <summary>
    /// Generate Token 🔖
    /// </summary>
    /// <param name="user"></param>\
    /// <param name="sysUserEventTypeEnum"></param>\
    /// <returns></returns>
    [NonAction]
    internal virtual async Task<LoginOutput> CreateToken(SysUser user, SysUserEventTypeEnum sysUserEventTypeEnum = SysUserEventTypeEnum.Login)
    {
        // Single user login
        await _sysOnlineUserService.SingleLogin(user.Id);

        // Generate Token token
        var tokenExpire = await _sysConfigService.GetTokenExpire();
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
        {
            { ClaimConst.UserId, user.Id },
            { ClaimConst.TenantId, user.TenantId },
            { ClaimConst.Account, user.Account },
            { ClaimConst.RealName, user.RealName },
            { ClaimConst.AccountType, user.AccountType },
            { ClaimConst.OrgId, user.OrgId },
            { ClaimConst.OrgName, user.SysOrg?.Name },
            { ClaimConst.OrgType, user.SysOrg?.Type },
            { ClaimConst.LangCode, user.LangCode }
        }, tokenExpire);

        // Generate refresh token
        var refreshTokenExpire = await _sysConfigService.GetRefreshTokenExpire();
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);

        // Set response header
        _httpContextAccessor.HttpContext.SetTokensOfResponseHeaders(accessToken, refreshToken);

        // Swagger Knife4UI-AfterScript login script
        // ke.global.setAllHeader('Authorization', 'Bearer ' + ke.response.headers['access-token']);

        // Update user login information
        user.LastLoginIp = _httpContextAccessor.HttpContext.GetRemoteIpAddressToIPv4(true);
        (user.LastLoginAddress, double? longitude, double? latitude) = CommonUtil.GetIpAddress(user.LastLoginIp);
        user.LastLoginTime = DateTime.Now;
        user.LastLoginDevice = CommonUtil.GetClientDeviceInfo(_httpContextAccessor.HttpContext?.Request?.Headers?.UserAgent);
        await _sysUserRep.AsUpdateable(user).UpdateColumns(u => new
        {
            u.LastLoginIp,
            u.LastLoginAddress,
            u.LastLoginTime,
            u.LastLoginDevice,
        }).ExecuteCommandAsync();

        var payload = new
        {
            Entity = user,
            Output = new LoginOutput
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Homepage = user.Homepage
            }
        };

        // Publish system user operation events
        await _eventPublisher.PublishAsync(sysUserEventTypeEnum, payload);
        return payload.Output;
    }

    /// <summary>
    /// Get login account 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get login account")]
    public virtual async Task<LoginUserOutput> GetUserInfo()
    {
        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Id == _userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D1011).StatusCode(401);
        // Get the institution
        var org = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysOrg>>().GetFirstAsync(u => u.Id == user.OrgId);
        // Get job
        var pos = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysPos>>().GetFirstAsync(u => u.Id == user.PosId);
        // Get button collection
        var buttons = await _sysMenuService.GetOwnBtnPermList();
        // Get role collection
        var roleIds = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserRole>>().AsQueryable()
            .Where(u => u.UserId == user.Id).Select(u => u.RoleId).ToListAsync();
        // Get the watermark text (if the system watermark is empty, the global watermark will be empty)
        var watermarkText = (await _sysUserRep.Context.Queryable<SysTenant>().FirstAsync(u => u.Id == user.TenantId))?.Watermark;
        if (!string.IsNullOrWhiteSpace(watermarkText)) watermarkText += $"-{user.RealName}";
        var loginUser = new LoginUserOutput
        {
            Id = user.Id,
            Account = user.Account,
            RealName = user.RealName,
            Phone = user.Phone,
            IdCardNum = user.IdCardNum,
            Email = user.Email,
            AccountType = user.AccountType,
            Avatar = user.Avatar,
            Address = user.Address,
            Signature = user.Signature,
            OrgId = user.OrgId,
            OrgName = org?.Name,
            OrgType = org?.Type,
            PosName = pos?.Name,
            Buttons = buttons,
            RoleIds = roleIds,
            TenantId = user.TenantId,
            WatermarkText = watermarkText,
            LangCode = user.LangCode,
        };

        //Update the current tenant ID in the login information to the tenant currently switched to
        long? currentTenantId = App.User.FindFirst(ClaimConst.TenantId)?.Value?.ToLong(0);
        loginUser.CurrentTenantId = currentTenantId > 0 ? currentTenantId : user.TenantId;

        return loginUser;
    }

    /// <summary>
    /// Get refresh token 🔖
    /// </summary>
    /// <param name="accessToken">oldAccessToken</param>
    /// <returns>New AccessToken and RefreshToken</returns>
    [DisplayName("Get Refresh Token")]
    public virtual async Task<LoginOutput> GetRefreshToken([FromQuery] string accessToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw Oops.Oh(ErrorCodeEnum.D1016);

        if (string.IsNullOrWhiteSpace(accessToken)) throw Oops.Oh(ErrorCodeEnum.D1011);

        if (string.IsNullOrWhiteSpace(_userManager.Account)) throw Oops.Oh(ErrorCodeEnum.D1011);

        // Blacklist verification
        if (_sysCacheService.ExistKey($"blacklist:token:{accessToken}")) throw Oops.Oh(ErrorCodeEnum.D1011);

        // Parse Token
        var (isValid, tokenData, validationResult) = JWTEncryption.Validate(accessToken);
        if (!isValid) throw Oops.Oh(ErrorCodeEnum.D1016);

        // Get userId
        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Id == _userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D1011).StatusCode(401);
        return await CreateToken(user, SysUserEventTypeEnum.RefreshToken);
    }

    /// <summary>
    /// Exit the system 🔖
    /// </summary>
    [DisplayName("Exit the system")]
    public async Task Logout()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw Oops.Oh(ErrorCodeEnum.D1016);

        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
            throw Oops.Oh(ErrorCodeEnum.D1011);

        if (string.IsNullOrWhiteSpace(_userManager.Account))
            throw Oops.Oh(ErrorCodeEnum.D1011);

        // Write to the blacklist (set expiration time to avoid Redis expansion)
        var tokenExpire = await _sysConfigService.GetTokenExpire();
        _sysCacheService.Set($"blacklist:token:{token}", "1", TimeSpan.FromMinutes(tokenExpire));

        // Post a logout event (user exits)
        var user = await _sysUserRep.GetByIdAsync(_userManager.UserId);
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.LoginOut, new { Entity = user });

        // Clear Swagger login information
        httpContext.SignoutToSwagger();
    }

    /// <summary>
    /// Get verification code 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [SuppressMonitor]
    [DisplayName("ObtainVerification Code")]
    public dynamic GetCaptcha()
    {
        var codeId = YitIdHelper.NextId().ToString();
        var captcha = _captcha.Generate(codeId);
        var expirySeconds = App.GetOptions<CaptchaOptions>()?.ExpirySeconds ?? 60;
        return new { Id = codeId, Img = captcha.Base64, ExpirySeconds = expirySeconds };
    }

    /// <summary>
    /// User registration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [AllowAnonymous]
    [HttpPost, ApiDescriptionSettings(Description = "User Registration", DisableInherite = true)]
    public async Task UserRegistration(UserRegistrationInput input)
    {
        // Verify verification code
        if (!_captcha.Validate(input.CodeId.ToString(), input.Code)) throw Oops.Oh(ErrorCodeEnum.D0008);
        _captcha.Generate(input.CodeId.ToString());

        // Hide the tenant when logging in and find the corresponding tenant information
        input.TenantId = input.TenantId <= 0 ? (await _sysTenantService.GetCurrentTenantSysInfo()).Id : input.TenantId;

        // Determine whether the tenant is valid and the registration function is enabled
        var tenant = await _sysUserRep.Context.Queryable<SysTenant>().FirstAsync(u => u.Id == input.TenantId && u.Status == StatusEnum.Enable);
        if (tenant?.EnableReg != YesNoEnum.Y) throw Oops.Oh(ErrorCodeEnum.D1034);

        // Find a registration plan
        var wayId = input.WayId <= 0 ? tenant.RegWayId : input.WayId;
        var regWay = await _sysUserRep.Context.Queryable<SysUserRegWay>().FirstAsync(u => u.Id == wayId) ?? throw Oops.Oh(ErrorCodeEnum.D1035);

        var addUserInput = new AddUserInput
        {
            AccountType = regWay.AccountType,
            NickName = "Registered User -" + input.Account,
            OrgId = regWay.OrgId,
            PosId = regWay.PosId,
            TenantId = input.TenantId,
            RoleIdList = new List<long> { regWay.RoleId },
        };
        addUserInput.Copy(input);
        await _sysUserService.RegisterUser(addUserInput);
    }

    /// <summary>
    /// Swagger login check 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/api/swagger/checkUrl"), NonUnify]
    [ApiDescriptionSettings(Description = "Swagger login check", DisableInherite = true)]
    public int SwaggerCheckUrl()
    {
        return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? 200 : 401;
    }

    /// <summary>
    /// Swagger login submission 🔖
    /// </summary>
    /// <param name="auth"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/api/swagger/submitUrl"), NonUnify]
    [ApiDescriptionSettings(Description = "Swagger Login Submission", DisableInherite = true)]
    public async Task<int> SwaggerSubmitUrl([FromForm] SpecificationAuth auth)
    {
        try
        {
            _sysCacheService.Set($"{CacheConst.KeyConfig}{ConfigConst.SysCaptcha}", false);

            await Login(new LoginInput
            {
                Account = auth.UserName,
                Password = CryptogramUtil.SM2Encrypt(auth.Password)
            });

            _sysCacheService.Remove($"{CacheConst.KeyConfig}{ConfigConst.SysCaptcha}");

            return 200;
        }
        catch (Exception)
        {
            return 401;
        }
    }
}