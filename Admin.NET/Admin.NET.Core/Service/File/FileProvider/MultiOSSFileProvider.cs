// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Multiple OSS file providers
/// </summary>
public class MultiOSSFileProvider : ICustomFileProvider, ITransient
{
    private readonly SysFileProviderService _fileProviderService;
    private readonly IOSSServiceManager _ossServiceManager;
    private readonly OSSProviderOptions _ossProviderOptions;

    public MultiOSSFileProvider(SysFileProviderService fileProviderService,
        IOSSServiceManager ossServiceManager,
        IOptions<OSSProviderOptions> ossProviderOptions)
    {
        _fileProviderService = fileProviderService;
        _ossServiceManager = ossServiceManager;
        _ossProviderOptions = ossProviderOptions.Value;
    }

    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file">document</param>
    /// <param name="sysFile">System file information</param>
    /// <param name="path">File storage location</param>
    /// <param name="finalName">file final name</param>
    /// <returns></returns>
    public async Task<SysFile> UploadFileAsync(IFormFile file, SysFile sysFile, string path, string finalName)
    {
        // Obtain OSS configuration (incoming file information is used for policy selection)
        var provider = await GetFileProvider(sysFile, file) ?? throw Oops.Oh("No available storage provider configuration found");

        // Obtain OSS services
        var ossService = await _ossServiceManager.GetOSSServiceAsync(provider);

        // Set file information
        sysFile.Provider = provider.Provider;
        sysFile.BucketName = provider.BucketName; // Save original bucket name

        var filePath = string.Concat(path, "/", finalName);

        // Upload files
        await ossService.PutObjectAsync(provider.BucketName, filePath, file.OpenReadStream());

        // Generate external link address
        sysFile.Url = GenerateFileUrl(provider, provider.BucketName, filePath);

        return sysFile;
    }

    /// <summary>
    /// Delete files
    /// </summary>
    /// <param name="sysFile">System file information</param>
    /// <returns></returns>
    public async Task DeleteFileAsync(SysFile sysFile)
    {
        // Obtain OSS configuration (unified method)
        var provider = await GetFileProvider(sysFile) ?? throw Oops.Oh($"Storage provider configuration not found: {sysFile.Provider}-{sysFile.BucketName}");
        var ossService = await _ossServiceManager.GetOSSServiceAsync(provider);
        var filePath = string.Concat(sysFile.FilePath, "/", $"{sysFile.Id}{sysFile.Suffix}");

        await ossService.RemoveObjectAsync(provider.BucketName, filePath);
    }

