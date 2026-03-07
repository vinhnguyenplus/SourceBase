// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user extension organization table
/// </summary>
[SugarTable(null, "System user extension organization table")]
[SysTable]
public partial class SysUserExtOrg : EntityBaseId
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
    /// InstitutionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Organization ID")]
    public long OrgId { get; set; }

    /// <summary>
    /// mechanism
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(OrgId))]
    public SysOrg SysOrg { get; set; }

    /// <summary>
    /// PositionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Job ID")]
    public long PosId { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(PosId))]
    public SysPos SysPos { get; set; }

    /// <summary>
    /// Job number
    /// </summary>
    [SugarColumn(ColumnDescription = "Job number", Length = 32)]
    [MaxLength(32)]
    public string? JobNum { get; set; }

    /// <summary>
    /// Rank
    /// </summary>
    [SugarColumn(ColumnDescription = "Rank", Length = 32)]
    [MaxLength(32)]
    public string? PosLevel { get; set; }

    /// <summary>
    /// Joining date
    /// </summary>
    [SugarColumn(ColumnDescription = "Date of Joining")]
    public DateTime? JoinDate { get; set; }
}