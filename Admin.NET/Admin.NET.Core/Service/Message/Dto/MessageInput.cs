// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public class MessageInput
{
    /// <summary>
    /// Receiver UserId
    /// </summary>
    public long ReceiveUserId { get; set; }

    /// <summary>
    /// Receiver name
    /// </summary>
    public string ReceiveUserName { get; set; }

    /// <summary>
    /// User ID list
    /// </summary>
    public List<long> UserIds { get; set; }

    /// <summary>
    /// Message title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Message type
    /// </summary>
    public MessageTypeEnum MessageType { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// SenderId
    /// </summary>
    public string SendUserId { get; set; }

    /// <summary>
    /// Sender name
    /// </summary>
    public string SendUserName { get; set; }

    /// <summary>
    /// Send time
    /// </summary>
    public DateTime SendTime { get; set; }
}