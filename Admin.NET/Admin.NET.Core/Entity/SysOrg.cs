// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System organization table
/// </summary>
[SugarTable(null, "System Organization Table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
[SugarIndex("index_{table}_T", nameof(Type), OrderByType.Asc)]
public partial class SysOrg : EntityBaseTenant
{
    /// <summary>
    /// ParentId
    /// </summary>
    [SugarColumn(ColumnDescription = "Parent ID")]
    public long Pid { get; set; }

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
    /// level
    /// </summary>
    [SugarColumn(ColumnDescription = "level")]
    public int? Level { get; set; }

    /// <summary>
    /// Institution Type-Data Dictionary
    /// </summary>
    [SugarColumn(ColumnDescription = "Institution type", Length = 64)]
    [MaxLength(64)]
    public virtual string? Type { get; set; }

    /// <summary>
    /// Person in charge ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Person in charge ID", IsNullable = true)]
    public long? DirectorId { get; set; }

    /// <summary>
    /// person in charge
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(DirectorId))]
    public SysUser Director { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 128)]
    [MaxLength(128)]
    public string? Remark { get; set; }

    /// <summary>
    /// Institutional child
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysOrg> Children { get; set; }

    /// <summary>
    /// Whether to disable selection
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public bool Disabled { get; set; }
}