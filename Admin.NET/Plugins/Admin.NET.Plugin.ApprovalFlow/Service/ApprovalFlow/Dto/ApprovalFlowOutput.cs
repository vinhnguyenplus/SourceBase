// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.ApprovalFlow.Service;

/// <summary>
/// Approval flow output parameters
/// </summary>
public class ApprovalFlowOutput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// No
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// form
    /// </summary>
    public string? FormJson { get; set; }

    /// <summary>
    /// process
    /// </summary>
    public string? FlowJson { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    public string? CreateUserName { get; set; }

    /// <summary>
    /// Modifier ID
    /// </summary>
    public long? UpdateUserId { get; set; }

    /// <summary>
    /// Modifier name
    /// </summary>
    public string? UpdateUserName { get; set; }

    /// <summary>
    /// Creator department ID
    /// </summary>
    public long? CreateOrgId { get; set; }

    /// <summary>
    /// Creator department name
    /// </summary>
    public string? CreateOrgName { get; set; }

    /// <summary>
    /// soft delete
    /// </summary>
    public bool IsDelete { get; set; }
}