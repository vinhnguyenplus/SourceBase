// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Elastic.Clients.Elasticsearch;

namespace Admin.NET.Core;

/// <summary>
/// ES client container
/// </summary>
public class ElasticSearchClientContainer
{
    private readonly Dictionary<EsClientTypeEnum, ElasticsearchClient> _clients;

    /// <summary>
    /// Initialize container (inject all clients via dictionary)
    /// </summary>
    public ElasticSearchClientContainer(Dictionary<EsClientTypeEnum, ElasticsearchClient> clients)
    {
        _clients = clients ?? throw new ArgumentNullException(nameof(clients));
    }

    /// <summary>
    /// Log dedicated client
    /// </summary>
    public ElasticsearchClient Logging => GetClient(EsClientTypeEnum.Logging);

    /// <summary>
    /// Business data synchronization client
    /// </summary>
    public ElasticsearchClient Business => GetClient(EsClientTypeEnum.Business);

    /// <summary>
    /// Get the client based on type (internal validation to avoid unregistered types)
    /// </summary>
    private ElasticsearchClient GetClient(EsClientTypeEnum type)
    {
        if (_clients.TryGetValue(type, out var client))
        {
            return client;
        }
        throw new KeyNotFoundException($"Unregistered ES client type: {type}, please check the registration configuration");
    }
}