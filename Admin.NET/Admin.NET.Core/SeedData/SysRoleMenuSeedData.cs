// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System role menu table seed data
/// </summary>
public class SysRoleMenuSeedData : ISqlSugarEntitySeedData<SysRoleMenu>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysRoleMenu> HasData()
    {
        var roleMenuList = new List<SysRoleMenu>();

        var roleList = new SysRoleSeedData().HasData().ToList();
        var menuList = new SysMenuSeedData().HasData().ToList();
        var defaultMenuList = new SysTenantMenuSeedData().HasData().ToList();

        // The first character has all the default tenant menus
        roleMenuList.AddRange(defaultMenuList.Select(u => new SysRoleMenu { Id = u.MenuId + (roleList[0].Id % 1300000000000), RoleId = roleList[0].Id, MenuId = u.MenuId }));

        // Other role permissions: workbench, system management, personal center, help documents, about project
        var otherRoleMenuList = menuList.ToChildList(u => u.Id, u => u.Pid, u => new[] { "Workbench", "Help documentation", "About the project", "Personal Center" }.Contains(u.Title)).ToList();
        otherRoleMenuList.Add(menuList.First(u => u.Type == MenuTypeEnum.Dir && u.Title == "System Management"));
        foreach (var role in roleList.Skip(1)) roleMenuList.AddRange(otherRoleMenuList.Select(u => new SysRoleMenu { Id = u.Id + (role.Id % 1300000000000), RoleId = role.Id, MenuId = u.Id }));

        return roleMenuList;
    }
}