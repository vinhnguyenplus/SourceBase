// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkCreateAndDeliverInput
{
    /// <summary>
    /// The userId of the card creator
    /// </summary>
    public string? userId { get; set; }

    /// <summary>
    /// Card content template ID
    /// </summary>
    [Required]
    public string cardTemplateId { get; set; }

    /// <summary>
    /// External card instance ID
    /// </summary>
    [Required]
    public string outTrackId { get; set; }

    /// <summary>
    /// Type of card callback: STREAM: stream mode HTTP: http mode
    /// </summary>
    public string? callbackType { get; set; }

    /// <summary>
    /// Routing Key when the card calls back in HTTP mode, used to query the registered callbackUrl.
    /// </summary>
    public string? callbackRouteKey { get; set; }

    /// <summary>
    /// card data
    /// </summary>
    [Required]
    public DingTalk_CardData cardData { get; set; }

    /// <summary>
    /// User's private data
    /// </summary>
    public PrivateData? crivateData { get; set; }

    /// <summary>
    /// Dynamic data source configuration
    /// </summary>
    public OpenDynamicDataConfig? openDynamicDataConfig { get; set; }

    /// <summary>
    /// IM single chat cool application field information
    /// </summary>
    public OpenSpaceModel? imSingleOpenSpaceModel { get; set; }

    /// <summary>
    /// IM group chat field information.
    /// </summary>
    public OpenSpaceModel? imGroupOpenSpaceModel { get; set; }

    /// <summary>
    /// IM robot single chat field information.
    /// </summary>
    public OpenSpaceModel? imRobotOpenSpaceModel { get; set; }

    /// <summary>
    /// Collaboration field information
    /// </summary>
    public OpenSpaceModel? coFeedOpenSpaceModel { get; set; }

    /// <summary>
    /// Ceiling area information
    /// </summary>
    public OpenSpaceModel? topOpenSpaceModel { get; set; }

    /// <summary>
    /// Represents the field and its field id
    /// </summary>
    /// <remarks>
    /// Its format is dtv1.card//spaceType1.spaceId1;spaceType2.spaceId2_1;spaceType2.spaceId2_2;spaceType3.spaceId3
    /// </remarks>
    [Required]
    public string openSpaceId { get; set; }

    /// <summary>
    /// Shanliaoku application field delivery parameters.
    /// </summary>
    public DingTalkOpenDeliverModel? imSingleOpenDeliverModel { get; set; }

    /// <summary>
    /// Group chat delivery parameters.
    /// </summary>
    public DingTalkOpenDeliverModel? imGroupOpenDeliverModel { get; set; }

    /// <summary>
    /// IM robot single chat delivery parameters.
    /// </summary>
    public DingTalkOpenDeliverModel? imRobotOpenDeliverModel { get; set; }

    /// <summary>
    /// Ceiling delivery parameters.
    /// </summary>
    public DingTalkOpenDeliverModel? topOpenDeliverModel { get; set; }

    /// <summary>
    /// Collaborative delivery parameters.
    /// </summary>
    public DingTalkOpenDeliverModel? coFeedOpenDeliverModel { get; set; }

    /// <summary>
    /// Document delivery parameters
    /// </summary>
    public DingTalkOpenDeliverModel? docOpenDeliverModel { get; set; }

    /// <summary>
    /// User userId type: 1 (default): userId mode 2: unionId mode
    /// </summary>
    public int UserIdType { get; set; }
}

public class DingTalk_CardData
{
    public DingTalk_CardParamMap cardParamMap { get; set; }
}

/// <summary>
/// Card template content replacement parameters
/// </summary>
public class DingTalk_CardParamMap
{
    /// <summary>
    /// Film template content replacement parameters
    /// </summary>
    [Newtonsoft.Json.JsonProperty("sys_full_json_obj")]
    [System.Text.Json.Serialization.JsonPropertyName("sys_full_json_obj")]
    public string sysFullJsonObj { get; set; }
}

public class PrivateData
{
    public Dictionary<string, DingTalk_CardParamMap> key { get; set; } = new Dictionary<string, DingTalk_CardParamMap>();
}

public class OpenDynamicDataConfig
{
    /// <summary>
    /// Dynamic data source configuration list.
    /// </summary>
    public List<DynamicDataSourceConfig>? dynamicDataSourceConfigs { get; set; }
}

