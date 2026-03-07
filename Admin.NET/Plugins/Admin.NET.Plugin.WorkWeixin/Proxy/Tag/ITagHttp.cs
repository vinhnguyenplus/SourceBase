// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy;

/// <summary>
/// Label remote call service
/// </summary>
public interface ITagHttp : IHttpDeclarative
{
    /// <summary>
    /// Create tags
    /// https://developer.work.weixin.qq.com/document/path/90210
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/create")]
    Task<BaseWorkIdOutput> Create([Query("access_token")] string accessToken, [Body] TagHttpInput body);

    /// <summary>
    /// Update tag name
    /// https://developer.work.weixin.qq.com/document/path/90211
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/update")]
    Task<TagIdHttpOutput> Update([Query("access_token")] string accessToken, [Body] TagHttpInput body);

    /// <summary>
    /// Delete tag
    /// https://developer.work.weixin.qq.com/document/path/90212
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/delete")]
    Task<BaseWorkOutput> Delete([Query("access_token")] string accessToken, [Query("tagid")] long tagId);

    /// <summary>
    /// Get label details
    /// https://developer.work.weixin.qq.com/document/path/90213
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/get")]
    Task<DepartmentOutput> Get([Query("access_token")] string accessToken, [Query("tagid")] long tagId);

    /// <summary>
    /// Add tag members
    /// https://developer.work.weixin.qq.com/document/path/90214
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/addtagusers")]
    Task<DepartmentOutput> AddTagUsers([Query("access_token")] string accessToken, [Body] TagUsersTagInput body);

    /// <summary>
    /// Delete tag members
    /// https://developer.work.weixin.qq.com/document/path/90215
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/deltagusers")]
    Task<DepartmentOutput> DelTagUsers([Query("access_token")] string accessToken, [Body] TagUsersTagInput body);

    /// <summary>
    /// Get tag list
    /// https://developer.work.weixin.qq.com/document/path/90216
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/list")]
    Task<TagListHttpOutput> List([Query("access_token")] string accessToken);
}