// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core.Service;

/// <summary>
/// System file storage provider service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 411, Description = "file storage provider")]
public class SysFileProviderService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFileProvider> _sysFileProviderRep;
    private readonly SysCacheService _sysCacheService;
    private readonly IOSSServiceFactory _ossServiceFactory;
    private readonly IOSSServiceManager _ossServiceManager;
    private static readonly string CacheKey = "sys_file_provider";

    public SysFileProviderService(UserManager userManager,
        SqlSugarRepository<SysFileProvider> sysFileProviderRep,
        SysCacheService sysCacheService,
        IOSSServiceFactory ossServiceFactory,
        IOSSServiceManager ossServiceManager)
    {
        _userManager = userManager;
        _sysFileProviderRep = sysFileProviderRep;
        _sysCacheService = sysCacheService;
        _ossServiceFactory = ossServiceFactory;
        _ossServiceManager = ossServiceManager;
    }

    /// <summary>
    /// Get a paginated list of file storage providers 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get a paginated list of file storage providers")]
    [NonAction]
    public async Task<SqlSugarPagedList<SysFileProvider>> GetFileProviderPage([FromQuery] PageFileProviderInput input)
    {
        return await _sysFileProviderRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Provider), u => u.Provider.Contains(input.Provider!))
            .WhereIF(!string.IsNullOrWhiteSpace(input.BucketName), u => u.BucketName.Contains(input.BucketName!))
            .WhereIF(input.IsEnable.HasValue, u => u.IsEnable == input.IsEnable)
            .OrderBy(u => u.OrderNo)
            .OrderBy(u => u.Id)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get a list of file storage providers 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the list of file storage providers")]
    [NonAction]
    public async Task<List<SysFileProvider>> GetFileProviderList()
    {
        return await _sysFileProviderRep.AsQueryable()
            .Where(u => u.IsEnable == true)
            .OrderBy(u => u.OrderNo)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Add file storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add file storage provider")]
    [NonAction]
    public async Task AddFileProvider(AddFileProviderInput input)
    {
        // Validate input parameters
        if (input == null)
            throw Oops.Oh("Input parameters cannot be empty").StatusCode(400);

        if (string.IsNullOrWhiteSpace(input.Provider))
            throw Oops.Oh("Storage provider cannot be null").StatusCode(400);

        if (string.IsNullOrWhiteSpace(input.BucketName))
            throw Oops.Oh("Bucket name cannot be empty").StatusCode(400);

        // Authentication provider type
        if (!Enum.TryParse<OSSProvider>(input.Provider, true, out _))
            throw Oops.Oh($"Unsupported storage provider type: {input.Provider}").StatusCode(400);

        var isExist = await _sysFileProviderRep.AsQueryable()
            .AnyAsync(u => u.Provider == input.Provider && u.BucketName == input.BucketName);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006).StatusCode(400);

        var fileProvider = input.Adapt<SysFileProvider>();

        // Verify configuration integrity
        await ValidateProviderConfiguration(fileProvider);

        // Handle default provider logic
        await HandleDefaultProviderLogic(fileProvider);

        await _sysFileProviderRep.InsertAsync(fileProvider);

        // clear cache
        await ClearCache();

        // Clear OSS service cache
        _ossServiceManager?.ClearCache();
    }

    /// <summary>
    /// Update file storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update file storage provider")]
    [NonAction]
    public async Task UpdateFileProvider(UpdateFileProviderInput input)
    {
        // Validate input parameters
        if (input == null)
            throw Oops.Oh("Input parameters cannot be empty").StatusCode(400);

        var isExist = await _sysFileProviderRep.AsQueryable()
            .AnyAsync(u => u.Provider == input.Provider && u.BucketName == input.BucketName && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D1006).StatusCode(400);

        var fileProvider = input.Adapt<SysFileProvider>();

        // Verify configuration integrity
        await ValidateProviderConfiguration(fileProvider);

        // Handle default provider logic
        await HandleDefaultProviderLogic(fileProvider);

        await _sysFileProviderRep.AsUpdateable(fileProvider).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();

        // clear cache
        await ClearCache();

        // Clear OSS service cache
        _ossServiceManager?.ClearCache();
    }

    /// <summary>
    /// Remove file storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Remove file storage provider")]
    [NonAction]
    public async Task DeleteFileProvider(DeleteFileProviderInput input)
    {
        // Check if it is the default provider
        var provider = await _sysFileProviderRep.GetByIdAsync(input.Id) ?? throw Oops.Oh("Storage provider does not exist").StatusCode(400);
        var isDefault = provider.IsDefault == true;

        await _sysFileProviderRep.DeleteByIdAsync(input.Id);

        // If the default provider is deleted, the first enabled provider is automatically set as the default.
        if (isDefault)
        {
            var firstEnabledProvider = await _sysFileProviderRep.AsQueryable()
                .Where(p => p.IsEnable == true)
                .OrderBy(p => p.OrderNo)
                .OrderBy(p => p.Id)
                .FirstAsync();

            if (firstEnabledProvider != null)
            {
                await _sysFileProviderRep.AsUpdateable()
                    .SetColumns(p => p.IsDefault == true)
                    .Where(p => p.Id == firstEnabledProvider.Id)
                    .ExecuteCommandAsync();

                Debug.WriteLine($"Automatically set the new default provider: {firstEnabledProvider.DisplayName}");
            }
        }

        // clear cache
        await ClearCache();

        // Clear OSS service cache
        _ossServiceManager?.ClearCache();
    }

    /// <summary>
    /// Get file storage provider details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get file storage provider details")]
    [NonAction]
    public async Task<SysFileProvider> GetFileProvider([FromQuery] QueryFileProviderInput input)
    {
        return await _sysFileProviderRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Get configuration based on provider and bucket
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<SysFileProvider?> GetFileProviderByBucket(string provider, string bucketName)
    {
        var providers = await GetCachedFileProviders();
        return providers.FirstOrDefault(x => x.Provider == provider && x.BucketName == bucketName && x.IsEnable == true);
    }

    /// <summary>
    /// Get configuration based on ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<SysFileProvider?> GetFileProviderById(long id)
    {
        var providers = await GetCachedFileProviders();
        return providers.FirstOrDefault(x => x.Id == id && x.IsEnable == true);
    }

    /// <summary>
    /// Get storage provider based on bucket name
    /// </summary>
    /// <param name="bucketName">bucket name</param>
    /// <returns></returns>
    [NonAction]
    public async Task<SysFileProvider?> GetProviderByBucketName(string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return null;

        var providers = await GetCachedFileProviders();
        return providers.FirstOrDefault(p => p.BucketName == bucketName);
    }

    /// <summary>
    /// Get default storage provider
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<SysFileProvider?> GetDefaultProvider()
    {
        var providers = await GetCachedFileProviders();

        // Providers marked as default are returned first
        var defaultProvider = providers.FirstOrDefault(p => p.IsDefault == true);
        if (defaultProvider != null)
            return defaultProvider;

        // If not marked as default, returns the first enabled provider (compatible with old logic)
        return providers.FirstOrDefault();
    }

    /// <summary>
    /// Get default storage provider information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get default storage provider information")]
    [NonAction]
    public async Task<SysFileProvider?> GetDefaultProviderInfo()
    {
        return await GetDefaultProvider();
    }

    /// <summary>
    /// Set default storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "SetDefault"), HttpPost]
    [DisplayName("Set default storage provider")]
    [NonAction]
    public async Task SetDefaultProvider(SetDefaultProviderInput input)
    {
        // Verify that the provider exists and is enabled
        var provider = await _sysFileProviderRep.GetByIdAsync(input.Id) ?? throw Oops.Oh("Storage provider does not exist").StatusCode(400);
        if (provider.IsEnable != true)
            throw Oops.Oh("Only enabled storage providers can be set as default").StatusCode(400);

        // Start transactions to ensure data consistency
        await _sysFileProviderRep.AsTenant().BeginTranAsync();
        try
        {
            // First set the default identity of all providers to false
            await _sysFileProviderRep.AsUpdateable()
                .SetColumns(p => p.IsDefault == false)
                .Where(p => p.IsDefault == true)
                .ExecuteCommandAsync();

            // Set the specified provider as default
            await _sysFileProviderRep.AsUpdateable()
                .SetColumns(p => p.IsDefault == true)
                .Where(p => p.Id == input.Id)
                .ExecuteCommandAsync();

            await _sysFileProviderRep.AsTenant().CommitTranAsync();

            // clear cache
            await ClearCache();

            // Clear OSS service cache
            _ossServiceManager?.ClearCache();

            Debug.WriteLine($"Default storage provider set: {provider.DisplayName}");
        }
        catch (Exception)
        {
            await _sysFileProviderRep.AsTenant().RollbackTranAsync();
            throw;
        }
    }

    /// <summary>
    /// Get a list of cached file providers
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysFileProvider>> GetCachedFileProviders()
    {
        return await _sysCacheService.AdGetAsync(CacheKey, async () =>
        {
            return await _sysFileProviderRep.AsQueryable()
                .Where(u => u.IsEnable == true)
                .OrderBy(u => u.OrderNo)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }, TimeSpan.FromMinutes(30));
    }

    /// <summary>
    /// clear cache
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task ClearCache()
    {
        _sysCacheService.Remove(CacheKey);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Get a list of all available buckets
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<string>> GetAvailableBuckets()
    {
        var providers = await GetCachedFileProviders();
        return providers.Select(p => p.BucketName).Distinct().OrderBy(b => b).ToList();
    }

    /// <summary>
    /// Get the mapping relationship between buckets and providers
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<Dictionary<string, List<SysFileProvider>>> GetBucketProviderMapping()
    {
        var providers = await GetCachedFileProviders();
        var mapping = new Dictionary<string, List<SysFileProvider>>();

        foreach (var provider in providers)
        {
            if (!mapping.TryGetValue(provider.BucketName, out List<SysFileProvider> value))
            {
                value = new List<SysFileProvider>();
                mapping[provider.BucketName] = value;
            }

            value.Add(provider);
        }

        return mapping;
    }

    /// <summary>
    /// Verify storage provider configuration
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <returns></returns>
    [NonAction]
    private async Task ValidateProviderConfiguration(SysFileProvider provider)
    {
        if (provider == null)
            throw Oops.Oh("Storage provider configuration cannot be empty").StatusCode(400);

        // Basic field validation
        if (string.IsNullOrWhiteSpace(provider.Provider))
            throw Oops.Oh("Storage provider type cannot be null").StatusCode(400);

        if (string.IsNullOrWhiteSpace(provider.BucketName))
            throw Oops.Oh("Bucket name cannot be empty").StatusCode(400);

        if (string.IsNullOrWhiteSpace(provider.Endpoint))
            throw Oops.Oh("Endpoint Addresscannot benull").StatusCode(400);

        // All providers require AccessKey and SecretKey
        if (string.IsNullOrWhiteSpace(provider.AccessKey))
            throw Oops.Oh($"{provider.Provider} AccessKey cannot be empty").StatusCode(400);
        if (string.IsNullOrWhiteSpace(provider.SecretKey))
            throw Oops.Oh($"{provider.Provider} SecretKey cannot be empty").StatusCode(400);

        // Validate specific fields against different providers
        switch (provider.Provider.ToUpper())
        {
            case "ALIYUN":
                if (string.IsNullOrWhiteSpace(provider.Region))
                    throw Oops.Oh("Alibaba Cloud Region cannot be empty").StatusCode(400);
                break;

            case "QCLOUD":
                if (string.IsNullOrWhiteSpace(provider.Endpoint))
                    throw Oops.Oh("Tencent Cloud Endpoint (AppId) cannot be empty").StatusCode(400);
                if (string.IsNullOrWhiteSpace(provider.Region))
                    throw Oops.Oh("Tencent Cloud Region cannot be empty").StatusCode(400);
                break;

            case "MINIO":
                // Minio only requires AccessKey and SecretKey, verified above
                break;

            default:
                throw Oops.Oh($"Unsupported storage provider type: {provider.Provider}").StatusCode(400);
        }

        // Verify bucket name format
        await ValidateBucketName(provider.Provider, provider.BucketName);
    }

    /// <summary>
    /// Verify bucket name format
    /// </summary>
    /// <param name="provider">Storage provider type</param>
    /// <param name="bucketName">bucket name</param>
    /// <returns></returns>
    [NonAction]
    private async Task ValidateBucketName(string provider, string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return;

        switch (provider.ToUpper())
        {
            case "ALIYUN":
                // Alibaba Cloud bucket naming rules
                if (bucketName.Length < 3 || bucketName.Length > 63)
                    throw Oops.Oh("Alibaba Cloud bucket name length must be between 3-63 characters").StatusCode(400);

                if (!Regex.IsMatch(bucketName, @"^[a-z0-9][a-z0-9\-]*[a-z0-9]$"))
                    throw Oops.Oh("Alibaba Cloud bucket names can only contain lowercase letters, numbers, and hyphens, and must start and end with a letter or number").StatusCode(400);
                break;

            case "QCLOUD":
                // Tencent Cloud storage bucket naming rules
                if (bucketName.Length < 1 || bucketName.Length > 40)
                    throw Oops.Oh("The Tencent Cloud storage bucket name must be between 1 and 40 characters long").StatusCode(400);

                if (!Regex.IsMatch(bucketName, @"^[a-z0-9][a-z0-9\-]*[a-z0-9]$"))
                    throw Oops.Oh("Tencent Cloud storage bucket names can only contain lowercase letters, numbers, and hyphens, and must start and end with a letter or number").StatusCode(400);
                break;

            case "MINIO":
                // Minio bucket naming rules
                if (bucketName.Length < 3 || bucketName.Length > 63)
                    throw Oops.Oh("Minio bucket name length must be between 3 and 63 characters").StatusCode(400);

                if (!Regex.IsMatch(bucketName, @"^[a-z0-9][a-z0-9\-\.]*[a-z0-9]$"))
                    throw Oops.Oh("Minio bucket names can only contain lowercase letters, numbers, hyphens, and dots, and must start and end with a letter or number").StatusCode(400);
                break;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Handle default provider logic
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <returns></returns>
    [NonAction]
    private async Task HandleDefaultProviderLogic(SysFileProvider provider)
    {
        // If set as default provider
        if (provider.IsDefault == true)
        {
            // Make sure there is only one default provider, set the other providers' default flags to false
            await _sysFileProviderRep.AsUpdateable()
                .SetColumns(p => p.IsDefault == false)
                .Where(p => p.IsDefault == true && p.Id != provider.Id)
                .ExecuteCommandAsync();
        }
        else
        // If the IsDefault value is not set, the default is false
        {
            provider.IsDefault ??= false;
        }

        // Check if there are other default providers, if there are none and the current provider is enabled, make it the default
        var hasDefaultProvider = await _sysFileProviderRep.AsQueryable()
            .Where(p => p.IsDefault == true && p.IsEnable == true && p.Id != provider.Id)
            .AnyAsync();

        if (!hasDefaultProvider && provider.IsEnable == true && provider.IsDefault != true)
        {
            // Set as default if there is no other default provider and current provider is enabled
            provider.IsDefault = true;
        }
    }
}