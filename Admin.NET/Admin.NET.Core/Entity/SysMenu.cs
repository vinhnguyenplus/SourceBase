// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System menu table
/// </summary>
[SugarTable(null, "System Menu Table")]
[SysTable]
[SugarIndex("index_{table}_T", nameof(Title), OrderByType.Asc)]
[SugarIndex("index_{table}_T2", nameof(Type), OrderByType.Asc)]
public partial class SysMenu : EntityBase
{
    /// <summary>
    /// ParentId
    /// </summary>
    [SugarColumn(ColumnDescription = "Parent ID")]
    public long Pid { get; set; }

    /// <summary>
    /// Menu type (1 directory 2 menu 3 button)
    /// </summary>
    [SugarColumn(ColumnDescription = "menuType")]
    public MenuTypeEnum Type { get; set; }

    /// <summary>
    /// Route name
    /// </summary>
    [SugarColumn(ColumnDescription = "Route Name", Length = 64)]
    [MaxLength(64)]
    public string? Name { get; set; }

    /// <summary>
    /// routing address
    /// </summary>
    [SugarColumn(ColumnDescription = "Routing Address", Length = 128)]
    [MaxLength(128)]
    public string? Path { get; set; }

    /// <summary>
    /// component path
    /// </summary>
    [SugarColumn(ColumnDescription = "component path", Length = 128)]
    [MaxLength(128)]
    public string? Component { get; set; }

    /// <summary>
    /// Redirect
    /// </summary>
    [SugarColumn(ColumnDescription = "Redirect", Length = 128)]
    [MaxLength(128)]
    public string? Redirect { get; set; }

    /// <summary>
    /// Permission ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Permission Identifier", Length = 128)]
    [MaxLength(128)]
    public string? Permission { get; set; }

    /// <summary>
    /// Menu name
    /// </summary>
    [SugarColumn(ColumnDescription = "Menu name", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string Title { get; set; }

    /// <summary>
    /// icon
    /// </summary>
    [SugarColumn(ColumnDescription = "icon", Length = 128)]
    [MaxLength(128)]
    public string? Icon { get; set; } = "ele-Menu";

    /// <summary>
    /// Whether to embed
    /// </summary>
    [SugarColumn(ColumnDescription = "YesnoEmbedded")]
    public bool IsIframe { get; set; }

    /// <summary>
    /// External links
    /// </summary>
    [SugarColumn(ColumnDescription = "External links", Length = 256)]
    [MaxLength(256)]
    public string? OutLink { get; set; }

    /// <summary>
    /// Whether to hide
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to hide")]
    public bool IsHide { get; set; }

    /// <summary>
    /// Whether to cache
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to cache")]
    public bool IsKeepAlive { get; set; } = true;

    /// <summary>
    /// Is it fixed?
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it fixed?")]
    public bool IsAffix { get; set; }

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
    [SugarColumn(ColumnDescription = "Remarks", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }

    /// <summary>
    /// menu item
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysMenu> Children { get; set; } = new();
}