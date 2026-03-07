// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;
using System.Security.Claims;

namespace Admin.NET.Core;

/// <summary>
/// Prevent duplicate request filter feature (this feature uses distributed locks, you need to ensure that the system supports distributed locks)
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class IdempotentAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Request interval time/second
    /// </summary>
    public int IntervalTime { get; set; } = 5;

    /// <summary>
    /// Error message content
    /// </summary>
    public string Message { get; set; } = "You are operating too frequently, please try again later!";

    /// <summary>
    /// Cache prefix: Key+request route+userId+request parameter
    /// </summary>
    public string CacheKey { get; set; } = CacheConst.KeyIdempotent;

    /// <summary>
    /// Whether to throw an exception directly: True, False returns the result of the last request
    /// </summary>
    public bool ThrowBah { get; set; }

    /// <summary>
    /// lock prefix
    /// </summary>
    public string LockPrefix { get; set; } = "lock_";

    public IdempotentAttribute()
    {
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var path = httpContext.Request.Path.Value.ToString();
        var userId = httpContext.User?.FindFirstValue(ClaimConst.UserId);
        var cacheExpireTime = TimeSpan.FromSeconds(IntervalTime);

        var parameters = JsonConvert.SerializeObject(context.ActionArguments, Formatting.None, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include
        });

        var cacheKey = CacheKey + MD5Encryption.Encrypt($"{path}{userId}{parameters}");
        var sysCacheService = httpContext.RequestServices.GetService<SysCacheService>();
        try
        {
            // Distributed lock
            using var distributedLock = sysCacheService.BeginCacheLock($"{LockPrefix}{cacheKey}") ?? throw Oops.Oh(Message);

            var cacheValue = sysCacheService.Get<ResponseData>(cacheKey);
            if (cacheValue != null)
            {
                if (ThrowBah) throw Oops.Oh(Message);
                context.Result = new ObjectResult(cacheValue.Value);
                return;
            }
            else
            {
                var resultContext = await next();
                // Cache request results, null values ​​are not cached
                if (resultContext.Result is ObjectResult { Value: { } } objectResult)
                {
                    var typeName = objectResult.Value.GetType().Name;
                    var responseData = new ResponseData
                    {
                        Type = typeName,
                        Value = objectResult.Value
                    };
                    sysCacheService.Set(cacheKey, responseData, cacheExpireTime);
                }
            }
        }
        catch (Exception ex)
        {
            throw Oops.Oh($"{Message}-{ex}");
        }
    }

    /// <summary>
    /// Request result data
    /// </summary>
    private class ResponseData
    {
        /// <summary>
        /// result type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Request results
        /// </summary>
        public dynamic Value { get; set; }
    }
}