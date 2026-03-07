// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System job trigger running record table
/// </summary>
[SugarTable(null, "System job trigger running record table")]
[SysTable]
public partial class SysJobTriggerRecord : EntityBaseId
{
    /// <summary>
    /// JobId
    /// </summary>
    [SugarColumn(ColumnDescription = "JobId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string JobId { get; set; }

    /// <summary>
    /// TriggerId
    /// </summary>
    [SugarColumn(ColumnDescription = "Trigger ID", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string TriggerId { get; set; }

    /// <summary>
    /// Current number of runs
    /// </summary>
    [SugarColumn(ColumnDescription = "Current number of runs")]
    public long NumberOfRuns { get; set; }

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
    /// trigger state
    /// </summary>
    [SugarColumn(ColumnDescription = "trigger state")]
    public TriggerStatus Status { get; set; } = TriggerStatus.Ready;

    /// <summary>
    /// The result of this execution
    /// </summary>
    [SugarColumn(ColumnDescription = "This execution result", Length = 128)]
    [MaxLength(128)]
    public string? Result { get; set; }

    /// <summary>
    /// This execution takes time
    /// </summary>
    [SugarColumn(ColumnDescription = "Execution time for this run")]
    public long ElapsedTime { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    [SugarColumn(ColumnDescription = "Creation Time")]
    public DateTime? CreatedTime { get; set; }
}