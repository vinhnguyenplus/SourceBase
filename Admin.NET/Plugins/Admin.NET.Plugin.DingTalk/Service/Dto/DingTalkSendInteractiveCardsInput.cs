// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkSendInteractiveCardsInput
{
    /// <summary>
    /// Message template ID of interactive card
    /// </summary>
    [Required(ErrorMessage = "The message template ID of the interactive card is required!")]
    public string? CardTemplateId { get; set; }

    /// <summary>
    /// GroupId
    /// </summary>
    /// <remarks>
    /// 1. A group created based on a group template.
    /// For internal enterprise applications, call the create group interface to obtain the open_conversation_id parameter value.
    /// 2. Install the group chat cool application.
    /// For internal enterprise applications, the callback parameter OpenConversationId parameter value is obtained through the cool application installation event in the group.
    /// </remarks>
    public string OpenConversationId { get; set; }

    /// <summary>
    /// Recipient userId list
    /// </summary>
    /// <remarks>
    /// Single chat: receiverUserIdList fills in the user ID, the maximum value is 20.
    /// Group chat: fill in the user ID in receiverUserIdList, indicating that the current users in the group with the corresponding ID are visible
    /// If the receiverUserIdList parameter is not filled in, it means that all users in the current group are visible.
    /// </remarks>
    [Required(ErrorMessage = "Recipient userId list is required!")]
    public List<string>? ReceiverUserIdList { get; set; }

    /// <summary>
    /// An external encoding that uniquely identifies the card
    /// </summary>
    [Required(ErrorMessage = "The external code that uniquely identifies the card is required!")]
    public string? OutTrackId { get; set; }

    /// <summary>
    /// Robot coding
    /// </summary>
    public string RobotCode { get; set; }

    /// <summary>
    /// Sent session type
    /// </summary>
    [Required(ErrorMessage = "Conversation type is required!")]
    public DingTalkConversationTypeEnum? ConversationType { get; set; }

    /// <summary>
    /// Routing Key during card callback, used to query the registered callbackUrl
    /// </summary>
    public string CallbackRouteKey { get; set; }

    /// <summary>
    /// Card public data
    /// </summary>
    [Required(ErrorMessage = "Card public data is required!")]
    public DingTalkCardData CardData { get; set; }
}

public class GetDingTalkCardMessageReadStatusInput
{
    /// <summary>
    /// Robot coding
    /// </summary>
    public string RobotCode { set; get; }

    /// <summary>
    /// The unique identifier of the message can be obtained through the processQueryKey field in the return parameter of the robot message interface in the conversation between the batch sender and the robot.
    /// </summary>
    public string ProcessQueryKey { set; get; }
}

public class GetDingTalkCardMessageReadStatusOutput
{
    /// <summary>
    /// Message sending status, SUCCESS: successful, RECALLED: withdrawn, PROCESSING: processing
    /// </summary>
    public string SendStatus { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DingTalkCardMessageReadInfoList MessageReadInfoList { get; set; }
}

/// <summary>
/// DingTalk card message read status
/// </summary>
public class DingTalkCardMessageReadInfoList
{
    /// <summary>
    /// Message recipient name
    /// </summary>
    public string Name { set; get; }

    /// <summary>
    /// The userId of the message recipient
    /// </summary>
    public string UserId { set; get; }

    /// <summary>
    /// Read status, READ: read, UNREAD: unread
    /// </summary>
    public string ReadStatus { set; get; }
}