public class DynamicDataSourceConfig
{
    /// <summary>
    /// The unique ID of the data source, specified by the caller.
    /// </summary>
    public string? dynamicDataSourceId { get; set; }

    /// <summary>
    /// Fixed parameters returned when calling back the data source. Example
    /// </summary>
    public Dictionary<string, string>? constParams { get; set; }

    /// <summary>
    /// Data source pull configuration.
    /// </summary>
    public PullConfig? pullConfig { get; set; }
}

public class PullConfig
{
    /// <summary>
    /// Pull strategy, optional values: NONE: no pull, no dynamic data INTERVAL: pull interval ONCE: pull only once
    /// </summary>
    public string pullStrategy { get; set; }

    /// <summary>
    /// The interval between pulls.
    /// </summary>
    public int interval { get; set; }

    /// <summary>
    /// The unit of the pulled interval time, optional values: SECONDS: seconds MINUTES: minutes HOURS: hours DAYS: days
    /// </summary>
    public string timeUnit { get; set; }
}

public class OpenSpaceModel
{
    /// <summary>
    /// Ceiling field attribute, by adding spaeType, the card supports the ceiling field.
    /// </summary>
    public string? spaceType { get; set; }

    /// <summary>
    /// Card title.
    /// </summary>
    public string? title { get; set; }

    /// <summary>
    /// Cool Apps for Coding.
    /// </summary>
    public string? coolAppCode { get; set; }

    /// <summary>
    /// Whether to support forwarding, the default is false.
    /// </summary>
    public bool? supportForward { get; set; }

    /// <summary>
    /// Supports internationalized LastMessage.
    /// </summary>
    public Dictionary<string, string>? lastMessageI18n { get; set; }

    /// <summary>
    /// Supports searchable fields in card messages.
    /// </summary>
    public SearchSupport? searchSupport { get; set; }

    /// <summary>
    /// Notification information.
    /// </summary>
    public Notification? notification { get; set; }
}

public class SearchSupport
{
    /// <summary>
    /// Type of icon for search display.
    /// </summary>
    public string searchIcon { get; set; }

    /// <summary>
    /// Card type name.
    /// </summary>
    public string searchTypeName { get; set; }

    /// <summary>
    /// Field for message display and search.
    /// </summary>
    public string searchDesc { get; set; }
}

public class Notification
{
    /// <summary>
    /// Field for message display and search.
    /// </summary>
    public string alertContent { get; set; }

    /// <summary>
    /// Whether to turn off push notifications: true: turn off false: do not turn off
    /// </summary>
    public bool notificationOff { get; set; }
}

public class DingTalkOpenDeliverModel
{
    /// <summary>
    /// Bot coding for sending cards.
    /// </summary>
    public string robotCode { get; set; }

    /// <summary>
    /// Message@people. Format: {"key":"value"}. key: userId of the user value: username
    /// </summary>
    public Dictionary<string, string> atUserIds { get; set; }

    /// <summary>
    /// Specify the userId of the recipient.
    /// </summary>
    public List<string> recipients { get; set; }

    /// <summary>
    /// Extended fields, examples are as follows: {"key":"value"}
    /// </summary>
    public Dictionary<string, string> extension { get; set; }

    /// <summary>
    /// If no other delivery attributes are set for IM robot private chat, spaeType needs to be set to IM_ROBOT.
    /// </summary>
    public string spaceType { get; set; }

    /// <summary>
    /// Expiration timestamp. If using the topOpenDeliverModel object, this field is required.
    /// </summary>
    public long expiredTimeMillis { get; set; }

    /// <summary>
    /// You can view the userId of the ceiling card.
    /// </summary>
    public List<string> userIds { get; set; }

    /// <summary>
    /// Devices that can view the ceiling card: android | ios | win | mac.
    /// </summary>
    public List<string> platforms { get; set; }

    /// <summary>
    /// Business identification.
    /// </summary>
    public string bizTag { get; set; }

    /// <summary>
    /// Sorting time in the collaborative field.
    /// </summary>
    public long gmtTimeLine { get; set; }

    /// <summary>
    /// Employee userId information
    /// </summary>
    public string userId { get; set; }
}