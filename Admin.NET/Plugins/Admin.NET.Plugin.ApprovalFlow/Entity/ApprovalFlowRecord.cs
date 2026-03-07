// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.ApprovalFlow;

/// <summary>
/// Approval flow process record
/// </summary>
[SugarTable(null, "Approval flow process record")]
public class ApprovalFlowRecord : EntityBaseOrg
{
    /// <summary>
    /// form name
    /// </summary>
    [SugarColumn(ColumnDescription = "formname", Length = 255)]
    public string? FormName { get; set; }

    /// <summary>
    /// form status
    /// </summary>
    [SugarColumn(ColumnDescription = "Form Status", Length = 32)]
    public string? FormStatus { get; set; }

    /// <summary>
    /// form trigger
    /// </summary>
    [SugarColumn(ColumnDescription = "form trigger", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormJson { get; set; }

    /// <summary>
    /// form results
    /// </summary>
    [SugarColumn(ColumnDescription = "form results", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormResult { get; set; }

    /// <summary>
    /// process structure
    /// </summary>
    [SugarColumn(ColumnDescription = "process structure", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FlowJson { get; set; }

    /// <summary>
    /// process results
    /// </summary>
    [SugarColumn(ColumnDescription = "Process Result", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FlowResult { get; set; }
}