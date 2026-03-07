// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System schedule
/// </summary>
[SugarTable(null, "System schedule")]
[SysTable]
public class SysSchedule : EntityBaseTenant
{
    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// Schedule date
    /// </summary>
    [SugarColumn(ColumnDescription = "Schedule Date")]
    public DateTime? ScheduleTime { get; set; }

    /// <summary>
    /// start time
    /// </summary>
    [SugarColumn(ColumnDescription = "start time", Length = 10)]
    public string? StartTime { get; set; }

    /// <summary>
    /// end time
    /// </summary>
    [SugarColumn(ColumnDescription = "end time", Length = 10)]
    public string? EndTime { get; set; }

    /// <summary>
    /// Schedule content
    /// </summary>
    [SugarColumn(ColumnDescription = "Schedule content", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string Content { get; set; }

    /// <summary>
    /// completion status
    /// </summary>
    [SugarColumn(ColumnDescription = "Completion Status")]
    public FinishStatusEnum Status { get; set; } = FinishStatusEnum.UnFinish;
}