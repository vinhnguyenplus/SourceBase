// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// DingTalk department information
/// </summary>
[SugarTable("ding_talk_dept", "DingTalk Department Table")]
public class DingTalkDept
{
    /// <summary>
    /// department id
    /// </summary>
    [SugarColumn(ColumnName = "Id", ColumnDescription = "Department ID", IsPrimaryKey = true, IsIdentity = false)]
    [Required]
    public long dept_id { get; set; }

    /// <summary>
    /// Superior department id
    /// </summary>
    [SugarColumn(ColumnDescription = "Superior department id")]
    [Required]
    public virtual long parent_id { get; set; }

    /// <summary>
    /// Department name
    /// </summary>
    [SugarColumn(ColumnDescription = "Department name", Length = 64)]
    [MaxLength(64)]
    public string? name { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    [SugarColumn(ColumnDescription = "Creation Time", IsNullable = true, IsOnlyIgnoreUpdate = true)]
    public virtual DateTime CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public virtual DateTime? UpdateTime { get; set; }
}