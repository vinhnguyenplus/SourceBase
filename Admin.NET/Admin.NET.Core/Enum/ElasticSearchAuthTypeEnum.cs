// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// ES authentication type enumeration
/// <para>https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/_options_on_elasticsearchclientsettings.html</para>
/// </summary>
[Description("ES authentication type enumeration")]
public enum ElasticSearchAuthTypeEnum
{
    /// <summary>
    /// BasicAuthentication
    /// </summary>
    [Description("BasicAuthentication")]
    Basic = 1,

    /// <summary>
    /// ApiKey
    /// </summary>
    [Description("ApiKey")]
    ApiKey = 2,

    /// <summary>
    /// Base64ApiKey
    /// </summary>
    [Description("Base64ApiKey")]
    Base64ApiKey = 3,

    /// <summary>
    /// No verification
    /// </summary>
    [Description("None")]
    None = 4
}