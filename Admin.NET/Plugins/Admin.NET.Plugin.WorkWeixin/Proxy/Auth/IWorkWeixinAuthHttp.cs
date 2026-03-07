// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy.AppChat;

/// <summary>
/// Authorization session remote service
/// </summary>
public interface IWorkWeixinAuthHttp : IHttpDeclarative
{
    /// <summary>
    /// Get interface credentials
    /// </summary>
    /// <param name="corpId">Enterprise ID</param>
    /// <param name="corpSecret">App's credential key</param>
    /// <returns></returns>
    /// <see href="https://developer.work.weixin.qq.com/document/path/91039"/>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/gettoken")]
    Task<AuthAccessTokenHttpOutput> GetToken([Query("corpid")] string corpId, [Query("corpsecret")] string corpSecret);
}