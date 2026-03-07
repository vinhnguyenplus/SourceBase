// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System template table
/// </summary>
[SysTable]
[SugarTable(null, "System Template Table")]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc, IsUnique = true)]
[SugarIndex("index_{table}_G", nameof(GroupName), OrderByType.Asc)]
public partial class SysTemplate : EntityBaseTenant
{
    /// <summary>
    /// name
    /// </summary>
    [MaxLength(128)]
    [SugarColumn(ColumnDescription = "name", Length = 128)]
    public virtual string Name { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [SugarColumn(ColumnDescription = "Group Name")]
    public virtual TemplateTypeEnum Type { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    [MaxLength(128)]
    [SugarColumn(ColumnDescription = "Encoding", Length = 128)]
    public virtual string Code { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [MaxLength(32)]
    [SugarColumn(ColumnDescription = "Group Name", Length = 32)]
    public virtual string GroupName { get; set; }

    /// <summary>
    /// Template content
    /// </summary>
    [SugarColumn(ColumnDescription = "Template content", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public virtual string Content { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [MaxLength(128)]
    [SugarColumn(ColumnDescription = "Remarks", Length = 128)]
    public virtual string? Remark { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public virtual int OrderNo { get; set; } = 100;
}