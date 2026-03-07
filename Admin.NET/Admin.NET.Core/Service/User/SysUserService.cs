// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System User Services 🧩
/// </summary>
[ApiDescriptionSettings(Order = 490)]
public class SysUserService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SysOrgService _sysOrgService;
    private readonly SysUserExtOrgService _sysUserExtOrgService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysUserMenuService _sysUserMenuService;
    private readonly SysCacheService _sysCacheService;
    private readonly SysUserLdapService _sysUserLdapService;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly IEventPublisher _eventPublisher;

    public SysUserService(UserManager userManager,
        SysOrgService sysOrgService,
        SysUserExtOrgService sysUserExtOrgService,
        SysUserRoleService sysUserRoleService,
        SysConfigService sysConfigService,
        SysOnlineUserService sysOnlineUserService,
        SysCacheService sysCacheService,
        SysUserLdapService sysUserLdapService,
        SqlSugarRepository<SysUser> sysUserRep,
        SysUserMenuService sysUserMenuService,
        IEventPublisher eventPublisher)
    {
        _userManager = userManager;
        _sysOrgService = sysOrgService;
        _sysUserExtOrgService = sysUserExtOrgService;
        _sysUserRoleService = sysUserRoleService;
        _sysConfigService = sysConfigService;
        _sysOnlineUserService = sysOnlineUserService;
        _sysCacheService = sysCacheService;
        _sysUserLdapService = sysUserLdapService;
        _sysUserMenuService = sysUserMenuService;
        _sysUserRep = sysUserRep;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Get user paginated list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get user paginated list")]
    public virtual async Task<SqlSugarPagedList<UserOutput>> Page(PageUserInput input)
    {
        //Get the child node ID collection (including itself)
        var orgList = await _sysOrgService.GetChildIdListWithSelfById(input.OrgId);

        return await _sysUserRep.AsQueryable()
            .LeftJoin<SysOrg>((u, a) => u.OrgId == a.Id)
            .LeftJoin<SysPos>((u, a, b) => u.PosId == b.Id)
            .Where(u => u.AccountType != AccountTypeEnum.SuperAdmin)
            .WhereIF(input.OrgId > 0, u => orgList.Contains(u.OrgId))
            .WhereIF(!_userManager.SuperAdmin, u => u.AccountType != AccountTypeEnum.SysAdmin)
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account.Contains(input.Account))
            .WhereIF(!string.IsNullOrWhiteSpace(input.RealName), u => u.RealName.Contains(input.RealName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PosName), (u, a, b) => b.Name.Contains(input.PosName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), u => u.Phone.Contains(input.Phone))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .Select((u, a, b) => new UserOutput
            {
                OrgName = a.Name,
                PosName = b.Name,
                RoleName = SqlFunc.Subqueryable<SysUserRole>().LeftJoin<SysRole>((m, n) => m.RoleId == n.Id).Where(m => m.UserId == u.Id).SelectStringJoin((m, n) => n.Name, ","),
                DomainAccount = SqlFunc.Subqueryable<SysUserLdap>().Where(m => m.UserId == u.Id).Select(m => m.Account)
            }, true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Add users 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("increaseUser")]
    public virtual async Task<long> AddUser(AddUserInput input)
    {
        var isExist = await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.Account);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1003);

        if (!string.IsNullOrWhiteSpace(input.Phone) && await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Phone == input.Phone))
            throw Oops.Oh(ErrorCodeEnum.D1032);

        // It is prohibited to add super administrators and system administrators beyond their authority
        if (_userManager.AccountType != AccountTypeEnum.SuperAdmin && input.AccountType is AccountTypeEnum.SuperAdmin or AccountTypeEnum.SysAdmin) throw Oops.Oh(ErrorCodeEnum.D1038);

        // If no password is set, the default password will be used.
        var password = !string.IsNullOrWhiteSpace(input.Password) ? input.Password : await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);
        var user = input.Adapt<SysUser>();
        user.Password = CryptogramUtil.Encrypt(password);
        var newUser = await _sysUserRep.AsInsertable(user).ExecuteReturnEntityAsync();

        input.Id = newUser.Id;
        await UpdateRoleAndExtOrg(input);

        // Add domain account
        if (!string.IsNullOrWhiteSpace(input.DomainAccount))
            await _sysUserLdapService.AddUserLdap(newUser.TenantId!.Value, newUser.Id, newUser.Account, input.DomainAccount);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.Add, new
        {
            Entity = newUser,
            Input = input
        });

        return newUser.Id;
    }

    /// <summary>
    /// Registered user 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [NonAction]
    public virtual async Task<long> RegisterUser(AddUserInput input)
    {
        var isExist = await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.Account);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1003);

        if (!string.IsNullOrWhiteSpace(input.Phone) && await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Phone == input.Phone))
            throw Oops.Oh(ErrorCodeEnum.D1032);

        // Unauthorized registration is prohibited
        if (input.AccountType is AccountTypeEnum.SuperAdmin or AccountTypeEnum.SysAdmin) throw Oops.Oh(ErrorCodeEnum.D1038);

        if (string.IsNullOrWhiteSpace(input.Password))
        {
            var password = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);
            input.Password = CryptogramUtil.Encrypt(password);
        }

        var user = input.Adapt<SysUser>();
        var newUser = await _sysUserRep.AsInsertable(user).ExecuteReturnEntityAsync();

        input.Id = newUser.Id;
        await UpdateRoleAndExtOrg(input);

        // Add domain account
        if (!string.IsNullOrWhiteSpace(input.DomainAccount))
            await _sysUserLdapService.AddUserLdap(newUser.TenantId!.Value, newUser.Id, newUser.Account, input.DomainAccount);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.Register, new
        {
            Entity = newUser,
            Input = input
        });

        return newUser.Id;
    }

    /// <summary>
    /// Update user 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update user")]
    public virtual async Task UpdateUser(UpdateUserInput input)
    {
        if (await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.Account && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D1003);

        if (!string.IsNullOrWhiteSpace(input.Phone) && await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Phone == input.Phone && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D1032);

        // It is prohibited to update super administrator or system administrator information without permission.
        if (_userManager.AccountType != AccountTypeEnum.SuperAdmin && input.AccountType is AccountTypeEnum.SuperAdmin or AccountTypeEnum.SysAdmin) throw Oops.Oh(ErrorCodeEnum.D1038);

        await _sysUserRep.AsUpdateable(input.Adapt<SysUser>()).IgnoreColumns(true)
            .IgnoreColumns(u => new { u.Password, u.Status, u.TenantId }).ExecuteCommandAsync();

        await UpdateRoleAndExtOrg(input);

        // Delete user organization cache
        SqlSugarFilter.DeleteUserOrgCache(input.Id, _sysUserRep.Context.CurrentConnectionConfig.ConfigId.ToString());

        // If the account's role and organizational structure change, the account will be forced to go offline for permission update.
        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Id == input.Id);
        var roleIds = await GetOwnRoleList(input.Id);
        if (input.OrgId != user.OrgId || !input.RoleIdList.OrderBy(u => u).SequenceEqual(roleIds.OrderBy(u => u)))
            await _sysOnlineUserService.ForceOffline(input.Id);
        // Update domain account
        await _sysUserLdapService.AddUserLdap(user.TenantId!.Value, user.Id, user.Account, input.DomainAccount);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.Update, new
        {
            Entity = user,
            Input = input
        });
    }

    /// <summary>
    /// Update current user language 🔖
    /// </summary>
    /// <param name="langCode"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SetLangCode"), HttpPost]
    [DisplayName("Update current user's language")]
    public virtual async Task SetLangCode(string langCode)
    {
        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Id == _userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D1011).StatusCode(401);
        user.LangCode = langCode;
        await _sysUserRep.AsUpdateable(user).UpdateColumns(it => it.LangCode).ExecuteCommandAsync();
    }

    /// <summary>
    /// Updated roles and expanded organizations
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task UpdateRoleAndExtOrg(AddUserInput input)
    {
        await GrantRole(new UserRoleInput { UserId = input.Id, RoleIdList = input.RoleIdList });

        await _sysUserExtOrgService.UpdateUserExtOrg(input.Id, input.ExtOrgIdList);
    }

    /// <summary>
    /// Delete user 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete User")]
    public virtual async Task DeleteUser(DeleteUserInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        user.ValidateIsSuperAdminAccountType();
        user.ValidateIsUserId(_userManager.UserId);

        // If the account is the tenant's default account, deletion is prohibited.
        var isTenantUser = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().IsAnyAsync(u => u.UserId == input.Id);
        if (isTenantUser) throw Oops.Oh(ErrorCodeEnum.D1029);

        // If the account is bound to an open interface, deletion is prohibited.
        var isOpenAccessUser = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysOpenAccess>>().IsAnyAsync(u => u.BindUserId == input.Id);
        if (isOpenAccessUser) throw Oops.Oh(ErrorCodeEnum.D1030);

        // Forced offline
        await _sysOnlineUserService.ForceOffline(user.Id);

        await _sysUserRep.DeleteAsync(user);

        // Delete user role
        await _sysUserRoleService.DeleteUserRoleByUserId(input.Id);

        // Delete user extension organization
        await _sysUserExtOrgService.DeleteUserExtOrgByUserId(input.Id);

        // Delete domain account
        await _sysUserLdapService.DeleteUserLdapByUserId(input.Id);

        // Delete user favorites menu
        await _sysUserMenuService.DeleteUserMenuList(input.Id);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.Delete, new
        {
            Entity = user,
            Input = input
        });
    }

    /// <summary>
    /// View basic user information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("View user basic information")]
    public virtual async Task<SysUser> GetBaseInfo()
    {
        return await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(c => c.Id == _userManager.UserId);
    }

    /// <summary>
    /// Query user organization information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Query user organization information")]
    public virtual async Task<List<OrgTreeOutput>> GetOrgInfo()
    {
        return await _sysOrgService.GetTree(new OrgInput { Id = 0 });
    }

    /// <summary>
    /// Update basic user information 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BaseInfo"), HttpPost]
    [DisplayName("Update basic user information")]
    public virtual async Task<int> UpdateBaseInfo(SysUser user)
    {
        return await _sysUserRep.AsUpdateable(user)
            .IgnoreColumns(u => new { u.CreateTime, u.Account, u.Password, u.AccountType, u.OrgId, u.PosId }).ExecuteCommandAsync();
    }

    /// <summary>
    /// Set user status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Set user status")]
    public virtual async Task<int> SetStatus(UserInput input)
    {
        if (_userManager.UserId == input.Id)
            throw Oops.Oh(ErrorCodeEnum.D1026);

        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        user.ValidateIsSuperAdminAccountType(ErrorCodeEnum.D1015);
        if (!Enum.IsDefined(typeof(StatusEnum), input.Status))
            throw Oops.Oh(ErrorCodeEnum.D3005);

        // If the account is disabled, the blacklist will be added; if the account is enabled, the blacklist will be removed.
        var sysCacheService = App.GetRequiredService<SysCacheService>();
        if (input.Status == StatusEnum.Disable)
        {
            sysCacheService.Set($"{CacheConst.KeyBlacklist}{user.Id}", $"{user.RealName}-{user.Phone}");

            // Forced offline
            await _sysOnlineUserService.ForceOffline(user.Id);
        }
        else
        {
            sysCacheService.Remove($"{CacheConst.KeyBlacklist}{user.Id}");
        }

        user.Status = input.Status;
        var rows = await _sysUserRep.AsUpdateable(user).UpdateColumns(u => new { u.Status }).ExecuteCommandAsync();

        // Publish system user operation events
        if (rows > 0)
            await _eventPublisher.PublishAsync(SysUserEventTypeEnum.SetStatus, new
            {
                Entity = user,
                Input = input
            });

        return rows;
    }

    /// <summary>
    /// Authorized user roles 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Authorized user roles")]
    public async Task GrantRole(UserRoleInput input)
    {
        //var user = await _sysUserRep.GetFirstAsync(u => u.Id == input.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        //if (user.AccountType == AccountTypeEnum.SuperAdmin)
        //    throw Oops.Oh(ErrorCodeEnum.D1022);

        await _sysUserRoleService.GrantUserRole(input);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.UpdateRole, input);
    }

    /// <summary>
    /// Change user password 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Change user password")]
    public virtual async Task<int> ChangePwd(ChangePwdInput input)
    {
        // National secret SM2 decryption (front-end password transmission SM2 encrypted)
        input.PasswordOld = CryptogramUtil.SM2Decrypt(input.PasswordOld);
        input.PasswordNew = CryptogramUtil.SM2Decrypt(input.PasswordNew);

        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(c => c.Id == _userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
        {
            if (user.Password != MD5Encryption.Encrypt(input.PasswordOld))
                throw Oops.Oh(ErrorCodeEnum.D1004);
        }
        else
        {
            if (CryptogramUtil.Decrypt(user.Password) != input.PasswordOld)
                throw Oops.Oh(ErrorCodeEnum.D1004);
        }

        if (input.PasswordOld == input.PasswordNew)
            throw Oops.Oh(ErrorCodeEnum.D1028);

        // Verify password strength
        if (CryptogramUtil.StrongPassword)
        {
            user.Password = input.PasswordNew.TryValidate(CryptogramUtil.PasswordStrengthValidation)
                ? CryptogramUtil.Encrypt(input.PasswordNew)
                : throw Oops.Oh(CryptogramUtil.PasswordStrengthValidationMsg);
        }
        else
        {
            user.Password = CryptogramUtil.Encrypt(input.PasswordNew);
        }

        var rows = await _sysUserRep.AsUpdateable(user).UpdateColumns(u => u.Password).ExecuteCommandAsync();

        // Publish system user operation events
        if (rows > 0)
            await _eventPublisher.PublishAsync(SysUserEventTypeEnum.ChangePwd, new
            {
                Entity = user,
                Input = input
            });

        return rows;
    }

    /// <summary>
    /// Reset user password 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Reset user password")]
    public virtual async Task<string> ResetPwd(ResetPwdUserInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        string randomPassword = new(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 6).Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        user.Password = CryptogramUtil.Encrypt(randomPassword);
        await _sysUserRep.AsUpdateable(user).UpdateColumns(u => u.Password).ExecuteCommandAsync();

        // Clear the number of incorrect passwords
        var keyErrorPasswordCount = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        _sysCacheService.Remove(keyErrorPasswordCount);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.ResetPwd, new
        {
            Entity = user,
            Input = input
        });

        return randomPassword;
    }

    /// <summary>
    /// Unlock login 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Remove login lock")]
    public virtual async Task UnlockLogin(UnlockLoginInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        // Clear the number of incorrect passwords
        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        _sysCacheService.Remove(keyPasswordErrorTimes);

        // Publish system user operation events
        await _eventPublisher.PublishAsync(SysUserEventTypeEnum.UnlockLogin, new
        {
            Entity = user,
            Input = input
        });
    }

    /// <summary>
    /// Get the set of roles owned by the user 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [DisplayName("Get the set of roles owned by the user")]
    public async Task<List<long>> GetOwnRoleList(long userId)
    {
        return await _sysUserRoleService.GetUserRoleIdList(userId);
    }

    /// <summary>
    /// Get user extension organization collection 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [DisplayName("Get user extended organization collection")]
    public async Task<List<SysUserExtOrg>> GetOwnExtOrgList(long userId)
    {
        return await _sysUserExtOrgService.GetUserExtOrgList(userId);
    }
}