// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System code generation configuration service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 260)]
public class SysCodeGenConfigService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;

    public SysCodeGenConfigService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// Get code generation configuration list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get a list of code generation configurations")]
    public async Task<List<CodeGenConfig>> GetList([FromQuery] CodeGenConfig input)
    {
        return await _db.Queryable<SysCodeGenConfig>()
            .Where(u => u.CodeGenId == input.CodeGenId)
            .Select<CodeGenConfig>()
            .Mapper(u =>
            {
                u.NetType = (u.EffectType == "EnumSelector" ? u.DictTypeCode : u.NetType);
                u.FkDisplayColumnList = u.FkDisplayColumns?.Split(",").ToList();
            })
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();
    }

    /// <summary>
    /// Update code generation configuration 🔖
    /// </summary>
    /// <param name="inputList"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update code generation configuration")]
    public async Task UpdateCodeGenConfig(List<CodeGenConfig> inputList)
    {
        if (inputList == null || inputList.Count < 1) return;
        inputList.ForEach(e =>
        {
            e.FkDisplayColumns = e.FkDisplayColumnList?.Count > 0 ? string.Join(",", e.FkDisplayColumnList) : null;
        });
        await _db.Updateable(inputList.Adapt<List<SysCodeGenConfig>>())
            .IgnoreColumns(u => new { u.ColumnLength, u.ColumnName, u.PropertyName })
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Remove code generation configuration
    /// </summary>
    /// <param name="codeGenId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task DeleteCodeGenConfig(long codeGenId)
    {
        await _db.Deleteable<SysCodeGenConfig>().Where(u => u.CodeGenId == codeGenId).ExecuteCommandAsync();
    }

    /// <summary>
    /// Get code generation configuration details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get code generation configuration details")]
    public async Task<SysCodeGenConfig> GetDetail([FromQuery] CodeGenConfig input)
    {
        return await _db.Queryable<SysCodeGenConfig>().FirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Add code generation configurations in batches
    /// </summary>
    /// <param name="tableColumnOutputList"></param>
    /// <param name="codeGenerate"></param>
    [NonAction]
    public void AddList(List<ColumnOuput> tableColumnOutputList, SysCodeGen codeGenerate)
    {
        if (tableColumnOutputList == null) return;

        var codeGenConfigs = new List<SysCodeGenConfig>();
        var orderNo = 100;
        foreach (var tableColumn in tableColumnOutputList)
        {
            var codeGenConfig = new SysCodeGenConfig();

            var yesOrNo = YesNoEnum.Y.ToString();
            if (Convert.ToBoolean(tableColumn.ColumnKey)) yesOrNo = YesNoEnum.N.ToString();

            if (CodeGenUtil.IsCommonColumn(tableColumn.PropertyName))
            {
                codeGenConfig.WhetherCommon = YesNoEnum.Y.ToString();
                yesOrNo = YesNoEnum.N.ToString();
            }
            else
            {
                codeGenConfig.WhetherCommon = YesNoEnum.N.ToString();
            }

            codeGenConfig.CodeGenId = codeGenerate.Id;
            codeGenConfig.ColumnName = tableColumn.ColumnName; // Field name
            codeGenConfig.PropertyName = tableColumn.PropertyName;// Entity attribute name
            codeGenConfig.ColumnLength = tableColumn.ColumnLength;// length
            codeGenConfig.ColumnComment = tableColumn.ColumnComment;
            codeGenConfig.NetType = tableColumn.NetType;
            codeGenConfig.DefaultValue = tableColumn.DefaultValue;
            codeGenConfig.WhetherRetract = YesNoEnum.N.ToString();

            // When generating code, the primary key is not a necessary input, so the primary key field must be excluded.
            codeGenConfig.WhetherRequired = (tableColumn.IsNullable || tableColumn.IsPrimarykey) ? YesNoEnum.N.ToString() : YesNoEnum.Y.ToString();
            codeGenConfig.WhetherQuery = yesOrNo;
            codeGenConfig.WhetherImport = yesOrNo;
            codeGenConfig.WhetherAddUpdate = yesOrNo;
            codeGenConfig.WhetherTable = yesOrNo;

            codeGenConfig.ColumnKey = tableColumn.ColumnKey;

            codeGenConfig.DataType = tableColumn.DataType;
            codeGenConfig.EffectType = CodeGenUtil.DataTypeToEff(codeGenConfig.NetType);
            codeGenConfig.QueryType = GetDefaultQueryType(codeGenConfig); // QueryTypeEnum.eq.ToString();
            codeGenConfig.OrderNo = orderNo;
            codeGenConfigs.Add(codeGenConfig);

            if (!string.IsNullOrWhiteSpace(tableColumn.DictTypeCode))
            {
                codeGenConfig.QueryType = "==";
                codeGenConfig.DictTypeCode = tableColumn.DictTypeCode;
                codeGenConfig.EffectType = tableColumn.DictTypeCode.EndsWith("Enum") ? "EnumSelector" : "DictSelector";
            }

            orderNo += 10; // Each configuration sort interval is 10
        }
        // Multi-library code generation---switch back to the main library here
        var provider = _db.AsTenant().GetConnectionScope(SqlSugarConst.MainConfigId);
        provider.Insertable(codeGenConfigs).ExecuteCommand();
    }

    /// <summary>
    /// Batch update code fields: delete first and then add, historical field operation types will be retained
    /// </summary>
    /// <param name="tableColumnOutputList"></param>
    /// <param name="codeGenId"></param>
    [NonAction]
    public async Task UpdateList(List<ColumnOuput> tableColumnOutputList, long codeGenId)
    {
        if (tableColumnOutputList == null) return;

        //Get historical data
        var oldList = await GetList(new CodeGenConfig() { CodeGenId = codeGenId, });
        //Delete historical data
        await DeleteCodeGenConfig(codeGenId);

        var codeGenConfigs = new List<SysCodeGenConfig>();
        var orderNo = 100;
        foreach (var tableColumn in tableColumnOutputList)
        {
            var oldItem = oldList.FirstOrDefault(u => u.ColumnName == tableColumn.ColumnName);

            var codeGenConfig = new SysCodeGenConfig();

            var yesOrNo = YesNoEnum.Y.ToString();
            if (Convert.ToBoolean(tableColumn.ColumnKey)) yesOrNo = YesNoEnum.N.ToString();

            if (CodeGenUtil.IsCommonColumn(tableColumn.PropertyName))
            {
                codeGenConfig.WhetherCommon = YesNoEnum.Y.ToString();
                yesOrNo = YesNoEnum.N.ToString();
            }
            else
            {
                codeGenConfig.WhetherCommon = YesNoEnum.N.ToString();
            }

            codeGenConfig.CodeGenId = codeGenId;
            codeGenConfig.ColumnName = tableColumn.ColumnName; // Field name
            codeGenConfig.PropertyName = tableColumn.PropertyName;// Entity attribute name
            codeGenConfig.ColumnLength = tableColumn.ColumnLength;// length
            codeGenConfig.ColumnComment = tableColumn.ColumnComment;
            codeGenConfig.NetType = tableColumn.NetType;
            codeGenConfig.DefaultValue = tableColumn.DefaultValue;
            codeGenConfig.WhetherRetract = YesNoEnum.N.ToString();

            // When generating code, the primary key is not a necessary input, so the primary key field must be excluded.
            codeGenConfig.WhetherRequired = (tableColumn.IsNullable || tableColumn.IsPrimarykey) ? YesNoEnum.N.ToString() : YesNoEnum.Y.ToString();
            codeGenConfig.WhetherQuery = yesOrNo;
            codeGenConfig.WhetherImport = yesOrNo;
            codeGenConfig.WhetherAddUpdate = yesOrNo;
            codeGenConfig.WhetherTable = yesOrNo;

            codeGenConfig.ColumnKey = tableColumn.ColumnKey;

            codeGenConfig.DataType = tableColumn.DataType;
            codeGenConfig.EffectType = CodeGenUtil.DataTypeToEff(codeGenConfig.NetType);
            codeGenConfig.QueryType = GetDefaultQueryType(codeGenConfig); // QueryTypeEnum.eq.ToString();
            codeGenConfig.OrderNo = orderNo;

            if (oldItem != null)
            {
                //Inherits if history exists
                codeGenConfig.WhetherQuery = oldItem.WhetherQuery;
                codeGenConfig.WhetherImport = oldItem.WhetherImport;
                codeGenConfig.WhetherAddUpdate = oldItem.WhetherAddUpdate;
                codeGenConfig.WhetherTable = oldItem.WhetherTable;

                codeGenConfig.EffectType = oldItem.EffectType;
                codeGenConfig.FkConfigId = oldItem.FkConfigId;
                codeGenConfig.FkEntityName = oldItem.FkEntityName;
                codeGenConfig.FkTableName = oldItem.FkTableName;
                codeGenConfig.FkDisplayColumns = oldItem.FkDisplayColumns;
                codeGenConfig.FkLinkColumnName = oldItem.FkLinkColumnName;
                codeGenConfig.FkColumnNetType = oldItem.FkColumnNetType;
            }

            codeGenConfigs.Add(codeGenConfig);

            if (!string.IsNullOrWhiteSpace(tableColumn.DictTypeCode))
            {
                codeGenConfig.QueryType = "==";
                codeGenConfig.DictTypeCode = tableColumn.DictTypeCode;
                codeGenConfig.EffectType = tableColumn.DictTypeCode.EndsWith("Enum") ? "EnumSelector" : "DictSelector";
            }

            orderNo += 10; // Each configuration sort interval is 10
        }
        // Multi-library code generation---switch back to the main library here
        var provider = _db.AsTenant().GetConnectionScope(SqlSugarConst.MainConfigId);
        provider.Insertable(codeGenConfigs).ExecuteCommand();
    }

    /// <summary>
    /// Default query type
    /// </summary>
    /// <param name="codeGenConfig"></param>
    /// <returns></returns>
    private static string GetDefaultQueryType(SysCodeGenConfig codeGenConfig)
    {
        return (codeGenConfig.NetType?.TrimEnd('?')) switch
        {
            "string" => "like",
            "DateTime" => "~",
            _ => "==",
        };
    }
}