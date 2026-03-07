// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Event type - system user operation enumeration
/// </summary>
[SuppressSniffer]
[Description("Event Type - System User Operation Enumeration")]
public enum SysUserEventTypeEnum
{
    /// <summary>
    /// Add user
    /// </summary>
    [Description("increaseUser")]
    Add = 111,

    /// <summary>
    /// Update user
    /// </summary>
    [Description("Update user")]
    Update = 222,

    /// <summary>
    /// Authorized user roles
    /// </summary>
    [Description("Authorized user roles")]
    UpdateRole = 333,

    /// <summary>
    /// Delete user
    /// </summary>
    [Description("Delete User")]
    Delete = 444,

    /// <summary>
    /// Set user status
    /// </summary>
    [Description("Set user status")]
    SetStatus = 555,

    /// <summary>
    /// Change password
    /// </summary>
    [Description("Change Password")]
    ChangePwd = 666,

    /// <summary>
    /// reset password
    /// </summary>
    [Description("reset password")]
    ResetPwd = 777,

    /// <summary>
    /// Unlock login
    /// </summary>
    [Description("Remove login lock")]
    UnlockLogin = 888,

    /// <summary>
    /// Registered user
    /// </summary>
    [Description("Registered user")]
    Register = 999,

    /// <summary>
    /// User login
    /// </summary>
    [Description("User login")]
    Login = 1000,

    /// <summary>
    /// User exits
    /// </summary>
    [Description("User logout")]
    LoginOut = 1001,

    /// <summary>
    /// RefreshToken
    /// </summary>
    [Description("Refresh Token")]
    RefreshToken = 1002,
}