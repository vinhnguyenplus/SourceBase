// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.Security.Claims;

namespace Admin.NET.Core.Service;

/// <summary>
/// Permission verification
/// </summary>
public class IdentityService : ITransient
{
    private readonly IHttpContextAccessor _context;
    private readonly List<APIJSON_Role> _roles;

    public IdentityService(IHttpContextAccessor context, IOptions<APIJSONOptions> roles)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _roles = roles.Value.Roles;
    }

    /// <summary>
    /// Get the current user ID
    /// </summary>
    /// <returns></returns>
    public string GetUserIdentity()
    {
        return _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Get the current user permission name
    /// </summary>
    /// <returns></returns>
    public string GetUserRoleName()
    {
        return _context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
    }

    /// <summary>
    /// Get current user permissions
    /// </summary>
    /// <returns></returns>
    public APIJSON_Role GetRole()
    {
        var role = string.IsNullOrEmpty(GetUserRoleName())
            ? _roles.FirstOrDefault()
            : _roles.FirstOrDefault(it => it.RoleName.Equals(GetUserRoleName(), StringComparison.CurrentCultureIgnoreCase));
        return role;
    }

    /// <summary>
    /// Get the queryable fields of the current table
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public (bool, string) GetSelectRole(string table)
    {
        var role = GetRole();
        if (role == null || role.Select == null || role.Select.Table == null)
            return (false, $"appsettings.jsonPermissionConfigurationNojustSure！");

        var tablerole = role.Select.Table.FirstOrDefault(it => it == "*" || it.Equals(table, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(tablerole))
            return (false, $"Table name {table} does not have permission to query!");

        var index = Array.IndexOf(role.Select.Table, tablerole);
        var selectrole = role.Select.Column[index];
        return (true, selectrole);
    }

    /// <summary>
    /// Whether the current column is in the role
    /// </summary>
    /// <param name="col"></param>
    /// <param name="selectrole"></param>
    /// <returns></returns>
    public bool ColIsRole(string col, string[] selectrole)
    {
        if (selectrole.Contains("*")) return true;

        if (col.Contains('(') && col.Contains(')'))
        {
            var reg = new Regex(@"\(([^)]*)\)");
            var match = reg.Match(col);
            return selectrole.Contains(match.Result("$1"), StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            return selectrole.Contains(col, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}