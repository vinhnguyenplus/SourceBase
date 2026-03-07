// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.K3Cloud.Service;

/// <summary>
/// Kingdee Cloud Starry Sky ERP Interface
/// </summary>
[HttpClientName("K3Cloud")]
public interface IK3CloudApi : IHttpDeclarative
{
    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUser.common.kdsvc")]
    Task<K3CloudLoginOutput> ValidateUser([Body] K3CloudLoginInput input, Action<HttpClient, HttpResponseMessage> action = default);

    /// <summary>
    /// save form
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save.common.kdsvc")]
    Task<K3CloudPushResultOutput> Save<T>([Body] K3CloudBaeInput<T> input, Action<HttpClient, HttpRequestMessage> action = default);

    /// <summary>
    /// Submit form
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Submit.common.kdsvc")]
    Task<K3CloudPushResultOutput> Submit<T>([Body] K3CloudBaeInput<T> input, Action<HttpClient, HttpRequestMessage> action = default);

    /// <summary>
    /// Review form
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Audit.common.kdsvc")]
    Task<K3CloudPushResultOutput> Audit<T>([Body] K3CloudBaeInput<T> input, Action<HttpClient, HttpRequestMessage> action = default);
}