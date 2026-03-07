// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System role table seed data
/// </summary>
public class SysRoleSeedData : ISqlSugarEntitySeedData<SysRole>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysRole> HasData()
    {
        return new[]
        {
            new SysRole{ Id=1300000000101, Name="system administrator", DataScope=DataScopeEnum.All, Code="sys_admin", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="system administrator", TenantId=SqlSugarConst.DefaultTenantId },
            new SysRole{ Id=1300000000102, Name="This department and the following data", DataScope=DataScopeEnum.DeptChild, Code="sys_deptChild", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="This department and the following data", TenantId=SqlSugarConst.DefaultTenantId },
            new SysRole{ Id=1300000000103, Name="Department data", DataScope=DataScopeEnum.Dept, Code="sys_dept", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Department data", TenantId=SqlSugarConst.DefaultTenantId },
            new SysRole{ Id=1300000000104, Name="Only personal data", DataScope=DataScopeEnum.Self, Code="sys_self", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Only personal data", TenantId=SqlSugarConst.DefaultTenantId },
            new SysRole{ Id=1300000000105, Name="custom data", DataScope=DataScopeEnum.Define, Code="sys_define", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="custom data", TenantId=SqlSugarConst.DefaultTenantId },
        };
    }
}