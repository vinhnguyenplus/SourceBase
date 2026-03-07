// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core;

/// <summary>
/// File upload configuration options
/// </summary>
public sealed class UploadOptions : IConfigurableOptions
{
    /// <summary>
    /// path
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// size
    /// </summary>
    public long MaxSize { get; set; }

    /// <summary>
    /// Upload format
    /// </summary>
    public List<string> ContentType { get; set; }

    /// <summary>
    /// Enable file MD5 verification
    /// </summary>
    /// <remarks>Prevent duplicate uploads</remarks>
    public bool EnableMd5 { get; set; }
}

/// <summary>
/// Object storage configuration options
/// </summary>
public sealed class OSSProviderOptions : OSSOptions, IConfigurableOptions
{
    /// <summary>
    /// Whether to enable OSS storage
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Custom bucket name cannot use Provider directly to replace the bucket name.
    /// Example: Alibaba Cloud 1. Can only include lowercase letters, numbers, and dashes (-) 2. Must start with a lowercase letter or number 3. The length must be between 3-63 bytes
    /// </summary>
    public string Bucket { get; set; }

    /// <summary>
    /// Custom Host
    /// Splice the Host of the external link. If empty, use Endpoint to splice it.
    /// </summary>
    /// <remarks></remarks>
    public string CustomHost { get; set; }
}