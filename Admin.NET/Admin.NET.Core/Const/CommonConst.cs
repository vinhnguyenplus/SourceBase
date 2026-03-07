// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Universal constant
/// </summary>
[Const("Platform Configuration")]
public class CommonConst
{
    /// <summary>
    /// Log group name
    /// </summary>
    public const string SysLogCategoryName = "System.Logging.LoggingMonitor";

    /// <summary>
    /// Event-Add exception log
    /// </summary>
    public const string AddExLog = "Add:ExLog";

    /// <summary>
    /// Event-Send abnormal email
    /// </summary>
    public const string SendErrorMail = "Send:ErrorMail";

    /// <summary>
    /// Default base role name
    /// </summary>
    public const string DefaultBaseRoleName = "Default basic role";

    /// <summary>
    /// Default basic role encoding
    /// </summary>
    public const string DefaultBaseRoleCode = "default_base_role";
}