// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System online user table
/// </summary>
[SugarTable(null, "System online user table")]
[SysTable]
public partial class SysOnlineUser : EntityBaseTenantId
{
    /// <summary>
    /// ConnectionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Connection ID")]
    public string? ConnectionId { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// account
    /// </summary>
    [SugarColumn(ColumnDescription = "Account number", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string UserName { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    [SugarColumn(ColumnDescription = "Real Name", Length = 32)]
    [MaxLength(32)]
    public string? RealName { get; set; }

    /// <summary>
    /// connection time
    /// </summary>
    [SugarColumn(ColumnDescription = "Connection Time")]
    public DateTime? Time { get; set; }

    /// <summary>
    /// Connect IP
    /// </summary>
    [SugarColumn(ColumnDescription = "Connect IP", Length = 256)]
    [MaxLength(256)]
    public string? Ip { get; set; }

    /// <summary>
    /// Browser
    /// </summary>
    [SugarColumn(ColumnDescription = "Browser", Length = 128)]
    [MaxLength(128)]
    public string? Browser { get; set; }

    /// <summary>
    /// operating system
    /// </summary>
    [SugarColumn(ColumnDescription = "operating system", Length = 128)]
    [MaxLength(128)]
    public string? Os { get; set; }
}