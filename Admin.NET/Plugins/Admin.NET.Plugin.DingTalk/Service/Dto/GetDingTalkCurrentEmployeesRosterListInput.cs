// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class GetDingTalkCurrentEmployeesRosterListInput
{
    /// <summary>
    /// A list of employee userIds. Use commas to separate multiple userids. A maximum of 100 values ​​can be passed at one time.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("userid_list")]
    [System.Text.Json.Serialization.JsonPropertyName("userid_list")]
    public string UserIdList { get; set; }

    /// <summary>
    /// A list of roster field field_code values ​​that need to be obtained. Use commas to separate multiple fields. A maximum of 100 values ​​can be passed at a time.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field_filter_list")]
    [System.Text.Json.Serialization.JsonPropertyName("field_filter_list")]
    public string FieldFilterList { get; set; }

    /// <summary>
    /// AgentId of the application
    /// </summary>
    [Newtonsoft.Json.JsonProperty("agentid")]
    [System.Text.Json.Serialization.JsonPropertyName("agentid")]
    public string AgentId { get; set; }
}