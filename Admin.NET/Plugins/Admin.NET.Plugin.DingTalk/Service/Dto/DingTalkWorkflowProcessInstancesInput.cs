// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkWorkflowProcessInstancesInput
{
    /// <summary>
    /// Initiator user ID
    /// </summary>
    public string OriginatorUserId { get; set; }

    /// <summary>
    /// Process coding of approval template
    /// </summary>
    public string ProcessCode { get; set; }

    /// <summary>
    /// Department ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// MicroapplicationAgentId
    /// </summary>
    public long MicroappAgentId { get; set; }

    /// <summary>
    /// Approver list (supports multiple nodes)
    /// </summary>
    public List<Approver> Approvers { get; set; }

    /// <summary>
    /// CC list
    /// </summary>
    public List<string> CcList { get; set; }

    /// <summary>
    /// CC position: START (beginning), MIDDLE (middle), END (end)
    /// </summary>
    public string CcPosition { get; set; }

    /// <summary>
    /// The target dynamically selects the handler (used in scenarios such as counter-signing or signing)
    /// </summary>
    public List<TargetSelectActioner> TargetSelectActioners { get; set; }

    /// <summary>
    /// form component value list
    /// </summary>
    public List<FormComponentValue> FormComponentValues { get; set; }

    /// <summary>
    /// Request ID, used for idempotent control
    /// </summary>
    public string RequestId { get; set; }
}

/// <summary>
/// Approver information
/// </summary>
public class Approver
{
    /// <summary>
    /// Node type: AGREE (agree), REFUSE (reject), etc.
    /// </summary>
    public string ActionType { get; set; }

    /// <summary>
    /// List of approver user IDs for this node
    /// </summary>
    public List<string> UserIds { get; set; }
}

/// <summary>
/// Dynamic selection of handlers
/// </summary>
public class TargetSelectActioner
{
    /// <summary>
    /// Key of the person in charge, corresponding to the key of the person selection control in the form
    /// </summary>
    public string ActionerKey { get; set; }

    /// <summary>
    /// List of user IDs selected by this control
    /// </summary>
    public List<string> ActionerUserIds { get; set; }
}

/// <summary>
/// form component value
/// </summary>
public class FormComponentValue
{
    public string ComponentType { get; set; }
    public string Name { get; set; }
    public string BizAlias { get; set; }
    public string Id { get; set; }
    public string Value { get; set; }
    public string ExtValue { get; set; }
}