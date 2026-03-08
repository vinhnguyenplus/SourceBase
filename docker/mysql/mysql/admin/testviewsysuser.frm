TYPE=VIEW
query=select `u`.`Id` AS `Id`,`u`.`Account` AS `Account`,`u`.`RealName` AS `RealName`,`u`.`NickName` AS `NickName`,`a`.`Name` AS `OrgName`,`b`.`Name` AS `PosName` from ((`admin`.`sysuser` `u` left join `admin`.`sysorg` `a` on((`u`.`OrgId` = `a`.`Id`))) left join `admin`.`syspos` `b` on((`u`.`PosId` = `b`.`Id`)))
md5=355be990e96e9416f1b68a9971e2656c
updatable=0
algorithm=0
definer_user=root
definer_host=%
suid=2
with_check_option=0
timestamp=2026-03-08 12:34:41
create-version=1
source=SELECT  `u`.`Id` AS `Id` , `u`.`Account` AS `Account` , `u`.`RealName` AS `RealName` , `u`.`NickName` AS `NickName` , `a`.`Name` AS `OrgName` , `b`.`Name` AS `PosName`  FROM `SysUser` `u` Left JOIN `SysOrg` `a` ON ( `u`.`OrgId` = `a`.`Id` )  Left JOIN `SysPos` `b` ON ( `u`.`PosId` = `b`.`Id` )
client_cs_name=utf8mb4
connection_cl_name=utf8mb4_general_ci
view_body_utf8=select `u`.`Id` AS `Id`,`u`.`Account` AS `Account`,`u`.`RealName` AS `RealName`,`u`.`NickName` AS `NickName`,`a`.`Name` AS `OrgName`,`b`.`Name` AS `PosName` from ((`admin`.`sysuser` `u` left join `admin`.`sysorg` `a` on((`u`.`OrgId` = `a`.`Id`))) left join `admin`.`syspos` `b` on((`u`.`PosId` = `b`.`Id`)))
