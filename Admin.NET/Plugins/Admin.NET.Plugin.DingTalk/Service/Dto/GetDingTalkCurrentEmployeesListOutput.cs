// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class GetDingTalkCurrentEmployeesListOutput
{
    /// <summary>
    /// List of queried employee userIds
    /// </summary>
    [Newtonsoft.Json.JsonProperty("data_list")]
    [System.Text.Json.Serialization.JsonPropertyName("data_list")]
    public List<string> DataList { get; set; }

    /// <summary>
    /// The offset value of the next paging call. When there is no next_cursor in the returned result, it means that paging is over.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("next_cursor")]
    [System.Text.Json.Serialization.JsonPropertyName("next_cursor")]
    public int? NextCursor { get; set; }
}