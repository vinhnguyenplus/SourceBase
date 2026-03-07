// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.ApprovalFlow;

/// <summary>
/// Approval flow form record
/// </summary>
[SugarTable(null, "Approval workflow form records")]
public class ApprovalFormRecord : EntityBaseOrg
{
    /// <summary>
    /// ProcessId
    /// </summary>
    [SugarColumn(ColumnDescription = "Process Id")]
    public long? FlowId { get; set; }

    /// <summary>
    /// form name
    /// </summary>
    [SugarColumn(ColumnDescription = "formname", Length = 32)]
    public string? FormName { get; set; }

    /// <summary>
    /// form type
    /// </summary>
    [SugarColumn(ColumnDescription = "Form Type", Length = 32)]
    public string? FormType { get; set; }

    /// <summary>
    /// form status
    /// </summary>
    [SugarColumn(ColumnDescription = "Form Status", Length = 11)]
    public string? FormStatus { get; set; }

    /// <summary>
    /// Before modification
    /// </summary>
    [SugarColumn(ColumnDescription = "Before modification", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormBefore { get; set; }

    /// <summary>
    /// After modification
    /// </summary>
    [SugarColumn(ColumnDescription = "After modification", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormAfter { get; set; }

    /// <summary>
    /// form results
    /// </summary>
    [SugarColumn(ColumnDescription = "form results", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? FormResult { get; set; }
}