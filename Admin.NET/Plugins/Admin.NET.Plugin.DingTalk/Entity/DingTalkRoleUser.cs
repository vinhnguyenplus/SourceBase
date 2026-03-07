// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// DingTalk character information
/// </summary>
[SugarTable(null, "DingTalk character list")]
public class DingTalkRoleUser : EntityBaseDel
{
    /// <summary>
    /// DingTalk user id
    /// </summary>
    [SugarColumn(ColumnDescription = "DingTalk user ID", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string? DingTalkUserId { get; set; }

    /// <summary>
    /// Role group id
    /// </summary>
    [SugarColumn(ColumnDescription = "Role group id")]
    [Required]
    public virtual long groupId { get; set; }

    /// <summary>
    /// Role group name
    /// </summary>
    [SugarColumn(ColumnDescription = "Role group name", Length = 64)]
    [MaxLength(64)]
    public string? groupName { get; set; }

    /// <summary>
    /// role id
    /// </summary>
    [SugarColumn(ColumnDescription = "Character ID")]
    [Required]
    public virtual long roleId { get; set; }

    /// <summary>
    /// Character name
    /// </summary>
    [SugarColumn(ColumnDescription = "Character name", Length = 64)]
    [MaxLength(64)]
    public string? roleName { get; set; }
}