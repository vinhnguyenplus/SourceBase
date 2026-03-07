// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System job information table
/// </summary>
[SugarTable(null, "System Job Information Table")]
[SysTable]
[SugarIndex("index_{table}_J", nameof(JobId), OrderByType.Asc)]
public partial class SysJobDetail : EntityBaseId
{
    /// <summary>
    /// JobId
    /// </summary>
    [SugarColumn(ColumnDescription = "JobId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string JobId { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [SugarColumn(ColumnDescription = "Group Name", Length = 128)]
    [MaxLength(128)]
    public string? GroupName { get; set; } = "default";

    /// <summary>
    /// Job typeFullName
    /// </summary>
    [SugarColumn(ColumnDescription = "Job type", Length = 128)]
    [MaxLength(128)]
    public string? JobType { get; set; }

    /// <summary>
    /// Assembly Name
    /// </summary>
    [SugarColumn(ColumnDescription = "Assembly", Length = 128)]
    [MaxLength(128)]
    public string? AssemblyName { get; set; }

    /// <summary>
    /// Description information
    /// </summary>
    [SugarColumn(ColumnDescription = "Description information", Length = 128)]
    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// Whether to execute in parallel
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to execute in parallel")]
    public bool Concurrent { get; set; } = true;

    /// <summary>
    /// Whether to scan for attribute triggers
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to scan for attribute triggers", ColumnName = "annotation")]
    public bool IncludeAnnotation { get; set; } = false;

    /// <summary>
    /// extra data
    /// </summary>
    [SugarColumn(ColumnDescription = "Extra data", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Properties { get; set; } = "{}";

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// Job creation type
    /// </summary>
    [SugarColumn(ColumnDescription = "Job creation type")]
    public JobCreateTypeEnum CreateType { get; set; } = JobCreateTypeEnum.BuiltIn;

    /// <summary>
    /// script code
    /// </summary>
    [SugarColumn(ColumnDescription = "script code", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ScriptCode { get; set; }
}