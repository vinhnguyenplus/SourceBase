// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class JobTriggerInput
{
    /// <summary>
    /// JobId
    /// </summary>
    public string JobId { get; set; }

    /// <summary>
    /// TriggerId
    /// </summary>
    public string TriggerId { get; set; }
}

public class AddJobTriggerInput : SysJobTrigger
{
    /// <summary>
    /// JobId
    /// </summary>
    [Required(ErrorMessage = "Job ID cannot be empty"), MinLength(2, ErrorMessage = "The assignment ID cannot be less than 2 characters")]
    public override string JobId { get; set; }

    /// <summary>
    /// TriggerId
    /// </summary>
    [Required(ErrorMessage = "Trigger ID cannot be empty"), MinLength(2, ErrorMessage = "Trigger ID cannot be less than 2 characters")]
    public override string TriggerId { get; set; }
}

public class UpdateJobTriggerInput : AddJobTriggerInput
{
}

public class DeleteJobTriggerInput : JobTriggerInput
{
}