// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System organization table seed data
/// </summary>
[IgnoreUpdateSeed]
public class SysOrgSeedData : ISqlSugarEntitySeedData<SysOrg>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysOrg> HasData()
    {
        var admin = new SysUserSeedData().HasData().First(u => u.Account == "Admin.NET");
        return new[]
        {
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId, Pid=0, Name="System Default", Code="1001", Type="101", Level=1, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="System Default", TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 1, Pid=SqlSugarConst.DefaultTenantId, Name="Marketing Department", Code="100101", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Marketing Department", CreateUserId=admin.Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 2, Pid=SqlSugarConst.DefaultTenantId, Name="Development Department", Code="100102", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Development Department", CreateUserId=admin.Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 3, Pid=SqlSugarConst.DefaultTenantId, Name="After-sales Department", Code="100103", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="After-sales Department", CreateUserId=admin.Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 4, Pid=SqlSugarConst.DefaultTenantId, Name="Other", Code="10010301", Level=3, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Other", CreateUserId=admin.Id, TenantId=SqlSugarConst.DefaultTenantId },
        };
    }
}