// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// DingTalk user table
/// </summary>
[SugarTable(null, "DingTalk user table")]
public class DingTalkUser : EntityBase
{
    /// <summary>
    /// System user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "System User ID")]
    public long SysUserId { get; set; }

    /// <summary>
    /// system user
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(SysUserId))]
    [JsonIgnore]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// DingTalk user id
    /// </summary>
    [SugarColumn(ColumnDescription = "DingTalk user ID", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string? DingTalkUserId { get; set; }

    /// <summary>
    /// UnionId
    /// </summary>
    [SugarColumn(ColumnDescription = "UnionId", Length = 64)]
    [MaxLength(64)]
    public string? UnionId { get; set; }

    /// <summary>
    /// username
    /// </summary>
    [SugarColumn(ColumnDescription = "Username", Length = 64)]
    [MaxLength(64)]
    public string? Name { get; set; }

    /// <summary>
    /// phone number
    /// </summary>
    [SugarColumn(ColumnDescription = "Mobile phone number", Length = 16)]
    [MaxLength(16)]
    public string? Mobile { get; set; }

    /// <summary>
    /// gender
    /// </summary>
    [SugarColumn(ColumnDescription = "gender")]
    public int? Sex { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    [SugarColumn(ColumnDescription = "Avatar", Length = 256)]
    [MaxLength(256)]
    public string? Avatar { get; set; }

    /// <summary>
    /// Job number
    /// </summary>
    [SugarColumn(ColumnDescription = "Job number", Length = 16)]
    [MaxLength(16)]
    public string? JobNumber { get; set; }

    /// <summary>
    /// Main department ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Primary Department ID", Length = 16)]
    [MaxLength(16)]
    public string? DeptId { get; set; }

    /// <summary>
    /// main department
    /// </summary>
    [SugarColumn(ColumnDescription = "Main Department", Length = 16)]
    [MaxLength(16)]
    public string? Dept { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    [SugarColumn(ColumnDescription = "Position", Length = 16)]
    [MaxLength(16)]
    public string? Position { get; set; }
}