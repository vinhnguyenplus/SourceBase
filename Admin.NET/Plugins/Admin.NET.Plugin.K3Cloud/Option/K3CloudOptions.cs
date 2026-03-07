// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.K3Cloud;

public sealed class K3CloudOptions : IConfigurableOptions
{
    /// <summary>
    /// ERP business site address
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Account ID (data center ID)
    /// </summary>
    public string AcctID { get; set; }

    /// <summary>
    /// ApplicationId
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// application key
    /// </summary>
    public string AppKey { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// User password
    /// </summary>
    public string UserPassword { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    public string LanguageCode { get; set; }
}