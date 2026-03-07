// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System tenant management service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 390)]
public class SysTenantService : IDynamicApiController, ITransient
{
    private static readonly SysMenuService SysMenuService = App.GetService<SysMenuService>();
    private readonly SqlSugarRepository<SysUserExtOrg> _sysUserExtOrgRep;
    private readonly SqlSugarRepository<SysTenantMenu> _sysTenantMenuRep;
    private readonly SqlSugarRepository<SysRoleMenu> _sysRoleMenuRep;
    private readonly SqlSugarRepository<SysUserRole> _userRoleRep;
    private readonly SqlSugarRepository<SysTenant> _sysTenantRep;
    private readonly SqlSugarRepository<SysRole> _sysRoleRep;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SqlSugarRepository<SysPos> _sysPosRep;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysCacheService _sysCacheService;
    private readonly UploadOptions _uploadOptions;

    public SysTenantService(
        SqlSugarRepository<SysUserExtOrg> sysUserExtOrgRep,
        SqlSugarRepository<SysTenantMenu> sysTenantMenuRep,
        SqlSugarRepository<SysRoleMenu> sysRoleMenuRep,
        SqlSugarRepository<SysUserRole> userRoleRep,
        SqlSugarRepository<SysTenant> sysTenantRep,
        SqlSugarRepository<SysUser> sysUserRep,
        SqlSugarRepository<SysRole> sysRoleRep,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SqlSugarRepository<SysPos> sysPosRep,
        IOptions<UploadOptions> uploadOptions,
        SysRoleMenuService sysRoleMenuService,
        SysConfigService sysConfigService,
        SysCacheService sysCacheService)
    {
        _sysTenantRep = sysTenantRep;
        _sysOrgRep = sysOrgRep;
        _sysRoleRep = sysRoleRep;
        _sysPosRep = sysPosRep;
        _sysUserRep = sysUserRep;
        _userRoleRep = userRoleRep;
        _sysRoleMenuRep = sysRoleMenuRep;
        _sysCacheService = sysCacheService;
        _uploadOptions = uploadOptions.Value;
        _sysConfigService = sysConfigService;
        _sysTenantMenuRep = sysTenantMenuRep;
        _sysUserExtOrgRep = sysUserExtOrgRep;
        _sysRoleMenuService = sysRoleMenuService;
    }

