// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;

namespace Admin.NET.Core;

/// <summary>
/// Object extension
/// </summary>
[SuppressSniffer]
public static partial class ObjectExtension
{
    /// <summary>
    /// Type attribute list mapping table
    /// </summary>
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    /// <summary>
    /// Desensitization feature cache mapping table
    /// </summary>
    private static readonly ConcurrentDictionary<PropertyInfo, DataMaskAttribute> AttributeCache = new();

    /// <summary>
    /// Determine whether a type implements a certain generic
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="generic">Generic type</param>
    /// <returns>bool</returns>
    public static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        // Check interface type
        var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
        if (isTheRawGenericType) return true;

        // Check type
        while (type != null && type != typeof(object))
        {
            isTheRawGenericType = IsTheRawGenericType(type);
            if (isTheRawGenericType) return true;
            type = type.BaseType;
        }

        return false;

        // Judgment logic
        bool IsTheRawGenericType(Type type) => generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
    }

    /// <summary>
    /// Convert dictionary to QueryString format
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="urlEncode"></param>
    /// <returns></returns>
    public static string ToQueryString(this Dictionary<string, string> dict, bool urlEncode = true)
    {
        return string.Join("&", dict.Select(p => $"{(urlEncode ? p.Key?.UrlEncode() : "")}={(urlEncode ? p.Value?.UrlEncode() : "")}"));
    }

    /// <summary>
    /// URL encode a string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string UrlEncode(this string str)
    {
        return string.IsNullOrEmpty(str) ? "" : System.Uri.EscapeDataString(str);
    }

    /// <summary>
    /// Object serialized into Json string
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToJson(this object obj)
    {
        var jsonSettings = SetNewtonsoftJsonSetting();
        return JSON.GetJsonSerializer().Serialize(obj, jsonSettings);
    }

    private static JsonSerializerSettings SetNewtonsoftJsonSetting()
    {
        JsonSerializerSettings setting = new JsonSerializerSettings();
        setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        setting.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // time formatting
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // Ignore circular references
                                                                      //setting.ContractResolver = new HelErpContractResolver("StartTime", customName);
        return setting;
    }

    /// <summary>
    /// Deserialize Json string into object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T ToObject<T>(this string json)
    {
        return JSON.GetJsonSerializer().Deserialize<T>(json);
    }

    /// <summary>
    /// Convert object to long, returning 0 if failed
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static long ParseToLong(this object obj)
    {
        try
        {
            return long.Parse(obj.ToString());
        }
        catch
        {
            return 0L;
        }
    }

    /// <summary>
    /// Convert object to long, returning the specified value if failed
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ParseToLong(this string str, long defaultValue)
    {
        try
        {
            return long.Parse(str);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Convert object to double, returning 0 if failed
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object obj)
    {
        try
        {
            return double.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Convert object to double, returning the specified value if failed
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object str, double defaultValue)
    {
        try
        {
            return double.Parse(str.ToString());
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Convert string to DateTime, returning the minimum date if failed
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DateTime.MinValue;
            }
            if (str.Contains('-') || str.Contains('/'))
            {
                return DateTime.Parse(str);
            }
            else
            {
                int length = str.Length;
                switch (length)
                {
                    case 4:
                        return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);

                    case 6:
                        return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);

                    case 8:
                        return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                    case 10:
                        return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);

                    case 12:
                        return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);

                    case 14:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                    default:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                }
            }
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Convert string to DateTime, returning default value if failed
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str, DateTime? defaultValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue.GetValueOrDefault();
            }
            if (str.Contains('-') || str.Contains('/'))
            {
                return DateTime.Parse(str);
            }
            else
            {
                int length = str.Length;
                switch (length)
                {
                    case 4:
                        return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);

                    case 6:
                        return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);

                    case 8:
                        return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                    case 10:
                        return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);

                    case 12:
                        return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);

                    case 14:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                    default:
                        return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                }
            }
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    /// <summary>
    /// Convert string time and date format into string such as {yyyy} => 2024
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ParseToDateTimeForRep(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            str = $"{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";

        var date = DateTime.Now;
        var reg = new Regex(@"(\{.+?})");
        var match = reg.Matches(str);
        match.ToList().ForEach(u =>
        {
            var temp = date.ToString(u.ToString().Substring(1, u.Length - 2));
            str = str.Replace(u.ToString(), temp);
        });
        return str;
    }

    /// <summary>
    /// Is it valuable?
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this object obj)
    {
        return obj == null || string.IsNullOrEmpty(obj.ToString());
    }

    /// <summary>
    /// String mask
    /// </summary>
    /// <param name="str">string</param>
    /// <param name="mask">mask character</param>
    /// <returns></returns>
    public static string Mask(this string str, char mask = '*')
    {
        if (string.IsNullOrWhiteSpace(str?.Trim()))
            return str;

        str = str.Trim();
        var masks = mask.ToString().PadLeft(4, mask);
        return str.Length switch
        {
            >= 11 => Regex.Replace(str, "(.{3}).*(.{4})", $"$1{masks}$2"),
            10 => Regex.Replace(str, "(.{3}).*(.{3})", $"$1{masks}$2"),
            9 => Regex.Replace(str, "(.{2}).*(.{3})", $"$1{masks}$2"),
            8 => Regex.Replace(str, "(.{2}).*(.{2})", $"$1{masks}$2"),
            7 => Regex.Replace(str, "(.{1}).*(.{2})", $"$1{masks}$2"),
            6 => Regex.Replace(str, "(.{1}).*(.{1})", $"$1{masks}$2"),
            _ => Regex.Replace(str, "(.{1}).*", $"$1{masks}")
        };
    }

    /// <summary>
    /// ID number mask
    /// </summary>
    /// <param name="idCard">ID number</param>
    /// <param name="mask">mask character</param>
    /// <returns></returns>
    public static string MaskIdCard(this string idCard, char mask = '*')
    {
        if (!idCard.TryValidate(ValidationTypes.IDCard).IsValid) return idCard;

        var masks = mask.ToString().PadLeft(8, mask);
        return Regex.Replace(idCard, @"^(.{6})(.*)(.{4})$", $"$1{masks}$3");
    }

    /// <summary>
    /// Email mask
    /// </summary>
    /// <param name="email">Mail</param>
    /// <param name="mask">mask character</param>
    /// <returns></returns>
    public static string MaskEmail(this string email, char mask = '*')
    {
        if (!email.TryValidate(ValidationTypes.EmailAddress).IsValid) return email;

        var pos = email.IndexOf("@");
        return Mask(email[..pos], mask) + email[pos..];
    }

    /// <summary>
    /// Convert the string to a value type. If it is not obtained or an error occurs, it returns empty.
    /// </summary>
    /// <typeparam name="T">Specify value type</typeparam>
    /// <param name="str">Pass in string</param>
    /// <returns>Nullable value</returns>
    public static T? ParseTo<T>(this string str) where T : struct
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                MethodInfo method = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null)
                {
                    T result = (T)method.Invoke(null, new string[] { str });
                    return result;
                }
            }
        }
        catch
        {
        }
        return null;
    }

    /// <summary>
    /// Convert the string to a value type. If it is not obtained or an error occurs, it returns empty.
    /// </summary>
    /// <param name="str">Pass in string</param>
    /// <param name="type">target type</param>
    /// <returns>Nullable value</returns>
    public static object ParseTo(this string str, Type type)
    {
        try
        {
            if (type.Name == "String")
                return str;

            if (!string.IsNullOrWhiteSpace(str))
            {
                var _type = type;
                if (type.Name.StartsWith("Nullable"))
                    _type = type.GetGenericArguments()[0];

                MethodInfo method = _type.GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null)
                    return method.Invoke(null, new string[] { str });
            }
        }
        catch
        {
        }
        return null;
    }

    /// <summary>
    /// Assign an object property value to another specified object property, copying only those with the same property
    /// </summary>
    /// <param name="src">original data object</param>
    /// <param name="target">target data object</param>
    /// <param name="changeProperties">Attribute set, the key is the original attribute and the value is the target attribute</param>
    /// <param name="unChangeProperties">Property set, properties that the target does not modify</param>
    public static void CopyTo(object src, object target, Dictionary<string, string> changeProperties = null, string[] unChangeProperties = null)
    {
        if (src == null || target == null)
            throw new ArgumentException("src == null || target == null ");

        var SourceType = src.GetType();
        var TargetType = target.GetType();

        if (changeProperties == null || changeProperties.Count == 0)
        {
            var fields = TargetType.GetProperties();
            changeProperties = fields.Select(m => m.Name).ToDictionary(m => m);
        }

        if (unChangeProperties == null || unChangeProperties.Length == 0)
        {
            foreach (var item in changeProperties)
            {
                var srcProperty = SourceType.GetProperty(item.Key);
                if (srcProperty != null)
                {
                    var sourceVal = srcProperty.GetValue(src, null);

                    var tarProperty = TargetType.GetProperty(item.Value);
                    tarProperty?.SetValue(target, sourceVal, null);
                }
            }
        }
        else
        {
            foreach (var item in changeProperties)
            {
                if (!unChangeProperties.Any(m => m == item.Value))
                {
                    var srcProperty = SourceType.GetProperty(item.Key);
                    if (srcProperty != null)
                    {
                        var sourceVal = srcProperty.GetValue(src, null);

                        var tarProperty = TargetType.GetProperty(item.Value);
                        tarProperty?.SetValue(target, sourceVal, null);
                    }
                }
            }
        }
    }

    /// <summary>
    /// deep copy
    /// </summary>
    /// <typeparam name="T">Deep copy source object</typeparam>
    /// <param name="obj">object</param>
    /// <returns></returns>
    public static T DeepCopy<T>(this T obj)
    {
        var jsonSettings = SetNewtonsoftJsonSetting();
        var json = JSON.Serialize(obj, jsonSettings);
        return JSON.Deserialize<T>(json);
    }

    /// <summary>
    /// Desensitize fields with the <see cref="DataMaskAttribute"/> attribute
    /// </summary>
    public static T MaskSensitiveData<T>(this T obj) where T : class
    {
        if (obj == null) return null;

        var type = typeof(T);

        // Get or cache a collection of properties
        var properties = PropertyCache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) && p.GetCustomAttribute<DataMaskAttribute>() != null)
                .ToArray());

        // Parallel processing of writable properties
        Parallel.ForEach(properties, prop =>
        {
            if (!prop.CanWrite) return;

            // Get or cache properties
            var maskAttr = AttributeCache.GetOrAdd(prop, p => p.GetCustomAttribute<DataMaskAttribute>());

            if (maskAttr == null) return;

            // Handling non-empty strings
            if (prop.GetValue(obj) is string { Length: > 0 } value)
            {
                prop.SetValue(obj, maskAttr.Mask(value));
            }
        });

        return obj;
    }
}