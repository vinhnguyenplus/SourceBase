// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.NET.Core;

/// <summary>
/// JSON time serialization yyyy-MM-dd HH:mm:ss
/// </summary>
public class ChinaDateTimeConverter : DateTimeConverterBase
{
    private static readonly IsoDateTimeConverter DtConverter = new() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return DtConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DtConverter.WriteJson(writer, value, serializer);
    }
}

/// <summary>
/// JSON time serializationyyyy-MM-dd HH:mm
/// </summary>
public class ChinaDateTimeConverterHH : DateTimeConverterBase
{
    private static readonly IsoDateTimeConverter DtConverter = new() { DateTimeFormat = "yyyy-MM-dd HH:mm" };

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return DtConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DtConverter.WriteJson(writer, value, serializer);
    }
}

/// <summary>
/// JSON time serializationyyyy-MM-dd
/// </summary>
public class ChinaDateTimeConverterDate : DateTimeConverterBase
{
    private static readonly IsoDateTimeConverter DtConverter = new() { DateTimeFormat = "yyyy-MM-dd" };

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return DtConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DtConverter.WriteJson(writer, value, serializer);
    }
}