// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// CI/CD configuration options
/// </summary>
public class CDConfigOptions : IConfigurableOptions
{
    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// username
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string Repo { get; set; }

    /// <summary>
    /// branch name
    /// </summary>
    public string Branch { get; set; }

    /// <summary>
    /// User authorization code
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Update interval limit (minutes) 0 No limit
    /// </summary>
    public int UpdateInterval { get; set; }

    /// <summary>
    /// Number of backup files to keep, 0 no limit
    /// </summary>
    public int BackupCount { get; set; }

    /// <summary>
    /// Output directory configuration
    /// </summary>
    public string BackendOutput { get; set; }

    /// <summary>
    /// Release configuration options
    /// </summary>
    public PublishOptions Publish { get; set; }

    /// <summary>
    /// Exclude file list
    /// </summary>
    public List<string> ExcludeFiles { get; set; }
}

/// <summary>
/// Compile release configuration options
/// </summary>
public class PublishOptions
{
    /// <summary>
    /// Release environment configuration
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// target framework
    /// </summary>
    public string TargetFramework { get; set; }

    /// <summary>
    /// Operating environment
    /// </summary>
    public string RuntimeIdentifier { get; set; }
}