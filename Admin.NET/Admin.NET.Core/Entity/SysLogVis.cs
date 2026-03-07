// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System access log table
/// </summary>
[SugarTable(null, "System Access Log Table")]
[SysTable]
[LogTable]
public partial class SysLogVis : EntityBaseTenant
{
    /// <summary>
    /// module name
    /// </summary>
    [SugarColumn(ColumnDescription = "Module Name", Length = 256)]
    [MaxLength(256)]
    public string? ControllerName { get; set; }

    /// <summary>
    /// method name
    ///</summary>
    [SugarColumn(ColumnDescription = "Method Name", Length = 256)]
    [MaxLength(256)]
    public string? ActionName { get; set; }

    /// <summary>
    /// display name
    ///</summary>
    [SugarColumn(ColumnDescription = "Display Name", Length = 256)]
    [MaxLength(256)]
    public string? DisplayTitle { get; set; }

    /// <summary>
    /// Execution status
    /// </summary>
    [SugarColumn(ColumnDescription = "Execution Status", Length = 32)]
    [MaxLength(32)]
    public string? Status { get; set; }

    /// <summary>
    /// IP address
    /// </summary>
    [SugarColumn(ColumnDescription = "IP address", Length = 256)]
    [MaxLength(256)]
    public string? RemoteIp { get; set; }

    /// <summary>
    /// Login location
    /// </summary>
    [SugarColumn(ColumnDescription = "Login location", Length = 128)]
    [MaxLength(128)]
    public string? Location { get; set; }

    /// <summary>
    /// longitude
    /// </summary>
    [SugarColumn(ColumnDescription = "longitude")]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// Dimensions
    /// </summary>
    [SugarColumn(ColumnDescription = "Dimension")]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Browser
    /// </summary>
    [SugarColumn(ColumnDescription = "Browser", Length = 1024)]
    [MaxLength(1024)]
    public string? Browser { get; set; }

    /// <summary>
    /// operating system
    /// </summary>
    [SugarColumn(ColumnDescription = "operating system", Length = 256)]
    [MaxLength(256)]
    public string? Os { get; set; }

    /// <summary>
    /// Operation time
    /// </summary>
    [SugarColumn(ColumnDescription = "Operation time")]
    public long? Elapsed { get; set; }

    /// <summary>
    /// Log time
    /// </summary>
    [SugarColumn(ColumnDescription = "Log Time")]
    public DateTime? LogDateTime { get; set; }

    /// <summary>
    /// Log level
    /// </summary>
    [SugarColumn(ColumnDescription = "Log level")]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// account
    /// </summary>
    [SugarColumn(ColumnDescription = "Account number", Length = 32)]
    [MaxLength(32)]
    public string? Account { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    [SugarColumn(ColumnDescription = "Real Name", Length = 32)]
    [MaxLength(32)]
    public string? RealName { get; set; }
}