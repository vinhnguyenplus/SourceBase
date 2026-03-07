// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// Login output
/// </summary>
public class GoViewLoginOutput
{
    /// <summary>
    /// User information
    /// </summary>
    public GoViewLoginUserInfo Userinfo { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public GoViewLoginToken Token { get; set; }
}

/// <summary>
/// Login Token
/// </summary>
public class GoViewLoginToken
{
    /// <summary>
    /// Token name
    /// </summary>
    public string TokenName { get; set; } = "Authorization";

    /// <summary>
    /// Token value
    /// </summary>
    public string TokenValue { get; set; }
}

/// <summary>
/// User information
/// </summary>
public class GoViewLoginUserInfo
{
    /// <summary>
    /// UserId
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    public string Nickname { get; set; }
}