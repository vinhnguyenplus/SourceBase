// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System dynamic plug-in table
/// </summary>
[SugarTable(null, "System dynamic plug-in table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
public partial class SysPlugin : EntityBaseTenant
{
    /// <summary>
    /// name
    /// </summary>
    [SugarColumn(ColumnDescription = "name", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string Name { get; set; }

    /// <summary>
    /// C# code
    /// </summary>
    [SugarColumn(ColumnDescription = "C# code", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    [Required]
    public virtual string CsharpCode { get; set; }

    /// <summary>
    /// Assembly name
    /// </summary>
    [SugarColumn(ColumnDescription = "Assembly Name", Length = 512)]
    [MaxLength(512)]
    public string? AssemblyName { get; set; }

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
}