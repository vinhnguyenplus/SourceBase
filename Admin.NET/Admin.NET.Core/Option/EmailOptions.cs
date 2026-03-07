// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Email configuration options
/// </summary>
public sealed class EmailOptions : IConfigurableOptions
{
    /// <summary>
    /// Host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Default sender email
    /// </summary>
    public string DefaultFromEmail { get; set; }

    /// <summary>
    /// Default recipient email
    /// </summary>
    public string DefaultToEmail { get; set; }

    /// <summary>
    /// Enable SSL
    /// </summary>
    public bool EnableSsl { get; set; }

    ///// <summary>
    ///// Whether to use default credentials
    ///// </summary>
    //public bool UseDefaultCredentials { get; set; }

    /// <summary>
    /// Email account
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Email password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Default email title
    /// </summary>
    public string DefaultFromName { get; set; }
}