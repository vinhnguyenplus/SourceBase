// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Global normalization results
/// </summary>
[UnifyModel(typeof(AdminResult<>))]
public class AdminResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// JWT authorization exception return value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnAuthorizeException(DefaultHttpContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(RESTfulResult(metadata.StatusCode, data: metadata.Data, msg: metadata.Errors), UnifyContext.GetSerializerSettings(context));
    }

    /// <summary>
    /// Exception return value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(RESTfulResult(metadata.StatusCode, data: metadata.Data, msg: metadata.Errors), UnifyContext.GetSerializerSettings(context));
    }

    /// <summary>
    /// Successful return value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(RESTfulResult(StatusCodes.Status200OK, true, data), UnifyContext.GetSerializerSettings(context));
    }

    /// <summary>
    /// Verification failure return value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(RESTfulResult(metadata.StatusCode ?? StatusCodes.Status400BadRequest, data: metadata.Data, msg: metadata.ValidationResult), UnifyContext.GetSerializerSettings(context));
    }

    /// <summary>
    /// Specific status code return value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="unifyResultSettings"></param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
    {
        // Set response status code
        UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

        switch (statusCode)
        {
            // Handling 401 status code
            case StatusCodes.Status401Unauthorized:
                var msg = "401 Login has expired, please log in again";
                // If there is an authentication failure message, the message content is returned.
                if (context.Items.TryGetValue(SignatureAuthenticationDefaults.AuthenticateFailMsgKey, out var authFailMsg))
                    msg = authFailMsg + "";
                await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, msg: msg),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // Handling 403 status codes
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, msg: "403 Forbidden, no permission"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // Handling 302 status code
            case StatusCodes.Status302Found:
                if (context.Response.Headers.TryGetValue("Location", out var redirectUrl))
                {
                    context.Response.Redirect(redirectUrl);
                }
                else
                {
                    var errorMessage = "302 redirection failed, no Location header provided";
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, msg: errorMessage),
                        App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                }
                break;
        }
    }

    /// <summary>
    /// Return successful result set
    /// </summary>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static AdminResult<object> Ok(string message, object data = default)
    {
        return RESTfulResult(StatusCodes.Status200OK, true, data, message);
    }

    /// <summary>
    /// Return failure result set
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static AdminResult<object> Error(string message, int code = StatusCodes.Status400BadRequest, object data = default)
    {
        return RESTfulResult(code, false, data, message);
    }

    /// <summary>
    /// Return RESTful style result set
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="succeeded"></param>
    /// <param name="data"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private static AdminResult<object> RESTfulResult(int statusCode, bool succeeded = default, object data = default, object msg = default)
    {
        //// Unified return value desensitization processing
        //if (data?.GetType() == typeof(String))
        //{
        //    data = App.GetRequiredService<ISensitiveDetectionProvider>().ReplaceAsync(data.ToString(), '*').GetAwaiter().GetResult();
        //}
        //else if (data?.GetType() == typeof(JsonResult))
        //{
        //    data = App.GetRequiredService<ISensitiveDetectionProvider>().ReplaceAsync(JSON.Serialize(data), '*').GetAwaiter().GetResult();
        //}

        return new AdminResult<object>
        {
            Code = statusCode,
            Message = msg is null or string ? (msg + "") : JSON.Serialize(msg),
            Result = data,
            Type = succeeded ? "success" : "error",
            Extras = UnifyContext.Take(),
            Time = DateTime.Now
        };
    }
}

/// <summary>
/// Global return results
/// </summary>
/// <typeparam name="T"></typeparam>
public class AdminResult<T>
{
    /// <summary>
    /// status code
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Type success, warning, error
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// error message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// data
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    public object Extras { get; set; }

    /// <summary>
    /// time
    /// </summary>
    public DateTime Time { get; set; }
}