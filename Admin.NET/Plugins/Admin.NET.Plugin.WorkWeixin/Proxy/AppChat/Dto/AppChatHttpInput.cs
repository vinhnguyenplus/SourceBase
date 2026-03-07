// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy;

/// <summary>
/// Create group chat session input parameters
/// </summary>
public class CreatAppChatInput
{
    /// <summary>
    /// Group name
    /// </summary>
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Group name cannot be empty"), MaxLength(50, ErrorMessage = "The group name cannot exceed 50 characters")]
    public string Name { get; set; }

    /// <summary>
    /// Group owner ID
    /// </summary>
    [JsonProperty("owner")]
    [JsonPropertyName("owner")]
    [Required(ErrorMessage = "group ownerId cannot be empty")]
    public string Owner { get; set; }

    /// <summary>
    /// Group member ID list
    /// </summary>
    [JsonProperty("userlist")]
    [JsonPropertyName("userlist")]
    [Core.NotEmpty(ErrorMessage = "The group member list cannot be empty")]
    public List<string> UserList { get; set; }

    /// <summary>
    /// GroupId
    /// </summary>
    [JsonProperty("chatid")]
    [JsonPropertyName("chatid")]
    [Required(ErrorMessage = "Group ID cannot be empty"), MaxLength(32, ErrorMessage = "The group ID cannot exceed 32 characters at most")]
    public string ChatId { get; set; }
}

/// <summary>
/// Modify group chat session input parameters
/// </summary>
public class UpdateAppChatInput
{
    /// <summary>
    /// GroupId
    /// </summary>
    [JsonProperty("chatid")]
    [JsonPropertyName("chatid")]
    [Required(ErrorMessage = "Group ID cannot be empty"), MaxLength(32, ErrorMessage = "The group ID cannot exceed 32 characters at most")]
    public string ChatId { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Group name cannot be empty"), MaxLength(50, ErrorMessage = "The group name cannot exceed 50 characters")]
    public string Name { get; set; }

    /// <summary>
    /// Group owner ID
    /// </summary>
    [JsonProperty("owner")]
    [JsonPropertyName("owner")]
    [Required(ErrorMessage = "group ownerId cannot be empty")]
    public string Owner { get; set; }

    /// <summary>
    /// Add member id list
    /// </summary>
    [JsonProperty("add_user_list")]
    [JsonPropertyName("add_user_list")]
    public List<string> AddUserList { get; set; }

    /// <summary>
    /// List of ids of kicked members
    /// </summary>
    [JsonProperty("del_user_list")]
    [JsonPropertyName("del_user_list")]
    public List<string> DelUserList { get; set; }
}

/// <summary>
/// Apply message push input base class parameters
/// </summary>
public class SendBaseAppChatInput
{
    /// <summary>
    /// GroupId
    /// </summary>
    [JsonProperty("chatid")]
    [JsonPropertyName("chatid")]
    [Required(ErrorMessage = "Group ID cannot be empty"), MaxLength(32, ErrorMessage = "The group ID cannot exceed 32 characters at most")]
    public string ChatId { get; set; }

    /// <summary>
    /// Message type
    /// </summary>
    /// <example>text: text message</example>
    /// <example>image: picture message</example>
    /// <example>voice: picture message</example>
    /// <example>video: video message</example>
    /// <example>file: file message</example>
    /// <example>textcard: text card</example>
    /// <example>news: graphic news</example>
    /// <example>mpnews: graphic news (stored in corporate WeChat)</example>
    /// <example>markdown: markdown message</example>
    [JsonProperty("msgtype")]
    [JsonPropertyName("msgtype")]
    [Required(ErrorMessage = "Message type cannot be empty")]
    protected string MsgType { get; set; }

    /// <summary>
    /// Is it confidential information?
    /// </summary>
    [JsonProperty("safe")]
    [JsonPropertyName("safe")]
    [Required(ErrorMessage = "Message type cannot be empty")]
    public int Safe { get; set; }

    public SendBaseAppChatInput(string chatId, string msgType, bool safe = false)
    {
        ChatId = chatId;
        MsgType = msgType;
        Safe = safe ? 1 : 0;
    }
}

/// <summary>
/// Push text message input parameters
/// </summary>
public class SendTextAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("text")]
    [JsonPropertyName("text")]
    public object Text { get; set; }

    /// <summary>
    /// text message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="content"></param>
    /// <param name="safe"></param>
    public SendTextAppChatInput(string chatId, string content, bool safe = false) : base(chatId, "text", safe)
    {
        Text = new { content };
    }
}

/// <summary>
/// Push picture message input parameters
/// </summary>
public class SendImageAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("image")]
    [JsonPropertyName("image")]
    public object Image { get; set; }

    /// <summary>
    /// Picture message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="mediaId"></param>
    /// <param name="safe"></param>
    public SendImageAppChatInput(string chatId, string mediaId, bool safe = false) : base(chatId, "image", safe)
    {
        Image = new { media_id = mediaId };
    }
}

