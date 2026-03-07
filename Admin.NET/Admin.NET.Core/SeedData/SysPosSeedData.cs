// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System job list seed data
/// </summary>
public class SysPosSeedData : ISqlSugarEntitySeedData<SysPos>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysPos> HasData()
    {
        return new[]
        {
            new SysPos{ Id=1300000000101, Name="Secretary of the Party Committee", Code="dwsj", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Secretary of the Party Committee", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000102, Name="Chairman", Code="dsz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Chairman", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000103, Name="Vice Chairman", Code="fdsz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Vice Chairman", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000104, Name="General manager", Code="zjl", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="General manager", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000105, Name="Deputy General Manager", Code="fzjl", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Deputy General Manager", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000106, Name="Department Manager", Code="bmjl", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Department Manager", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000107, Name="Deputy manager of department", Code="bmfjl", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Deputy manager of department", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000108, Name="Director", Code="zr", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Director", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000109, Name="Deputy Director", Code="fzr", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Deputy Director", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000110, Name="Director", Code="jz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Director", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000111, Name="Deputy Director", Code="fjz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Deputy Director", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000112, Name="Section Chief", Code="kz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Section Chief", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000113, Name="Deputy Section Chief", Code="fkz", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Deputy Section Chief", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000114, Name="Finance", Code="cw", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Finance", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000115, Name="Staff", Code="zy", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Staff", TenantId=SqlSugarConst.DefaultTenantId },
            new SysPos{ Id=1300000000116, Name="Other", Code="qt", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="Other", TenantId=SqlSugarConst.DefaultTenantId },
        };
    }
}