// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Admin.NET.Core.Service;

/// <summary>
/// System general services 🧩
/// </summary>
[ApiDescriptionSettings(Order = 101)]
[AllowAnonymous]
public class SysCommonService : IDynamicApiController, ITransient
{
    private readonly IApiDescriptionGroupCollectionProvider _apiProvider;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly CDConfigOptions _cdConfigOptions;
    private readonly UserManager _userManager;
    private readonly HttpClient _httpClient;

    public SysCommonService(IApiDescriptionGroupCollectionProvider apiProvider,
        SqlSugarRepository<SysUser> sysUserRep,
        IOptions<CDConfigOptions> giteeOptions,
        IHttpClientFactory httpClientFactory,
        UserManager userManager)
    {
        _sysUserRep = sysUserRep;
        _apiProvider = apiProvider;
        _userManager = userManager;
        _cdConfigOptions = giteeOptions.Value;
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Obtain the national secret public key and private key pair 🏆
    /// </summary>
    /// <returns></returns>
    [DisplayName("Obtain the national secret public key and private key pair")]
    public SmKeyPairOutput GetSmKeyPair()
    {
        var kp = GM.GenerateKeyPair();
        var privateKey = Hex.ToHexString(((ECPrivateKeyParameters)kp.Private).D.ToByteArray()).ToUpper();
        var publicKey = Hex.ToHexString(((ECPublicKeyParameters)kp.Public).Q.GetEncoded()).ToUpper();

        return new SmKeyPairOutput
        {
            PrivateKey = privateKey,
            PublicKey = publicKey,
        };
    }

    /// <summary>
    /// Get all interfaces/dynamic APIs 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get all interfaces/dynamic APIs")]
    public List<ApiOutput> GetApiList()
    {
        var apiList = new List<ApiOutput>();
        foreach (var item in _apiProvider.ApiDescriptionGroups.Items)
        {
            foreach (var apiDescription in item.Items)
            {
                var displayName = apiDescription.TryGetMethodInfo(out MethodInfo apiMethodInfo) ? apiMethodInfo.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName : "";

                apiList.Add(new ApiOutput
                {
                    GroupName = item.GroupName,
                    DisplayName = displayName,
                    RouteName = apiDescription.RelativePath
                });
            }
        }
        return apiList;
    }

    /// <summary>
    /// Download temporary Excel with errors flagged (global)
    /// </summary>
    /// <returns></returns>
    [DisplayName("Download Temporarily Marked Incorrect Excel (Global)")]
    public async Task<IActionResult> DownloadErrorExcelTemp([FromQuery] string fileName = null)
    {
        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        var resultStream = App.GetRequiredService<SysCacheService>().Get<MemoryStream>(CacheConst.KeyExcelTemp + userId);

        if (resultStream == null) throw Oops.Oh("The error flag file is expired.");

        return await Task.FromResult(new FileStreamResult(resultStream, "application/octet-stream")
        {
            FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? "mistakeMark＿" + DateTime.Now.ToString("yyyyMMddhhmmss") : fileName)}.xlsx"
        });
    }

    /// <summary>
    /// Encrypted string 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("Encrypted string")]
    public dynamic EncryptPlainText([Required] string plainText)
    {
        return CryptogramUtil.Encrypt(plainText);
    }

    /// <summary>
    /// Interface pressure test 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Interface pressure test")]
    public async Task<StressTestOutput> StressTest(StressTestInput input)
    {
        // Restrict that only super-admin users can use this function
        if (!_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.SA001);

        var stopwatch = new Stopwatch();
        var responseTimes = new List<double>();  // Response time collection
        input.RequestMethod = input.RequestMethod.ToUpper();
        long totalRequests = 0, successfulRequests = 0, failedRequests = 0;

        stopwatch.Start();
        var semaphore = new SemaphoreSlim(input.MaxDegreeOfParallelism!.Value > 0 ? input.MaxDegreeOfParallelism.Value : Environment.ProcessorCount);

        #region ParameterConstruct

        // Build base URI (excluding path and query parameters)
        var baseUriBuilder = new UriBuilder(input.RequestUri);
        var queryString = HttpUtility.ParseQueryString(baseUriBuilder.Query);

        // Replace path parameter to baseUriBuilder.Path
        foreach (var param in input.PathParameters)
        {
            baseUriBuilder.Path = baseUriBuilder.Path.Replace($"{{{param.Key}}}", param.Value, StringComparison.OrdinalIgnoreCase);
        }

        // Build Query parameters
        foreach (var param in input.QueryParameters)
        {
            queryString[param.Key] = param.Value;
        }

        baseUriBuilder.Query = queryString.ToString() ?? string.Empty;
        var fullUri = baseUriBuilder.Uri;

        // Create a one-time HttpRequestMessage template
        HttpRequestMessage requestTemplate = CreateRequestMessage(input, fullUri);

        #endregion ParameterConstruct

        var tasks = Enumerable.Range(0, input.NumberOfRounds!.Value * input.NumberOfRequests!.Value).Select(async _ =>
        {
            await semaphore.WaitAsync();
            try
            {
                var requestStopwatch = new Stopwatch();
                requestStopwatch.Start();

                using (var request = requestTemplate.DeepCopy())
                {
                    if (!string.Equals(input.RequestMethod, "GET", StringComparison.OrdinalIgnoreCase) && input.RequestParameters.Any())
                    {
                        var content = new FormUrlEncodedContent(input.RequestParameters);
                        request.Content = content;
                    }

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode(); // Throws error status code exception

                        requestStopwatch.Stop();
                        responseTimes.Add(requestStopwatch.Elapsed.TotalMilliseconds);
                        Interlocked.Increment(ref successfulRequests);
                    }
                }
            }
            catch
            {
                Interlocked.Increment(ref failedRequests);
            }
            finally
            {
                Interlocked.Increment(ref totalRequests);
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        var totalTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
        var qps = totalTimeInSeconds > 0 ? totalRequests / totalTimeInSeconds : 0;
        var orderResponseTimes = responseTimes.OrderBy(t => t).ToList();
        var averageResponseTime = responseTimes.Any() ? responseTimes.Average() : 0;
        var minResponseTime = responseTimes.Any() ? responseTimes.Min() : 0;
        var maxResponseTime = responseTimes.Any() ? responseTimes.Max() : 0;

        return new StressTestOutput
        {
            TotalRequests = totalRequests,
            TotalTimeInSeconds = totalTimeInSeconds,
            SuccessfulRequests = successfulRequests,
            FailedRequests = failedRequests,
            QueriesPerSecond = qps,
            MinResponseTime = minResponseTime,
            MaxResponseTime = maxResponseTime,
            AverageResponseTime = averageResponseTime,
            Percentile10ResponseTime = CalculatePercentile(orderResponseTimes, 0.1),
            Percentile25ResponseTime = CalculatePercentile(orderResponseTimes, 0.25),
            Percentile50ResponseTime = CalculatePercentile(orderResponseTimes, 0.5),
            Percentile75ResponseTime = CalculatePercentile(orderResponseTimes, 0.75),
            Percentile90ResponseTime = CalculatePercentile(orderResponseTimes, 0.9),
            Percentile99ResponseTime = CalculatePercentile(orderResponseTimes, 0.99),
            Percentile999ResponseTime = CalculatePercentile(orderResponseTimes, 0.999)
        };
    }

    /// <summary>
    /// Create request message
    /// </summary>
    /// <param name="input">input parameters</param>
    /// <param name="fullUri">url</param>
    /// <returns></returns>
    private HttpRequestMessage CreateRequestMessage(StressTestInput input, Uri fullUri)
    {
        HttpRequestMessage request = input.RequestMethod switch
        {
            "GET" => new HttpRequestMessage(HttpMethod.Get, fullUri),
            "PUT" => new HttpRequestMessage(HttpMethod.Put, fullUri),
            "POST" => new HttpRequestMessage(HttpMethod.Post, fullUri),
            "DELETE" => new HttpRequestMessage(HttpMethod.Delete, fullUri),
            _ => throw Oops.Bah("Abnormal request method")
        };

        // Set request header
        foreach (var header in input.Headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
        return request;
    }

    /// <summary>
    /// Calculate percentile request time
    /// </summary>
    /// <param name="times">Request time-consuming list</param>
    /// <param name="percentile">percentile</param>
    /// <returns></returns>
    private double CalculatePercentile(List<double> times, double percentile)
    {
        if (!times.Any()) return 0;
        var index = (int)Math.Ceiling(percentile * times.Count) - 1;
        return times[index < times.Count ? index : times.Count - 1];
    }
}