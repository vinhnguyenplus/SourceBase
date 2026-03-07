// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user role table
/// </summary>
[SugarTable(null, "systemUserRoleTable")]
[SysTable]
public class SysUserRole : EntityBaseId
{
    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// user
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// RoleId
    /// </summary>
    [SugarColumn(ColumnDescription = "RoleId")]
    public long RoleId { get; set; }

    /// <summary>
    /// Role
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(RoleId))]
    public SysRole SysRole { get; set; }
}