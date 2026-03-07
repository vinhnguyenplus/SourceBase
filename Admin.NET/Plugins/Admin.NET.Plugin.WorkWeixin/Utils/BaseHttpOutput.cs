// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// Enterprise WeChat interface output base class
/// </summary>
public class BaseWorkOutput
{
    /// <summary>
    /// return code
    /// </summary>
    [JsonProperty("errcode")]
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// Text description of the return code
    /// </summary>
    [JsonProperty("errmsg")]
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; }
}

/// <summary>
/// Output parameter with id
/// </summary>
public class BaseWorkIdOutput : BaseWorkOutput
{
    /// <summary>
    /// id
    /// </summary>
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public long? Id { get; set; }
}