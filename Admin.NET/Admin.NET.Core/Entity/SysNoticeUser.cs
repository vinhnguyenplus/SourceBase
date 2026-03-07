// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System notification announcement user table
/// </summary>
[SugarTable(null, "System Notification Announcement User Table")]
[SysTable]
public partial class SysNoticeUser : EntityBaseId
{
    /// <summary>
    /// Notification Announcement ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Notices and AnnouncementsId")]
    public long NoticeId { get; set; }

    /// <summary>
    /// Notices and Announcements
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(NoticeId))]
    public SysNotice SysNotice { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// reading time
    /// </summary>
    [SugarColumn(ColumnDescription = "reading time")]
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// Status (0 unread 1 read)
    /// </summary>
    [SugarColumn(ColumnDescription = "Status (0 unread 1 read)")]
    public NoticeUserStatusEnum ReadStatus { get; set; } = NoticeUserStatusEnum.UNREAD;
}