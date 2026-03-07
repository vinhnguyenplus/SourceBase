// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// HTTP request method enumeration
/// </summary>
[Description("HTTP request method enumeration")]
public enum HttpMethodEnum
{
    /// <summary>
    ///  HTTP "GET" method.
    /// </summary>
    [Description("HTTP \"GET\" method.")]
    Get,

    /// <summary>
    ///  HTTP "POST" method.
    /// </summary>
    [Description("HTTP \"POST\" method.")]
    Post,

    /// <summary>
    /// HTTP "PUT" method.
    /// </summary>
    [Description(" HTTP \"PUT\" method.")]
    Put,

    /// <summary>
    /// HTTP "DELETE" method.
    /// </summary>
    [Description("HTTP \"DELETE\" method.")]
    Delete,

    /// <summary>
    /// HTTP "PATCH" method.
    /// </summary>
    [Description("HTTP \"PATCH\" method. ")]
    Patch,

    /// <summary>
    /// HTTP "HEAD" method.
    /// </summary>
    [Description("HTTP \"HEAD\" method.")]
    Head,

    /// <summary>
    /// HTTP "OPTIONS" method.
    /// </summary>
    [Description("HTTP \"OPTIONS\" method.")]
    Options,

    /// <summary>
    /// HTTP "TRACE" method.
    /// </summary>
    [Description(" HTTP \"TRACE\" method.")]
    Trace,

    /// <summary>
    ///  HTTP "CONNECT" method.
    /// </summary>
    [Description("HTTP \"CONNECT\" method.")]
    Connect
}