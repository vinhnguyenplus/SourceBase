// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System dictionary value table seed data
/// </summary>
public class SysDictDataSeedData : ISqlSugarEntitySeedData<SysDictData>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysDictData> HasData()
    {
        var typeList = new SysDictTypeSeedData().HasData().ToList();
        return new[]
        {
            new SysDictData{ Id=1300000000101, DictTypeId=typeList[0].Id, Label="Input box", Value="Input", OrderNo=100, Remark="Input box", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000102, DictTypeId=typeList[0].Id, Label="dictionary selector", Value="DictSelector", OrderNo=100, Remark="dictionary selector", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000103, DictTypeId=typeList[0].Id, Label="constant selector", Value="ConstSelector", OrderNo=100, Remark="constant selector", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000104, DictTypeId=typeList[0].Id, Label="enum selector", Value="EnumSelector", OrderNo=100, Remark="enum selector", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000105, DictTypeId=typeList[0].Id, Label="tree selector", Value="ApiTreeSelector", OrderNo=100, Remark="tree selector", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000106, DictTypeId=typeList[0].Id, Label="foreign key", Value="ForeignKey", OrderNo=100, Remark="foreign key", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000107, DictTypeId=typeList[0].Id, Label="Numeric input box", Value="InputNumber", OrderNo=100, Remark="Numeric input box", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000108, DictTypeId=typeList[0].Id, Label="Time Selection", Value="DatePicker", OrderNo=100, Remark="Time Selection", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000109, DictTypeId=typeList[0].Id, Label="Text area", Value="InputTextArea", OrderNo=100, Remark="Text area", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000110, DictTypeId=typeList[0].Id, Label="upload", Value="Upload", OrderNo=100, Remark="upload", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000111, DictTypeId=typeList[0].Id, Label="switch", Value="Switch", OrderNo=100, Remark="switch", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000112, DictTypeId=typeList[0].Id, Label="Upload leaflet file", Value="Upload_SingleFile", OrderNo=120, Remark="Upload leaflet file", Status=StatusEnum.Enable, CreateTime= DateTime.Now },
            new SysDictData{ Id=1300000000113, DictTypeId=typeList[0].Id, Label="Rich text editor", Value="Editor", OrderNo=130, Remark="Rich text editor", Status=StatusEnum.Enable,   CreateTime=DateTime.Parse("2025-12-25 00:00:00") },

            new SysDictData{ Id=1300000000201, DictTypeId=typeList[1].Id, Label="equals", Value="==", OrderNo=1, Remark="equals", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000202, DictTypeId=typeList[1].Id, Label="Blurry", Value="like", OrderNo=1, Remark="Blurry", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000203, DictTypeId=typeList[1].Id, Label="Greater than", Value=">", OrderNo=1, Remark="Greater than", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000204, DictTypeId=typeList[1].Id, Label="less than", Value="<", OrderNo=1, Remark="less than", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000205, DictTypeId=typeList[1].Id, Label="not equal to", Value="!=", OrderNo=1, Remark="not equal to", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000206, DictTypeId=typeList[1].Id, Label="greater than or equal to", Value=">=", OrderNo=1, Remark="greater than or equal to", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000207, DictTypeId=typeList[1].Id, Label="less than or equal to", Value="<=", OrderNo=1, Remark="less than or equal to", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000208, DictTypeId=typeList[1].Id, Label="Not empty", Value="isNotNull", OrderNo=1, Remark="Not empty", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000209, DictTypeId=typeList[1].Id, Label="time range", Value="~", OrderNo=1, Remark="time range", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=1300000000301, DictTypeId=typeList[2].Id, Label="long", Value="long", OrderNo=1, Remark="long", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000302, DictTypeId=typeList[2].Id, Label="string", Value="string", OrderNo=1, Remark="string", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000303, DictTypeId=typeList[2].Id, Label="DateTime", Value="DateTime", OrderNo=1, Remark="DateTime", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000304, DictTypeId=typeList[2].Id, Label="bool", Value="bool", OrderNo=1, Remark="bool", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000305, DictTypeId=typeList[2].Id, Label="int", Value="int", OrderNo=1, Remark="int", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000306, DictTypeId=typeList[2].Id, Label="double", Value="double", OrderNo=1, Remark="double", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000307, DictTypeId=typeList[2].Id, Label="float", Value="float", OrderNo=1, Remark="float", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000308, DictTypeId=typeList[2].Id, Label="decimal", Value="decimal", OrderNo=1, Remark="decimal", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000309, DictTypeId=typeList[2].Id, Label="Guid", Value="Guid", OrderNo=1, Remark="Guid", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000310, DictTypeId=typeList[2].Id, Label="DateTimeOffset", Value="DateTimeOffset", OrderNo=1, Remark="DateTimeOffset", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=1300000000401, DictTypeId=typeList[3].Id, Label="Download the compressed package", Value="100", OrderNo=1, Remark="Download the compressed package", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000402, DictTypeId=typeList[3].Id, Label="Download the compressed package (front-end)", Value="111", OrderNo=2, Remark="Download the compressed package (front-end)", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000403, DictTypeId=typeList[3].Id, Label="Download the compressed package(Backend)", Value="121", OrderNo=3, Remark="Download the compressed package(Backend)", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000404, DictTypeId=typeList[3].Id, Label="Generate to this project", Value="200", OrderNo=4, Remark="Generate to this project", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000405, DictTypeId=typeList[3].Id, Label="Generate to this project (frontend)", Value="211", OrderNo=5, Remark="Generate to this project (frontend)", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000406, DictTypeId=typeList[3].Id, Label="Generate to this project (backend)", Value="221", OrderNo=6, Remark="Generate to this project (backend)", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=1300000000501, DictTypeId=typeList[4].Id, Label="EntityBaseId [Basic Entity Id]", Value="EntityBaseId", OrderNo=1, Remark="[Basic entity ID]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000502, DictTypeId=typeList[4].Id, Label="EntityBase[Basic Entity]", Value="EntityBase", OrderNo=1, Remark="[Basic Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000503, DictTypeId=typeList[4].Id, Label="EntityBaseDel [Basic Soft Delete Entity]", Value="EntityBaseDel", OrderNo=1, Remark="[Basic Soft Delete Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000504, DictTypeId=typeList[4].Id, Label="EntityBaseOrg【Institutional Entity】", Value="EntityBaseOrg", OrderNo=1, Remark="[Organizational Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000505, DictTypeId=typeList[4].Id, Label="EntityBaseOrgDel [Organization Soft Delete Entity]", Value="EntityBaseOrgDel", OrderNo=1, Remark="[Organization soft delete entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000506, DictTypeId=typeList[4].Id, Label="EntityBaseTenantId [Tenant Entity Id]", Value="EntityBaseTenantId", OrderNo=1, Remark="[Tenant Entity Id]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000507, DictTypeId=typeList[4].Id, Label="EntityBaseTenant [Tenant Entity]", Value="EntityBaseTenant", OrderNo=1, Remark="[Tenant entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000508, DictTypeId=typeList[4].Id, Label="EntityBaseTenantDel [Tenant Soft Delete Entity]", Value="EntityBaseTenantDel", OrderNo=1, Remark="[Tenant Soft Delete Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000509, DictTypeId=typeList[4].Id, Label="EntityBaseTenantOrg [Tenant Organization Entity]", Value="EntityBaseTenantOrg", OrderNo=1, Remark="[Tenant Organization Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=1300000000510, DictTypeId=typeList[4].Id, Label="EntityBaseTenantOrgDel[Tenant Organization Soft Delete Entity]", Value="EntityBaseTenantOrgDel", OrderNo=1, Remark="[Tenant Organization Soft Delete Entity]", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=1300000000601, DictTypeId=typeList[5].Id, Label="unnecessary", Value="off", OrderNo=100, Remark="No print support required", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-12-04 00:00:00") },
            new SysDictData{ Id=1300000000602, DictTypeId=typeList[5].Id, Label="Bind print template", Value="custom", OrderNo=101, Remark="Bind print template", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-12-04 00:00:00") },

            new SysDictData{ Id=1300000000701, DictTypeId=typeList[6].Id, Label="Group", Value="101", OrderNo=100, Remark="Group", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
            new SysDictData{ Id=1300000000702, DictTypeId=typeList[6].Id, Label="Company", Value="201", OrderNo=101, Remark="Company", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
            new SysDictData{ Id=1300000000703, DictTypeId=typeList[6].Id, Label="Department", Value="301", OrderNo=102, Remark="Department", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
            new SysDictData{ Id=1300000000704, DictTypeId=typeList[6].Id, Label="area", Value="401", OrderNo=103, Remark="area", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
            new SysDictData{ Id=1300000000705, DictTypeId=typeList[6].Id, Label="group", Value="501", OrderNo=104, Remark="group", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
        };
    }
}