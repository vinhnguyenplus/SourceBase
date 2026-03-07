// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System user extension agency services
/// </summary>
public class SysUserExtOrgService : ITransient
{
    private readonly SqlSugarRepository<SysUserExtOrg> _sysUserExtOrgRep;

    public SysUserExtOrgService(SqlSugarRepository<SysUserExtOrg> sysUserExtOrgRep)
    {
        _sysUserExtOrgRep = sysUserExtOrgRep;
    }

    /// <summary>
    /// Get user extended organization collection
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysUserExtOrg>> GetUserExtOrgList(long userId)
    {
        return await _sysUserExtOrgRep.GetListAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Update user extension organization
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="extOrgList"></param>
    /// <returns></returns>
    public async Task UpdateUserExtOrg(long userId, List<SysUserExtOrg> extOrgList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);

        if (extOrgList == null || extOrgList.Count < 1) return;
        extOrgList.ForEach(u =>
        {
            u.UserId = userId;
        });
        await _sysUserExtOrgRep.InsertRangeAsync(extOrgList);
    }

    /// <summary>
    /// Delete extended institutions based on institution ID set
    /// </summary>
    /// <param name="orgIdList"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByOrgIdList(List<long> orgIdList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => orgIdList.Contains(u.OrgId));
    }

    /// <summary>
    /// Delete extended organization based on userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByUserId(long userId)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Determine whether there is a user based on the organization ID
    /// </summary>
    /// <param name="orgId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserOrg(long orgId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.OrgId == orgId);
    }

    /// <summary>
    /// Determine whether there is a user based on the job ID
    /// </summary>
    /// <param name="posId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserPos(long posId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.PosId == posId);
    }
}