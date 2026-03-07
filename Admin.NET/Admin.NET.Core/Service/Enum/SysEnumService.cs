// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System enumeration service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 275)]
public class SysEnumService : IDynamicApiController, ITransient
{
    private readonly EnumOptions _enumOptions;

    public SysEnumService(IOptions<EnumOptions> enumOptions)
    {
        _enumOptions = enumOptions.Value;
    }

    /// <summary>
    /// Get all enum types 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get all enum types")]
    public List<EnumTypeOutput> GetEnumTypeList()
    {
        var enumTypeList = App.EffectiveTypes.Where(t => t.IsEnum)
            .Where(t => _enumOptions.EntityAssemblyNames.Contains(t.Assembly.GetName().Name) || _enumOptions.EntityAssemblyNames.Any(name => t.Assembly.GetName().Name!.Contains(name)))
            .Where(t => t.GetCustomAttributes(typeof(IgnoreEnumToDictAttribute), false).Length == 0) // Exclude types that ignore the dictionary conversion attribute
            .Where(t => t.GetCustomAttributes(typeof(ErrorCodeTypeAttribute), false).Length == 0) // Exclude error code types
            .OrderBy(u => u.Name).ThenBy(u => u.FullName)
            .ToList();

        // If there is an enumeration class with the same name, add the "_No" suffix in sequence.
        var list = enumTypeList.Select(GetEnumDescription).ToList();
        foreach (var enumType in list.GroupBy(u => u.TypeName).Where(g => g.Count() > 1))
        {
            int i = 1;
            foreach (var item in list.Where(u => u.TypeName == enumType.Key).Skip(1)) item.TypeName = $"{item.TypeName}_{i++}";
        }
        return list;
    }

    /// <summary>
    /// Get dictionary description
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static EnumTypeOutput GetEnumDescription(Type type)
    {
        string description = type.Name;
        var attrs = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attrs.Length != 0)
        {
            var att = ((DescriptionAttribute[])attrs)[0];
            description = att.Description;
        }
        var enumType = App.EffectiveTypes.FirstOrDefault(t => t.IsEnum && t.Name == type.Name);
        return new EnumTypeOutput
        {
            TypeDescribe = description,
            TypeName = type.Name,
            TypeRemark = description,
            TypeFullName = type.FullName,
            EnumEntities = (enumType ?? type).EnumToList()
        };
    }

    /// <summary>
    /// Get the enumeration value collection through the enumeration type 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the set of enum values through the enum type")]
    public List<EnumEntity> GetEnumDataList([FromQuery] EnumInput input)
    {
        var enumType = App.EffectiveTypes.FirstOrDefault(u => u.IsEnum && u.Name == input.EnumName);
        if (enumType is not { IsEnum: true }) throw Oops.Oh(ErrorCodeEnum.D1503);

        return enumType.EnumToList();
    }

    /// <summary>
    /// Obtain the related enumeration value set through the field name of the entity (currently only supports enumeration types) 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the related enumeration value set through the field name of the entity")]
    public static List<EnumEntity> GetEnumDataListByField([FromQuery] QueryEnumDataInput input)
    {
        // Get entity type attributes
        Type entityType = App.EffectiveTypes.FirstOrDefault(u => u.Name == input.EntityName) ?? throw Oops.Oh(ErrorCodeEnum.D1504);

        // Get field type
        var fieldType = entityType.GetProperties().FirstOrDefault(u => u.Name == input.FieldName)?.PropertyType;
        if (fieldType is not { IsEnum: true }) throw Oops.Oh(ErrorCodeEnum.D1503);

        return fieldType.EnumToList();
    }
}