// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkEmpRosterFieldVo
{
    /// <summary>
    /// corporate corpid
    /// </summary>
    [Newtonsoft.Json.JsonProperty("corp_id")]
    [System.Text.Json.Serialization.JsonPropertyName("corp_id")]
    public string CorpId { get; set; }

    /// <summary>
    /// Returned field information list
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field_data_list")]
    [System.Text.Json.Serialization.JsonPropertyName("field_data_list")]
    public List<DingTalkEmpFieldDataVo> FieldDataList { get; set; }

    /// <summary>
    /// employee userid
    /// </summary>
    [Newtonsoft.Json.JsonProperty("userid")]
    [System.Text.Json.Serialization.JsonPropertyName("userid")]
    public string UserId { get; set; }
}