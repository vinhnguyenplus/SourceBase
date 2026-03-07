// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// File Storage Provider Management Controller 🧩
/// </summary>
[ApiDescriptionSettings(Order = 412, Description = "File storage provider management")]
public class SysFileProviderController : IDynamicApiController, ITransient
{
    private readonly SysFileProviderService _fileProviderService;

    public SysFileProviderController(SysFileProviderService fileProviderService)
    {
        _fileProviderService = fileProviderService;
    }

    /// <summary>
    /// Get a list of storage providers 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get a list of storage providers")]
    public async Task<List<SysFileProvider>> GetProviderList()
    {
        return await _fileProviderService.GetFileProviderList();
    }

    /// <summary>
    /// Get a paginated list of storage providers 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get paginated list of storage providers")]
    public async Task<SqlSugarPagedList<SysFileProvider>> GetProviderPage(PageFileProviderInput input)
    {
        return await _fileProviderService.GetFileProviderPage(input);
    }

    /// <summary>
    /// Get storage provider details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get storage provider details")]
    public async Task<SysFileProvider> GetProvider([FromQuery] QueryFileProviderInput input)
    {
        return await _fileProviderService.GetFileProvider(input);
    }

    /// <summary>
    /// Add storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add storage provider")]
    public async Task AddProvider(AddFileProviderInput input)
    {
        await _fileProviderService.AddFileProvider(input);
    }

    /// <summary>
    /// Update storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update storage provider")]
    public async Task UpdateProvider(UpdateFileProviderInput input)
    {
        await _fileProviderService.UpdateFileProvider(input);
    }

    /// <summary>
    /// Remove storage provider 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Remove storage provider")]
    public async Task DeleteProvider(DeleteFileProviderInput input)
    {
        await _fileProviderService.DeleteFileProvider(input);
    }

    /// <summary>
    /// Get storage provider based on bucket name 🔖
    /// </summary>
    /// <param name="bucketName">bucket name</param>
    /// <returns></returns>
    [DisplayName("Get storage provider based on bucket name")]
    public async Task<SysFileProvider?> GetProviderByBucketName(string bucketName)
    {
        return await _fileProviderService.GetProviderByBucketName(bucketName);
    }

    /// <summary>
    /// Clear storage provider cache 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Clear storage provider cache")]
    public async Task ClearCache()
    {
        await _fileProviderService.ClearCache();
    }

    /// <summary>
    /// Enable/disable storage providers in bulk 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchEnable"), HttpPost]
    [DisplayName("Enable/disable storage providers in bulk")]
    public async Task BatchEnableProvider(BatchEnableProviderInput input)
    {
        foreach (var id in input.Ids)
        {
            var provider = await _fileProviderService.GetFileProviderById(id);
            if (provider != null)
            {
                var updateInput = new UpdateFileProviderInput
                {
                    Id = id,
                    Provider = provider.Provider,
                    BucketName = provider.BucketName,
                    IsEnable = input.IsEnable
                };
                await _fileProviderService.UpdateFileProvider(updateInput);
            }
        }
    }

    /// <summary>
    /// Get storage provider statistics 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get storage provider statistics")]
    public async Task<object> GetProviderStatistics()
    {
        var providers = await _fileProviderService.GetCachedFileProviders();

        var statistics = new
        {
            Total = providers.Count,
            Enabled = providers.Count(p => p.IsEnable == true),
            Disabled = providers.Count(p => p.IsEnable != true),
            ByProvider = providers.GroupBy(p => p.Provider)
                .Select(g => new { Provider = g.Key, Count = g.Count() })
                .ToList(),
            ByRegion = providers.Where(p => !string.IsNullOrEmpty(p.Region))
                .GroupBy(p => p.Region)
                .Select(g => new { Region = g.Key, Count = g.Count() })
                .ToList()
        };

        return statistics;
    }

    /// <summary>
    /// Get a list of all available buckets 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get a list of all available buckets")]
    public async Task<List<string>> GetAvailableBuckets()
    {
        return await _fileProviderService.GetAvailableBuckets();
    }

    /// <summary>
    /// Get the mapping relationship between bucket and provider 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Obtain the mapping relationship between buckets and providers")]
    public async Task<Dictionary<string, List<SysFileProvider>>> GetBucketProviderMapping()
    {
        return await _fileProviderService.GetBucketProviderMapping();
    }
}

/// <summary>
/// Bulk enable/disable storage provider input parameters
/// </summary>
public class BatchEnableProviderInput
{
    /// <summary>
    /// Store list of provider IDs
    /// </summary>
    [Required]
    public List<long> Ids { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool IsEnable { get; set; }
}