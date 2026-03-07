// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Admin.NET.Core;

public class ElasticSearchClientFactory
{
    /// <summary>
    /// Create ES client (generic approach)
    /// </summary>
    /// <typeparam name="TOptions">Configuration type (supports general or scene-specific)</typeparam>
    /// <param name="configPath">Configuration file path (such as "ElasticSearch:Logging")</param>
    /// <returns>ES client instance (or null if not enabled)</returns>
    public static ElasticsearchClient? CreateClient<TOptions>(string configPath) where TOptions : ElasticSearchOptions, new()
    {
        // Read the configuration of the current scene from the configuration file
        var options = App.GetConfig<TOptions>(configPath);
        if (options == null)
            throw Oops.Oh($"Configuration item {configPath} not found");

        if (!options.Enabled)
            return null;

        // Verify service address
        if (options.ServerUris == null || !options.ServerUris.Any())
            throw new ArgumentException($"ES configuration {configPath} ServerUris not set");

        // Build a connection pool (support clustering)
        var uris = options.ServerUris.Select(uri => new Uri(uri)).ToList();
        var connectionPool = new StaticNodePool(uris);
        var connectionSettings = new ElasticsearchClientSettings(connectionPool)
            .DefaultIndex(options.DefaultIndex) // Set default index
            .DisableDirectStreaming()  // Enable request/response logs to facilitate troubleshooting
            .OnRequestCompleted(response =>
            {
                if (response.HttpStatusCode == 401)
                {
                    Console.WriteLine("ES request denied: No valid authentication information provided");
                }
            });

        // Configure authentication
        ConfigureAuthentication(connectionSettings, options);

        // Configure HTTPS certificate fingerprint
        if (!string.IsNullOrEmpty(options.Fingerprint))
            connectionSettings.CertificateFingerprint(options.Fingerprint);

        return new ElasticsearchClient(connectionSettings);
    }

    /// <summary>
    /// Configure authentication (general logic)
    /// </summary>
    private static void ConfigureAuthentication(ElasticsearchClientSettings settings, ElasticSearchOptions options)
    {
        switch (options.AuthType)
        {
            case ElasticSearchAuthTypeEnum.Basic:
                settings.Authentication(new BasicAuthentication(options.User, options.Password));
                break;

            case ElasticSearchAuthTypeEnum.ApiKey:
                settings.Authentication(new ApiKey(options.ApiKey));
                break;

            case ElasticSearchAuthTypeEnum.Base64ApiKey:
                settings.Authentication(new Base64ApiKey(options.Base64ApiKey));
                break;

            case ElasticSearchAuthTypeEnum.None:
                // No certification required
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.AuthType));
        }
    }
}