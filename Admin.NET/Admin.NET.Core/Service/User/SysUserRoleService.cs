// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System user role service
/// </summary>
public class SysUserRoleService : ITransient
{
    private readonly SysCacheService _sysCacheService;
    private readonly SqlSugarRepository<SysUserRole> _sysUserRoleRep;

    public SysUserRoleService(SysCacheService sysCacheService,
        SqlSugarRepository<SysUserRole> sysUserRoleRep)
    {
        _sysCacheService = sysCacheService;
        _sysUserRoleRep = sysUserRoleRep;
    }

    /// <summary>
    /// Authorized user roles
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task GrantUserRole(UserRoleInput input)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == input.UserId);

        if (input.RoleIdList == null || input.RoleIdList.Count < 1) return;
        var roles = input.RoleIdList.Select(u => new SysUserRole
        {
            UserId = input.UserId,
            RoleId = u
        }).ToList();
        await _sysUserRoleRep.InsertRangeAsync(roles);
        _sysCacheService.Remove(CacheConst.KeyUserButton + input.UserId);
    }

    /// <summary>
    /// Delete user role based on roleId
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByRoleId(long roleId)
    {
        await _sysUserRoleRep.AsQueryable()
             .Where(u => u.RoleId == roleId)
             .Select(u => u.UserId)
             .ForEachAsync(userId =>
             {
                 _sysCacheService.Remove(CacheConst.KeyUserButton + userId);
             });

        await _sysUserRoleRep.DeleteAsync(u => u.RoleId == roleId);
    }

    /// <summary>
    /// Delete user role based on userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByUserId(long userId)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == userId);
        _sysCacheService.Remove(CacheConst.KeyUserButton + userId);
    }

    /// <summary>
    /// Get role collection based on user ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysRole>> GetUserRoleList(long userId)
    {
        var sysUserRoleList = await _sysUserRoleRep.AsQueryable()
            .Includes(u => u.SysRole)
            .Where(u => u.UserId == userId).ToListAsync();
        return sysUserRoleList.Where(u => u.SysRole != null).Select(u => u.SysRole).ToList();
    }

    /// <summary>
    /// Get the role ID collection based on the user ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserRoleIdList(long userId)
    {
        return await _sysUserRoleRep.AsQueryable()
            .Where(u => u.UserId == userId).Select(u => u.RoleId).ToListAsync();
    }

    /// <summary>
    /// Get user ID collection based on role ID
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserIdList(long roleId)
    {
        return await _sysUserRoleRep.AsQueryable()
            .Where(u => u.RoleId == roleId).Select(u => u.UserId).ToListAsync();
    }
}