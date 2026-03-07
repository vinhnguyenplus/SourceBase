// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user role table seed data
/// </summary>
public class SysUserRoleSeedData : ISqlSugarEntitySeedData<SysUserRole>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUserRole> HasData()
    {
        var userList = new SysUserSeedData().HasData().ToList();
        var roleList = new SysRoleSeedData().HasData().ToList();
        return new[]
        {
            new SysUserRole{ Id=1300000000101, UserId=userList.First(u => u.Account == "TestUser1").Id, RoleId=roleList.First(u => u.Code == "sys_deptChild").Id },
            new SysUserRole{ Id=1300000000102, UserId=userList.First(u => u.Account == "TestUser2").Id, RoleId=roleList.First(u => u.Code == "sys_dept").Id },
            new SysUserRole{ Id=1300000000103, UserId=userList.First(u => u.Account == "TestUser3").Id, RoleId=roleList.First(u => u.Code == "sys_self").Id },
            new SysUserRole{ Id=1300000000104, UserId=userList.First(u => u.Account == "TestUser4").Id, RoleId=roleList.First(u => u.Code == "sys_define").Id },
        };
    }
}