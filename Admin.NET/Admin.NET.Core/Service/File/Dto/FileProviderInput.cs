// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// File storage provider paginated query input parameters
/// </summary>
public class PageFileProviderInput : BasePageInput
{
    /// <summary>
    /// storage provider
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// bucket name
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool? IsEnable { get; set; }
}

/// <summary>
/// Add file storage provider input parameters
/// </summary>
public class AddFileProviderInput
{
    /// <summary>
    /// storage provider
    /// </summary>
    [Required(ErrorMessage = "Storage provider cannot be null")]
    public string Provider { get; set; }

    /// <summary>
    /// bucket name
    /// </summary>
    [Required(ErrorMessage = "Bucket name cannot be empty")]
    public string BucketName { get; set; }

    /// <summary>
    /// Access key ID (all cloud service providers use this field uniformly)
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// key
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// area
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// endpoint address
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Whether to enable HTTPS
    /// </summary>
    public bool? IsEnableHttps { get; set; } = true;

    /// <summary>
    /// Whether to enable caching
    /// </summary>
    public bool? IsEnableCache { get; set; } = true;

    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool? IsEnable { get; set; } = true;

    /// <summary>
    /// Whether to use the default provider
    /// </summary>
    public bool? IsDefault { get; set; } = false;

    /// <summary>
    /// Custom domain name
    /// </summary>
    public string? SinceDomain { get; set; }

    /// <summary>
    /// sequence number
    /// </summary>
    public int? OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Supported business types (JSON format)
    /// </summary>
    public string? BusinessTypes { get; set; }

    /// <summary>
    /// priority
    /// </summary>
    public int Priority { get; set; } = 100;
}

/// <summary>
/// Update file storage provider input parameters
/// </summary>
public class UpdateFileProviderInput : AddFileProviderInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long Id { get; set; }
}

/// <summary>
/// Remove file storage provider input parameters
/// </summary>
public class DeleteFileProviderInput : BaseIdInput
{
}

/// <summary>
/// Query file storage provider input parameters
/// </summary>
public class QueryFileProviderInput : BaseIdInput
{
}

/// <summary>
/// Test connection input parameters
/// </summary>
public class TestConnectionInput : BaseIdInput
{
}

/// <summary>
/// Set default storage provider input parameters
/// </summary>
public class SetDefaultProviderInput
{
    /// <summary>
    /// Store provider ID
    /// </summary>
    [Required(ErrorMessage = "Storage provider ID cannot be empty")]
    public long Id { get; set; }
}

/// <summary>
/// File upload select storage provider input parameters
/// </summary>
public class SelectProviderInput
{
    /// <summary>
    /// File type
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// Business type
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// Specify provider ID
    /// </summary>
    public long? ProviderId { get; set; }

    /// <summary>
    /// Specify bucket name
    /// </summary>
    public string? BucketName { get; set; }
}