// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System notification announcement form
/// </summary>
[SugarTable(null, "System notification announcement form")]
[SysTable]
[SugarIndex("index_{table}_T", nameof(Type), OrderByType.Asc)]
public partial class SysNotice : EntityBase
{
    /// <summary>
    /// title
    /// </summary>
    [SugarColumn(ColumnDescription = "title", Length = 32)]
    [Required, MaxLength(32)]
    [SensitiveDetection('*')]
    public virtual string Title { get; set; }

    /// <summary>
    /// content
    /// </summary>
    [SugarColumn(ColumnDescription = "content", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    [Required]
    [SensitiveDetection('*')]
    public virtual string Content { get; set; }

    /// <summary>
    /// Type (1 notification 2 announcement)
    /// </summary>
    [SugarColumn(ColumnDescription = "Type (1 notification 2 announcement)")]
    public NoticeTypeEnum Type { get; set; }

    /// <summary>
    /// PublisherId
    /// </summary>
    [SugarColumn(ColumnDescription = "PublisherId")]
    public long PublicUserId { get; set; }

    /// <summary>
    /// Publisher name
    /// </summary>
    [SugarColumn(ColumnDescription = "Publisher name", Length = 32)]
    [MaxLength(32)]
    public string? PublicUserName { get; set; }

    /// <summary>
    /// Publisher ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Issuing Organization Id")]
    public long PublicOrgId { get; set; }

    /// <summary>
    /// Issuing organization name
    /// </summary>
    [SugarColumn(ColumnDescription = "Issuing organization name", Length = 64)]
    [MaxLength(64)]
    public string? PublicOrgName { get; set; }

    /// <summary>
    /// Release time
    /// </summary>
    [SugarColumn(ColumnDescription = "Release Time")]
    public DateTime? PublicTime { get; set; }

    /// <summary>
    /// Withdrawal time
    /// </summary>
    [SugarColumn(ColumnDescription = "Withdrawal Time")]
    public DateTime? CancelTime { get; set; }

    /// <summary>
    /// Status (0 draft 1 published 2 withdrawn 3 deleted)
    /// </summary>
    [SugarColumn(ColumnDescription = "state（0draft 1release 2Withdraw 3Delete）")]
    public NoticeStatusEnum Status { get; set; }
}