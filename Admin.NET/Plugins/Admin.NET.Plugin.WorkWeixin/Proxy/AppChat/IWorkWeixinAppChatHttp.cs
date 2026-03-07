// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy.AppChat;

/// <summary>
/// Group chat session remote call service
/// </summary>
public interface IWorkWeixinAppChatHttp : IHttpDeclarative
{
    /// <summary>
    /// Create a group chat session
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <see href="https://developer.work.weixin.qq.com/document/path/90245"/>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/appchat/create")]
    Task<CreatAppChatOutput> Create([Query("access_token")] string accessToken, [Body] CreatAppChatInput body);

    /// <summary>
    /// Modify group chat session
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <see href="https://developer.work.weixin.qq.com/document/path/98913"/>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/appchat/update")]
    Task<CreatAppChatOutput> Update([Query("access_token")] string accessToken, [Body] UpdateAppChatInput body);

    /// <summary>
    /// Get group chat session
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <see href="https://developer.work.weixin.qq.com/document/path/98914"/>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/appchat/get")]
    Task<CreatAppChatOutput> Get([Query("access_token")] string accessToken, [Query("chatid")] string chatId);

    /// <summary>
    /// Application push message
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <see href="https://developer.work.weixin.qq.com/document/path/90248"/>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/appchat/send")]
    Task<BaseWorkOutput> Send([Query("access_token")] string accessToken, [Body] SendBaseAppChatInput body);
}