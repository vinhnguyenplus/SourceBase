// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Elastic.Clients.Elasticsearch;

namespace Admin.NET.Core.ElasticSearch;

/// <summary>
/// ES service registration
/// </summary>
public static class ElasticSearchSetup
{
    /// <summary>
    /// Register all ES clients (log + business)
    /// </summary>
    public static void AddElasticSearchClients(this IServiceCollection services)
    {
        // 1. Create a client dictionary (enumeration → client instance)
        var clients = new Dictionary<EsClientTypeEnum, ElasticsearchClient>();

        // 2. Register log client
        var loggingClient = ElasticSearchClientFactory.CreateClient<ElasticSearchOptions>(configPath: "ElasticSearch:Logging");
        if (loggingClient != null)
        {
            clients[EsClientTypeEnum.Logging] = loggingClient;
        }

        // 3. Register business client
        var businessClient = ElasticSearchClientFactory.CreateClient<ElasticSearchOptions>(configPath: "ElasticSearch:Business");
        if (businessClient != null)
        {
            clients[EsClientTypeEnum.Business] = businessClient;
        }

        // 4. Register the client container as a singleton (globally unique)
        services.AddSingleton(new ElasticSearchClientContainer(clients));
    }
}