    /// <summary>
    /// Get paginated list of tenants 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get a paginated list of tenants")]
    public async Task<SqlSugarPagedList<TenantOutput>> Page(PageTenantInput input)
    {
        return await _sysTenantRep.AsQueryable()
            .LeftJoin<SysUser>((u, a) => u.UserId == a.Id).ClearFilter()
            .LeftJoin<SysOrg>((u, a, b) => u.OrgId == b.Id).ClearFilter()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), (u, a) => a.Phone.Contains(input.Phone.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), (u, a, b) => b.Name.Contains(input.Name.Trim()))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .Select((u, a, b) => new TenantOutput
            {
                Id = u.Id,
                OrgId = b.Id,
                Name = b.Name,
                UserId = a.Id,
                AdminAccount = a.Account,
                Phone = a.Phone,
                Host = u.Host,
                Email = a.Email,
                TenantType = u.TenantType,
                DbType = u.DbType,
                Connection = u.Connection,
                ConfigId = u.ConfigId,
                OrderNo = u.OrderNo,
                Remark = u.Remark,
                Status = u.Status,
                CreateTime = u.CreateTime,
                CreateUserName = u.CreateUserName,
                UpdateTime = u.UpdateTime,
                UpdateUserName = u.UpdateUserName,
            }, true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get tenant list
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Get tenant list"), HttpGet]
    public async Task<dynamic> GetList()
    {
        return await _sysTenantRep.AsQueryable()
           .LeftJoin<SysOrg>((u, a) => u.OrgId == a.Id).ClearFilter()
           .Where(u => u.Status == StatusEnum.Enable)
           .Select((u, a) => new
           {
               Label = SqlFunc.HasValue(u.Title) ? $"{u.Title}-{a.Name}" : a.Name,
               Host = u.Host.ToLower(),
               Value = u.Id,
           }).ToListAsync();
    }

    /// <summary>
    /// Get current tenant system information
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<SysTenant> GetCurrentTenantSysInfo()
    {
        var tenantId = long.Parse(App.User?.FindFirst(ClaimConst.TenantId)?.Value ?? "0");
        var host = App.HttpContext.Request.Host.Host.ToLower();
        var tenant = await _sysTenantRep.AsQueryable()
            .WhereIF(tenantId > 0, u => u.Id == tenantId && SqlFunc.ToLower(u.Host).Contains(host))
            .WhereIF(!(tenantId > 0), u => SqlFunc.ToLower(u.Host).Contains(host))
            .FirstAsync();
        tenant ??= await _sysTenantRep.GetFirstAsync(u => u.Id == SqlSugarConst.DefaultTenantId);
        _ = tenant ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return tenant;
    }

    /// <summary>
    /// Get the list of tenants isolated by the library
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysTenant>> GetTenantDbList()
    {
        return await _sysTenantRep.GetListAsync(u => u.TenantType == TenantTypeEnum.Db && u.Status == StatusEnum.Enable);
    }

    /// <summary>
    /// Add tenant 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add tenant")]
    public async Task AddTenant(AddTenantInput input)
    {
        var isExist = await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1300);

        input.Host = input.Host?.ToLower();
        isExist = await _sysTenantRep.IsAnyAsync(u => !string.IsNullOrWhiteSpace(u.Host) && u.Host == input.Host);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1303);

        isExist = await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.AdminAccount);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1301);

        // Judging from the library configuration
        if (!string.IsNullOrWhiteSpace(input.SlaveConnections) && !JSON.IsValid(input.SlaveConnections)) throw Oops.Oh(ErrorCodeEnum.D1302);

        switch (input.TenantType)
        {
            // When the ID is isolated, the settings are consistent with the main library.
            case TenantTypeEnum.Id:
                var config = _sysTenantRep.AsSugarClient().CurrentConnectionConfig;
                input.DbType = config.DbType;
                input.Connection = config.ConnectionString;
                break;

            case TenantTypeEnum.Db:
                if (string.IsNullOrWhiteSpace(input.Connection))
                    throw Oops.Oh(ErrorCodeEnum.Z1004);
                break;

            default:
                throw Oops.Oh(ErrorCodeEnum.D3004);
        }
        if (input.EnableReg == YesNoEnum.N) input.RegWayId = null;
        var tenant = input.Adapt<TenantOutput>();

        // Set logo
        SetLogoUrl(tenant, input.LogoBase64, input.LogoFileName);

        tenant.Id = _sysTenantRep.InsertReturnEntity(tenant).Id;
        await InitNewTenant(tenant);

        await CacheTenant();
    }

    /// <summary>
    /// Set logo
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="logoBase64"></param>
    /// <param name="logoFileName"></param>
    [NonAction]
    public void SetLogoUrl(SysTenant tenant, string logoBase64, string logoFileName)
    {
        if (string.IsNullOrEmpty(tenant?.Logo) && string.IsNullOrEmpty(tenant?.Logo)) return;

        // Old icon file relative path
        var oldSysLogoRelativeFilePath = tenant.Logo ?? "";
        var oldSysLogoAbsoluteFilePath = Path.Combine(App.WebHostEnvironment.WebRootPath, oldSysLogoRelativeFilePath.TrimStart('/'));

        var groups = Regex.Match(logoBase64, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups;

        //var type = groups["type"].Value;
        var base64Data = groups["data"].Value;
        var binData = Convert.FromBase64String(base64Data);

        // Get extension based on file name
        var ext = string.IsNullOrWhiteSpace(logoFileName) ? ".png" : Path.GetExtension(logoFileName);

        // Local icon saving path
        var fileName = $"{tenant.ViceTitle}-logo{ext}".ToLower();
        var path = _uploadOptions.Path.Replace("/{yyyy}/{MM}/{dd}", "");
        path = path.StartsWith("/") || Regex.IsMatch(path, "^[A-Z|a-z]:") ? path : Path.Combine(App.WebHostEnvironment.WebRootPath, path);
        var absoluteFilePath = Path.Combine(path, fileName);

        // Delete existing files
        if (File.Exists(oldSysLogoAbsoluteFilePath)) File.Delete(oldSysLogoAbsoluteFilePath);

        // Create folder
        var absoluteFileDir = Path.GetDirectoryName(absoluteFilePath);
        if (!Directory.Exists(absoluteFileDir)) Directory.CreateDirectory(absoluteFileDir);

        // Save icon file
        File.WriteAllBytesAsync(absoluteFilePath, binData);

        // Save icon configuration
        tenant.Logo = $"/upload/{fileName}";
    }

    /// <summary>
    /// Set tenant status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Set tenant status")]
    public async Task<int> SetStatus(TenantInput input)
    {
        var tenant = await _sysTenantRep.GetFirstAsync(u => u.Id == input.Id);
        if (tenant == null || tenant.ConfigId == SqlSugarConst.MainConfigId) throw Oops.Oh(ErrorCodeEnum.Z1001);

        if (!Enum.IsDefined(typeof(StatusEnum), input.Status)) throw Oops.Oh(ErrorCodeEnum.D3005);

        tenant.Status = input.Status;
        return await _sysTenantRep.AsUpdateable(tenant).UpdateColumns(u => new { u.Status }).ExecuteCommandAsync();
    }

    /// <summary>
    /// New tenant initialization
    /// </summary>
    /// <param name="tenant"></param>
    private async Task InitNewTenant(TenantOutput tenant)
    {
        var tenantId = tenant.Id;
        var tenantName = tenant.Name;

        // Initialization mechanism
        var newOrg = new SysOrg { TenantId = tenantId, Pid = 0, Name = tenantName, Code = tenantName, Remark = tenantName, };
        await _sysOrgRep.InsertAsync(newOrg);

        // Initialize default role
        var newRole = new SysRole { TenantId = tenantId, Name = CommonConst.DefaultBaseRoleName, Code = CommonConst.DefaultBaseRoleCode, DataScope = DataScopeEnum.Self, Remark = "This role is automatically created by the system" };
        var baseRole = await _sysRoleRep.InsertReturnEntityAsync(newRole);
        var baseRoleMenuIdList = GetBaseRoleMenuIdList().ToList();
        await _sysRoleMenuService.GrantRoleMenu(new RoleMenuInput { Id = baseRole.Id, MenuIdList = baseRoleMenuIdList.Select(u => u.MenuId).ToList() });

        // Initialize position
        var newPos = new SysPos { TenantId = tenantId, Name = "Administrator-" + tenantName, Code = tenantName, Remark = tenantName };
        await _sysPosRep.InsertAsync(newPos);

        // Initialize tenant administrator account
        var password = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);
        var newUser = new SysUser
        {
            TenantId = tenantId,
            Account = tenant.AdminAccount,
            Password = CryptogramUtil.Encrypt(password),
            NickName = "system administrator",
            Email = tenant.Email,
            Phone = tenant.Phone,
            AccountType = AccountTypeEnum.SysAdmin,
            OrgId = newOrg.Id,
            PosId = newPos.Id,
            Birthday = DateTime.Parse("2000-01-01"),
            RealName = "system administrator",
            Remark = "system administrator" + tenantName,
        };
        await _sysUserRep.InsertAsync(newUser);

        // Associate tenant organization and admin user
        await _sysTenantRep.UpdateAsync(u => new SysTenant { UserId = newUser.Id, OrgId = newOrg.Id }, u => u.Id == tenantId);

        // Default tenant administrator role menu collection
        var menuList = GetTenantDefaultMenuList().ToList();
        await GrantMenu(new TenantMenuInput { Id = tenantId, MenuIdList = menuList.Select(u => u.MenuId).ToList() });
    }

    /// <summary>
    /// Get tenant default menu
    /// </summary>
    /// <param name="ignoreHome">If a tenant needs to customize the homepage, you can ignore it.</param>
    /// <returns></returns>
    [NonAction]
    public IEnumerable<SysTenantMenu> GetTenantDefaultMenuList(bool ignoreHome = false)
    {
        var menuList = new List<SysMenu>();

        // Default database configuration
        var defaultConfig = App.GetOptions<DbConnectionOptions>().ConnectionConfigs.FirstOrDefault();
        //Get the seed menu data from the assembly. The seed menu exists in other class libraries and needs to be loaded dynamically.
        var menuSeedDataTypeList = GetSeedDataTypes(defaultConfig, nameof(SysMenuSeedData));
        var allMenuList = new List<SysMenu>();
        foreach (var menu in menuSeedDataTypeList)
        {
            var menuSeedDataList = ((IEnumerable)menu.GetMethod("HasData")?.Invoke(Activator.CreateInstance(menu), null))?.Cast<SysMenu>();
            if (menuSeedDataList != null)
            {
                allMenuList.AddRange(menuSeedDataList);
            }
        }

        //Implement a three-level menu
        var topMenuList = allMenuList.Where(u => u.Pid == 0 && u.Type == MenuTypeEnum.Dir).ToList();
        menuList.AddRange(topMenuList);

        var childMenuList = allMenuList.ToChildList(u => u.Id, u => u.Pid, u => topMenuList.Select(p => p.Id).Contains(u.Pid));
        menuList.AddRange(childMenuList);

        var endMenuList = allMenuList.ToChildList(u => u.Id, u => u.Pid, u => childMenuList.Select(p => p.Id).Contains(u.Pid));
        if (endMenuList != null)
        {
            menuList.AddRange(endMenuList);
        }
        //Do you need to exclude the homepage menu?
        if (ignoreHome) menuList = menuList.Where(u => !(u.Type == MenuTypeEnum.Menu && u.Name == "home")).ToList();

        menuList = menuList.Distinct().ToList();

        return menuList.Select(u => new SysTenantMenu
        {
            Id = CommonUtil.GetFixedHashCode("" + SqlSugarConst.DefaultTenantId + u.Id, 1300000000000),
            TenantId = SqlSugarConst.DefaultTenantId,
            MenuId = u.Id
        });
    }

    /// <summary>
    /// Get seed data type
    /// </summary>
    /// <param name="config">Database connection configuration</param>
    /// <param name="typeName"></param>
    /// <returns>List of seed data types</returns>
    [NonAction]
    private List<Type> GetSeedDataTypes(DbConnectionConfig config, string typeName)
    {
        return App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.Name == typeName && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))))
            .WhereIF(config.SeedSettings.EnableIncreSeed, u => u.IsDefined(typeof(IncreSeedAttribute), false))
            .OrderBy(u => u.GetCustomAttributes(typeof(SeedDataAttribute), false).Length > 0 ? ((SeedDataAttribute)u.GetCustomAttributes(typeof(SeedDataAttribute), false)[0]).Order : 0)
            .ToList();
    }

    /// <summary>
    /// Get tenant default menu
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public IEnumerable<SysTenantMenu> GetBaseRoleMenuIdList()
    {
        var menuList = new List<SysMenu>();
        var allMenuList = new SysMenuSeedData().HasData().ToList();

        var dashboardMenu = allMenuList.First(u => u.Type == MenuTypeEnum.Dir && u.Title == "Workbench");
        menuList.AddRange(allMenuList.ToChildList(u => u.Id, u => u.Pid, dashboardMenu.Id));

        var systemMenu = allMenuList.First(u => u.Type == MenuTypeEnum.Dir && u.Title == "System Management");
        menuList.Add(systemMenu);
        menuList.AddRange(allMenuList.ToChildList(u => u.Id, u => u.Pid, u => u.Pid == systemMenu.Id && new[] { "Organizational Management", "Personal Center" }.Contains(u.Title)));
        menuList = menuList.Where(u => !new[] { "increase", "Edit", "Delete" }.Contains(u.Title)).ToList();

        return menuList.Select(u => new SysTenantMenu
        {
            Id = CommonUtil.GetFixedHashCode("" + SqlSugarConst.DefaultTenantId + u.Id, 1300000000000),
            TenantId = SqlSugarConst.DefaultTenantId,
            MenuId = u.Id
        });
    }

    /// <summary>
    /// Delete tenant 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete tenant")]
    public async Task DeleteTenant(DeleteTenantInput input)
    {
        // Disable deletion of default tenant
        if (input.Id.ToString() == SqlSugarConst.MainConfigId) throw Oops.Oh(ErrorCodeEnum.D1023);

        // If the account is an open interface bound to a tenant, deletion is prohibited.
        var isOpenAccessTenant = await _sysTenantRep.ChangeRepository<SqlSugarRepository<SysOpenAccess>>().IsAnyAsync(u => u.BindTenantId == input.Id);
        if (isOpenAccessTenant) throw Oops.Oh(ErrorCodeEnum.D1031);

        await _sysTenantRep.DeleteAsync(u => u.Id == input.Id);

        await CacheTenant(input.Id);

        // Delete table data related to tenant
        await _sysTenantMenuRep.AsDeleteable().Where(u => u.TenantId == input.Id).ExecuteCommandAsync();
        await _sysTenantRep.Context.Deleteable<SysTenantConfigData>().Where(u => u.TenantId == input.Id).ExecuteCommandAsync();

        var users = await _sysUserRep.AsQueryable().ClearFilter().Where(u => u.TenantId == input.Id).ToListAsync();
        var userIds = users.Select(u => u.Id).ToList();
        await _sysUserRep.AsDeleteable().Where(u => userIds.Contains(u.Id)).ExecuteCommandAsync();
        await _userRoleRep.AsDeleteable().Where(u => userIds.Contains(u.UserId)).ExecuteCommandAsync();
        await _sysUserExtOrgRep.AsDeleteable().Where(u => userIds.Contains(u.UserId)).ExecuteCommandAsync();
        await _sysTenantRep.Context.Deleteable<SysUserMenu>().Where(u => userIds.Contains(u.UserId)).ExecuteCommandAsync();
        await _sysTenantRep.Context.Deleteable<SysUserConfigData>().Where(u => userIds.Contains(u.UserId)).ExecuteCommandAsync();

        var roleIds = await _sysRoleRep.AsQueryable().ClearFilter().Where(u => u.TenantId == input.Id).Select(u => u.Id).ToListAsync();
        await _sysRoleRep.AsDeleteable().Where(u => u.TenantId == input.Id).ExecuteCommandAsync();
        await _sysRoleMenuRep.AsDeleteable().Where(u => roleIds.Contains(u.RoleId)).ExecuteCommandAsync();
        await _sysTenantRep.Context.Deleteable<SysRoleOrg>().Where(u => roleIds.Contains(u.RoleId)).ExecuteCommandAsync();

        await _sysOrgRep.AsDeleteable().Where(u => u.TenantId == input.Id).ExecuteCommandAsync();

        await _sysPosRep.AsDeleteable().Where(u => u.TenantId == input.Id).ExecuteCommandAsync();
    }

    /// <summary>
    /// Update tenant 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update tenant")]
    public async Task UpdateTenant(UpdateTenantInput input)
    {
        var isExist = await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Id != input.OrgId);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1300);

        input.Host = input.Host?.ToLower();
        isExist = await _sysTenantRep.IsAnyAsync(u => !string.IsNullOrWhiteSpace(u.Host) && u.Host == input.Host && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1303);

        isExist = await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.AdminAccount && u.Id != input.UserId);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1301);

        // When the ID is isolated, the settings are consistent with the main library.
        switch (input.TenantType)
        {
            case TenantTypeEnum.Id:
                var config = _sysTenantRep.AsSugarClient().CurrentConnectionConfig;
                input.DbType = config.DbType;
                input.Connection = config.ConnectionString;
                break;

            case TenantTypeEnum.Db:
                if (string.IsNullOrWhiteSpace(input.Connection))
                    throw Oops.Oh(ErrorCodeEnum.Z1004);
                break;

            default:
                throw Oops.Oh(ErrorCodeEnum.D3004);
        }
        // Judging from the library configuration
        if (!string.IsNullOrWhiteSpace(input.SlaveConnections) && !JSON.IsValid(input.SlaveConnections))
            throw Oops.Oh(ErrorCodeEnum.D1302);

        // Set logo
        var tenant = input.Adapt<SysTenant>();
        if (!string.IsNullOrWhiteSpace(input.LogoBase64)) SetLogoUrl(tenant, input.LogoBase64, input.LogoFileName);

        // Update tenant information
        await _sysTenantRep.AsUpdateable(tenant).IgnoreColumns(true).ExecuteCommandAsync();

        // Update system organization
        await _sysOrgRep.UpdateAsync(u => new SysOrg() { Name = input.Name }, u => u.Id == input.OrgId);

        // Update system user
        await _sysUserRep.UpdateAsync(u => new SysUser() { Account = input.AdminAccount, Phone = input.Phone, Email = input.Email }, u => u.Id == input.UserId);

        await CacheTenant(input.Id);
    }

    /// <summary>
    /// Authorized Tenant Menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Authorized Tenant Menu")]
    public async Task GrantMenu(TenantMenuInput input)
    {
        // Get a list of menus that require authorization
        var menuList = await _sysTenantRep.Context.Queryable<SysMenu>()
            .Where(u => input.MenuIdList.Contains(u.Id))
            .InnerJoin<SysTenantMenu>((u, t) => t.TenantId == input.Id && u.Id == t.MenuId)
            .ToListAsync();

        // Check if duplicate menu exists
        if (menuList.Where(u => u.Type != MenuTypeEnum.Btn).GroupBy(u => new { u.Pid, u.Title }).Any(u => u.Count() > 1) ||
            menuList.Where(u => u.Type == MenuTypeEnum.Btn).GroupBy(u => u.Permission).Any(u => u.Count() > 1))
            throw Oops.Oh(ErrorCodeEnum.D1304);

        // Check if routes are duplicated
        if (menuList.Where(u => !string.IsNullOrWhiteSpace(u.Name)).GroupBy(u => u.Name).Any(u => u.Count() > 1))
            throw Oops.Oh(ErrorCodeEnum.D4009);

        //Obtain the default tenant authorization menu, and the seed data primary key ID remains unchanged to prevent duplication.
        var tenantMenuList = input.Id == SqlSugarConst.DefaultTenantId ? await _sysTenantMenuRep.AsQueryable().Where(u => u.TenantId == input.Id).ToListAsync() : null;

        List<long> tenantIdList = [input.Id];
        if (input.TenantIdList?.Count > 0) tenantIdList.AddRange(input.TenantIdList);
        // Delete old records
        await _sysTenantMenuRep.AsDeleteable().Where(u => tenantIdList.Contains(u.TenantId)).ExecuteCommandAsync();

        // Append parent menu
        var allIdList = await _sysTenantRep.Context.Queryable<SysMenu>().Select(u => new { u.Id, u.Pid }).ToListAsync();
        var pIdList = allIdList.ToChildList(u => u.Pid, u => u.Id, u => input.MenuIdList.Contains(u.Id)).Select(u => u.Pid).Distinct().ToList();
        input.MenuIdList = input.MenuIdList.Concat(pIdList).Distinct().Where(u => u != 0).ToList();

        // Save tenant menu
        List<SysTenantMenu> sysTenantMenuList = new();
        tenantIdList.ForEach(tenantId =>
        {
            sysTenantMenuList.AddRange(input.MenuIdList.Select(menuId => new SysTenantMenu { TenantId = tenantId, MenuId = menuId }));
        });

        //The default tenant authorization menu primary key ID remains unchanged
        foreach (var item in sysTenantMenuList)
        {
            var tenantMenu = tenantMenuList.FirstOrDefault(u => u.TenantId == item.TenantId && u.MenuId == item.MenuId);
            if (tenantMenu != null) item.Id = tenantMenu.Id;
        }
        await _sysTenantMenuRep.InsertRangeAsync(sysTenantMenuList);

        // Clear menu permission cache
        SysMenuService.DeleteMenuCache();
    }

    /// <summary>
    /// Get tenant menu ID collection 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the tenant menu ID collection")]
    public async Task<List<long>> GetTenantMenuList([FromQuery] BaseIdInput input)
    {
        var menuIds = await _sysTenantMenuRep.AsQueryable().Where(u => u.TenantId == input.Id).Select(u => u.MenuId).ToListAsync();
        return await SysMenuService.ExcludeParentMenuOfFullySelected(menuIds);
    }

    /// <summary>
    /// Reset tenant administrator password 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("resetTenant managementmemberpassword")]
    public async Task<string> ResetPwd(TenantUserInput input)
    {
        var password = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);
        var encryptPassword = CryptogramUtil.Encrypt(password);
        await _sysUserRep.UpdateAsync(u => new SysUser { Password = encryptPassword }, u => u.Id == input.UserId);
        return password;
    }

    /// <summary>
    /// Switch tenant 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Switch Tenant")]
    public async Task<LoginOutput> ChangeTenant(BaseIdInput input)
    {
        var userId = (App.HttpContext?.User.FindFirst(ClaimConst.UserId)?.Value)?.ToLong();
        _ = await _sysTenantRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == userId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        user.TenantId = input.Id;

        return await GetAccessTokenInNotSingleLogin(user);
    }

    /// <summary>
    /// Enter the rental management terminal 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Enter the rental management terminal")]
    public async Task<LoginOutput> GoTenant(BaseIdInput input)
    {
        var tenant = await _sysTenantRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == tenant.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return await GetAccessTokenInNotSingleLogin(user);
    }

    /// <summary>
    /// Synchronize authorization menu (used to synchronize authorization data after version update) 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Synchronous Authorization Menu")]
    public async Task SyncGrantMenu(BaseIdInput input)
    {
        var menuIdList = input.Id == SqlSugarConst.DefaultTenantId
            ? new SysMenuSeedData().HasData().Select(u => u.Id).ToList()
            : await _sysRoleRep.AsQueryable().ClearFilter()
              .InnerJoin<SysTenant>((u, t) => t.Id == input.Id && u.TenantId == t.Id)
              .InnerJoin<SysRoleMenu>((u, t, rm) => u.Id == rm.RoleId)
              .Select((u, t, rm) => rm.MenuId)
              .Distinct()
              .ToListAsync() ?? throw Oops.Oh(ErrorCodeEnum.D1019);
        var adminRole = await _sysRoleRep.AsQueryable().ClearFilter().FirstAsync(u => u.TenantId == input.Id && u.Code == "sys_admin");
        if (adminRole != null)
        {
            await _sysRoleRep.Context.Deleteable<SysUserRole>().Where(u => u.RoleId == adminRole.Id).ExecuteCommandAsync();
            await App.GetService<SysRoleService>().DeleteRole(new DeleteRoleInput { Id = adminRole.Id });
        }
        await GrantMenu(new TenantMenuInput { Id = input.Id, MenuIdList = menuIdList });
    }

    /// <summary>
    /// Obtain login token in non-single-user login mode
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<LoginOutput> GetAccessTokenInNotSingleLogin(SysUser user)
    {
        // Log in using non-single-user mode
        var singleLogin = _sysCacheService.Get<bool>($"{CacheConst.KeyConfig}{ConfigConst.SysSingleLogin}");
        try
        {
            _sysCacheService.Set($"{CacheConst.KeyConfig}{ConfigConst.SysSingleLogin}", false);
            return await App.GetService<SysAuthService>().CreateToken(user);
        }
        finally
        {
            // Restore single-user login parameters
            if (singleLogin) _sysCacheService.Set($"{CacheConst.KeyConfig}{ConfigConst.SysSingleLogin}", true);
        }
    }

    /// <summary>
    /// Cache all tenants
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task CacheTenant(long tenantId = 0)
    {
        // Remove library connection in ISqlSugarClient and exclude default main library
        if (tenantId > 0 && tenantId.ToString() != SqlSugarConst.MainConfigId)
            _sysTenantRep.AsTenant().RemoveConnection(tenantId);

        var tenantList = await _sysTenantRep.GetListAsync();

        // SM2 encryption for tenant library connections
        foreach (var tenant in tenantList.Where(tenant => !string.IsNullOrWhiteSpace(tenant.Connection)))
            tenant.Connection = CryptogramUtil.SM2Encrypt(tenant.Connection);

        _sysCacheService.Set(CacheConst.KeyTenant, tenantList);
    }

    /// <summary>
    /// Create tenant database 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "CreateDb"), HttpPost]
    [DisplayName("Create tenant database")]
    public async Task CreateDb(TenantInput input)
    {
        var tenant = await _sysTenantRep.GetSingleAsync(u => u.Id == input.Id);
        if (tenant == null) return;

        if (tenant.DbType == SqlSugar.DbType.Oracle)
            throw Oops.Oh(ErrorCodeEnum.Z1002);

        if (string.IsNullOrWhiteSpace(tenant.Connection) || tenant.Connection.Length < 10)
            throw Oops.Oh(ErrorCodeEnum.Z1004);

        // Default database configuration
        var defaultConfig = App.GetOptions<DbConnectionOptions>().ConnectionConfigs.FirstOrDefault();

        var config = new DbConnectionConfig
        {
            ConfigId = tenant.Id.ToString(),
            DbType = tenant.DbType,
            ConnectionString = tenant.Connection,
            DbSettings = new DbSettings()
            {
                EnableInitDb = true,
                EnableDiffLog = false,
                EnableUnderLine = defaultConfig!.DbSettings.EnableUnderLine,
            }
        };
        SqlSugarSetup.InitTenantDatabase(App.GetRequiredService<ISqlSugarClient>().AsTenant(), config);
    }

    /// <summary>
    /// Get the user list under the tenant 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the user list under the tenant")]
    public async Task<List<SysUser>> UserList(TenantIdInput input)
    {
        return await _sysUserRep.AsQueryable().ClearFilter().Where(u => u.TenantId == input.TenantId).ToListAsync();
    }

    /// <summary>
    /// Get tenant database connection
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public SqlSugarScopeProvider GetTenantDbConnectionScope(long tenantId)
    {
        var iTenant = _sysTenantRep.AsTenant();

        // If there is already a tenant library connection, return directly
        if (iTenant.IsAnyConnection(tenantId.ToString())) return iTenant.GetConnectionScope(tenantId.ToString());

        lock (iTenant)
        {
            // Get tenant information from cache
            var tenant = _sysCacheService.Get<List<SysTenant>>(CacheConst.KeyTenant)?.FirstOrDefault(u => u.Id == tenantId);
            if (tenant == null || tenant.TenantType == TenantTypeEnum.Id) return null;

            // Get the default library connection configuration
            var dbOptions = App.GetOptions<DbConnectionOptions>();
            var mainConnConfig = dbOptions.ConnectionConfigs.First(u => u.ConfigId.ToString() == SqlSugarConst.MainConfigId);

            // Set tenant library connection configuration
            var tenantConnConfig = new DbConnectionConfig
            {
                ConfigId = tenant.Id.ToString(),
                DbType = tenant.DbType,
                TenantType = tenant.TenantType,
                IsAutoCloseConnection = true,
                ConnectionString = CryptogramUtil.SM2Decrypt(tenant.Connection), // SM2 decryption of tenant library connections
                DbSettings = new DbSettings()
                {
                    EnableUnderLine = mainConnConfig.DbSettings.EnableUnderLine,
                },
                SlaveConnectionConfigs = JSON.IsValid(tenant.SlaveConnections) ? JSON.Deserialize<List<SlaveConnectionConfig>>(tenant.SlaveConnections) : null // Slave library connection configuration
            };
            iTenant.AddConnection(tenantConnConfig);

            var sqlSugarScopeProvider = iTenant.GetConnectionScope(tenantId.ToString());
            SqlSugarSetup.SetDbConfig(tenantConnConfig);
            SqlSugarSetup.SetDbAop(sqlSugarScopeProvider, dbOptions.EnableConsoleSql, dbOptions.SuperAdminIgnoreIDeletedFilter);

            return sqlSugarScopeProvider;
        }
    }
}