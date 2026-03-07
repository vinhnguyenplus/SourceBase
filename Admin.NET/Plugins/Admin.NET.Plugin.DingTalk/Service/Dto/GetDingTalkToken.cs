// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class GetDingTalkTokenOutput
{
    /// <summary>
    /// Generated access_token
    /// </summary>
    [Newtonsoft.Json.JsonProperty("access_token")]
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Expiration time of access_token, in seconds
    /// </summary>
    [Newtonsoft.Json.JsonProperty("expires_in")]
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Return code description
    /// </summary>
    public string ErrMsg { get; set; }

    /// <summary>
    /// return code
    /// </summary>
    public int ErrCode { get; set; }
}