// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System job cluster table
/// </summary>
[SugarTable(null, "System job cluster table")]
[SysTable]
public partial class SysJobCluster : EntityBaseId
{
    /// <summary>
    /// Job cluster ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Job Cluster Id", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string ClusterId { get; set; }

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
    public ClusterStatus Status { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public DateTime? UpdatedTime { get; set; }
}