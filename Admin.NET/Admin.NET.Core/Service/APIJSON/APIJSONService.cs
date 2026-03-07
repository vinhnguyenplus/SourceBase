// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// APIJSON service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class APIJSONService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly IdentityService _identityService;
    private readonly TableMapper _tableMapper;
    private readonly SelectTable _selectTable;

    public APIJSONService(ISqlSugarClient db,
        IdentityService identityService,
        TableMapper tableMapper)
    {
        _db = db;
        _tableMapper = tableMapper;
        _identityService = identityService;
        _selectTable = new SelectTable(_identityService, _tableMapper, _db);
    }

    /// <summary>
    /// Unified query portal 🔖
    /// </summary>
    /// <param name="jobject"></param>
    /// <remarks>Parameters: {"[]":{"SYSLOGOP":{}}}</remarks>
    /// <returns></returns>
    [HttpPost("get")]
    [DisplayName("APIJSON Unified Query")]
    public JObject Query([FromBody] JObject jobject)
    {
        var database = jobject["@database"]?.ToString();
        if (!string.IsNullOrEmpty(database))
        {
            // Set up database
            var provider = _db.AsTenant().GetConnectionScope(database);
            jobject.Remove("@database");
            return new SelectTable(_identityService, _tableMapper, provider).Query(jobject);
        }
        return _selectTable.Query(jobject);
    }

    /// <summary>
    /// Query 🔖
    /// </summary>
    /// <param name="table"></param>
    /// <param name="jobject"></param>
    /// <returns></returns>
    [HttpPost("get/{table}")]
    [DisplayName("APIJSON query")]
    public JObject QueryByTable([FromRoute] string table, [FromBody] JObject jobject)
    {
        var ht = new JObject
        {
            { table + "[]", jobject }
        };

        // Automatically add total quantity
        if (jobject["query"] != null && jobject["query"].ToString() != "0" && jobject["total@"] == null)
            ht.Add("total@", "");

        // Maximum 1000 pieces of data per page
        if (jobject["count"] != null && int.Parse(jobject["count"].ToString()) > 1000)
            throw Oops.Bah("The maximum number of count paging cannot exceed 1000");

        jobject.Remove("@debug");

        var hasTableKey = false;
        var ignoreConditions = new List<string> { "page", "count", "query" };
        var tableConditions = new JObject(); // Other query conditions for the table, such as filtering, fields, etc.
        foreach (var item in jobject)
        {
            if (item.Key.Equals(table, StringComparison.CurrentCultureIgnoreCase))
            {
                hasTableKey = true;
                break;
            }
            if (!ignoreConditions.Contains(item.Key.ToLower()))
                tableConditions.Add(item.Key, item.Value);
        }

        foreach (var removeKey in tableConditions)
        {
            jobject.Remove(removeKey.Key);
        }

        if (!hasTableKey)
            jobject.Add(table, tableConditions);

        return Query(ht);
    }

    /// <summary>
    /// New 🔖
    /// </summary>
    /// <param name="tables">Table object or array, if no Id is passed, the backend will generate the Id.</param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("APIJSONAdd New")]
    [UnitOfWork]
    public JObject Add([FromBody] JObject tables)
    {
        var ht = new JObject();
        foreach (var table in tables)
        {
            var talbeName = table.Key.Trim();
            var role = _identityService.GetRole();
            if (!role.Insert.Table.Contains(talbeName, StringComparer.CurrentCultureIgnoreCase))
                throw Oops.Bah($"No permission to add {talbeName}");

            JToken result;
            // Batch insert
            if (table.Value is JArray)
            {
                var ids = new List<object>();
                foreach (var record in table.Value)
                {
                    var cols = record.ToObject<JObject>();
                    var id = _selectTable.InsertSingle(talbeName, cols, role);
                    ids.Add(id);
                }
                result = JToken.FromObject(new { id = ids, count = ids.Count });
            }
            // Single insert
            else
            {
                var cols = table.Value.ToObject<JObject>();
                var id = _selectTable.InsertSingle(talbeName, cols, role);
                result = JToken.FromObject(new { id });
            }
            ht.Add(talbeName, result);
        }
        return ht;
    }

    /// <summary>
    /// Update (only supports Id as condition) 🔖
    /// </summary>
    /// <param name="tables">Supports batch updates of multiple tables and multiple IDs</param>
    /// <returns></returns>
    [HttpPost("update")]
    [DisplayName("APIJSON update")]
    [UnitOfWork]
    public JObject Edit([FromBody] JObject tables)
    {
        var ht = new JObject();
        foreach (var table in tables)
        {
            var tableName = table.Key.Trim();
            var role = _identityService.GetRole();
            var count = _selectTable.UpdateSingleTable(tableName, table.Value, role);
            ht.Add(tableName, JToken.FromObject(new { count }));
        }
        return ht;
    }

    /// <summary>
    /// Delete (supports non-Id conditions, supports batch) 🔖
    /// </summary>
    /// <param name="tables"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("APIJSON Delete")]
    [UnitOfWork]
    public JObject Delete([FromBody] JObject tables)
    {
        var ht = new JObject();
        var role = _identityService.GetRole();
        foreach (var table in tables)
        {
            var talbeName = table.Key.Trim();
            if (role.Delete == null || role.Delete.Table == null)
                throw Oops.Bah("delete permission is not configured");
            if (!role.Delete.Table.Contains(talbeName, StringComparer.CurrentCultureIgnoreCase))
                throw Oops.Bah($"No permission to delete {tableName}");
            //if (!value.ContainsKey("id"))
            //    throw Oops.Bah("Primary key id not passed");

            var value = JObject.Parse(table.Value.ToString());
            var sb = new StringBuilder(100);
            var parameters = new List<SugarParameter>();
            foreach (var f in value)
            {
                if (f.Value is JArray)
                {
                    sb.Append($"{f.Key} in (@{f.Key}) and ");
                    var paraArray = FuncList.TransJArrayToSugarPara(f.Value);
                    parameters.Add(new SugarParameter($"@{f.Key}", paraArray));
                }
                else
                {
                    sb.Append($"{f.Key}=@{f.Key} and ");
                    parameters.Add(new SugarParameter($"@{f.Key}", FuncList.TransJObjectToSugarPara(f.Value)));
                }
            }
            if (!parameters.Any())
                throw Oops.Bah("Please enter the deletion criteria");

            var whereSql = sb.ToString().TrimEnd(" and ");
            var count = _db.Deleteable<object>().AS(talbeName).Where(whereSql, parameters).ExecuteCommand(); // Delete without entity
            value.Add("count", count); // Number of hits
            ht.Add(talbeName, value);
        }
        return ht;
    }
}