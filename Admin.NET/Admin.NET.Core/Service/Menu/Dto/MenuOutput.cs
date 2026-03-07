// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System menu returns results
/// </summary>
public class MenuOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// ParentId
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// Menu type (0 Directory 1 Menu 2 Button)
    /// </summary>
    public MenuTypeEnum Type { get; set; }

    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// routing address
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// component path
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Permission ID
    /// </summary>
    public string Permission { get; set; }

    /// <summary>
    /// Redirect
    /// </summary>
    public string Redirect { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    public int OrderNo { get; set; }

    /// <summary>
    /// state
    /// </summary>
    public StatusEnum Status { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public virtual DateTime CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    public virtual DateTime UpdateTime { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    public virtual string CreateUserName { get; set; }

    /// <summary>
    /// Modifier name
    /// </summary>
    public virtual string UpdateUserName { get; set; }

    /// <summary>
    /// MenuMeta
    /// </summary>
    public SysMenuMeta Meta { get; set; }

    /// <summary>
    /// menu item
    /// </summary>
    public List<MenuOutput> Children { get; set; }
}

/// <summary>
/// Menu Meta configuration
/// </summary>
public class SysMenuMeta
{
    /// <summary>
    /// title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Whether to embed
    /// </summary>
    public bool IsIframe { get; set; }

    /// <summary>
    /// External links
    /// </summary>
    public string IsLink { get; set; }

    /// <summary>
    /// Whether to hide
    /// </summary>
    public bool IsHide { get; set; }

    /// <summary>
    /// Whether to cache
    /// </summary>
    public bool IsKeepAlive { get; set; }

    /// <summary>
    /// Is it fixed?
    /// </summary>
    public bool IsAffix { get; set; }
}

/// <summary>
/// Configure menu object mapping
/// </summary>
public class SysMenuMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<SysMenu, MenuOutput>()
            .Map(t => t.Meta.Title, o => o.Title)
            .Map(t => t.Meta.Icon, o => o.Icon)
            .Map(t => t.Meta.IsIframe, o => o.IsIframe)
            .Map(t => t.Meta.IsLink, o => o.OutLink)
            .Map(t => t.Meta.IsHide, o => o.IsHide)
            .Map(t => t.Meta.IsKeepAlive, o => o.IsKeepAlive)
            .Map(t => t.Meta.IsAffix, o => o.IsAffix);
    }
}