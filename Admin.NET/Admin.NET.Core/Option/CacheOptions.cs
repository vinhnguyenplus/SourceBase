// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Cache configuration options
/// </summary>
public sealed class CacheOptions : IConfigurableOptions<CacheOptions>
{
    /// <summary>
    /// cache prefix
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// cache type
    /// </summary>
    public string CacheType { get; set; }

    /// <summary>
    /// Redis cache
    /// </summary>
    public RedisOption Redis { get; set; }

    public void PostConfigure(CacheOptions options, IConfiguration configuration)
    {
        options.Prefix = string.IsNullOrWhiteSpace(options.Prefix) ? "" : options.Prefix.Trim();
    }
}

/// <summary>
/// Redis cache
/// </summary>
public sealed class RedisOption : RedisOptions
{
    /// <summary>
    /// Maximum message size
    /// </summary>
    public int MaxMessageSize { get; set; }
}

/// <summary>
/// Cluster configuration options
/// </summary>
public sealed class ClusterOptions : IConfigurableOptions
{
    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Server ID
    /// </summary>
    public string ServerId { get; set; }

    /// <summary>
    /// Server IP
    /// </summary>
    public string ServerIp { get; set; }

    /// <summary>
    /// SignalR configuration
    /// </summary>
    public ClusterSignalR SignalR { get; set; }

    /// <summary>
    /// Data protection key
    /// </summary>
    public string DataProtecteKey { get; set; }

    /// <summary>
    /// Sentry mode or not
    /// </summary>
    public bool IsSentinel { get; set; }

    /// <summary>
    /// Sentinel configuration
    /// </summary>
    public StackExchangeSentinelConfig SentinelConfig { get; set; }
}

/// <summary>
/// Cluster SignalR configuration
/// </summary>
public sealed class ClusterSignalR
{
    /// <summary>
    /// Redis connection string
    /// </summary>
    public string RedisConfiguration { get; set; }

    /// <summary>
    /// cache prefix
    /// </summary>
    public string ChannelPrefix { get; set; }
}

/// <summary>
/// Sentinel configuration
/// </summary>
public sealed class StackExchangeSentinelConfig
{
    /// <summary>
    /// master name
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// master access password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Sentinel access password
    /// </summary>
    public string SentinelPassword { get; set; }

    /// <summary>
    /// sentry port
    /// </summary>
    public List<string> EndPoints { get; set; }

    /// <summary>
    /// Default library
    /// </summary>
    public int DefaultDb { get; set; }

    /// <summary>
    /// main prefix
    /// </summary>
    public string MainPrefix { get; set; }

    /// <summary>
    /// SignalR prefix
    /// </summary>
    public string SignalRChannelPrefix { get; set; }
}