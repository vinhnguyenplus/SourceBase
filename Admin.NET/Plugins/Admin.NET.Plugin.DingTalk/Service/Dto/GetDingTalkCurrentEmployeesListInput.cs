// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// Get active employee list parameters
/// </summary>
public class GetDingTalkCurrentEmployeesListInput
{
    /// <summary>
    /// Filter active employee status to query multiple statuses. Use commas to separate different states. 2: Probationary period, 3: Official, 5: Pending resignation, -1: No status
    /// </summary>
    [Newtonsoft.Json.JsonProperty("status_list")]
    [System.Text.Json.Serialization.JsonPropertyName("status_list")]
    public string StatusList { get; set; }

    /// <summary>
    /// Paging cursor, starting from 0. Determine whether there is a next page based on whether next_cursor in the returned result is empty, and offset is set to the value of next_cursor when called again.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Paging size, maximum 50.
    /// </summary>
    public int Size { get; set; }
}