/// <summary>
/// Push voice message input parameters
/// </summary>
public class SendVoiceAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("voice")]
    [JsonPropertyName("voice")]
    public object Voice { get; set; }

    /// <summary>
    /// voice message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="mediaId"></param>
    /// <param name="safe"></param>
    public SendVoiceAppChatInput(string chatId, string mediaId, bool safe = false) : base(chatId, "voice", safe)
    {
        Voice = new { media_id = mediaId };
    }
}

/// <summary>
/// Push video message input parameters
/// </summary>
public class SendVideoAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("video")]
    [JsonPropertyName("video")]
    public object Video { get; set; }

    /// <summary>
    /// video message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title"></param>
    /// <param name="description"></param>
    /// <param name="mediaId"></param>
    /// <param name="safe"></param>
    public SendVideoAppChatInput(string chatId, string title, string description, string mediaId, bool safe = false) : base(chatId, "video", safe)
    {
        Video = new
        {
            media_id = mediaId,
            description,
            title
        };
    }
}

/// <summary>
/// Push video message input parameters
/// </summary>
public class SendFileAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("file")]
    [JsonPropertyName("file")]
    public object File { get; set; }

    /// <summary>
    /// file message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="mediaId"></param>
    /// <param name="safe"></param>
    public SendFileAppChatInput(string chatId, string mediaId, bool safe = false) : base(chatId, "video", safe)
    {
        File = new { media_id = mediaId };
    }
}

/// <summary>
/// Push text card message input parameters
/// </summary>
public class SendTextCardAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("textcard")]
    [JsonPropertyName("textcard")]
    public object TextCard { get; set; }

    /// <summary>
    /// text card message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title">title</param>
    /// <param name="description">describe</param>
    /// <param name="url">Links that jump after clicking</param>
    /// <param name="btnTxt">button text</param>
    /// <param name="safe"></param>
    public SendTextCardAppChatInput(string chatId, string title, string description, string url, string btnTxt, bool safe = false) : base(chatId, "textcard", safe)
    {
        TextCard = new
        {
            title,
            description,
            url,
            btntxt = btnTxt
        };
    }
}

/// <summary>
/// Graphic message items
/// </summary>
public class SendNewsItem
{
    /// <summary>
    /// title
    /// </summary>
    [JsonProperty("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// describe
    /// </summary>
    [JsonProperty("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// describe
    /// </summary>
    [JsonProperty("url")]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// Picture link of graphic message (recommended large picture 1068 * 455, small picture 150 * 150)
    /// </summary>
    [JsonProperty("picurl")]
    [JsonPropertyName("picurl")]
    public string PicUrl { get; set; }
}

/// <summary>
/// Push graphic message input parameters
/// </summary>
public class SendNewsAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("news")]
    [JsonPropertyName("news")]
    public object News { get; set; }

    /// <summary>
    /// Graphic message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="newsList">Graphic message list</param>
    /// <param name="safe"></param>
    public SendNewsAppChatInput(string chatId, List<SendNewsItem> newsList, bool safe = false) : base(chatId, "news", safe)
    {
        News = new { articles = newsList };
    }
}

/// <summary>
/// Graphic message items
/// </summary>
public class SendMpNewsItem
{
    /// <summary>
    /// title
    /// </summary>
    [JsonProperty("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Thumbnail media_id
    /// </summary>
    [JsonProperty("thumb_media_id")]
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; }

    /// <summary>
    /// author
    /// </summary>
    [JsonProperty("author")]
    [JsonPropertyName("author")]
    public string Author { get; set; }

    /// <summary>
    /// Click the page link after "Read the original text"
    /// </summary>
    [JsonProperty("content_source_url")]
    [JsonPropertyName("content_source_url")]
    public string ContentSourceUrl { get; set; }

    /// <summary>
    /// Contents of graphic messages
    /// </summary>
    [JsonProperty("content")]
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// Description of graphic message
    /// </summary>
    [JsonProperty("digest")]
    [JsonPropertyName("digest")]
    public string Digest { get; set; }
}

/// <summary>
/// Push graphic messages (stored in corporate WeChat) input parameters
/// </summary>
public class SendMpNewsAppChatInput : SendBaseAppChatInput
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonProperty("mpnews")]
    [JsonPropertyName("mpnews")]
    public object MpNews { get; set; }

    /// <summary>
    /// Graphic message
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="mpNewsList">Graphic message list</param>
    /// <param name="safe"></param>
    public SendMpNewsAppChatInput(string chatId, List<SendMpNewsItem> mpNewsList, bool safe = false) : base(chatId, "mpnews", safe)
    {
        MpNews = new { articles = mpNewsList };
    }
}