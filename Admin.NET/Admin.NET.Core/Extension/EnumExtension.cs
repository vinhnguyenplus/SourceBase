// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Enumeration extension
/// </summary>
public static class EnumExtension
{
    // Enum showing dictionary cache
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumDisplayValueDict = new();

    // Enum value dictionary cache
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumNameValueDict = new();

    // Enum type cache
    private static ConcurrentDictionary<string, Type> _enumTypeDict;

    /// <summary>
    /// Get the dictionary of enumeration object keys and names (cache)
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static Dictionary<int, string> GetEnumDictionary(this Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");

        // Query cache
        var enumDic = EnumNameValueDict.TryGetValue(enumType, out var value) ? value : new Dictionary<int, string>();
        if (enumDic.Count != 0)
            return enumDic;
        // Get the Key/Value dictionary collection of enumeration type
        enumDic = GetEnumDictionaryItems(enumType);

        // cache
        EnumNameValueDict[enumType] = enumDic;

        return enumDic;
    }

    /// <summary>
    /// Get a dictionary of keys and names of enumeration objects
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    private static Dictionary<int, string> GetEnumDictionaryItems(this Type enumType)
    {
        // Get the fields of the type and initialize a dictionary of limited length
        var enumFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        Dictionary<int, string> enumDic = new(enumFields.Length);

        // Traverse the field array to obtain key and name
        foreach (var enumField in enumFields)
        {
            var intValue = (int)enumField.GetValue(enumType)!;
            enumDic[intValue] = enumField.Name;
        }

        return enumDic;
    }

    /// <summary>
    /// Get a dictionary of enumeration type keys and descriptions (cache)
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Dictionary<int, string> GetEnumDescDictionary(this Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");

        // Query cache
        var enumDic = EnumDisplayValueDict.TryGetValue(enumType, out var value)
            ? value
            : new Dictionary<int, string>();
        if (enumDic.Count != 0)
            return enumDic;
        // Get the Key/Value dictionary collection of enumeration type
        enumDic = GetEnumDescDictionaryItems(enumType);

        // cache
        EnumDisplayValueDict[enumType] = enumDic;

        return enumDic;
    }

    /// <summary>
    /// Get the dictionary of enumeration type key and description (if there is no description, get the name)
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Dictionary<int, string> GetEnumDescDictionaryItems(this Type enumType)
    {
        // Get the fields of the type and initialize a dictionary of limited length
        var enumFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        Dictionary<int, string> enumDic = new(enumFields.Length);

        // Traverse the field array to obtain key and name
        foreach (var enumField in enumFields)
        {
            var intValue = (int)enumField.GetValue(enumType)!;
            var desc = enumField.GetDescriptionValue<DescriptionAttribute>();
            enumDic[intValue] = desc != null && !string.IsNullOrEmpty(desc.Description) ? desc.Description : enumField.Name;
        }

        return enumDic;
    }

    /// <summary>
    /// Find the specified enumeration type from the assembly
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type TryToGetEnumType(Assembly assembly, string typeName)
    {
        // If the enumeration cache is empty, reload the enumeration type dictionary.
        _enumTypeDict ??= LoadEnumTypeDict(assembly);

        // Find by name
        return _enumTypeDict.TryGetValue(typeName, out var value) ? value : null;
    }

    /// <summary>
    /// Load all enum types from assembly
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    private static ConcurrentDictionary<string, Type> LoadEnumTypeDict(Assembly assembly)
    {
        // Get all types in assembly
        var typeArray = assembly.GetTypes();

        // Filter non-enumeration types, convert them into dictionary format and return
        var dict = typeArray.Where(o => o.IsEnum).ToDictionary(o => o.Name, o => o);
        ConcurrentDictionary<string, Type> enumTypeDict = new(dict);
        return enumTypeDict;
    }

    /// <summary>
    /// Get the Description of the enumeration
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetEnumDescription(this Enum value)
    {
        return value.GetType().GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// Get the Description of the enumeration
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetEnumDescription(this object value)
    {
        return value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// Get the Theme of the enumeration
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetTheme(this object value)
    {
        return value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<ThemeAttribute>()?.Theme;
    }

    /// <summary>
    /// Convert enumeration into enumeration information collection
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<EnumEntity> EnumToList(this Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = Enum.GetNames(type);
        return arr.Select(sl =>
        {
            var item = Enum.Parse(type, sl);
            return new EnumEntity
            {
                Name = item.ToString(),
                Describe = item.GetEnumDescription() ?? item.ToString(),
                Theme = item.GetTheme() ?? string.Empty,
                Value = item.GetHashCode()
            };
        }).ToList();
    }

    /// <summary>
    /// EnumToList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<T> EnumToList<T>(this Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = Enum.GetNames(type);
        return arr.Select(name => (T)Enum.Parse(type, name)).ToList();
    }
}

/// <summary>
/// enumeration entities
/// </summary>
public class EnumEntity
{
    /// <summary>
    /// Description of the enumeration
    /// </summary>
    public string Describe { get; set; }

    /// <summary>
    /// enum style
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// enum name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// enum object value
    /// </summary>
    public int Value { get; set; }
}