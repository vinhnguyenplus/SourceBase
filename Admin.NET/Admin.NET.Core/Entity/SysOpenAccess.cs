// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Open interface identity table
/// </summary>
[SugarTable(null, "Open interface identity table")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(AccessKey), OrderByType.Asc)]
public partial class SysOpenAccess : EntityBase
{
    /// <summary>
    /// Identity mark
    /// </summary>
    [SugarColumn(ColumnDescription = "Identity mark", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string AccessKey { get; set; }

    /// <summary>
    /// key
    /// </summary>
    [SugarColumn(ColumnDescription = "key", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string AccessSecret { get; set; }

    /// <summary>
    /// Bind tenant ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind Tenant Id")]
    public long BindTenantId { get; set; }

    /// <summary>
    /// Bind tenant
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(BindTenantId))]
    public SysTenant BindTenant { get; set; }

    /// <summary>
    /// Bind user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind user ID")]
    public virtual long BindUserId { get; set; }

    /// <summary>
    /// Bind user
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(BindUserId))]
    public SysUser BindUser { get; set; }
}