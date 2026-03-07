// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System menu service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 450)]
public class SysMenuService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysTenantMenu> _sysTenantMenuRep;
    private readonly SqlSugarRepository<SysMenu> _sysMenuRep;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysUserMenuService _sysUserMenuService;
    private readonly SysCacheService _sysCacheService;
    private readonly UserManager _userManager;
    private readonly SysLangTextCacheService _sysLangTextCacheService;
    private readonly SysLangTextService _sysLangTextService;

    public SysMenuService(
        SqlSugarRepository<SysTenantMenu> sysTenantMenuRep,
        SqlSugarRepository<SysMenu> sysMenuRep,
        SysRoleMenuService sysRoleMenuService,
        SysUserRoleService sysUserRoleService,
        SysUserMenuService sysUserMenuService,
        SysCacheService sysCacheService,
        UserManager userManager,
        SysLangTextCacheService sysLangTextCacheService,
        SysLangTextService sysLangTextService)
    {
        _userManager = userManager;
        _sysMenuRep = sysMenuRep;
        _sysRoleMenuService = sysRoleMenuService;
        _sysUserRoleService = sysUserRoleService;
        _sysUserMenuService = sysUserMenuService;
        _sysTenantMenuRep = sysTenantMenuRep;
        _sysCacheService = sysCacheService;
        _sysLangTextCacheService = sysLangTextCacheService;
        _sysLangTextService = sysLangTextService;
    }

    /// <summary>
    /// Get login menu tree 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get login menu tree")]
    public async Task<List<MenuOutput>> GetLoginMenuTree()
    {
        var sysDefaultLang = App.GetOptions<LocalizationSettingsOptions>().DefaultCulture;
        var langCode = _userManager.LangCode;
        var (query, _) = GetSugarQueryableAndTenantId(_userManager.TenantId);

        // Query menu main table (filter non-buttons and disabled)
        var menuQuery = query.Where(u => u.Type != MenuTypeEnum.Btn && u.Status == StatusEnum.Enable);

        if (!(_userManager.SuperAdmin || _userManager.SysAdmin))
        {
            var menuIdList = await GetMenuIdList();
            menuQuery = menuQuery.Where(u => menuIdList.Contains(u.Id));
        }

        // Query the main table (no more LEFT JOIN)
        var menuList = await menuQuery
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();

        // Translation is only performed when the user language is different from the system default language to avoid unnecessary performance overhead.
        if (langCode != sysDefaultLang)
        {
            // Call cached translation: translate the Title field
            var fields = new List<LangFieldMap<SysMenu>>
            {
                new LangFieldMap<SysMenu>
                {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
                }
            };
            await _sysLangTextCacheService.TranslateMultiFields(menuList, fields, langCode);
        }

        // Construction tree
        var menuTree = menuList.ToTree(
            it => it.Children, it => it.Pid, 0
        );

        // Convert to output DTO
        return menuTree.Adapt<List<MenuOutput>>();
    }

    /// <summary>
    /// Get menu list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("ObtainmenuList")]
    public async Task<List<SysMenu>> GetList([FromQuery] MenuInput input)
    {
        var langCode = _userManager.LangCode;
        var menuIdList = _userManager.SuperAdmin || _userManager.SysAdmin ? new List<long>() : await GetMenuIdList();
        var (query, _) = GetSugarQueryableAndTenantId(input.TenantId);

        // Conditionally query the menu list directly (with Title and Type filtering)
        if (!string.IsNullOrWhiteSpace(input.Title) || input.Type is > 0)
        {
            var menuList = await query
                .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title))
                .WhereIF(input.Type is > 0, u => u.Type == input.Type)
                .WhereIF(menuIdList.Count > 0, u => menuIdList.Contains(u.Id))
                .OrderBy(u => new { u.OrderNo, u.Id })
                .ToListAsync();

            // Use cached batch translation
            var fields = new List<LangFieldMap<SysMenu>>
            {
                new LangFieldMap<SysMenu>
                {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
                }
            };
            await _sysLangTextCacheService.TranslateMultiFields(menuList, fields, langCode);

            return menuList.Distinct().ToList();
        }

        // If there are no filter conditions, the entire tree structure will be used (with permissions)
        if (!(_userManager.SuperAdmin || _userManager.SysAdmin))
        {
            query = query.Where(u => menuIdList.Contains(u.Id));
        }

        var menuFullList = await query
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();

        // Use cached batch translation
        var treeFields = new List<LangFieldMap<SysMenu>>
        {
            new LangFieldMap<SysMenu>
            {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
            }
        };
        await _sysLangTextCacheService.TranslateMultiFields(menuFullList, treeFields, langCode);

        // Assembly tree
        var menuTree = menuFullList.ToTree(it => it.Children, it => it.Pid, 0);
        return menuTree.ToList();
    }

    /// <summary>
    /// Add menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add menu")]
    public async Task<long> AddMenu(AddMenuInput input)
    {
        var (query, tenantId) = GetSugarQueryableAndTenantId(input.TenantId);

        var isExist = input.Type != MenuTypeEnum.Btn
            ? await query.AnyAsync(u => u.Title == input.Title && u.Pid == input.Pid)
            : await query.AnyAsync(u => u.Permission == input.Permission);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D4000);

        if (!string.IsNullOrWhiteSpace(input.Name) && await query.AnyAsync(u => u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D4009);

        if (input.Pid != 0 && await query.AnyAsync(u => u.Id == input.Pid && u.Type == MenuTypeEnum.Btn)) throw Oops.Oh(ErrorCodeEnum.D4010);

        // Verify menu parameters
        var sysMenu = input.Adapt<SysMenu>();
        CheckMenuParam(sysMenu);

        // Save tenant menu permissions
        await _sysMenuRep.InsertAsync(sysMenu);
        await _sysTenantMenuRep.InsertAsync(new SysTenantMenu { TenantId = tenantId, MenuId = sysMenu.Id });

        // clear cache
        DeleteMenuCache();

        return sysMenu.Id;
    }

    /// <summary>
    /// Update menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Updatemenu")]
    public async Task UpdateMenu(UpdateMenuInput input)
    {
        if (!_userManager.SuperAdmin && new SysMenuSeedData().HasData().Any(u => u.Id == input.Id)) throw Oops.Oh(ErrorCodeEnum.D4012);

        if (input.Id == input.Pid) throw Oops.Oh(ErrorCodeEnum.D4008);
        var (query, _) = GetSugarQueryableAndTenantId(input.TenantId);

        var isExist = input.Type != MenuTypeEnum.Btn
            ? await query.AnyAsync(u => u.Title == input.Title && u.Type == input.Type && u.Pid == input.Pid && u.Id != input.Id)
            : await query.AnyAsync(u => u.Permission == input.Permission && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D4000);

        if (!string.IsNullOrWhiteSpace(input.Name) && await query.AnyAsync(u => u.Id != input.Id && u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D4009);

        if (input.Pid != 0 && await query.AnyAsync(u => u.Id == input.Pid && u.Type == MenuTypeEnum.Btn)) throw Oops.Oh(ErrorCodeEnum.D4010);

        // Verify menu parameters
        var sysMenu = input.Adapt<SysMenu>();
        CheckMenuParam(sysMenu);

        await _sysMenuRep.AsTenant().UseTranAsync(async () =>
        {
            // Update menu
            await _sysMenuRep.AsUpdateable(sysMenu).ExecuteCommandAsync();

            // Synchronously update the translation table
            var menuTranslation = await _sysLangTextCacheService.GetTranslationEntity("SysMenu", "Title", sysMenu.Id, _userManager.LangCode);
            if (!menuTranslation.IsNullOrEmpty())
            {
                await _sysLangTextService.Update(new UpdateSysLangTextInput
                {
                    Id = menuTranslation.Id,
                    EntityName = "SysMenu",
                    EntityId = sysMenu.Id,
                    FieldName = "Title",
                    LangCode = _userManager.LangCode,
                    Content = sysMenu.Title
                });
            }
        }, err =>
        {
            Oops.Oh("UpdateDatatimehappenmistake", err.Message);
        });

        // clear cache
        DeleteMenuCache();
    }

    /// <summary>
    /// Delete menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete Menu")]
    public async Task DeleteMenu(DeleteMenuInput input)
    {
        if (!_userManager.SuperAdmin && new SysMenuSeedData().HasData().Any(u => u.Id == input.Id)) throw Oops.Oh(ErrorCodeEnum.D4013);

        var menuTreeList = await _sysMenuRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id);
        var menuIdList = menuTreeList.Select(u => u.Id).ToList();

        await _sysMenuRep.DeleteAsync(u => menuIdList.Contains(u.Id));

        // Cascade delete tenant menu data
        await _sysTenantMenuRep.AsDeleteable().Where(u => menuIdList.Contains(u.MenuId)).ExecuteCommandAsync();

        // Cascade delete character menu data
        await _sysRoleMenuService.DeleteRoleMenuByMenuIdList(menuIdList);

        // Cascade delete user favorite menu
        await _sysUserMenuService.DeleteMenuList(menuIdList);

        // clear cache
        DeleteMenuCache();
    }

    /// <summary>
    /// Set menu status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Set menu status")]
    public virtual async Task<int> SetStatus(MenuStatusInput input)
    {
        if (_userManager.UserId == input.Id)
            throw Oops.Oh(ErrorCodeEnum.D1026);

        var menu = await _sysMenuRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        menu.Status = input.Status;
        var rows = await _sysMenuRep.AsUpdateable(menu).UpdateColumns(u => new { u.Status }).ExecuteCommandAsync();
        return rows;
    }

    /// <summary>
    /// Check menu data when adding and editing
    /// </summary>
    /// <param name="menu"></param>
    private static void CheckMenuParam(SysMenu menu)
    {
        var permission = menu.Permission;
        if (menu.Type == MenuTypeEnum.Btn)
        {
            menu.Name = null;
            menu.Path = null;
            menu.Component = null;
            menu.Icon = null;
            menu.Redirect = null;
            menu.OutLink = null;
            menu.IsHide = false;
            menu.IsKeepAlive = true;
            menu.IsAffix = false;
            menu.IsIframe = false;

            if (string.IsNullOrEmpty(permission)) throw Oops.Oh(ErrorCodeEnum.D4003);
            if (!permission.Contains(':')) throw Oops.Oh(ErrorCodeEnum.D4004);
        }
        else
        {
            menu.Permission = null;
        }
    }

    /// <summary>
    /// Get the set of button permissions that the user has (cache) 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Obtain the set of button permissions")]
    public async Task<List<string>> GetOwnBtnPermList()
    {
        var userId = _userManager.UserId;
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyUserButton + userId);
        if (permissions != null) return permissions;

        var menuIdList = _userManager.SuperAdmin ? new() : await GetMenuIdList();
        if (menuIdList.Count <= 0 && !_userManager.SuperAdmin && !_userManager.SysAdmin)
        {
            //_sysCacheService.Set(CacheConst.KeyUserButton + userId, new List<string>(), TimeSpan.FromDays(7));
            return new List<string>();
        }

        permissions = await _sysMenuRep.AsQueryable()
            .InnerJoinIF<SysTenantMenu>(!_userManager.SuperAdmin, (u, t) => t.TenantId == _userManager.TenantId && u.Id == t.MenuId)
            .Where(u => u.Type == MenuTypeEnum.Btn)
            .WhereIF(menuIdList.Count > 0, u => menuIdList.Contains(u.Id))
            .Select(u => u.Permission).ToListAsync();

        _sysCacheService.Set(CacheConst.KeyUserButton + userId, permissions, TimeSpan.FromDays(7));

        return permissions;
    }

    /// <summary>
    /// Get the permission set of all buttons in the system (cache)
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<string>> GetAllBtnPermList()
    {
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyUserButton + 0);
        if (permissions != null && permissions.Count != 0) return permissions;

        permissions = await _sysMenuRep.AsQueryable()
            .Where(u => u.Type == MenuTypeEnum.Btn)
            .Select(u => u.Permission).ToListAsync();
        _sysCacheService.Set(CacheConst.KeyUserButton + 0, permissions);

        return permissions;
    }

    /// <summary>
    /// Get the build menu joint table query instance based on the tenant ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [NonAction]
    public (ISugarQueryable<SysMenu, SysTenantMenu> query, long tenantId) GetSugarQueryableAndTenantId(long tenantId)
    {
        if (!_userManager.SuperAdmin) tenantId = _userManager.TenantId;

        // Super-managed user menu range: seed menu + tenant id menu
        ISugarQueryable<SysMenu, SysTenantMenu> query;
        if (_userManager.SuperAdmin)
        {
            if (tenantId <= 0)
            {
                query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(false, (u, t) => true);
            }
            else
            {
                // Tenant-specific menu
                var menuIds = _sysTenantMenuRep.AsQueryable().Where(u => u.TenantId == tenantId).ToList(u => u.MenuId) ?? new();

                // Seed menu
                //menuIds.AddRange(new SysMenuSeedData().HasData().Select(u => u.Id).ToList());

                menuIds = menuIds.Distinct().ToList();
                query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(false, (u, t) => true).Where(u => menuIds.Contains(u.Id));
            }
        }
        else
        {
            query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(tenantId > 0, (u, t) => t.TenantId == tenantId && u.Id == t.MenuId);
        }

        return (query, tenantId);
    }

    /// <summary>
    /// Clear menu and button cache
    /// </summary>
    [NonAction]
    public void DeleteMenuCache()
    {
        // _sysCacheService.RemoveByPrefixKey(CacheConst.KeyUserMenu);
        _sysCacheService.RemoveByPrefixKey(CacheConst.KeyUserButton);
    }

    /// <summary>
    /// Get the current user menu ID collection
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetMenuIdList()
    {
        var roleIdList = await _sysUserRoleService.GetUserRoleIdList(_userManager.UserId);
        return await _sysRoleMenuService.GetRoleMenuIdList(roleIdList);
    }

    /// <summary>
    /// Exclude the existence of a select-all parent menu on the front end
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> ExcludeParentMenuOfFullySelected(List<long> menuIds)
    {
        // Get the current user menu
        var (query, _) = GetSugarQueryableAndTenantId(0);
        var menuList = await query.ToListAsync();

        // Exclude the list to prevent front-end select-all problems
        var exceptList = new List<long>();
        foreach (var id in menuIds)
        {
            // exclude button menu
            if (menuList.Any(u => u.Id == id && u.Type == MenuTypeEnum.Btn)) continue;

            // If there is no subset or all subset permissions
            var children = menuList.ToChildList(u => u.Id, u => u.Pid, id, false).ToList();
            if (children.Count == 0 || children.All(u => menuIds.Contains(u.Id))) continue;

            // Exclude menus without full subset permissions
            exceptList.Add(id);
        }
        return menuIds.Except(exceptList).ToList();
    }
}