// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System constant service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 280)]
public class SysConstService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;

    public SysConstService(SysCacheService sysCacheService)
    {
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// Get a list of all constants 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get all constants list")]
    public async Task<List<ConstOutput>> GetList()
    {
        var key = $"{CacheConst.KeyConst}list";
        var constList = _sysCacheService.Get<List<ConstOutput>>(key);
        if (constList == null)
        {
            var typeList = GetConstAttributeList();
            constList = typeList.Select(u => new ConstOutput
            {
                Name = u.CustomAttributes.ToList().FirstOrDefault()?.ConstructorArguments.ToList().FirstOrDefault().Value?.ToString() ?? u.Name,
                Code = u.Name,
                Data = GetData(Convert.ToString(u.Name))
            }).ToList();
            _sysCacheService.Set(key, constList);
        }
        return await Task.FromResult(constList);
    }

    /// <summary>
    /// Get constant data based on class name 🔖
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    [DisplayName("According to the class nameObtainConstantData")]
    public async Task<List<ConstOutput>> GetData([Required] string typeName)
    {
        var key = $"{CacheConst.KeyConst}{typeName.ToUpper()}";
        var constList = _sysCacheService.Get<List<ConstOutput>>(key);
        if (constList == null)
        {
            var typeList = GetConstAttributeList();
            var type = typeList.FirstOrDefault(u => u.Name == typeName);
            if (type != null)
            {
                var isEnum = type.BaseType!.Name == "Enum";
                constList = type.GetFields()?
                    .Where(isEnum, u => u.FieldType.Name == typeName)
                    .Select(u => new ConstOutput
                    {
                        Name = u.Name,
                        Code = isEnum ? (int)u.GetValue(BindingFlags.Instance)! : u.GetValue(BindingFlags.Instance)
                    }).ToList();
                _sysCacheService.Set(key, constList);
            }
        }
        return await Task.FromResult(constList);
    }

    /// <summary>
    /// Get a list of constant attribute types
    /// </summary>
    /// <returns></returns>
    private List<Type> GetConstAttributeList()
    {
        return App.EffectiveTypes.Where(u => u.CustomAttributes.Any(c => c.AttributeType == typeof(ConstAttribute))).ToList();
    }
}