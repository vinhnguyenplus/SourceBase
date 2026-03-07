// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System exception log table
/// </summary>
[SugarTable(null, "System Error Log Table")]
[SysTable]
[LogTable]
public partial class SysLogEx : SysLogVis
{
    /// <summary>
    /// Request method
    /// </summary>
    [SugarColumn(ColumnDescription = "Request Method", Length = 32)]
    [MaxLength(32)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Request address
    /// </summary>
    [SugarColumn(ColumnDescription = "Request address", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Request parameters
    /// </summary>
    [SugarColumn(ColumnDescription = "Request parameters", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RequestParam { get; set; }

    /// <summary>
    /// Return results
    /// </summary>
    [SugarColumn(ColumnDescription = "Return result", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ReturnResult { get; set; }

    /// <summary>
    /// EventId
    /// </summary>
    [SugarColumn(ColumnDescription = "Event ID")]
    public int? EventId { get; set; }

    /// <summary>
    /// ThreadId
    /// </summary>
    [SugarColumn(ColumnDescription = "ThreadId")]
    public int? ThreadId { get; set; }

    /// <summary>
    /// Request Tracking ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Request Tracking ID", Length = 128)]
    [MaxLength(128)]
    public string? TraceId { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [SugarColumn(ColumnDescription = "Exception information", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Exception { get; set; }

    /// <summary>
    /// Log message Json
    /// </summary>
    [SugarColumn(ColumnDescription = "Log message Json", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Message { get; set; }
}