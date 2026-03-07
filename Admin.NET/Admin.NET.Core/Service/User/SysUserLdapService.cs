// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// User domain account service
/// </summary>
public class SysUserLdapService : ITransient
{
    private readonly SqlSugarRepository<SysUserLdap> _sysUserLdapRep;

    public SysUserLdapService(SqlSugarRepository<SysUserLdap> sysUserLdapRep)
    {
        _sysUserLdapRep = sysUserLdapRep;
    }

    /// <summary>
    /// Insert domain accounts in batches
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="sysUserLdapList"></param>
    /// <returns></returns>
    public async Task InsertUserLdapList(long tenantId, List<SysUserLdap> sysUserLdapList)
    {
        await _sysUserLdapRep.DeleteAsync(u => u.TenantId == tenantId);

        await _sysUserLdapRep.InsertRangeAsync(sysUserLdapList);

        await _sysUserLdapRep.AsUpdateable()
            .InnerJoin<SysUser>((l, u) => l.EmployeeId == u.Account)
            .SetColumns((l, u) => new SysUserLdap { UserId = u.Id })
            .Where((l, u) => l.TenantId == tenantId && u.Status == StatusEnum.Enable)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Add domain account
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <param name="account"></param>
    /// <param name="domainAccount"></param>
    /// <returns></returns>
    public async Task AddUserLdap(long tenantId, long userId, string account, string domainAccount)
    {
        var userLdap = await _sysUserLdapRep.GetFirstAsync(u => u.TenantId == tenantId && (u.Account == account || u.UserId == userId || u.EmployeeId == domainAccount));
        if (userLdap != null) await _sysUserLdapRep.DeleteByIdAsync(userLdap.Id);

        if (!string.IsNullOrWhiteSpace(domainAccount))
            await _sysUserLdapRep.InsertAsync(new SysUserLdap { EmployeeId = account, TenantId = tenantId, UserId = userId, Account = domainAccount });
    }

    /// <summary>
    /// Delete domain account
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserLdapByUserId(long userId)
    {
        await _sysUserLdapRep.DeleteAsync(u => u.UserId == userId);
    }
}