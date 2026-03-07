// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System dictionary value table
/// </summary>
[SugarTable(null, "System Dictionary Value Table")]
[SysTable]
[SugarIndex("index_{table}_TV", nameof(DictTypeId), OrderByType.Asc, nameof(Value), OrderByType.Asc, IsUnique = true)]
public partial class SysDictData : EntityBase
{
    /// <summary>
    /// Dictionary typeId
    /// </summary>
    [SugarColumn(ColumnDescription = "Dictionary Type Id")]
    public long DictTypeId { get; set; }

    /// <summary>
    /// dictionary type
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(DictTypeId))]
    public SysDictType DictType { get; set; }

    /// <summary>
    /// display text
    /// </summary>
    [SugarColumn(ColumnDescription = "Display Text", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string Label { get; set; }

    /// <summary>
    /// value
    /// </summary>
    [SugarColumn(ColumnDescription = "value", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string Value { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    /// <remarks>
    /// </remarks>
    [SugarColumn(ColumnDescription = "Encoding", Length = 256)]
    public virtual string? Code { get; set; }

    /// <summary>
    /// name
    /// </summary>
    [SugarColumn(ColumnDescription = "name", Length = 256)]
    [MaxLength(256)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Display style-label color
    /// </summary>
    [SugarColumn(ColumnDescription = "Display style-label color", Length = 16)]
    [MaxLength(16)]
    public string? TagType { get; set; }

    /// <summary>
    /// Display style-Style (control display style)
    /// </summary>
    [SugarColumn(ColumnDescription = "Display style-Style", Length = 512)]
    [MaxLength(512)]
    public string? StyleSetting { get; set; }

    /// <summary>
    /// Display style-Class (control display style)
    /// </summary>
    [SugarColumn(ColumnDescription = "DisplayStyle-Class", Length = 512)]
    [MaxLength(512)]
    public string? ClassSetting { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort", DefaultValue = "100")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 2048)]
    [MaxLength(2048)]
    public string? Remark { get; set; }

    /// <summary>
    /// Expansion data (save configuration items for business functions)
    /// </summary>
    [SugarColumn(ColumnDescription = "Expansion data (save configuration items for business functions)", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ExtData { get; set; }

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state", DefaultValue = "1")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}