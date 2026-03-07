// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Custom method
/// </summary>
public class FuncList
{
    /// <summary>
    /// String addition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public string Merge(object a, object b)
    {
        return a.ToString() + b.ToString();
    }

    /// <summary>
    /// Object merge
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public object MergeObj(object a, object b)
    {
        return new { a, b };
    }

    /// <summary>
    /// Does it contain
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsContain(object a, object b)
    {
        return a.ToString().Split(',').Contains(b);
    }

    /// <summary>
    /// Convert SugarParameter according to the actual type of jtoken to avoid converting it all into a string
    /// </summary>
    /// <param name="jToken"></param>
    /// <returns></returns>
    public static dynamic TransJObjectToSugarPara(JToken jToken)
    {
        JTokenType jTokenType = jToken.Type;
        return jTokenType switch
        {
            JTokenType.Integer => jToken.ToObject(typeof(long)),
            JTokenType.Float => jToken.ToObject(typeof(decimal)),
            JTokenType.Boolean => jToken.ToObject(typeof(bool)),
            JTokenType.Date => jToken.ToObject(typeof(DateTime)),
            JTokenType.Bytes => jToken.ToObject(typeof(byte)),
            JTokenType.Guid => jToken.ToObject(typeof(Guid)),
            JTokenType.TimeSpan => jToken.ToObject(typeof(TimeSpan)),
            JTokenType.Array => TransJArrayToSugarPara(jToken),
            _ => jToken
        };
    }

    /// <summary>
    /// Convert SugarParameter according to the actual type of jArray to avoid converting it all into a string
    /// </summary>
    /// <param name="jToken"></param>
    /// <returns></returns>
    public static dynamic TransJArrayToSugarPara(JToken jToken)
    {
        if (jToken is not JArray) return jToken;
        if (jToken.Any())
        {
            JTokenType jTokenType = jToken.First().Type;
            return jTokenType switch
            {
                JTokenType.Integer => jToken.ToObject<long[]>(),
                JTokenType.Float => jToken.ToObject<decimal[]>(),
                JTokenType.Boolean => jToken.ToObject<bool[]>(),
                JTokenType.Date => jToken.ToObject<DateTime[]>(),
                JTokenType.Bytes => jToken.ToObject<byte[]>(),
                JTokenType.Guid => jToken.ToObject<Guid[]>(),
                JTokenType.TimeSpan => jToken.ToObject<TimeSpan[]>(),
                _ => jToken.ToArray()
            };
        }

        return (JArray)jToken;
    }

    /// <summary>
    /// Get the true type of the value in a string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetValueCSharpType(string input)
    {
        if (DateTime.TryParse(input, out _))
            return "DateTime";
        else if (int.TryParse(input, out _))
            return "int";
        else if (long.TryParse(input, out _))
            return "long";
        else if (decimal.TryParse(input, out _))
            return "decimal";
        else if (bool.TryParse(input, out _))
            return "bool";
        else
            return "string";
    }
}