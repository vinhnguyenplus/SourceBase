// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System dictionary type table
/// </summary>
[SugarTable(null, "System Dictionary Type Table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
public partial class SysDictType : EntityBase
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
    [Required, MaxLength(64)]
    public virtual string Code { get; set; }

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

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state", DefaultValue = "1")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;

    /// <summary>
    /// Whether it is a built-in dictionary (Y-yes, N-no)
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether it is a built-in dictionary", DefaultValue = "1")]
    public virtual YesNoEnum SysFlag { get; set; } = YesNoEnum.Y;

    /// <summary>
    /// Whether it is a tenant dictionary (Y-yes, N-no)
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it a tenant dictionary?", DefaultValue = "2")]
    public virtual YesNoEnum IsTenant { get; set; } = YesNoEnum.N;

    /// <summary>
    /// collection of dictionary values
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(SysDictData.DictTypeId))]
    public List<SysDictData> Children { get; set; }
}