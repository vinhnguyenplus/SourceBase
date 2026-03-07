// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System job trigger table
/// </summary>
[SugarTable(null, "System Job Trigger Table")]
[SysTable]
public partial class SysJobTrigger : EntityBaseId
{
    /// <summary>
    /// TriggerId
    /// </summary>
    [SugarColumn(ColumnDescription = "Trigger ID", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string TriggerId { get; set; }

    /// <summary>
    /// JobId
    /// </summary>
    [SugarColumn(ColumnDescription = "JobId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string JobId { get; set; }

    /// <summary>
    /// Trigger typeFullName
    /// </summary>
    [SugarColumn(ColumnDescription = "Trigger type", Length = 128)]
    [MaxLength(128)]
    public string? TriggerType { get; set; }

    /// <summary>
    /// Assembly Name
    /// </summary>
    [SugarColumn(ColumnDescription = "Assembly", Length = 128)]
    [MaxLength(128)]
    public string? AssemblyName { get; set; } = "Furion.Pure";

    /// <summary>
    /// parameter
    /// </summary>
    [SugarColumn(ColumnDescription = "Parameter", Length = 128)]
    [MaxLength(128)]
    public string? Args { get; set; }

    /// <summary>
    /// Description information
    /// </summary>
    [SugarColumn(ColumnDescription = "Description information", Length = 128)]
    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public TriggerStatus Status { get; set; } = TriggerStatus.Ready;

    /// <summary>
    /// start time
    /// </summary>
    [SugarColumn(ColumnDescription = "start time")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// end time
    /// </summary>
    [SugarColumn(ColumnDescription = "end time")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Last running time
    /// </summary>
    [SugarColumn(ColumnDescription = "Recent run time")]
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// Next run time
    /// </summary>
    [SugarColumn(ColumnDescription = "Next Run Time")]
    public DateTime? NextRunTime { get; set; }

    /// <summary>
    /// Number of triggers
    /// </summary>
    [SugarColumn(ColumnDescription = "Number of triggers")]
    public long NumberOfRuns { get; set; }

    /// <summary>
    /// Maximum number of triggers (0: no limit, n: N times)
    /// </summary>
    [SugarColumn(ColumnDescription = "Maximum trigger count")]
    public long MaxNumberOfRuns { get; set; }

    /// <summary>
    /// Number of errors
    /// </summary>
    [SugarColumn(ColumnDescription = "Number of errors")]
    public long NumberOfErrors { get; set; }

    /// <summary>
    /// Maximum number of errors (0: no limit, n: N times)
    /// </summary>
    [SugarColumn(ColumnDescription = "Maximum number of errors")]
    public long MaxNumberOfErrors { get; set; }

    /// <summary>
    /// Number of retries
    /// </summary>
    [SugarColumn(ColumnDescription = "Number of retries")]
    public int NumRetries { get; set; }

    /// <summary>
    /// Retry interval (ms)
    /// </summary>
    [SugarColumn(ColumnDescription = "Retry interval (ms)")]
    public int RetryTimeout { get; set; } = 1000;

    /// <summary>
    /// Whether to start immediately
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to start immediately")]
    public bool StartNow { get; set; } = true;

    /// <summary>
    /// Whether to execute once at startup
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to execute once at startup")]
    public bool RunOnStart { get; set; } = false;

    /// <summary>
    /// Whether to reset jobs with a maximum number of triggers equal to one on startup
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to reset the number of triggers")]
    public bool ResetOnlyOnce { get; set; } = true;

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public DateTime? UpdatedTime { get; set; }
}