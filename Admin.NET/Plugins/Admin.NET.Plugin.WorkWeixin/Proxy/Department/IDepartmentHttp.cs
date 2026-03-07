// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy.AppChat;

/// <summary>
/// Department remote calling service
/// </summary>
public interface IDepartmentHttp : IHttpDeclarative
{
    /// <summary>
    /// Create department
    /// https://developer.work.weixin.qq.com/document/path/90205
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/department/create")]
    Task<BaseWorkIdOutput> Create([Query("access_token")] string accessToken, [Body] DepartmentHttpInput body);

    /// <summary>
    /// Modify department
    /// https://developer.work.weixin.qq.com/document/path/90206
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/department/update")]
    Task<BaseWorkOutput> Update([Query("access_token")] string accessToken, [Body] DepartmentHttpInput body);

    /// <summary>
    /// Delete department
    /// https://developer.work.weixin.qq.com/document/path/90207
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/delete")]
    Task<BaseWorkOutput> Delete([Query("access_token")] string accessToken, [Query] long id);

    /// <summary>
    /// Get a list of department IDs
    /// https://developer.work.weixin.qq.com/document/path/90208
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/simplelist")]
    Task<DepartmentIdOutput> SimpleList([Query("access_token")] string accessToken, [Query] long id);

    /// <summary>
    /// Get department details
    /// https://developer.work.weixin.qq.com/document/path/90208
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/get")]
    Task<DepartmentOutput> Get([Query("access_token")] string accessToken, [Query] long id);
}