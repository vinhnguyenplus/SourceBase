// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System exception log service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 350)]
public class SysLogExService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogEx> _sysLogExRep;
    private readonly UserManager _userManager;

    public SysLogExService(UserManager userManager, SqlSugarRepository<SysLogEx> sysLogExRep)
    {
        _sysLogExRep = sysLogExRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Get the paging list of exception logs 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("Get the exception log paging list")]
    public async Task<SqlSugarPagedList<SysLogEx>> Page(PageExLogInput input)
    {
        return await _sysLogExRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account == input.Account)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ControllerName), u => u.ControllerName == input.ControllerName)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ActionName), u => u.ActionName == input.ActionName)
            .WhereIF(!string.IsNullOrWhiteSpace(input.RemoteIp), u => u.RemoteIp == input.RemoteIp)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Elapsed.ToString()), u => u.Elapsed >= input.Elapsed)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status == "200", u => u.Status == "200")
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status != "200", u => u.Status != "200")
            //.OrderBy(u => u.CreateTime, OrderByType.Desc)
            .IgnoreColumns(u => new { u.RequestParam, u.ReturnResult, u.Message })
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get exception log details 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("Get exception log details")]
    public async Task<SysLogEx> GetDetail(long id)
    {
        return await _sysLogExRep.GetFirstAsync(u => u.Id == id);
    }

    /// <summary>
    /// Clear exception log 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("ClearException Log")]
    public void Clear()
    {
        _sysLogExRep.AsSugarClient().DbMaintenance.TruncateTable<SysLogEx>();
    }

    /// <summary>
    /// Export exception log 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Export"), NonUnify]
    [DisplayName("Export exception log")]
    public async Task<IActionResult> ExportLogEx(LogInput input)
    {
        var logExList = await _sysLogExRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                    u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .Select<ExportLogDto>().ToListAsync();

        IExcelExporter excelExporter = new ExcelExporter();
        var res = await excelExporter.ExportAsByteArray(logExList);
        return new FileStreamResult(new MemoryStream(res), "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmm") + "Exception log.xlsx" };
    }
}