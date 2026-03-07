// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user extended organization table seed data
/// </summary>
public class SysUserExtOrgSeedData : ISqlSugarEntitySeedData<SysUserExtOrg>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUserExtOrg> HasData()
    {
        var userList = new SysUserSeedData().HasData().ToList();
        var orgList = new SysOrgSeedData().HasData().ToList();
        var posList = new SysPosSeedData().HasData().ToList();
        var admin = userList.First(u => u.Account == "Admin.NET");
        var user3 = userList.First(u => u.Account == "TestUser3");
        var org1 = orgList.First(u => u.Name == "System Default");
        var org2 = orgList.First(u => u.Name == "Development Department");
        var pos1 = posList.First(u => u.Name == "Department Manager");
        var pos2 = posList.First(u => u.Name == "Director");
        return new[]
        {
            new SysUserExtOrg{ Id=1300000000101, UserId=admin.Id, OrgId=org1.Id, PosId=pos1.Id },
            new SysUserExtOrg{ Id=1300000000102, UserId=user3.Id, OrgId=org2.Id, PosId=pos2.Id  }
        };
    }
}