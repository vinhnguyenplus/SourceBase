// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// WeChat API client
/// </summary>
public partial class WechatApiClientFactory : ISingleton
{
    private readonly IHttpClientFactory _httpClientFactory;
    public readonly WechatOptions _wechatOptions;
    private readonly SysCacheService _sysCacheService;

    public WechatApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<WechatOptions> wechatOptions, SysCacheService sysCacheService)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _wechatOptions = wechatOptions.Value ?? throw new ArgumentNullException(nameof(wechatOptions));
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// WeChat public account
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWechatClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WechatAppId) || string.IsNullOrEmpty(_wechatOptions.WechatAppSecret))
            throw Oops.Oh("WeChat Official Account configuration error");

        var client = WechatApiClientBuilder.Create(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WechatAppId,
            AppSecret = _wechatOptions.WechatAppSecret,
            PushToken = _wechatOptions.WechatToken,
            PushEncodingAESKey = _wechatOptions.WechatEncodingAESKey,
        })
        .UseHttpClient(_httpClientFactory.CreateClient(), disposeClient: false) // Set HttpClient not to be destroyed with the client
        .Build();

        client.Configure(config =>
        {
            JsonSerializerSettings jsonSerializerSettings = NewtonsoftJsonSerializer.GetDefaultSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            config.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings); // Specify System.Text.Json JSON serialization
                                                                                          // config.JsonSerializer = new SystemTextJsonSerializer(jsonSerializerOptions); // Specify Newtonsoft.Json JSON serialization
        });

        return client;
    }

    /// <summary>
    /// WeChat applet
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWxOpenClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WxOpenAppId) || string.IsNullOrEmpty(_wechatOptions.WxOpenAppSecret))
            throw Oops.Oh("WeChat applet configuration error");

        var client = WechatApiClientBuilder.Create(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WxOpenAppId,
            AppSecret = _wechatOptions.WxOpenAppSecret,
            PushToken = _wechatOptions.WxToken,
            PushEncodingAESKey = _wechatOptions.WxEncodingAESKey,
        })
        .UseHttpClient(_httpClientFactory.CreateClient(), disposeClient: false) // Set HttpClient not to be destroyed with the client
        .Build();

        client.Configure(config =>
        {
            JsonSerializerSettings jsonSerializerSettings = NewtonsoftJsonSerializer.GetDefaultSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            config.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings); // Specify System.Text.Json JSON serialization
                                                                                          // config.JsonSerializer = new SystemTextJsonSerializer(jsonSerializerOptions); // Specify Newtonsoft.Json JSON serialization
        });

        return client;
    }

    /// <summary>
    /// Obtain WeChat public account AccessToken
    /// </summary>
    /// <returns></returns>
    public async Task<string> TryGetWechatAccessTokenAsync()
    {
        if (!_sysCacheService.ExistKey($"WxAccessToken_{_wechatOptions.WechatAppId}") || string.IsNullOrEmpty(_sysCacheService.Get<string>($"WxAccessToken_{_wechatOptions.WechatAppId}")))
        {
            var client = CreateWechatClient();
            var reqCgibinToken = new CgibinTokenRequest();
            var resCgibinToken = await client.ExecuteCgibinTokenAsync(reqCgibinToken);
            if (resCgibinToken.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
                throw Oops.Oh(resCgibinToken.ErrorMessage + " " + resCgibinToken.ErrorCode);
            _sysCacheService.Set($"WxAccessToken_{_wechatOptions.WechatAppId}", resCgibinToken.AccessToken, TimeSpan.FromSeconds(resCgibinToken.ExpiresIn - 60));
        }

        return _sysCacheService.Get<string>($"WxAccessToken_{_wechatOptions.WechatAppId}");
    }

    /// <summary>
    /// Obtain WeChat applet AccessToken
    /// </summary>
    /// <returns></returns>
    public async Task<string> TryGetWxOpenAccessTokenAsync()
    {
        if (!_sysCacheService.ExistKey($"WxAccessToken_{_wechatOptions.WxOpenAppId}") || string.IsNullOrEmpty(_sysCacheService.Get<string>($"WxAccessToken_{_wechatOptions.WxOpenAppId}")))
        {
            var client = CreateWxOpenClient();
            var reqCgibinToken = new CgibinTokenRequest();
            var resCgibinToken = await client.ExecuteCgibinTokenAsync(reqCgibinToken);
            if (resCgibinToken.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
                throw Oops.Oh(resCgibinToken.ErrorMessage + " " + resCgibinToken.ErrorCode);
            _sysCacheService.Set($"WxAccessToken_{_wechatOptions.WxOpenAppId}", resCgibinToken.AccessToken, TimeSpan.FromSeconds(resCgibinToken.ExpiresIn - 60));
        }

        return _sysCacheService.Get<string>($"WxAccessToken_{_wechatOptions.WxOpenAppId}");
    }

    /// <summary>
    /// Check WeChat public account AccessToken
    /// </summary>
    /// <returns></returns>
    public async Task CheckWechatAccessTokenAsync()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WechatAppId) || string.IsNullOrEmpty(_wechatOptions.WechatAppSecret)) return;

        var req = new CgibinOpenApiQuotaGetRequest
        {
            AccessToken = await TryGetWechatAccessTokenAsync(),
            CgiPath = "/cgi-bin/token"
        };
        var client = CreateWechatClient();
        var res = await client.ExecuteCgibinOpenApiQuotaGetAsync(req);

        var originColor = Console.ForegroundColor;
        if (res.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
        {
            _sysCacheService.Remove($"WxAccessToken_{_wechatOptions.WechatAppId}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("【" + DateTime.Now + "】" + _wechatOptions.WxOpenAppId + " WeChat Official Account token is invalid");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("【" + DateTime.Now + "】" + _wechatOptions.WxOpenAppId + " WeChat Official Account token is valid");
        }
        Console.ForegroundColor = originColor;
    }

    /// <summary>
    /// Check WeChat applet AccessToken
    /// </summary>
    /// <returns></returns>
    public async Task CheckWxOpenAccessTokenAsync()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WxOpenAppId) || string.IsNullOrEmpty(_wechatOptions.WxOpenAppSecret)) return;

        var req = new CgibinOpenApiQuotaGetRequest
        {
            AccessToken = await TryGetWxOpenAccessTokenAsync(),
            CgiPath = "/cgi-bin/token"
        };
        var client = CreateWxOpenClient();
        var res = await client.ExecuteCgibinOpenApiQuotaGetAsync(req);

        var originColor = Console.ForegroundColor;
        if (res.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
        {
            _sysCacheService.Remove($"WxAccessToken_{_wechatOptions.WxOpenAppId}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("【" + DateTime.Now + "】" + _wechatOptions.WxOpenAppId + " WeChat Mini ProgramToken Noneeffect");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("【" + DateTime.Now + "】" + _wechatOptions.WxOpenAppId + " WeChat Mini Program Token Valid");
        }
        Console.ForegroundColor = originColor;
    }

    /// <summary>
    /// Get WeChat JS interface temporary ticket jsapi_ticket
    /// </summary>
    /// <returns></returns>
    public async Task<string> TryGetWechatJsApiTicketAsync()
    {
        if (!_sysCacheService.ExistKey($"WxJsApiTicket_{_wechatOptions.WechatAppId}") || string.IsNullOrEmpty(_sysCacheService.Get<string>($"WxJsApiTicket_{_wechatOptions.WechatAppId}")))
        {
            var accessToken = await TryGetWechatAccessTokenAsync();
            var client = CreateWechatClient();
            var request = new CgibinTicketGetTicketRequest()
            {
                AccessToken = accessToken
            };
            var response = await client.ExecuteCgibinTicketGetTicketAsync(request);
            if (!response.IsSuccessful())
                throw Oops.Oh(response.ErrorMessage + " " + response.ErrorCode);
            _sysCacheService.Set($"WxJsApiTicket_{_wechatOptions.WechatAppId}", response.Ticket, TimeSpan.FromSeconds(response.ExpiresIn - 60));
        }
        return _sysCacheService.Get<string>($"WxJsApiTicket_{_wechatOptions.WechatAppId}");
    }
}