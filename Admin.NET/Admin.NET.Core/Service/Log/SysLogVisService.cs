// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System access log service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 340)]
public class SysLogVisService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogVis> _sysLogVisRep;
    private readonly UserManager _userManager;

    public SysLogVisService(UserManager userManager, SqlSugarRepository<SysLogVis> sysLogVisRep)
    {
        _sysLogVisRep = sysLogVisRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Get access log paginated list 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("Get access log paginated list")]
    public async Task<SqlSugarPagedList<SysLogVis>> Page(PageVisLogInput input)
    {
        return await _sysLogVisRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account == input.Account)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ActionName), u => u.ActionName == input.ActionName)
            .WhereIF(!string.IsNullOrWhiteSpace(input.RemoteIp), u => u.RemoteIp == input.RemoteIp)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Elapsed.ToString()), u => u.Elapsed >= input.Elapsed)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status == "200", u => u.Status == "200")
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status != "200", u => u.Status != "200")
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Clear access log 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("Clear access logs")]
    public void Clear()
    {
        _sysLogVisRep.AsSugarClient().DbMaintenance.TruncateTable<SysLogVis>();
    }

    /// <summary>
    /// Get access log list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get access log list")]
    public async Task<List<LogVisOutput>> GetList()
    {
        return await _sysLogVisRep.AsQueryable()
            .Where(u => u.Longitude > 0 && u.Longitude > 0)
            .Select(u => new LogVisOutput
            {
                Location = u.Location,
                Longitude = (double?)u.Longitude,
                Latitude = (double?)u.Latitude,
                RealName = u.RealName,
                LogDateTime = u.LogDateTime
            }).ToListAsync();
    }
}