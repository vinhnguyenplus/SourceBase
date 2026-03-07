// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

[SugarTable("ding_talk_wokerflow_log", "DingTalk Approval Log")]
public class DingTalkWokerflowLog
{
    /// <summary>
    /// Approval instance ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Approval instance ID", IsPrimaryKey = true, IsIdentity = false)]
    public string instanceId { get; set; }

    /// <summary>
    /// Approval order number
    /// </summary>
    [SugarColumn(ColumnDescription = "Approval order number")]
    public string? WorkflowId { get; set; }

    /// <summary>
    /// Source document
    /// </summary>
    [SugarColumn(ColumnDescription = "Source document")]
    public string SourceDocument { get; set; }

    /// <summary>
    /// Approval completion time
    /// </summary>
    [SugarColumn(ColumnDescription = "Approval completion time")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Other information
    /// </summary>
    [SugarColumn(ColumnDescription = "Other information", IsJson = true)]
    public Dictionary<string, object>? other_info { get; set; }

    /// <summary>
    /// Whether to return the results to a third party
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to return results")]
    public bool? isReturn { get; set; }

    /// <summary>
    /// Approval status
    /// </summary>
    /// <remarks>
    /// RUNNING: Approval in progress TERMINATED: Revoked COMPLETED: Approval completed
    /// /// </remarks>
    [SugarColumn(ColumnDescription = "Approval Status")]
    public string Status { get; set; }

    /// <summary>
    /// Task ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Task ID")]
    public long? taskId { get; set; }

    /// <summary>
    /// Approval result agree: agree refuse: refuse
    /// </summary>
    [SugarColumn(ColumnDescription = "Approval Result")]
    public string? Result { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    [SugarColumn(ColumnDescription = "Creator name", Length = 64, IsOnlyIgnoreUpdate = true)]
    public string? CreateUserName { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    [SugarColumn(ColumnDescription = "Creation Time", IsNullable = true, IsOnlyIgnoreUpdate = true)]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public virtual DateTime? UpdateTime { get; set; }
}