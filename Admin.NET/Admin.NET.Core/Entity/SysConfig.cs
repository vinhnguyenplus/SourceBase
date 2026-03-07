// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System configuration parameter table
/// </summary>
[SugarTable(null, "System Configuration Parameter Table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc, IsUnique = true)]
public partial class SysConfig : EntityBase
{
    /// <summary>
    /// name
    /// </summary>
    [SugarColumn(ColumnDescription = "name", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string Name { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    [SugarColumn(ColumnDescription = "Encoding", Length = 64)]
    [MaxLength(64)]
    public string? Code { get; set; }

    /// <summary>
    /// Parameter value
    /// </summary>
    [SugarColumn(ColumnDescription = "Parameter value", Length = 512)]
    [MaxLength(512)]
    [IgnoreUpdateSeedColumn]
    public string? Value { get; set; }

    /// <summary>
    /// Whether it is a built-in parameter (Y-yes, N-no)
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it a built-in parameter?", DefaultValue = "1")]
    public YesNoEnum SysFlag { get; set; } = YesNoEnum.Y;

    /// <summary>
    /// block coding
    /// </summary>
    [SugarColumn(ColumnDescription = "GroupEncoding", Length = 64)]
    [MaxLength(64)]
    public string? GroupCode { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort", DefaultValue = "100")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }
}