// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System schedule service
/// </summary>
[ApiDescriptionSettings(Order = 295)]
public class SysScheduleService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysSchedule> _sysSchedule;

    public SysScheduleService(UserManager userManager,
        SqlSugarRepository<SysSchedule> sysSchedule)
    {
        _userManager = userManager;
        _sysSchedule = sysSchedule;
    }

    /// <summary>
    /// Get schedule list
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get schedule list")]
    public async Task<List<SysSchedule>> Page(ListScheduleInput input)
    {
        return await _sysSchedule.AsQueryable()
            .Where(u => u.UserId == _userManager.UserId)
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.ScheduleTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.ScheduleTime <= input.EndTime)
            .OrderBy(u => u.StartTime, OrderByType.Asc)
            .ToListAsync();
    }

    /// <summary>
    /// Get schedule details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("Get schedule details")]
    public async Task<SysSchedule> GetDetail(long id)
    {
        return await _sysSchedule.GetFirstAsync(u => u.Id == id);
    }

    /// <summary>
    /// Add schedule
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add schedule")]
    public async Task AddUserSchedule(AddScheduleInput input)
    {
        input.UserId = _userManager.UserId;
        await _sysSchedule.InsertAsync(input.Adapt<SysSchedule>());
    }

    /// <summary>
    /// Update schedule
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update schedule")]
    public async Task UpdateUserSchedule(UpdateScheduleInput input)
    {
        await _sysSchedule.AsUpdateable(input.Adapt<SysSchedule>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete schedule
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete schedule")]
    public async Task DeleteUserSchedule(DeleteScheduleInput input)
    {
        await _sysSchedule.DeleteAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Set schedule status
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Set schedule status")]
    public async Task<int> SetStatus(ScheduleInput input)
    {
        if (!Enum.IsDefined(typeof(FinishStatusEnum), input.Status)) throw Oops.Oh(ErrorCodeEnum.D3005);

        return await _sysSchedule.AsUpdateable()
            .SetColumns(u => u.Status == input.Status)
            .Where(u => u.Id == input.Id)
            .ExecuteCommandAsync();
    }
}