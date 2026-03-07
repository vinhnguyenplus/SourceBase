// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// DingTalk foundation response results
/// </summary>
/// <typeparam name="T">Data</typeparam>
public class DingTalkBaseResponse<T>
{
    /// <summary>
    /// Return results
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// return code
    /// </summary>
    public int ErrCode { get; set; }

    /// <summary>
    /// Return code description.
    /// </summary>
    public string ErrMsg { get; set; }

    /// <summary>
    /// Whether the call was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// RequestId
    /// </summary>
    [Newtonsoft.Json.JsonProperty("request_id")]
    [System.Text.Json.Serialization.JsonPropertyName("request_id")]
    public string RequestId { get; set; }
}