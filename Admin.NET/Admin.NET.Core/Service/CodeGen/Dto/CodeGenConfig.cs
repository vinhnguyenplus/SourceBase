// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Code generation detailed configuration parameters
/// </summary>
public class CodeGenConfig
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Code to generate main table ID
    /// </summary>
    public long CodeGenId { get; set; }

    /// <summary>
    /// Database field name
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// primary foreign key
    /// </summary>
    public string ColumnKey { get; set; }

    /// <summary>
    /// Entity attribute name
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Field data length
    /// </summary>
    public int ColumnLength { get; set; }

    /// <summary>
    /// Database field name (first letter lowercase)
    /// </summary>
    public string LowerPropertyName => string.IsNullOrWhiteSpace(PropertyName) ? null : PropertyName[..1].ToLower() + PropertyName[1..];

    /// <summary>
    /// Field description
    /// </summary>
    public string ColumnComment { get; set; }

    /// <summary>
    /// .NET type
    /// </summary>
    public string NetType { get; set; }

    /// <summary>
    /// Type in database (physical type)
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Field data default value
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Nullable .NET Type
    /// </summary>
    public string NullableNetType => Regex.IsMatch(NetType ?? "", "(.*?Enum|bool|char|int|long|double|float|decimal)[?]?") ? NetType.TrimEnd('?') + "?" : NetType;

    /// <summary>
    /// Action type (dictionary)
    /// </summary>
    public string EffectType { get; set; }

    /// <summary>
    /// Foreign key library identifier
    /// </summary>
    public string FkConfigId { get; set; }

    /// <summary>
    /// Foreign key entity name
    /// </summary>
    public string FkEntityName { get; set; }

    /// <summary>
    /// Foreign key table name
    /// </summary>
    public string FkTableName { get; set; }

    /// <summary>
    /// Foreign key entity name (first letter lowercase)
    /// </summary>
    public string LowerFkEntityName => string.IsNullOrWhiteSpace(FkEntityName) ? null : FkEntityName[..1].ToLower() + FkEntityName[1..];

    /// <summary>
    /// Foreign key link field
    /// </summary>
    public string FkLinkColumnName { get; set; }

    /// <summary>
    /// Foreign key display field
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string FkDisplayColumns { get; set; }

    /// <summary>
    /// Foreign key display field
    /// </summary>
    public List<string> FkDisplayColumnList { get; set; }

    /// <summary>
    /// Foreign key display field (first letter lowercase)
    /// </summary>
    public List<string> LowerFkDisplayColumnsList => FkDisplayColumnList?.Select(name => name[..1].ToLower() + name[1..]).ToList();

    /// <summary>
    /// Foreign key display field .NET type
    /// </summary>
    public string FkColumnNetType { get; set; }

    /// <summary>
    /// parent field
    /// </summary>
    public string PidColumn { get; set; }

    /// <summary>
    /// Dictionary code
    /// </summary>
    public string DictTypeCode { get; set; }

    /// <summary>
    /// Query method
    /// </summary>
    public string QueryType { get; set; }

    /// <summary>
    /// Is it a query condition?
    /// </summary>
    public string WhetherQuery { get; set; }

    /// <summary>
    /// Whether the list is indented (dictionary)
    /// </summary>
    public string WhetherRetract { get; set; }

    /// <summary>
    /// Is it required (dictionary)
    /// </summary>
    public string WhetherRequired { get; set; }

    /// <summary>
    /// Is it sortable (dictionary)
    /// </summary>
    public string WhetherSortable { get; set; }

    /// <summary>
    /// List display
    /// </summary>
    public string WhetherTable { get; set; }

    /// <summary>
    /// Additions and changes
    /// </summary>
    public string WhetherAddUpdate { get; set; }

    /// <summary>
    /// import
    /// </summary>
    public string WhetherImport { get; set; }

    /// <summary>
    /// Is it a general field?
    /// </summary>
    public string WhetherCommon { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    public int OrderNo { get; set; }

    /// <summary>
    /// Is it a selector control?
    /// </summary>
    public bool IsSelectorEffectType => Regex.IsMatch(EffectType ?? "", "Selector$|ForeignKey", RegexOptions.IgnoreCase);

    /// <summary>
    /// Remove the attribute name from the trailing Id
    /// </summary>
    public string PropertyNameTrimEndId => PropertyName.TrimEnd("Id");

    /// <summary>
    /// Remove the attribute name from the trailing Id
    /// </summary>
    public string LowerPropertyNameTrimEndId => LowerPropertyName.TrimEnd("Id");

    /// <summary>
    /// extended attribute name
    /// </summary>
    public string ExtendedPropertyName => EffectType switch
    {
        "ForeignKey" => $"{PropertyName.TrimEnd("Id")}FkDisplayName",
        "ApiTreeSelector" => $"{PropertyName.TrimEnd("Id")}DisplayName",
        "DictSelector" => $"{PropertyName.TrimEnd("Id")}DictLabel",
        "Upload" => $"{PropertyName.TrimEnd("Id")}Attachment",
        "Upload_SingleFile" => $"{PropertyName.TrimEnd("Id")}Attachment",
        _ => PropertyName
    };

    /// <summary>
    /// Extended attribute name with lowercase first letter
    /// </summary>
    public string LowerExtendedPropertyName
    {
        get
        {
            var displayPropertyName = ExtendedPropertyName;
            if (string.IsNullOrWhiteSpace(displayPropertyName)) return null;
            return displayPropertyName[..1].ToLower() + displayPropertyName[1..];
        }
    }

    /// <summary>
    /// Get foreign key display value statement
    /// </summary>
    /// <param name="tableAlias">table alias</param>
    /// <param name="separator">Connector for multiple fields</param>
    /// <returns></returns>
    public string GetDisplayColumn(string tableAlias, string separator = "-") => "$\"" + string.Join(separator, FkDisplayColumnList?.Select(name => $"{{{tableAlias}.{name}}}") ?? new List<string>()) + "\"";
}