// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// ES configuration options
/// </summary>
public class ElasticSearchOptions
{
    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// ES authentication type, optional Basic, ApiKey, Base64ApiKey
    /// </summary>
    public ElasticSearchAuthTypeEnum AuthType { get; set; }

    /// <summary>
    /// Username for Basic authentication
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Basic authentication password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// ApiId certified by ApiKey
    /// </summary>
    public string ApiId { get; set; }

    /// <summary>
    /// ApiKey certified ApiKey
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The encrypted string encrypted during Base64ApiKey authentication
    /// </summary>
    public string Base64ApiKey { get; set; }

    /// <summary>
    /// Certificate fingerprint when ES uses HTTPS. Please implement it yourself when using the certificate.
    /// <para>https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/connecting.html</para>
    /// </summary>
    public string Fingerprint { get; set; }

    /// <summary>
    /// address
    /// </summary>
    public List<string> ServerUris { get; set; } = new List<string>();

    /// <summary>
    /// index
    /// </summary>
    public string DefaultIndex { get; set; }
}