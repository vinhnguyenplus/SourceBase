// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System Organization Services 🧩
/// </summary>
[ApiDescriptionSettings(Order = 470)]
public class SysOrgService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SysCacheService _sysCacheService;
    private readonly SysUserExtOrgService _sysUserExtOrgService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysRoleOrgService _sysRoleOrgService;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;

    public SysOrgService(UserManager userManager,
        SysCacheService sysCacheService,
        SysUserExtOrgService sysUserExtOrgService,
        SysUserRoleService sysUserRoleService,
        SysRoleOrgService sysRoleOrgService,
        SqlSugarRepository<SysOrg> sysOrgRep)
    {
        _userManager = userManager;
        _sysCacheService = sysCacheService;
        _sysUserExtOrgService = sysUserExtOrgService;
        _sysUserRoleService = sysUserRoleService;
        _sysRoleOrgService = sysRoleOrgService;
        _sysOrgRep = sysOrgRep;
    }

    /// <summary>
    /// Get list of institutions 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the list of organizations")]
    public async Task<List<SysOrg>> GetList([FromQuery] OrgInput input)
    {
        // Get the set of owned institution IDs
        var userOrgIdList = await GetUserOrgIdList();

        var queryable = _sysOrgRep.AsQueryable().WhereIF(input.TenantId > 0, u => u.TenantId == input.TenantId).OrderBy(u => new { u.OrderNo, u.Id });
        // Return list data when filtering with conditions
        if (!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.Code) || !string.IsNullOrWhiteSpace(input.Type))
        {
            return await queryable.WhereIF(userOrgIdList.Count > 0, u => userOrgIdList.Contains(u.Id))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code == input.Code)
                .WhereIF(!string.IsNullOrWhiteSpace(input.Type), u => u.Type == input.Type)
                .ToListAsync();
        }

        List<SysOrg> orgTree;
        if (_userManager.SuperAdmin)
        {
            orgTree = await queryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id);
        }
        else
        {
            orgTree = await queryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id, userOrgIdList.Select(d => (object)d).ToArray());
            // Recursively disable organizations without permissions (prevent users from modifying or creating organizations and users without permissions)
            HandlerOrgTree(orgTree, userOrgIdList);
        }

        var sysOrg = await _sysOrgRep.GetSingleAsync(u => u.Id == input.Id);
        if (sysOrg == null) return orgTree;

        sysOrg.Children = orgTree;
        orgTree = new List<SysOrg> { sysOrg };
        return orgTree;
    }

    /// <summary>
    /// Recursively disable organizations without authority
    /// </summary>
    /// <param name="orgTree"></param>
    /// <param name="userOrgIdList"></param>
    private static void HandlerOrgTree(List<SysOrg> orgTree, List<long> userOrgIdList)
    {
        foreach (var org in orgTree)
        {
            org.Disabled = !userOrgIdList.Contains(org.Id); // Setting disabled/not selectable
            if (org.Children != null)
                HandlerOrgTree(org.Children, userOrgIdList);
        }
    }

    /// <summary>
    /// Get the institution tree 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get organization tree")]
    public async Task<List<OrgTreeOutput>> GetTree([FromQuery] OrgInput input)
    {
        // Get the set of owned institution IDs
        var userOrgIdList = await GetUserOrgIdList();

        var queryable = _sysOrgRep.AsQueryable().WhereIF(input.TenantId > 0, u => u.TenantId == input.TenantId).OrderBy(u => new { u.OrderNo, u.Id });
        List<OrgTreeOutput> orgTree;
        if (_userManager.SuperAdmin)
        {
            orgTree = await queryable.Select<OrgTreeOutput>().ToTreeAsync(u => u.Children, u => u.Pid, input.Id);
        }
        else
        {
            orgTree = await queryable.Select<OrgTreeOutput>().ToTreeAsync(u => u.Children, u => u.Pid, input.Id, userOrgIdList.Select(d => (object)d).ToArray());
            // Recursively disable organizations without permissions (prevent users from modifying or creating organizations and users without permissions)
            HandlerOrgTree(orgTree, userOrgIdList);
        }

        var sysOrg = await _sysOrgRep.AsQueryable().Select<OrgTreeOutput>().FirstAsync(u => u.Id == input.Id);
        if (sysOrg == null) return orgTree;

        sysOrg.Children = orgTree;
        orgTree = new List<OrgTreeOutput> { sysOrg };
        return orgTree;
    }

    /// <summary>
    /// Recursively disable organizations without authority
    /// </summary>
    /// <param name="orgTree"></param>
    /// <param name="userOrgIdList"></param>
    private static void HandlerOrgTree(List<OrgTreeOutput> orgTree, List<long> userOrgIdList)
    {
        foreach (var org in orgTree)
        {
            org.Disabled = !userOrgIdList.Contains(org.Id); // Setting disabled/not selectable
            if (org.Children != null)
                HandlerOrgTree(org.Children, userOrgIdList);
        }
    }

    /// <summary>
    /// Add organization 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add institutions")]
    public async Task<long> AddOrg(AddOrgInput input)
    {
        if (!_userManager.SuperAdmin && input.Pid == 0)
            throw Oops.Oh(ErrorCodeEnum.D2009);

        if (await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        if (!_userManager.SuperAdmin && input.Pid != 0)
        {
            // If the parent ID of the new organization is not 0, permission verification will be performed.
            var orgIdList = await GetUserOrgIdList();
            // The parent organization of the newly added organization is not within the scope of your own data
            if (orgIdList.Count < 1 || !orgIdList.Contains(input.Pid))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // Delete user institution cache related to this parent institution
        if (input.Pid == 0)
        {
            DeleteAllUserOrgCache(0, 0);
        }
        else
        {
            var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
            if (pOrg != null)
                DeleteAllUserOrgCache(pOrg.Id, pOrg.Pid);
        }

        var newOrg = await _sysOrgRep.AsInsertable(input.Adapt<SysOrg>()).ExecuteReturnEntityAsync();
        return newOrg.Id;
    }

    /// <summary>
    /// Add institutions in batches
    /// </summary>
    /// <param name="orgs"></param>
    /// <returns></returns>
    [NonAction]
    public async Task BatchAddOrgs(List<SysOrg> orgs)
    {
        DeleteAllUserOrgCache(0, 0);
        await _sysOrgRep.AsDeleteable().ExecuteCommandAsync();
        await _sysOrgRep.AsInsertable(orgs).ExecuteCommandAsync();
    }

    /// <summary>
    /// Update organization 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update organization")]
    public async Task UpdateOrg(UpdateOrgInput input)
    {
        if (!_userManager.SuperAdmin && input.Pid == 0)
            throw Oops.Oh(ErrorCodeEnum.D2012);

        if (input.Pid != 0)
        {
            //var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
            //_ = pOrg ?? throw Oops.Oh(ErrorCodeEnum.D2000);

            // If the parent organization changes, clear the user organization cache.
            var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id);
            if (sysOrg != null && sysOrg.Pid != input.Pid)
            {
                // Delete user institution cache related to this institution, new parent institution
                DeleteAllUserOrgCache(sysOrg.Id, input.Pid);
            }
        }
        if (input.Id == input.Pid)
            throw Oops.Oh(ErrorCodeEnum.D2001);

        if (await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        // The parent ID cannot be its own child node
        var childIdList = await GetChildIdListWithSelfById(input.Id);
        if (childIdList.Contains(input.Pid))
            throw Oops.Oh(ErrorCodeEnum.D2001);

        // Do you have permission to operate this organization?
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetUserOrgIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(input.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        await _sysOrgRep.AsUpdateable(input.Adapt<SysOrg>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete organization 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete institution")]
    public async Task DeleteOrg(DeleteOrgInput input)
    {
        var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        // Do you have permission to operate this organization?
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetUserOrgIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(sysOrg.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // If the institution is the tenant's default institution, deletion is prohibited.
        var isTenantOrg = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysTenant>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (isTenantOrg)
            throw Oops.Oh(ErrorCodeEnum.D2008);

        // If the organization has users, deletion is prohibited
        var orgHasEmp = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (orgHasEmp)
            throw Oops.Oh(ErrorCodeEnum.D2004);

        // If the extension organization has users, deletion is prohibited.
        var hasExtOrgEmp = await _sysUserExtOrgService.HasUserOrg(sysOrg.Id);
        if (hasExtOrgEmp)
            throw Oops.Oh(ErrorCodeEnum.D2005);

        // If the sub-organization has users, deletion is prohibited.
        var childOrgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var childOrgIdList = childOrgTreeList.Select(u => u.Id).ToList();

        // If the sub-organization has users, deletion is prohibited.
        var cOrgHasEmp = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => childOrgIdList.Contains(u.OrgId));
        if (cOrgHasEmp) throw Oops.Oh(ErrorCodeEnum.D2007);

        // If there is a binding registration plan, deletion is prohibited.
        var hasUserRegWay = await _sysOrgRep.Context.Queryable<SysUserRegWay>().AnyAsync(u => u.OrgId == input.Id);
        if (hasUserRegWay) throw Oops.Oh(ErrorCodeEnum.D2010);

        // Delete user organization cache related to this organization and parent organization
        DeleteAllUserOrgCache(sysOrg.Id, sysOrg.Pid);

        // Cascade deletion of organization sub-nodes
        await _sysOrgRep.DeleteAsync(u => childOrgIdList.Contains(u.Id));

        // Cascade deletion of role organization data
        await _sysRoleOrgService.DeleteRoleOrgByOrgIdList(childOrgIdList);

        // Cascade deletion of user organization data
        await _sysUserExtOrgService.DeleteUserExtOrgByOrgIdList(childOrgIdList);
    }

    /// <summary>
    /// Delete user organization cache related to this organization and parent organization
    /// </summary>
    /// <param name="orgId"></param>
    /// <param name="orgPid"></param>
    private void DeleteAllUserOrgCache(long orgId, long orgPid)
    {
        var userOrgKeyList = _sysCacheService.GetKeysByPrefixKey(CacheConst.KeyUserOrg);
        if (userOrgKeyList is not { Count: > 0 }) return;

        foreach (var userOrgKey in userOrgKeyList)
        {
            var userOrgList = _sysCacheService.Get<List<long>>(userOrgKey);
            var userId = long.Parse(userOrgKey.Substring(CacheConst.KeyUserOrg));
            if (userOrgList != null && (userOrgList.Contains(orgId) || userOrgList.Contains(orgPid)))
                SqlSugarFilter.DeleteUserOrgCache(userId, _sysOrgRep.Context.CurrentConnectionConfig.ConfigId.ToString());

            if (orgPid != 0) continue;

            var dataScope = _sysCacheService.Get<int>($"{CacheConst.KeyRoleMaxDataScope}{userId}");
            if (dataScope == (int)DataScopeEnum.All)
                SqlSugarFilter.DeleteUserOrgCache(userId, _sysOrgRep.Context.CurrentConnectionConfig.ConfigId.ToString());
        }
    }

    /// <summary>
    /// Get the current user institution ID collection
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetUserOrgIdList()
    {
        if (_userManager.SuperAdmin) return new();
        return await GetUserOrgIdList(_userManager.UserId, _userManager.OrgId);
    }

    /// <summary>
    /// Get the institution ID collection based on the specified user ID
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetUserOrgIdList(long userId, long userOrgId)
    {
        var orgIdList = _sysCacheService.Get<List<long>>($"{CacheConst.KeyUserOrg}{userId}"); // Get cache
        if (orgIdList is { Count: >= 1 }) return orgIdList;

        // I create an organization collection
        var orgList0 = await _sysOrgRep.AsQueryable().Where(u => u.CreateUserId == userId).Select(u => u.Id).ToListAsync();

        // Extended Institutional Collection
        var orgList1 = await _sysUserExtOrgService.GetUserExtOrgList(userId);

        // role agency collection
        var orgList2 = await GetUserRoleOrgIdList(userId, userOrgId);

        // Organizational union
        orgIdList = orgList1.Select(u => u.OrgId).Union(orgList2).Union(orgList0).ToList();

        // Current affiliation
        if (!orgIdList.Contains(userOrgId)) orgIdList.Add(userOrgId);

        _sysCacheService.Set($"{CacheConst.KeyUserOrg}{userId}", orgIdList, TimeSpan.FromDays(7)); // cache
        return orgIdList;
    }

    /// <summary>
    /// Get the user role organization ID collection
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userOrgId">User's institution ID</param>
    /// <returns></returns>
    private async Task<List<long>> GetUserRoleOrgIdList(long userId, long userOrgId)
    {
        var roleList = await _sysUserRoleService.GetUserRoleList(userId);

        if (roleList.Count < 1) return new(); // Empty organization ID collection

        return await GetUserOrgIdList(roleList, userId, userOrgId);
    }

    /// <summary>
    /// Determine whether the user has certain role permissions
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="role">role code</param>
    /// <returns></returns>
    [NonAction]
    public async Task<bool> GetUserHasRole(long userId, SysRole role)
    {
        if (_userManager.SuperAdmin)
            return true;
        var userOrgId = _userManager.OrgId;
        var roleList = await _sysUserRoleService.GetUserRoleList(userId);
        if (roleList != null && roleList.Exists(r => r.Code == role.Code) == true)
            return true;
        roleList = new List<SysRole> { role };
        var orgIds = await GetUserOrgIdList(roleList, userId, userOrgId);
        return orgIds.Contains(userOrgId);
    }

    /// <summary>
    /// Get the organization ID set based on the role ID set
    /// </summary>
    /// <param name="roleList"></param>
    /// <param name="userId"></param>
    /// <param name="userOrgId">User's institution ID</param>
    /// <returns></returns>
    private async Task<List<long>> GetUserOrgIdList(List<SysRole> roleList, long userId, long userOrgId)
    {
        // Set according to the maximum scope policy (if you have both ALL and SELF permissions, the result is ALL)
        int strongerDataScopeType = (int)DataScopeEnum.Self;

        // Role collection for custom data ranges
        var customDataScopeRoleIdList = new List<long>();

        // Institutional collection of data ranges
        var dataScopeOrgIdList = new List<long>();

        if (roleList is { Count: > 0 })
        {
            roleList.ForEach(u =>
            {
                if (u.DataScope == DataScopeEnum.Define)
                {
                    customDataScopeRoleIdList.Add(u.Id);
                    strongerDataScopeType = (int)u.DataScope; // When customizing data permissions, the maximum range must also be updated.
                }
                else if ((int)u.DataScope <= strongerDataScopeType)
                {
                    strongerDataScopeType = (int)u.DataScope;
                    // Get the institution collection based on the data range
                    var orgIds = GetOrgIdListByDataScope(userOrgId, strongerDataScopeType).GetAwaiter().GetResult();
                    dataScopeOrgIdList = dataScopeOrgIdList.Union(orgIds).ToList();
                }
            });
        }

        // Cache the current user's maximum role data range
        _sysCacheService.Set(CacheConst.KeyRoleMaxDataScope + userId, strongerDataScopeType, TimeSpan.FromDays(7));

        // Get the organization collection based on the role collection
        var roleOrgIdList = await _sysRoleOrgService.GetRoleOrgIdList(customDataScopeRoleIdList);

        // Union set of institutions
        return roleOrgIdList.Union(dataScopeOrgIdList).ToList();
    }

    /// <summary>
    /// Get the institution ID collection based on the data range
    /// </summary>
    /// <param name="userOrgId">User's institution ID</param>
    /// <param name="dataScope"></param>
    /// <returns></returns>
    private async Task<List<long>> GetOrgIdListByDataScope(long userOrgId, int dataScope)
    {
        var orgId = userOrgId;//var orgId = _userManager.OrgId;
        var orgIdList = new List<long>();
        switch (dataScope)
        {
            // If the data range is all, get the set of all institution IDs
            case (int)DataScopeEnum.All:
                orgIdList = await _sysOrgRep.AsQueryable().Select(u => u.Id).ToListAsync();
                break;
            // If the data range is this department and below, obtain the set of this node and sub-nodes.
            case (int)DataScopeEnum.DeptChild:
                orgIdList = await GetChildIdListWithSelfById(orgId);
                break;
            // If the data range is this department and does not contain child nodes, then it will be returned directly to this department.
            case (int)DataScopeEnum.Dept:
                orgIdList.Add(orgId);
                break;
        }
        return orgIdList;
    }

    /// <summary>
    /// Get the set of child node IDs based on the node ID (including itself)
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetChildIdListWithSelfById(long pid)
    {
        var orgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, pid, true);
        return orgTreeList.Select(u => u.Id).ToList();
    }
}