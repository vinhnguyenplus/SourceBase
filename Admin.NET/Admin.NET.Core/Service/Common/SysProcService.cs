// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System stored procedure service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 102)]
public class SysProcService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;

    public SysProcService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// Export stored procedure data - specify columns, unspecified fields will be hidden 🔖
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PocExport2(ExportProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var dt = await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);

        var headers = new Dictionary<string, Tuple<string, int>>();
        var index = 1;
        foreach (var val in input.EHeader)
        {
            headers.Add(val.Key.ToUpper(), new Tuple<string, int>(val.Value, index));
            index++;
        }
        var excelExporter = new ExcelExporter();
        var da = await excelExporter.ExportAsByteArray(dt, new ProcExporterHeaderFilter(headers));
        return new FileContentResult(da, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// Export stored procedure data based on template 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IActionResult> PocExport(ExportProcByTMPInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var dt = await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);

        var excelExporter = new ExcelExporter();
        string template = AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/template/" + input.Template + ".xlsx";
        var bs = await excelExporter.ExportBytesByTemplate(dt, template);
        return new FileContentResult(bs, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// Get the stored procedure return table - Oracle and Dameng parameter order cannot be wrong 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DataTable> ProcTable(BaseProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);
    }

    /// <summary>
    /// Obtain the data set returned by the stored procedure-Oracle and Dameng parameter order cannot be wrong
    /// Oracle returns table, table1, others return table1, table2. Suitable for reports, complex detailed pages, etc. 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<DataSet> CommonDataSet(BaseProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataSetAllAsync(input.ProcId, input.ProcParams);
    }

    ///// <summary>
    ///// Get the mapping stored procedure according to the configuration table
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //public async Task<DataTable> ProcEnitybyConfig(BaseProcInput input)
    //{
    //    var key = "ProcConfig";
    //    var ds = _sysCacheService.Get<Dictionary<string, string>>(key);
    //    if (ds == null || ds.Count == 0 || !ds.ContainsKey(input.ProcId))
    //    {
    //        var datas = await _db.Queryable<ProcConfig>().ToListAsync();
    //        ds = datas.ToDictionary(m => m.ProcId, m => m.ProcName);
    //        _sysCacheService.Set(key, ds);
    //    }
    //    var procName = ds[input.ProcId];
    //    return await _db.Ado.UseStoredProcedure().GetDataTableAsync(procName, input.ProcParams);
    //}
}