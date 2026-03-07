// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Notification announcement status enumeration
/// </summary>
[Description("Notification Announcement Status Enumeration")]
public enum NoticeStatusEnum
{
    /// <summary>
    /// draft
    /// </summary>
    [Description("draft"), Theme("info")]
    DRAFT = 0,

    /// <summary>
    /// release
    /// </summary>
    [Description("release")]
    PUBLIC = 1,

    /// <summary>
    /// withdraw
    /// </summary>
    [Description("Withdraw")]
    CANCEL = 2,

    /// <summary>
    /// delete
    /// </summary>
    [Description("Delete")]
    DELETED = 3
}