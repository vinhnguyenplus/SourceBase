// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System difference log table
/// </summary>
[SugarTable(null, "System Difference Log Table")]
[SysTable]
[LogTable]
public partial class SysLogDiff : EntityBaseTenant
{
    /// <summary>
    /// differential data
    /// </summary>
    [SugarColumn(ColumnDescription = "differential data", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? DiffData { get; set; }

    /// <summary>
    /// Sql
    /// </summary>
    [SugarColumn(ColumnDescription = "Sql", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Sql { get; set; }

    /// <summary>
    /// Parameters manually passed in parameters
    /// </summary>
    [SugarColumn(ColumnDescription = "Parameter", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Parameters { get; set; }

    /// <summary>
    /// business object
    /// </summary>
    [SugarColumn(ColumnDescription = "business object", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? BusinessData { get; set; }

    /// <summary>
    /// difference operation
    /// </summary>
    [SugarColumn(ColumnDescription = "difference operation", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? DiffType { get; set; }

    /// <summary>
    /// time consuming
    /// </summary>
    [SugarColumn(ColumnDescription = "Time consuming")]
    public long? Elapsed { get; set; }
}