// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.WorkWeixin.Proxy;

/// <summary>
/// Department ID list output parameters
/// </summary>
public class DepartmentIdOutput : BaseWorkOutput
{
    /// <summary>
    /// id
    /// </summary>
    [JsonProperty("department_id")]
    [JsonPropertyName("department_id")]
    public List<DepartmentItemOutput> DepartmentList { get; set; }
}

/// <summary>
/// DepartmentId output parameter
/// </summary>
public class DepartmentItemOutput
{
    /// <summary>
    /// Department name
    /// </summary>
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Parent department id
    /// </summary>
    [JsonProperty("parentid")]
    [JsonPropertyName("parentid")]
    public long? ParentId { get; set; }

    /// <summary>
    /// No
    /// </summary>
    [JsonProperty("order")]
    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

/// <summary>
/// Department output parameters
/// </summary>
public class DepartmentOutput
{
    /// <summary>
    /// Department name
    /// </summary>
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Parent department id
    /// </summary>
    [JsonProperty("parentid")]
    [JsonPropertyName("parentid")]
    public long? ParentId { get; set; }

    /// <summary>
    /// Department name
    /// </summary>
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// English name
    /// </summary>
    [JsonProperty("name_en")]
    [JsonPropertyName("name_en")]
    public string NameEn { get; set; }

    /// <summary>
    /// List of department heads
    /// </summary>
    [JsonProperty("department_leader")]
    [JsonPropertyName("department_leader")]
    public List<string> Leaders { get; set; }

    /// <summary>
    /// No
    /// </summary>
    [JsonProperty("order")]
    [JsonPropertyName("order")]
    public int? Order { get; set; }
}