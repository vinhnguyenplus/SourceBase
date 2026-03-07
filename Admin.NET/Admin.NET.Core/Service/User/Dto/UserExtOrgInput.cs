// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class UserExtOrgInput : BaseIdInput
{
    /// <summary>
    /// InstitutionId
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// PositionId
    /// </summary>
    public long PosId { get; set; }

    /// <summary>
    /// Job number
    /// </summary>
    public string JobNum { get; set; }

    /// <summary>
    /// Rank
    /// </summary>
    public string PosLevel { get; set; }

    /// <summary>
    /// Joining date
    /// </summary>
    public DateTime? JoinDate { get; set; }
}