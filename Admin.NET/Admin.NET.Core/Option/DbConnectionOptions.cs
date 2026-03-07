// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Database configuration options
/// </summary>
public sealed class DbConnectionOptions : IConfigurableOptions<DbConnectionOptions>
{
    /// <summary>
    /// Enable console printing SQL
    /// </summary>
    public bool EnableConsoleSql { get; set; }

    /// <summary>
    /// Whether super admin ignores tombstone filters
    /// </summary>
    public bool SuperAdminIgnoreIDeletedFilter { get; set; }

    /// <summary>
    /// Database collection
    /// </summary>
    public List<DbConnectionConfig> ConnectionConfigs { get; set; }

    public void PostConfigure(DbConnectionOptions options, IConfiguration configuration)
    {
        foreach (var dbConfig in options.ConnectionConfigs)
        {
            if (dbConfig.ConfigId == null || string.IsNullOrWhiteSpace(dbConfig.ConfigId.ToString()))
                dbConfig.ConfigId = SqlSugarConst.MainConfigId;
        }
    }
}

/// <summary>
/// Database connection configuration
/// </summary>
public sealed class DbConnectionConfig : ConnectionConfig
{
    /// <summary>
    /// Database name
    /// </summary>
    public string DbNickName { get; set; }

    /// <summary>
    /// Database configuration
    /// </summary>
    public DbSettings DbSettings { get; set; }

    /// <summary>
    /// table configuration
    /// </summary>
    public TableSettings TableSettings { get; set; }

    /// <summary>
    /// Seed configuration
    /// </summary>
    public SeedSettings SeedSettings { get; set; }

    /// <summary>
    /// Isolation method
    /// </summary>
    public TenantTypeEnum TenantType { get; set; } = TenantTypeEnum.Id;

    /// <summary>
    /// Database storage directory (only SqlServer supports specified directory creation)
    /// </summary>
    public string DatabaseDirectory { get; set; }
}

/// <summary>
/// Database configuration
/// </summary>
public sealed class DbSettings
{
    /// <summary>
    /// Enable library table initialization
    /// </summary>
    public bool EnableInitDb { get; set; }

    /// <summary>
    /// Enable view initialization
    /// </summary>
    public bool EnableInitView { get; set; }

    /// <summary>
    /// Enable database table difference logging
    /// </summary>
    public bool EnableDiffLog { get; set; }

    /// <summary>
    /// Enable CamelCase to Underline
    /// </summary>
    public bool EnableUnderLine { get; set; }

    /// <summary>
    /// Enable database connection string encryption policy
    /// </summary>
    public bool EnableConnStringEncrypt { get; set; }
}

/// <summary>
/// table configuration
/// </summary>
public sealed class TableSettings
{
    /// <summary>
    /// Enable table initialization
    /// </summary>
    public bool EnableInitTable { get; set; }

    /// <summary>
    /// Enable table incremental updates
    /// </summary>
    public bool EnableIncreTable { get; set; }
}

/// <summary>
/// Seed configuration
/// </summary>
public sealed class SeedSettings
{
    /// <summary>
    /// Enable seed initialization
    /// </summary>
    public bool EnableInitSeed { get; set; }

    /// <summary>
    /// Enable seed incremental updates
    /// </summary>
    public bool EnableIncreSeed { get; set; }
}