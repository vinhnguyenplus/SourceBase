// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// Card public data
/// </summary>
public class DingTalkCardData
{
    /// <summary>
    /// Card template content replacement parameter, ordinary text type.
    /// </summary>
    public DingTalkCardParamMap CardParamMap { get; set; }

    /// <summary>
    /// Card template content replacement parameters, multimedia type.
    /// </summary>
    public string CardMediaIdParamMap { get; set; }
}

/// <summary>
/// Card template content replacement parameters
/// </summary>
public class DingTalkCardParamMap
{
    /// <summary>
    /// Film template content replacement parameters
    /// </summary>
    [Newtonsoft.Json.JsonProperty("sys_full_json_obj")]
    [System.Text.Json.Serialization.JsonPropertyName("sys_full_json_obj")]
    public string SysFullJsonObj { get; set; }
}