    /// <summary>
    /// Get file stream
    /// </summary>
    /// <param name="sysFile">System file information</param>
    /// <param name="fileName">file name</param>
    /// <returns></returns>
    public async Task<FileStreamResult> GetFileStreamResultAsync(SysFile sysFile, string fileName)
    {
        // Obtain OSS configuration (unified method)
        var provider = await GetFileProvider(sysFile) ?? throw Oops.Oh($"Storage provider configuration not found: {sysFile.Provider}-{sysFile.BucketName}");
        var ossService = await _ossServiceManager.GetOSSServiceAsync(provider);
        var filePath = Path.Combine(sysFile.FilePath ?? "", sysFile.Id + sysFile.Suffix);

        var httpRemoteService = App.GetRequiredService<IHttpRemoteService>();
        var stream = await httpRemoteService.GetAsStreamAsync(await ossService.PresignedGetObjectAsync(provider.BucketName, filePath, 5));

        return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = fileName + sysFile.Suffix };
    }

    /// <summary>
    /// Download file in Base64 format
    /// </summary>
    /// <param name="sysFile">System file information</param>
    /// <returns></returns>
    public async Task<string> DownloadFileBase64Async(SysFile sysFile)
    {
        using var httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(sysFile.Url);
        if (response.IsSuccessStatusCode)
        {
            byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
            return Convert.ToBase64String(fileBytes);
        }
        throw Oops.Oh($"Failed to download file, status code: {response.StatusCode}");
    }

    /// <summary>
    /// Obtain file provider configuration (unified method, supports upload, delete, download scenarios)
    /// </summary>
    /// <param name="sysFile">System file information</param>
    /// <param name="file">Uploaded file (optional, only passed in when uploading)</param>
    /// <returns></returns>
    private async Task<SysFileProvider?> GetFileProvider(SysFile sysFile, IFormFile? file = null)
    {
        // 1. If the bucket has been specified, use it directly
        if (!string.IsNullOrEmpty(sysFile.BucketName))
        {
            var provider = await _fileProviderService.GetFileProviderByBucket(sysFile.Provider, sysFile.BucketName);
            if (provider != null) return provider;

            // If it is not found in the database, try configuring the file.
            if (_ossProviderOptions.Enabled && _ossProviderOptions.Bucket == sysFile.BucketName)
            {
                return await CreateProviderFromConfiguration();
            }
        }

        // 2. If there is upload file information, use policy selection (only upload scenarios)
        if (file != null)
        {
            var uploadInput = new UploadFileInput
            {
                File = file,
                BucketName = sysFile.BucketName,
                FileType = sysFile.FileType
            };

            return await SelectProviderAsync(file, uploadInput);
        }

        // 3. The last resort: Use the default storage provider
        return await _fileProviderService.GetDefaultProvider();
    }

    /// <summary>
    /// Choosing the right OSS storage provider (inline version)
    /// </summary>
    /// <param name="file">Uploaded files</param>
    /// <param name="input">Upload input parameters</param>
    /// <returns></returns>
    private async Task<SysFileProvider?> SelectProviderAsync(IFormFile file, UploadFileInput input)
    {
        // 1. Prioritize the use of specified provider IDs
        if (input.ProviderId.HasValue)
        {
            var provider = await _fileProviderService.GetFileProviderById(input.ProviderId.Value);
            if (provider != null) return provider;
        }

        // 2. Secondly use the specified bucket name
        if (!string.IsNullOrEmpty(input.BucketName))
        {
            var providers = await _fileProviderService.GetCachedFileProviders();
            var provider = providers.FirstOrDefault(p => p.BucketName == input.BucketName);
            if (provider != null) return provider;
        }

        // 3. Use the default provider
        var defaultProvider = await _fileProviderService.GetDefaultProvider();
        if (defaultProvider != null) return defaultProvider;

        // 4. Bottom line: If there is no configuration in the database, try to create a default provider from the configuration file
        return await CreateProviderFromConfiguration();
    }

    /// <summary>
    /// Generate file URL (inline version)
    /// </summary>
    /// <param name="provider">Storage provider configuration</param>
    /// <param name="bucketName">bucket name</param>
    /// <param name="filePath">file path</param>
    /// <returns></returns>
    private static string GenerateFileUrl(SysFileProvider provider, string bucketName, string filePath)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var protocol = provider.IsEnableHttps == true ? "https" : "http";

        // If you have a custom domain name, use it directly
        if (!string.IsNullOrWhiteSpace(provider.SinceDomain))
        {
            return $"{provider.SinceDomain.TrimEnd('/')}/{filePath.TrimStart('/')}";
        }

        // Generate URLs based on different providers
        return provider.Provider.ToUpper() switch
        {
            "ALIYUN" => $"{protocol}://{bucketName}.{provider.Endpoint}/{filePath.TrimStart('/')}",
            "QCLOUD" => $"{protocol}://{bucketName}-{provider.Endpoint}.cos.{provider.Region}.myqcloud.com/{filePath.TrimStart('/')}",
            "MINIO" => $"{protocol}://{provider.Endpoint}/{bucketName}/{filePath.TrimStart('/')}",
            _ => throw Oops.Oh($"Unsupported OSS provider: {provider.Provider}")
        };
    }

    /// <summary>
    /// Create default provider from configuration file (cover-up mechanism)
    /// </summary>
    /// <returns></returns>
    private Task<SysFileProvider?> CreateProviderFromConfiguration()
    {
        try
        {
            // Check if OSS configuration is enabled
            if (!_ossProviderOptions.Enabled && !App.Configuration["MultiOSS:Enabled"].ToBoolean())
                return Task.FromResult<SysFileProvider?>(null);

            // Verify necessary configuration
            if (string.IsNullOrWhiteSpace(_ossProviderOptions.AccessKey) ||
                string.IsNullOrWhiteSpace(_ossProviderOptions.SecretKey) ||
                string.IsNullOrWhiteSpace(_ossProviderOptions.Bucket))
            {
                return Task.FromResult<SysFileProvider?>(null);
            }

            // Create temporary provider configuration using existing OSSProviderOptions (not saved to database)
            var provider = new SysFileProvider
            {
                Id = 0, // Temporary ID
                Provider = Enum.GetName(_ossProviderOptions.Provider),
                BucketName = _ossProviderOptions.Bucket,
                AccessKey = _ossProviderOptions.AccessKey,
                SecretKey = _ossProviderOptions.SecretKey,
                Endpoint = _ossProviderOptions.Endpoint,
                Region = _ossProviderOptions.Region,
                IsEnableHttps = _ossProviderOptions.IsEnableHttps,
                IsEnableCache = _ossProviderOptions.IsEnableCache,
                IsEnable = true,
                IsDefault = true,
                SinceDomain = _ossProviderOptions.CustomHost,
                CreateTime = DateTime.Now
            };

            return Task.FromResult<SysFileProvider?>(provider);
        }
        catch (Exception)
        {
            // Configuration reading failed and null was returned.
            return Task.FromResult<SysFileProvider?>(null);
        }
    }
}