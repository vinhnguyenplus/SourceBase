// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System tenant configuration parameter value table
/// </summary>
[SugarTable(null, "System tenant configuration parameter value table")]
[SysTable]
[SugarIndex("index_{table}_UC", nameof(UserId), OrderByType.Asc, nameof(ConfigId), OrderByType.Asc)]
public class SysUserConfigData : EntityBaseId
{
    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// Configuration item ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Configuration Item Id")]
    public long ConfigId { get; set; }

    /// <summary>
    /// Parameter value
    /// </summary>
    [SugarColumn(ColumnDescription = "Parameter value", Length = 512)]
    [MaxLength(512)]
    public string? Value { get; set; }
}