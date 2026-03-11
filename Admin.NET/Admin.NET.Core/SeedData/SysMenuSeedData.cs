// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System menu table seed data
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
            new SysMenu{ Id=1300100000101, Pid=0, Title="Workbench", Path="/dashboard", Name="dashboard", Component="Layout", Icon="ele-HomeFilled", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=0 },
            new SysMenu{ Id=1300100010101, Pid=1300100000101, Title="Workbench", Path="/dashboard/home", Name="home", Component="/home/index", IsAffix=true, Icon="ele-HomeFilled", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300100010201, Pid=1300100000101, Title="Inbox message", Path="/dashboard/notice", Name="notice", Component="/home/notice/index", Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=101 },

            // It is recommended to place specific business application menus between the ID ranges here.

            #region System Management

            new SysMenu{ Id=1300200000101, Pid=0, Title="System Management", Path="/system", Name="system", Component="Layout", Icon="ele-Setting", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=10000 },

            // Account management
            new SysMenu{ Id=1300200010101, Pid=1300200000101, Title="Account Management", Path="/system/user", Name="sysUser", Component="/system/user/index", Icon="ele-User", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010201, Pid=1300200010101, Title="Query", Permission="sysUser:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010301, Pid=1300200010101, Title="Edit", Permission="sysUser:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010401, Pid=1300200010101, Title="increase", Permission="sysUser:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010501, Pid=1300200010101, Title="Delete", Permission="sysUser:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010601, Pid=1300200010101, Title="Details", Permission="sysUser:detail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010701, Pid=1300200010101, Title="Authorization role", Permission="sysUser:grantRole", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010801, Pid=1300200010101, Title="reset password", Permission="sysUser:resetPwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200010901, Pid=1300200010101, Title="Set status", Permission="sysUser:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200011001, Pid=1300200010101, Title="Forced offline", Permission="sysOnlineUser:forceOffline", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200011101, Pid=1300200010101, Title="Unlock", Permission="sysUser:unlockLogin", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // role management
            new SysMenu{ Id=1300200020101, Pid=1300200000101, Title="role management", Path="/system/role", Name="sysRole", Component="/system/role/index", Icon="ele-Help", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1300200020201, Pid=1300200020101, Title="Query", Permission="sysRole:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020301, Pid=1300200020101, Title="Edit", Permission="sysRole:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020401, Pid=1300200020101, Title="increase", Permission="sysRole:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020501, Pid=1300200020101, Title="Delete", Permission="sysRole:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020601, Pid=1300200020101, Title="Authorization Menu", Permission="sysRole:grantMenu", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020701, Pid=1300200020101, Title="Authorized Data", Permission="sysRole:grantDataScope", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200020801, Pid=1300200020101, Title="Set status", Permission="sysRole:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Organizational Management
            new SysMenu{ Id=1300200030101, Pid=1300200000101, Title="Organizational Management", Path="/system/org", Name="sysOrg", Component="/system/org/index", Icon="ele-OfficeBuilding", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1300200030201, Pid=1300200030101, Title="Query", Permission="sysOrg:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200030301, Pid=1300200030101, Title="Edit", Permission="sysOrg:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200030401, Pid=1300200030101, Title="increase", Permission="sysOrg:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200030501, Pid=1300200030101, Title="Delete", Permission="sysOrg:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Position management
            new SysMenu{ Id=1300200040101, Pid=1300200000101, Title="Position Management", Path="/system/pos", Name="sysPos", Component="/system/pos/index",Icon="ele-Mug", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1300200040201, Pid=1300200040101, Title="Query", Permission="sysPos:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200040301, Pid=1300200040101, Title="Edit", Permission="sysPos:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200040401, Pid=1300200040101, Title="increase", Permission="sysPos:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200040501, Pid=1300200040101, Title="Delete", Permission="sysPos:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Personal center
            new SysMenu{ Id=1300200050101, Pid=1300200000101, Title="Personal Center", Path="/system/userCenter", Name="sysUserCenter", Component="/system/user/component/userCenter",Icon="ele-Medal", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1300200050201, Pid=1300200050101, Title="Change Password", Permission="sysUser:changePwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200050301, Pid=1300200050101, Title="Basic Information", Permission="sysUser:baseInfo", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200050401, Pid=1300200050101, Title="Electronic signature", Permission="sysFile:uploadSignature", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200050501, Pid=1300200050101, Title="Upload Avatar", Permission="sysFile:uploadAvatar", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Notices and Announcements
            new SysMenu{ Id=1300200060101, Pid=1300200000101, Title="Notices and Announcements", Path="/system/notice", Name="sysNotice", Component="/system/notice/index",Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=150 },
            new SysMenu{ Id=1300200060201, Pid=1300200060101, Title="Query", Permission="sysNotice:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200060301, Pid=1300200060101, Title="Edit", Permission="sysNotice:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200060401, Pid=1300200060101, Title="increase", Permission="sysNotice:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200060501, Pid=1300200060101, Title="Delete", Permission="sysNotice:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200060601, Pid=1300200060101, Title="release", Permission="sysNotice:public", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200060701, Pid=1300200060101, Title="Withdraw", Permission="sysNotice:cancel", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Third party account
            new SysMenu{ Id=1300200070101, Pid=1300200000101, Title="Third-party account", Path="/system/weChatUser", Name="sysWechatUser", Component="/system/weChatUser/index",Icon="ele-ChatDotRound", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=160 },
            new SysMenu{ Id=1300200070201, Pid=1300200070101, Title="Query", Permission="sysWechatUser:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200070301, Pid=1300200070101, Title="Edit", Permission="sysWechatUser:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200070401, Pid=1300200070101, Title="increase", Permission="sysWechatUser:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200070501, Pid=1300200070101, Title="Delete", Permission="sysWechatUser:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // AD domain configuration
            new SysMenu{ Id=1300200080101, Pid=1300200000101, Title="AD domain configuration", Path="/system/ldap", Name="sysLdap", Component="/system/ldap/index",Icon="ele-Place", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=170 },
            new SysMenu{ Id=1300200080201, Pid=1300200080101, Title="Query", Permission="sysLdap:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200080301, Pid=1300200080101, Title="Details", Permission="sysLdap:detail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1300200080401, Pid=1300200080101, Title="Edit", Permission="sysLdap:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1300200080501, Pid=1300200080101, Title="increase", Permission="sysLdap:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1300200080601, Pid=1300200080101, Title="Delete", Permission="sysLdap:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1300200080701, Pid=1300200080101, Title="Sync domain accounts", Permission="sysLdap:syncUser", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=150 },
            new SysMenu{ Id=1300200080801, Pid=1300200080101, Title="Synchronize domain organization", Permission="sysLdap:syncOrg", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=160 },

            #endregion System Management

            #region Platform Management

            new SysMenu{ Id=1300300000101, Pid=0, Title="Platform Management", Path="/platform", Name="platform", Component="Layout", Icon="ele-Operation", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=11000 },

            // Tenant management
            new SysMenu{ Id=1300300010101, Pid=1300300000101, Title="Tenant management", Path="/platform/tenant", Name="sysTenant", Component="/system/tenant/index", Icon="ele-School", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010201, Pid=1300300010101, Title="Query", Permission="sysTenant:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010301, Pid=1300300010101, Title="Edit", Permission="sysTenant:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010401, Pid=1300300010101, Title="increase", Permission="sysTenant:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010501, Pid=1300300010101, Title="Delete", Permission="sysTenant:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010601, Pid=1300300010101, Title="Authorization Menu", Permission="sysTenant:grantMenu", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010701, Pid=1300300010101, Title="reset password", Permission="sysTenant:resetPwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010801, Pid=1300300010101, Title="Generation Library", Permission="sysTenant:createDb", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300010901, Pid=1300300010101, Title="Set status", Permission="sysTenant:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300011001, Pid=1300300010101, Title="Synchronous authorization", Permission="sysTenant:syncGrantMenu", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300011101, Pid=1300300010101, Title="Switch Tenant", Permission="sysTenant:changeTenant", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300011201, Pid=1300300010101, Title="Enter the rental management terminal", Permission="sysTenant:goTenant", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Registration plan
            new SysMenu{ Id=1300300020101, Pid=1300300000101, Title="Registration plan", Path="/platform/regWay", Name="sysUserRegWay", Component="/system/userRegWay/index", Icon="ele-Pointer", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=105 },
            new SysMenu{ Id=1300300020201, Pid=1300300020101, Title="Query", Permission="sysUserRegWay:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300020301, Pid=1300300020101, Title="Edit", Permission="sysUserRegWay:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300020401, Pid=1300300020101, Title="increase", Permission="sysUserRegWay:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300020501, Pid=1300300020101, Title="Delete", Permission="sysUserRegWay:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Menu management
            new SysMenu{ Id=1300300030101, Pid=1300300000101, Title="Menu management", Path="/platform/menu", Name="sysMenu", Component="/system/menu/index", Icon="ele-Menu", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1300300030201, Pid=1300300030101, Title="Query", Permission="sysMenu:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300030301, Pid=1300300030101, Title="Edit", Permission="sysMenu:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300030401, Pid=1300300030101, Title="increase", Permission="sysMenu:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300030501, Pid=1300300030101, Title="Delete", Permission="sysMenu:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Platform parameters
            new SysMenu{ Id=1300300040101, Pid=1300300000101, Title="Platform parameters", Path="/platform/config", Name="sysConfig", Component="/system/config/index", Icon="ele-DocumentCopy", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1300300040201, Pid=1300300040101, Title="Query", Permission="sysConfig:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300040301, Pid=1300300040101, Title="Edit", Permission="sysConfig:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300040401, Pid=1300300040101, Title="increase", Permission="sysConfig:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300040501, Pid=1300300040101, Title="Delete", Permission="sysConfig:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Dictionary management
            new SysMenu{ Id=1300300050101, Pid=1300300000101, Title="Dictionary Management", Path="/platform/dict", Name="sysDict", Component="/system/dict/index", Icon="ele-Collection", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1300300050201, Pid=1300300050101, Title="Query", Permission="sysDictType:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050301, Pid=1300300050101, Title="Edit", Permission="sysDictType:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050401, Pid=1300300050101, Title="increase", Permission="sysDictType:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050501, Pid=1300300050101, Title="Delete", Permission="sysDictType:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050601, Pid=1300300050101, Title="Add dictionary value", Permission="sysDictData:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050701, Pid=1300300050101, Title="Delete dictionary value", Permission="sysDictData:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050801, Pid=1300300050101, Title="Edit dictionary value", Permission="sysDictData:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300050901, Pid=1300300050101, Title="dictionary migration", Permission="sysDictType:move", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Template management
            new SysMenu{ Id=1300300051101, Pid=1300300000101, Title="Template management", Path="/platform/template", Name="sysTemplate", Component="/system/template/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=135 },
            new SysMenu{ Id=1300300051201, Pid=1300300051101, Title="Query", Permission="sysTemplate:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300051301, Pid=1300300051101, Title="Edit", Permission="sysTemplate:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300051401, Pid=1300300051101, Title="increase", Permission="sysTemplate:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300051501, Pid=1300300051101, Title="Delete", Permission="sysTemplate:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300051601, Pid=1300300051101, Title="Preview", Permission="sysTemplate:preview", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Task scheduling
            new SysMenu{ Id=1300300060101, Pid=1300300000101, Title="Task Scheduling", Path="/platform/job", Name="sysJob", Component="/system/job/index", Icon="ele-AlarmClock", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1300300060201, Pid=1300300060101, Title="Query", Permission="sysJob:pageJobDetail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300060301, Pid=1300300060101, Title="Edit", Permission="sysJob:updateJobDetail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300060401, Pid=1300300060101, Title="increase", Permission="sysJob:addJobDetail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300060501, Pid=1300300060101, Title="Delete", Permission="sysJob:deleteJobDetail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // System parameters
            new SysMenu{ Id=1300200090101, Pid=1300200000101, Title="System Parameters", Path="/system/tenantConfig", Name="sysTenantConfig", Component="/system/tenantConfig/index", Icon="ele-DocumentCopy", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=180 },
            new SysMenu{ Id=1300200090201, Pid=1300200090101, Title="Query", Permission="sysTenantConfig:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200090301, Pid=1300200090101, Title="Edit", Permission="sysTenantConfig:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200090401, Pid=1300200090101, Title="increase", Permission="sysTenantConfig:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200090501, Pid=1300200090101, Title="Delete", Permission="sysTenantConfig:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Language management
            new SysMenu{ Id=1300200100101, Pid=1300200000101, Title="Language management", Path="/system/lang", Name="sysLang", Component="/system/lang/index", Icon="iconfont icon-diqiu", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2025-06-28 00:00:00"), OrderNo=190 },
            new SysMenu{ Id=1300200100201, Pid=1300200100101, Title="Query", Permission="sysLang:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200100301, Pid=1300200100101, Title="Edit", Permission="sysLang:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200100401, Pid=1300200100101, Title="increase", Permission="sysLang:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200100501, Pid=1300200100101, Title="Delete", Permission="sysLang:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Translation management
            new SysMenu{ Id=1300200200101, Pid=1300200000101, Title="Translation management", Path="/system/langText", Name="sysLangText", Component="/system/langText/index", Icon="iconfont icon-zhongyingwen", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2025-06-28 00:00:00"), OrderNo=200 },
            new SysMenu{ Id=1300200200201, Pid=1300200200101, Title="Query", Permission="sysLangText:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200200301, Pid=1300200200101, Title="Edit", Permission="sysLangText:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200200401, Pid=1300200200101, Title="increase", Permission="sysLangText:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300200200501, Pid=1300200200101, Title="Delete", Permission="sysLangText:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // System monitoring
            new SysMenu{ Id=1300300070101, Pid=1300300000101, Title="System Monitoring", Path="/platform/server", Name="sysServer", Component="/system/server/index", Icon="ele-Monitor", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=150 },

            // Cache management
            new SysMenu{ Id=1300300080101, Pid=1300300000101, Title="Cache Management", Path="/platform/cache", Name="sysCache", Component="/system/cache/index", Icon="ele-Loading", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=160 },
            new SysMenu{ Id=1300300080201, Pid=1300300080101, Title="Query", Permission="sysCache:keyList", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300080301, Pid=1300300080101, Title="Delete", Permission="sysCache:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300080401, Pid=1300300080101, Title="Clear", Permission="sysCache:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Administrative region
            new SysMenu{ Id=1300300090101, Pid=1300300000101, Title="Administrative region", Path="/platform/region", Name="sysRegion", Component="/system/region/index", Icon="ele-LocationInformation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=170 },
            new SysMenu{ Id=1300300090201, Pid=1300300090101, Title="Query", Permission="sysRegion:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300090301, Pid=1300300090101, Title="Edit", Permission="sysRegion:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300090401, Pid=1300300090101, Title="increase", Permission="sysRegion:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300090501, Pid=1300300090101, Title="Delete", Permission="sysRegion:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300090601, Pid=1300300090101, Title="sync", Permission="sysRegion:sync", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // File management
            new SysMenu{ Id=1300300100101, Pid=1300300000101, Title="File Management", Path="/platform/file", Name="sysFile", Component="/system/file/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=180 },
            new SysMenu{ Id=1300300100201, Pid=1300300100101, Title="Query", Permission="sysFile:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300100301, Pid=1300300100101, Title="upload", Permission="sysFile:uploadFile", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300100401, Pid=1300300100101, Title="Download", Permission="sysFile:downloadFile", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300100501, Pid=1300300100101, Title="Delete", Permission="sysFile:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300100601, Pid=1300300100101, Title="Edit", Permission="sysFile:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2023-10-27 00:00:00"), OrderNo=100 },

            // Print template
            new SysMenu{ Id=1300300110101, Pid=1300300000101, Title="Print template", Path="/platform/print", Name="sysPrint", Component="/system/print/index", Icon="ele-Printer", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=190 },
            new SysMenu{ Id=1300300110201, Pid=1300300110101, Title="Query", Permission="sysPrint:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300110301, Pid=1300300110101, Title="Edit", Permission="sysPrint:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300110401, Pid=1300300110101, Title="increase", Permission="sysPrint:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300110501, Pid=1300300110101, Title="Delete", Permission="sysPrint:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // dynamic plug-in
            new SysMenu{ Id=1300300120101, Pid=1300300000101, Title="dynamic plug-in", Path="/platform/plugin", Name="sysPlugin", Component="/system/plugin/index", Icon="ele-Connection", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=200 },
            new SysMenu{ Id=1300300120201, Pid=1300300120101, Title="Query", Permission="sysPlugin:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300120301, Pid=1300300120101, Title="Edit", Permission="sysPlugin:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300120401, Pid=1300300120101, Title="increase", Permission="sysPlugin:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300120501, Pid=1300300120101, Title="Delete", Permission="sysPlugin:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // open interface
            new SysMenu{ Id=1300300130101, Pid=1300300000101, Title="Open interface", Path="/platform/openAccess", Name="sysOpenAccess", Component="/system/openAccess/index", Icon="ele-Link", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=210 },
            new SysMenu{ Id=1300300130201, Pid=1300300130101, Title="Query", Permission="sysOpenAccess:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300130301, Pid=1300300130101, Title="Edit", Permission="sysOpenAccess:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300130401, Pid=1300300130101, Title="increase", Permission="sysOpenAccess:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300130501, Pid=1300300130101, Title="Delete", Permission="sysOpenAccess:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // System configuration
            new SysMenu{ Id=1300300140101, Pid=1300300000101, Title="System Configuration", Path="/platform/infoSetting", Name="sysInfoSetting", Component="/system/infoSetting/index", Icon="ele-Setting", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=220 },
            // System update
            new SysMenu{ Id=1300300160101, Pid=1300300000101, Title="System update", Path="/platform/update", Name="sysUpdate", Component="/system/update/index", Icon="ele-Upload", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=240 },
            new SysMenu{ Id=1300300160201, Pid=1300300160101, Title="Update", Permission="sysUpdate:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300160301, Pid=1300300160101, Title="restore", Permission="sysUpdate:restore", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300160401, Pid=1300300160101, Title="Backup list", Permission="sysUpdate:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300160501, Pid=1300300160101, Title="Log List", Permission="sysUpdate:logs", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300160601, Pid=1300300160101, Title="clear log", Permission="sysUpdate:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300300160701, Pid=1300300160101, Title="Get key", Permission="sysUpdate:webHookKey", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

	        #endregion Platform Management

            #region Log management

            new SysMenu{ Id=1300400000101, Pid=0, Title="Log management", Path="/log", Name="log", Component="Layout", Icon="ele-DocumentCopy", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=12000 },

            // access log
            new SysMenu{ Id=1300400010101, Pid=1300400000101, Title="Access Log", Path="/log/vislog", Name="sysVisLog", Component="/system/log/vislog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400010201, Pid=1300400010101, Title="Query", Permission="sysVislog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400010301, Pid=1300400010101, Title="Clear", Permission="sysVislog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Operation log
            new SysMenu{ Id=1300400020101, Pid=1300400000101, Title="Operation Log", Path="/log/oplog", Name="sysOpLog", Component="/system/log/oplog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1300400020201, Pid=1300400020101, Title="Query", Permission="sysOplog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400020301, Pid=1300400020101, Title="Clear", Permission="sysOplog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400020401, Pid=1300400020101, Title="Export", Permission="sysOplog:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Exception log
            new SysMenu{ Id=1300400030101, Pid=1300400000101, Title="Exception Log", Path="/log/exlog", Name="sysExLog", Component="/system/log/exlog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1300400030201, Pid=1300400030101, Title="Query", Permission="sysExlog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400030301, Pid=1300400030101, Title="Clear", Permission="sysExlog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400030401, Pid=1300400030101, Title="Export", Permission="sysExlog:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // Difference log
            new SysMenu{ Id=1300400040101, Pid=1300400000101, Title="Difference log", Path="/log/difflog", Name="sysDiffLog", Component="/system/log/difflog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1300400040201, Pid=1300400040101, Title="Query", Permission="sysDifflog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300400040301, Pid=1300400040101, Title="Clear", Permission="sysDifflog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            #endregion Log management

            #region development tools

            // development tools
            new SysMenu{ Id=1300500000101, Pid=0, Title="development tools", Path="/develop", Name="develop", Component="Layout", Icon="ele-Cpu", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=13000 },
            new SysMenu{ Id=1300500010101, Pid=1300500000101, Title="Library table management", Path="/develop/database", Name="sysDatabase", Component="/system/database/index",Icon="ele-Coin", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300500020101, Pid=1300500000101, Title="code generation", Path="/develop/codeGen", Name="sysCodeGen", Component="/system/codeGen/index", Icon="ele-Crop", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=1300500030101, Pid=1300500000101, Title="Form Design", Path="/develop/formDes", Name="sysFormDes", Component="/system/formDes/index", Icon="ele-Edit", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=1300500040101, Pid=1300500000101, Title="Interface pressure test", Path="/develop/stressTest", Name="SysStressTest", Component="/system/stressTest/index", Icon="ele-DataLine", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=1300500050101, Pid=1300500000101, Title="System interface", Path="/develop/api", Name="sysApi", Component="layout/routerView/iframe", IsIframe=true, OutLink="http://localhost:5005", Icon="ele-Help", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },

            #endregion development tools

            // About the project
            new SysMenu{ Id=1300700000101, Pid=0, Title="About the project", Path="/about", Name="about", Component="/about/index", Icon="ele-InfoFilled", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2023-03-12 00:00:00"), OrderNo=15000 },
        };
    }
}