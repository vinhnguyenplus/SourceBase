// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;

namespace Admin.NET.Plugin.ReZero;

/// <summary>
/// Super API menu table seed data
/// </summary>
public class SysMenuSeedData : ISqlSugarEntitySeedData<SysMenu>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysMenu> HasData()
    {
        return new[]
        {
            new SysMenu{ Id=1310500010101, Pid=1300500000101, Title="Super API", Path="/develop/reZero", Name="sysReZero", Component="Layout", Icon="ele-MagicStick", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1310500020101, Pid=1310500010101, Title="dynamic interface", Path="/develop/reZero/dynamicApi", Name="sysReZeroDynamicApi", Component="layout/routerView/iframe", Icon="ele-Menu", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/rezero/dynamic_interface.html?model=small", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1310500030101, Pid=1310500010101, Title="Database Management", Path="/develop/reZero/database", Name="sysReZeroDatabase", Component="layout/routerView/iframe", Icon="ele-Menu", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/rezero/database_manager.html?model=small", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1310500040101, Pid=1310500010101, Title="Entity Table Management", Path="/develop/reZero/entity", Name="sysReZeroEntity", Component="layout/routerView/iframe", Icon="ele-Menu", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/rezero/entity_manager.html?model=small", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1310500050101, Pid=1310500010101, Title="Interface classification", Path="/develop/reZero/apiCategory", Name="sysReZeroApiCategory", Component="layout/routerView/iframe", Icon="ele-Menu", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/rezero/interface_categroy.html?model=small", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1310500060101, Pid=1310500010101, Title="Interface Definition", Path="/develop/reZero/apiDefine", Name="sysReZeroApiDefine", Component="layout/routerView/iframe", Icon="ele-Menu", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/rezero/interface_manager.html?model=small", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
        };
    }
}