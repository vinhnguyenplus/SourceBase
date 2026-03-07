// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Custom template engine
/// </summary>
public class CustomViewEngine : ViewEngineModel
{
    /// <summary>
    /// library locator
    /// </summary>
    public string ConfigId { get; set; } = SqlSugarConst.MainConfigId;

    public string AuthorName { get; set; }

    public string BusName { get; set; }

    public string NameSpace { get; set; }

    public string ClassName { get; set; }

    public string LowerClassName { get; set; }

    public string ProjectLastName { get; set; }

    public string PagePath { get; set; } = "main";

    public string PrintType { get; set; }

    public string PrintName { get; set; }

    public bool HasLikeQuery { get; set; }

    public bool HasJoinTable { get; set; }

    public bool HasEnumField { get; set; }

    public bool HasDictField { get; set; }

    public bool HasConstField { get; set; }

    public bool HasSetStatus => TableField.Any(IsStatus);

    public List<CodeGenConfig> TableField { get; set; }

    public List<CodeGenConfig> ImportFieldList { get; set; }

    public List<CodeGenConfig> UploadFieldList { get; set; }

    public List<CodeGenConfig> QueryWhetherList { get; set; }

    public List<CodeGenConfig> ApiTreeFieldList { get; set; }

    public List<CodeGenConfig> DropdownFieldList { get; set; }

    public List<CodeGenConfig> AddUpdateFieldList { get; set; }

    public List<CodeGenConfig> PrimaryKeyFieldList { get; set; }

    public List<TableUniqueConfigItem> TableUniqueConfigList { get; set; }

    public List<CodeGenConfig> IgnoreUpdateFieldList => TableField.Where(u => u.WhetherAddUpdate == "N" && u.ColumnKey != "True" && u.WhetherCommon != "Y").ToList();

    /// <summary>
    /// Format primary key query conditions
    /// Example: PrimaryKeysFormat(" || ", "u.{0} == input.{0}")
    /// Single primary key returns u.Id == input.Id
    /// The combined primary key returns u.Id == input.Id || u.FkId == input.FkId
    /// </summary>
    /// <param name="separator">delimiter</param>
    /// <param name="format">template string</param>
    /// <param name="lowerFirstLetter">Lowercase first letter of field</param>
    /// <returns></returns>
    public string PrimaryKeysFormat(string separator, string format, bool lowerFirstLetter = false) => string.Join(separator, PrimaryKeyFieldList.Select(u => string.Format(format, lowerFirstLetter ? u.LowerPropertyName : u.PropertyName)));

    /// <summary>
    /// Injected services
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> InjectServiceMap
    {
        get
        {
            var injectMap = new Dictionary<string, string>();
            if (UploadFieldList.Count > 0) injectMap.Add(nameof(SysFileService), ToLowerFirstLetter(nameof(SysFileService)));
            if (DropdownFieldList.Count > 0 || ImportFieldList.Count > 0) injectMap.Add(nameof(ISqlSugarClient), ToLowerFirstLetter(nameof(ISqlSugarClient).TrimStart('I')));
            if (ImportFieldList.Any(c => c.EffectType == "DictSelector")) injectMap.Add(nameof(SysDictTypeService), ToLowerFirstLetter(nameof(SysDictTypeService)));
            return injectMap;
        }
    }

    /// <summary>
    /// Service construction parameters
    /// </summary>
    public string InjectServiceArgs => InjectServiceMap.Count > 0 ? ", " + string.Join(", ", InjectServiceMap.Select(kv => $"{kv.Key} {kv.Value}")) : "";

    /// <summary>
    /// Default value list
    /// </summary>
    public List<CodeGenConfig> DefaultValueList { get; set; }

    /// <summary>
    /// Determine whether the field is a status field
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public bool IsStatus(CodeGenConfig column) => column.NetType == nameof(StatusEnum);

    /// <summary>
    /// Get the first letter of a string in lowercase
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string ToLowerFirstLetter(string text) => string.IsNullOrWhiteSpace(text) ? text : text[..1].ToLower() + text[1..];

    /// <summary>
    /// Convert basic field type to nullable type
    /// </summary>
    /// <param name="netType"></param>
    /// <returns></returns>
    public string GetNullableNetType(string netType) => Regex.IsMatch(netType, "(.*?Enum|bool|char|int|long|double|float|decimal)[?]?") ? netType.TrimEnd('?') + "?" : netType;

    /// <summary>
    /// Get the properties defined by the front-end table column
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public string GetElTableColumnCustomProperty(CodeGenConfig column)
    {
        var content = $"prop='{column.LowerPropertyName}' label='{column.ColumnComment}'";
        if (IsStatus(column)) content += $" v-auth=\"'{LowerClassName}:setStatus'\"";
        if (column.WhetherSortable == "Y") content += " sortable='custom'";
        return content;
    }

    /// <summary>
    /// Set default value
    /// </summary>
    /// <returns></returns>
    public string GetAddDefaultValue()
    {
        var content = "";
        if (DefaultValueList.Count == 0)
        {
            var status = TableField.FirstOrDefault(IsStatus);
            var orderNo = TableField.FirstOrDefault(c => c.NetType.TrimEnd('?') == "int" && c.PropertyName == nameof(SysUser.OrderNo));
            if (status != null) content += $"{status.LowerPropertyName}: {(int)StatusEnum.Enable},";
            if (orderNo != null) content += $"{orderNo.LowerPropertyName}: 100,";
        }
        else
        {
            foreach (var item in DefaultValueList)
            {
                if (!string.IsNullOrWhiteSpace(item.DefaultValue))
                {
                    switch (item.EffectType)
                    {
                        case "InputNumber":
                        case "EnumSelector":// Enumeration and number box, extract numbers through regular expression: such as ('0')
                            content += $"{item.LowerPropertyName}: {Regex.Match(item.DefaultValue, @"\d+").Value},";
                            break;

                        case "Switch":
                            content += $"{item.LowerPropertyName}: {(item.DefaultValue == "1" ? true.ToString().ToLower() : false.ToString().ToLower())},";
                            break;

                        case "DatePicker":// Ignore adapting date format
                            break;

                        default:
                            content += $"{item.LowerPropertyName}: \"{item.DefaultValue}\",";// If it is a string DefaultValue=('male')
                            break;
                    }
                }
            }
        }
        return content;
    }
}