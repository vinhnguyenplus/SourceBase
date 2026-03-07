// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// APIJSON configuration options
/// </summary>
public sealed class APIJSONOptions : IConfigurableOptions
{
    /// <summary>
    /// role collection
    /// </summary>
    public List<APIJSON_Role> Roles { get; set; }
}

/// <summary>
/// APIJSON role permissions
/// </summary>
public class APIJSON_Role
{
    /// <summary>
    /// Character name
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// Query
    /// </summary>
    public APIJSON_RoleItem Select { get; set; }

    /// <summary>
    /// Increase
    /// </summary>
    public APIJSON_RoleItem Insert { get; set; }

    /// <summary>
    /// renew
    /// </summary>
    public APIJSON_RoleItem Update { get; set; }

    /// <summary>
    /// delete
    /// </summary>
    public APIJSON_RoleItem Delete { get; set; }
}

/// <summary>
/// APIJSON role permission content
/// </summary>
public class APIJSON_RoleItem
{
    /// <summary>
    /// table collection
    /// </summary>
    public string[] Table { get; set; }

    /// <summary>
    /// Column set
    /// </summary>
    public string[] Column { get; set; }

    /// <summary>
    /// filter
    /// </summary>
    public string[] Filter { get; set; }
}