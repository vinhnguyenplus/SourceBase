using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core.Service;

/// <summary>
/// OSS service manager interface
/// </summary>
public interface IOSSServiceManager : IDisposable
{
    /// <summary>
    /// Obtain OSS service instance
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <returns></returns>
    Task<IOSSService> GetOSSServiceAsync(SysFileProvider provider);

    /// <summary>
    /// clear cache
    /// </summary>
    void ClearCache();
}

/// <summary>
/// OSS service manager implementation
/// </summary>
public class OSSServiceManager : IOSSServiceManager, ITransient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, IOSSService> _ossServiceCache;
    private readonly object _lockObject = new object();
    private bool _disposed = false;

    public OSSServiceManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _ossServiceCache = new ConcurrentDictionary<string, IOSSService>();
    }

    /// <summary>
    /// Obtain OSS service instance (with cache)
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <returns></returns>
    public async Task<IOSSService> GetOSSServiceAsync(SysFileProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        var cacheKey = provider.ConfigKey;

        // Try to get from cache
        if (_ossServiceCache.TryGetValue(cacheKey, out var cachedService))
        {
            return cachedService;
        }

        // Verify configuration
        if (!await ValidateConfigurationAsync(provider))
        {
            throw new InvalidOperationException($"OSSProviderConfigurationNoneeffect: {provider.DisplayName}");
        }

        // Thread-safely create new services
        lock (_lockObject)
        {
            // Double check lock mode
            if (_ossServiceCache.TryGetValue(cacheKey, out cachedService))
            {
                return cachedService;
            }

            // Convert configuration and create service
            var ossOptions = ConvertToOSSOptions(provider);
            var ossService = CreateOSSService(ossOptions);

            // add to cache
            _ossServiceCache.TryAdd(cacheKey, ossService);

            return ossService;
        }
    }

    /// <summary>
    /// Create an OSS service instance
    /// </summary>
    /// <param name="options">OSS configuration options</param>
    /// <returns></returns>
    private IOSSService CreateOSSService(OSSOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            // Use the existing IOSSServiceFactory, but need to register the configuration first
            var providerName = Enum.GetName(options.Provider);
            var configSectionName = $"TempOSS_{Guid.NewGuid():N}";

            // Create temporary configuration
            var configData = new Dictionary<string, string>
            {
                [$"{configSectionName}:Provider"] = providerName,
                [$"{configSectionName}:Endpoint"] = options.Endpoint ?? "",
                [$"{configSectionName}:AccessKey"] = options.AccessKey ?? "",
                [$"{configSectionName}:SecretKey"] = options.SecretKey ?? "",
                [$"{configSectionName}:Region"] = options.Region ?? "",
                [$"{configSectionName}:IsEnableHttps"] = options.IsEnableHttps.ToString(),
                [$"{configSectionName}:IsEnableCache"] = options.IsEnableCache.ToString()
            };

            var tempConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Create a temporary service collection but do not release it immediately
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(tempConfig);
            services.AddLogging();
            services.AddOSSService(providerName, configSectionName);

            // Build service providers and create OSS services
            var tempServiceProvider = services.BuildServiceProvider();
            var ossServiceFactory = tempServiceProvider.GetRequiredService<IOSSServiceFactory>();
            var ossService = ossServiceFactory.Create(providerName);

            // Note: Do not release tempServiceProvider because ossService may depend on it
            // Here we accept this memory overhead because caching will reduce the creation frequency

            return ossService;
        }
        catch (Exception ex)
        {
            throw Oops.Oh($"Failed to create OSS service: {ex.Message}");
        }
    }

    /// <summary>
    /// Verify configuration
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <returns></returns>
    private Task<bool> ValidateConfigurationAsync(SysFileProvider provider)
    {
        if (provider == null) return Task.FromResult(false);

        // Basic field validation
        var isValid = !string.IsNullOrWhiteSpace(provider.Provider) &&
                     !string.IsNullOrWhiteSpace(provider.BucketName) &&
                     !string.IsNullOrWhiteSpace(provider.AccessKey) &&
                     !string.IsNullOrWhiteSpace(provider.SecretKey);

        // Minio additionally requires Endpoint
        if (provider.Provider.ToUpper() == "MINIO")
        {
            isValid = isValid && !string.IsNullOrWhiteSpace(provider.Endpoint);
        }

        return Task.FromResult(isValid);
    }

    /// <summary>
    /// Convert SysFileProvider to OSSOptions
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    private OSSOptions ConvertToOSSOptions(SysFileProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        var ossOptions = new OSSOptions
        {
            Provider = Enum.Parse<OSSProvider>(provider.Provider),
            Endpoint = provider.Endpoint,
            Region = provider.Region,
            IsEnableHttps = provider.IsEnableHttps ?? true,
            IsEnableCache = provider.IsEnableCache ?? true
        };

        // Set authentication information (all providers now use unified fields)
        ossOptions.AccessKey = provider.AccessKey;
        ossOptions.SecretKey = provider.SecretKey;

        return ossOptions;
    }

    /// <summary>
    /// clear cache
    /// </summary>
    public void ClearCache()
    {
        lock (_lockObject)
        {
            _ossServiceCache.Clear();
        }
    }

    /// <summary>
    /// Release resources
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            lock (_lockObject)
            {
                _ossServiceCache.Clear();
            }
            _disposed = true;
        }
    }
}