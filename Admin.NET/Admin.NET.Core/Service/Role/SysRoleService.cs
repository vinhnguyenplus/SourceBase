// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System role service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 480)]
public class SysRoleService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysRole> _sysRoleRep;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysRoleOrgService _sysRoleOrgService;
    private readonly SysMenuService _sysMenuService;
    private readonly SysOrgService _sysOrgService;
    private readonly SysCacheService _sysCacheService;

    public SysRoleService(UserManager userManager,
        SysOrgService sysOrgService,
        SysMenuService sysMenuService,
        SysRoleOrgService sysRoleOrgService,
        SqlSugarRepository<SysRole> sysRoleRep,
        SysRoleMenuService sysRoleMenuService,
        SysUserRoleService sysUserRoleService,
        SysCacheService sysCacheService)
    {
        _userManager = userManager;
        _sysRoleRep = sysRoleRep;
        _sysOrgService = sysOrgService;
        _sysMenuService = sysMenuService;
        _sysRoleOrgService = sysRoleOrgService;
        _sysRoleMenuService = sysRoleMenuService;
        _sysUserRoleService = sysUserRoleService;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// Get paginated list of roles 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get a paginated list of roles")]
    public async Task<SqlSugarPagedList<SysRole>> Page(PageRoleInput input)
    {
        // A collection of roles owned by the current user
        var roleIdList = _userManager.SuperAdmin ? new List<long>() : await _sysUserRoleService.GetUserRoleIdList(_userManager.UserId);
        return await _sysRoleRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!_userManager.SuperAdmin, u => u.TenantId == _userManager.TenantId) // If it is not super-managed, you can only operate the role of this tenant.
            .WhereIF(!_userManager.SuperAdmin && !_userManager.SysAdmin, u => u.CreateUserId == _userManager.UserId || roleIdList.Contains(u.Id)) // If you are not a super administrator and are not a system administrator, you can only operate the roles you created | the roles you own.
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get character list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get role list")]
    public async Task<List<RoleOutput>> GetList()
    {
        // A collection of roles owned by the current user
        var roleIdList = _userManager.SuperAdmin ? new List<long>() : await _sysUserRoleService.GetUserRoleIdList(_userManager.UserId);

        return await _sysRoleRep.AsQueryable()
            .WhereIF(!_userManager.SuperAdmin, u => u.TenantId == _userManager.TenantId) // If it is not super-managed, you can only operate the role of this tenant.
            .WhereIF(!_userManager.SuperAdmin && !_userManager.SysAdmin, u => u.CreateUserId == _userManager.UserId || roleIdList.Contains(u.Id)) // If you are not a super administrator and are not a system administrator, only the roles you created and already own will be displayed.
            .Where(u => u.Status != StatusEnum.Disable) // Not prohibited
            .OrderBy(u => new { u.OrderNo, u.Id }).Select<RoleOutput>().ToListAsync();
    }

    /// <summary>
    /// Add roles 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add character")]
    public async Task AddRole(AddRoleInput input)
    {
        if (await _sysRoleRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code))
            throw Oops.Oh(ErrorCodeEnum.D1006);

        var newRole = await _sysRoleRep.AsInsertable(input.Adapt<SysRole>()).ExecuteReturnEntityAsync();
        input.Id = newRole.Id;
        await UpdateRoleMenu(input);
    }

    /// <summary>
    /// Update role menu permissions
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task UpdateRoleMenu(AddRoleInput input)
    {
        if (input.MenuIdList == null || input.MenuIdList.Count < 1) return;
        await GrantMenu(new RoleMenuInput()
        {
            Id = input.Id,
            MenuIdList = input.MenuIdList.ToList()
        });
    }

    /// <summary>
    /// Update character 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update Character")]
    public async Task UpdateRole(UpdateRoleInput input)
    {
        if (await _sysRoleRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D1006);

        await _sysRoleRep.AsUpdateable(input.Adapt<SysRole>()).IgnoreColumns(true)
            .IgnoreColumns(u => new { u.DataScope }).ExecuteCommandAsync();

        await UpdateRoleMenu(input);
    }

    /// <summary>
    /// Delete role 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete role")]
    public async Task DeleteRole(DeleteRoleInput input)
    {
        // If the role has users, deletion is prohibited
        var userIds = await _sysUserRoleService.GetUserIdList(input.Id);
        if (userIds != null && userIds.Count > 0) throw Oops.Oh(ErrorCodeEnum.D1025);

        // If there is a binding registration plan, deletion is prohibited.
        var hasUserRegWay = await _sysRoleRep.Context.Queryable<SysUserRegWay>().AnyAsync(u => u.RoleId == input.Id);
        if (hasUserRegWay) throw Oops.Oh(ErrorCodeEnum.D1033);

        var sysRole = await _sysRoleRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _sysRoleRep.DeleteAsync(sysRole);

        // Cascade deletion of role organization data
        await _sysRoleOrgService.DeleteRoleOrgByRoleId(sysRole.Id);

        // Cascade delete user role data
        await _sysUserRoleService.DeleteUserRoleByRoleId(sysRole.Id);

        // Cascade delete character menu data
        await _sysRoleMenuService.DeleteRoleMenuByRoleId(sysRole.Id);
    }

    /// <summary>
    /// Authorized role menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Authorized Role Menu")]
    public async Task GrantMenu(RoleMenuInput input)
    {
        if (input.MenuIdList == null || input.MenuIdList.Count < 1) return;

        await ClearUserApiCache(input.Id);

        await _sysRoleMenuService.GrantRoleMenu(input);
    }

    /// <summary>
    /// Authorized role data range 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Authorized Role Data Scope")]
    public async Task GrantDataScope(RoleOrgInput input)
    {
        // Delete the user organization cache associated with this role
        var userIdList = await _sysUserRoleService.GetUserIdList(input.Id);
        foreach (var userId in userIdList)
        {
            SqlSugarFilter.DeleteUserOrgCache(userId, _sysRoleRep.Context.CurrentConnectionConfig.ConfigId.ToString());
        }

        var role = await _sysRoleRep.GetFirstAsync(u => u.Id == input.Id);
        var dataScope = input.DataScope;
        if (!_userManager.SuperAdmin)
        {
            switch (dataScope)
            {
                // Non-super administrators do not have full data range permissions
                case (int)DataScopeEnum.All: throw Oops.Oh(ErrorCodeEnum.D1016);
                // If the data range is customized, determine whether the authorized data range has permissions.
                case (int)DataScopeEnum.Define:
                    {
                        var grantOrgIdList = input.OrgIdList;
                        if (grantOrgIdList.Count > 0)
                        {
                            var orgIdList = await _sysOrgService.GetUserOrgIdList();
                            if (orgIdList.Count < 1)
                                throw Oops.Oh(ErrorCodeEnum.D1016);
                            if (!grantOrgIdList.All(u => orgIdList.Any(c => c == u)))
                                throw Oops.Oh(ErrorCodeEnum.D1016);
                        }

                        break;
                    }
            }
        }
        role.DataScope = (DataScopeEnum)dataScope;
        await _sysRoleRep.AsUpdateable(role).UpdateColumns(u => new { u.DataScope }).ExecuteCommandAsync();
        await _sysRoleOrgService.GrantRoleOrg(input);
    }

    /// <summary>
    /// Get the menu ID collection based on the role ID 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get menu ID collection based on role ID")]
    public async Task<List<long>> GetOwnMenuList([FromQuery] RoleInput input)
    {
        var menuIds = await _sysRoleMenuService.GetRoleMenuIdList(new List<long> { input.Id });
        return await _sysMenuService.ExcludeParentMenuOfFullySelected(menuIds);
    }

    /// <summary>
    /// Get the organization ID collection based on the role ID 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the organization ID collection based on the role ID")]
    public async Task<List<long>> GetOwnOrgList([FromQuery] RoleInput input)
    {
        return await _sysRoleOrgService.GetRoleOrgIdList(new List<long> { input.Id });
    }

    /// <summary>
    /// Set character status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Set character status")]
    public async Task<int> SetStatus(RoleInput input)
    {
        if (!Enum.IsDefined(typeof(StatusEnum), input.Status)) throw Oops.Oh(ErrorCodeEnum.D3005);

        return await _sysRoleRep.AsUpdateable()
            .SetColumns(u => u.Status == input.Status)
            .Where(u => u.Id == input.Id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete the user interface cache associated with this role
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task ClearUserApiCache(long roleId)
    {
        var userIdList = await _sysUserRoleService.GetUserIdList(roleId);
        foreach (var userId in userIdList)
        {
            _sysCacheService.Remove($"{CacheConst.KeyUserButton}{userId}");
        }
    }
}