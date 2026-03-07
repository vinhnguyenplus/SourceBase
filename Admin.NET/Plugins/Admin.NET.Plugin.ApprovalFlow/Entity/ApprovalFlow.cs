// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.ApprovalFlow;

/// <summary>
/// Approval Process Information Sheet
/// </summary>
[SugarTable(null, "Approval Process Information Form")]
public class ApprovalFlow : EntityBaseOrgDel
{
    /// <summary>
    /// No
    /// </summary>
    [SugarColumn(ColumnDescription = "Number", Length = 32)]
    [MaxLength(32)]
    public string? Code { get; set; }

    /// <summary>
    /// name
    /// </summary>
    [SugarColumn(ColumnDescription = "name", Length = 32)]
    [MaxLength(32)]
    public string Name { get; set; }

    /// <summary>
    /// form
    /// </summary>
    [SugarColumn(ColumnDescription = "form", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormJson { get; set; }

    /// <summary>
    /// process
    /// </summary>
    [SugarColumn(ColumnDescription = "Process", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FlowJson { get; set; }

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public int? Status { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }
}