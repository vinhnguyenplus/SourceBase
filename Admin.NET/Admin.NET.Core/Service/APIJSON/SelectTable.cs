// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using AspectCore.Extensions.Reflection;
using System.Dynamic;

namespace Admin.NET.Core.Service;

/// <summary>
///
/// </summary>
public class SelectTable : ISingleton
{
    private readonly IdentityService _identitySvc;
    private readonly TableMapper _tableMapper;
    private readonly ISqlSugarClient _db;

    public SelectTable(IdentityService identityService, TableMapper tableMapper, ISqlSugarClient dbClient)
    {
        _identitySvc = identityService;
        _tableMapper = tableMapper;
        _db = dbClient;
    }

    /// <summary>
    /// Determine whether the table name is correct, if not, throw an exception
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public virtual bool IsTable(string table)
    {
        return _db.DbMaintenance.GetTableInfoList().Any(it => it.Name.Equals(table, StringComparison.CurrentCultureIgnoreCase))
            ? true
            : throw new Exception($"The table name [{table}] is incorrect!");
    }

    /// <summary>
    /// Determine whether the column name of the table is correct. If it is incorrect, throw an exception and expose it to the caller earlier.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public virtual bool IsCol(string table, string col)
    {
        return _db.DbMaintenance.GetColumnInfosByTableName(table).Any(it => it.DbColumnName.Equals(col, StringComparison.CurrentCultureIgnoreCase))
            ? true
            : throw new Exception($"Column 【{col}】 does not exist in table 【{table}】! Please check the input parameters");
    }

    /// <summary>
    /// Query list data
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="page"></param>
    /// <param name="count"></param>
    /// <param name="query"></param>
    /// <param name="json"></param>
    /// <param name="dd"></param>
    /// <returns></returns>
    public virtual Tuple<dynamic, int> GetTableData(string subtable, int page, int count, int query, string json, JObject dd)
    {
        var role = _identitySvc.GetSelectRole(subtable);
        if (!role.Item1)
            throw new Exception(role.Item2);

        var selectrole = role.Item2;
        subtable = _tableMapper.GetTableName(subtable);

        var values = JObject.Parse(json);
        page = values["page"] == null ? page : int.Parse(values["page"].ToString());
        count = values["count"] == null ? count : int.Parse(values["count"].ToString());
        query = values["query"] == null ? query : int.Parse(values["query"].ToString());
        values.Remove("page");
        values.Remove("count");
        // Construct query process
        var tb = SugarQueryable(subtable, selectrole, values, dd);

        // It will actually be executed here
        if (query == 1) // 1-total
        {
            return new Tuple<dynamic, int>(null, tb.MergeTable().Count());
        }
        else
        {
            if (page > 0) // Pagination
            {
                int total = 0;
                if (query == 0)
                    return new Tuple<dynamic, int>(tb.ToPageList(page, count), total); // 0-object
                else
                    return new Tuple<dynamic, int>(tb.ToPageList(page, count, ref total), total); // 2-All of the above
            }
            else // list
            {
                IList l = tb.ToList();
                return query == 0 ? new Tuple<dynamic, int>(l, 0) : new Tuple<dynamic, int>(l, l.Count);
            }
        }
    }

    /// <summary>
    /// Parse and query
    /// </summary>
    /// <param name="queryJson"></param>
    /// <returns></returns>
    public virtual JObject Query(string queryJson)
    {
        var queryJobj = JObject.Parse(queryJson);
        return Query(queryJobj);
    }

    /// <summary>
    /// Single table query
    /// </summary>
    /// <param name="queryObj"></param>
    /// <param name="nodeName">The node name of the returned data defaults to infos</param>
    /// <returns></returns>
    public virtual JObject QuerySingle(JObject queryObj, string nodeName = "infos")
    {
        var resultObj = new JObject();

        var total = 0;
        foreach (var item in queryObj)
        {
            var key = item.Key.Trim();
            if (key.EndsWith("[]"))
            {
                total = QuerySingleList(resultObj, item, nodeName);
            }
            else if (key.Equals("func"))
            {
                ExecFunc(resultObj, item);
            }
            else if (key.Equals("total@") || key.Equals("total"))
            {
                resultObj.Add("total", total);
            }
        }
        return resultObj;
    }

    /// <summary>
    /// Get query statement
    /// </summary>
    /// <param name="queryObj"></param>
    /// <returns></returns>
    public virtual string ToSql(JObject queryObj)
    {
        foreach (var item in queryObj)
        {
            if (item.Key.Trim().EndsWith("[]"))
                return ToSql(item);
        }
        return string.Empty;
    }

    /// <summary>
    /// Parse and query
    /// </summary>
    /// <param name="queryObj"></param>
    /// <returns></returns>
    public virtual JObject Query(JObject queryObj)
    {
        var resultObj = new JObject();

        int total;
        foreach (var item in queryObj)
        {
            var key = item.Key.Trim();
            if (key.Equals("[]")) // list
            {
                total = QueryMoreList(resultObj, item);
                resultObj.Add("total", total); // As long as the list query is performed, the total number will be automatically returned.
            }
            else if (key.EndsWith("[]"))
            {
                total = QuerySingleList(resultObj, item);
            }
            else if (key.Equals("func"))
            {
                ExecFunc(resultObj, item);
            }
            else if (key.Equals("total@") || key.Equals("total"))
            {
                // resultObj.Add("total", total);
                continue;
            }
            else // Single
            {
                var template = GetFirstData(key, item.Value.ToString(), resultObj);
                if (template != null)
                    resultObj.Add(key, JToken.FromObject(template));
            }
        }
        return resultObj;
    }

    // Dynamically calling methods
    private static object ExecFunc(string funcname, object[] param, Type[] types)
    {
        var method = typeof(FuncList).GetMethod(funcname);
        var reflector = method.GetReflector();
        var result = reflector.Invoke(new FuncList(), param);
        return result;
    }

    // generate sql
    private string ToSql(string subtable, int page, int count, int query, string json)
    {
        var values = JObject.Parse(json);
        page = values["page"] == null ? page : int.Parse(values["page"].ToString());
        count = values["count"] == null ? count : int.Parse(values["count"].ToString());
        _ = values["query"] == null ? query : int.Parse(values["query"].ToString());
        values.Remove("page");
        values.Remove("count");
        subtable = _tableMapper.GetTableName(subtable);
        var tb = SugarQueryable(subtable, "*", values, null);
        var sqlObj = tb.Skip((page - 1) * count).Take(10).ToSql();
        return sqlObj.Key;
    }

    /// <summary>
    /// Query the first piece of data
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="json"></param>
    /// <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private dynamic GetFirstData(string subtable, string json, JObject job)
    {
        var role = _identitySvc.GetSelectRole(subtable);
        if (!role.Item1)
            throw new Exception(role.Item2);

        var selectrole = role.Item2;
        subtable = _tableMapper.GetTableName(subtable);

        var values = JObject.Parse(json);
        values.Remove("page");
        values.Remove("count");
        var tb = SugarQueryable(subtable, selectrole, values, job).First();
        var dic = (IDictionary<string, object>)tb;
        foreach (var item in values.Properties().Where(it => it.Name.EndsWith("()")))
        {
            if (item.Value.IsNullOrEmpty())
            {
                var func = item.Value.ToString().Substring(0, item.Value.ToString().IndexOf("("));
                var param = item.Value.ToString().Substring(item.Value.ToString().IndexOf("(") + 1).TrimEnd(')');
                var types = new List<Type>();
                var paramss = new List<object>();
                foreach (var va in param.Split(','))
                {
                    types.Add(typeof(object));
                    paramss.Add(tb.Where(it => it.Key.Equals(va)).Select(i => i.Value));
                }
                dic[item.Name] = ExecFunc(func, paramss.ToArray(), types.ToArray());
            }
        }
        return tb;
    }

    // Single table query, the returned data is in the specified NodeName node
    private int QuerySingleList(JObject resultObj, KeyValuePair<string, JToken> item, string nodeName)
    {
        var key = item.Key.Trim();
        var jb = JObject.Parse(item.Value.ToString());
        int page = jb["page"] == null ? 0 : int.Parse(jb["page"].ToString());
        int count = jb["count"] == null ? 10 : int.Parse(jb["count"].ToString());
        int query = jb["query"] == null ? 2 : int.Parse(jb["query"].ToString()); // Default output data and quantity
        int total = 0;

        jb.Remove("page"); jb.Remove("count"); jb.Remove("query");

        var htt = new JArray();
        foreach (var t in jb)
        {
            var datas = GetTableData(t.Key, page, count, query, t.Value.ToString(), null);
            if (query > 0)
                total = datas.Item2;

            foreach (var data in datas.Item1)
            {
                htt.Add(JToken.FromObject(data));
            }
        }

        if (!string.IsNullOrEmpty(nodeName))
            resultObj.Add(nodeName, htt);
        else
            resultObj.Add(key, htt);

        return total;
    }

    // generate sql
    private string ToSql(KeyValuePair<string, JToken> item)
    {
        var jb = JObject.Parse(item.Value.ToString());
        int page = jb["page"] == null ? 0 : int.Parse(jb["page"].ToString());
        int count = jb["count"] == null ? 10 : int.Parse(jb["count"].ToString());
        int query = jb["query"] == null ? 2 : int.Parse(jb["query"].ToString()); // Default output data and quantity

        jb.Remove("page"); jb.Remove("count"); jb.Remove("query");
        foreach (var t in jb)
        {
            return ToSql(t.Key, page, count, query, t.Value.ToString());
        }
        return string.Empty;
    }

    // Single table query
    private int QuerySingleList(JObject resultObj, KeyValuePair<string, JToken> item)
    {
        var key = item.Key.TrimEnd("[]");
        return QuerySingleList(resultObj, item, key);
    }

    /// <summary>
    /// Multiple list query
    /// </summary>
    /// <param name="resultObj"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private int QueryMoreList(JObject resultObj, KeyValuePair<string, JToken> item)
    {
        int total = 0;

        var jb = JObject.Parse(item.Value.ToString());
        var page = jb["page"] == null ? 0 : int.Parse(jb["page"].ToString());
        var count = jb["count"] == null ? 10 : int.Parse(jb["count"].ToString());
        var query = jb["query"] == null ? 2 : int.Parse(jb["query"].ToString()); // Default output data and quantity
        jb.Remove("page"); jb.Remove("count"); jb.Remove("query");
        var htt = new JArray();
        List<string> tables = new List<string>(), where = new List<string>();
        foreach (var t in jb)
        {
            tables.Add(t.Key); where.Add(t.Value.ToString());
        }
        if (tables.Count > 0)
        {
            string table = tables[0].TrimEnd("[]");
            var temp = GetTableData(table, page, count, query, where[0], null);
            if (query > 0)
                total = temp.Item2;

            // For related queries, first check the sub-table data, and then query the main table in a loop based on the foreign key.
            foreach (var dd in temp.Item1)
            {
                var zht = new JObject
                {
                    { table, JToken.FromObject(dd) }
                };
                for (int i = 1; i < tables.Count; i++) // Start looping from the second table
                {
                    string subtable = tables[i];
                    // There is a bug, and the [] branch is not supported yet.
                    //if (subtable.EndsWith("[]"))
                    //{
                    //   string tableName = subtable.TrimEnd("[]".ToCharArray());
                    //    var jbb = JObject.Parse(where[i]);
                    //    page = jbb["page"] == null ? 0 : int.Parse(jbb["page"].ToString());
                    //    count = jbb["count"] == null ? 0 : int.Parse(jbb["count"].ToString());

                    //    var lt = new JArray();
                    //    foreach (var d in GetTableData(tableName, page, count, query, item.Value[subtable].ToString(), zht).Item1)
                    //    {
                    //        lt.Add(JToken.FromObject(d));
                    //    }
                    //    zht.Add(tables[i], lt);
                    //}
                    //else
                    //{
                    var ddf = GetFirstData(subtable, where[i].ToString(), zht);
                    if (ddf != null)
                        zht.Add(subtable, JToken.FromObject(ddf));
                }
                htt.Add(zht);
            }
        }
        if (query != 1)
            resultObj.Add("[]", htt);

        // Pagination automatically adds the current page number and quantity
        if (page > 0 && count > 0)
        {
            resultObj.Add("page", page);
            resultObj.Add("count", count);
            resultObj.Add("max", (int)Math.Ceiling((decimal)total / count));
        }

        return total;
    }

    // Execution method
    private void ExecFunc(JObject resultObj, KeyValuePair<string, JToken> item)
    {
        var jb = JObject.Parse(item.Value.ToString());

        var dataJObj = new JObject();
        foreach (var f in jb)
        {
            var types = new List<Type>();
            var param = new List<object>();
            foreach (var va in JArray.Parse(f.Value.ToString()))
            {
                types.Add(typeof(object));
                param.Add(va);
            }
            dataJObj.Add(f.Key, JToken.FromObject(ExecFunc(f.Key, param.ToArray(), types.ToArray())));
        }
        resultObj.Add("func", dataJObj);
    }

    /// <summary>
    /// Construct query process
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="selectrole"></param>
    /// <param name="values"></param>
    /// <param name="dd"></param>
    /// <returns></returns>
    private ISugarQueryable<ExpandoObject> SugarQueryable(string subtable, string selectrole, JObject values, JObject dd)
    {
        IsTable(subtable);

        var tb = _db.Queryable(subtable, "tb");

        // select
        if (!values["@column"].IsNullOrEmpty())
        {
            ProcessColumn(subtable, selectrole, values, tb);
        }
        else
        {
            tb.Select(selectrole);
        }

        // first few lines
        ProcessLimit(values, tb);

        // where
        ProcessWhere(subtable, values, tb, dd);

        // sort
        ProcessOrder(subtable, values, tb);

        // Group
        PrccessGroup(subtable, values, tb);

        // Having
        ProcessHaving(values, tb);

        return tb;
    }

    // Process field renaming "@column":"toId:parentId", the corresponding SQL is toId AS parentId, change the query field toId to parentId and return
    private void ProcessColumn(string subtable, string selectrole, JObject values, ISugarQueryable<ExpandoObject> tb)
    {
        var str = new System.Text.StringBuilder(100);
        foreach (var item in values["@column"].ToString().Split(','))
        {
            var ziduan = item.Split(':');
            var colName = ziduan[0];
            var ma = new Regex(@"\((\w+)\)").Match(colName);
            // Process functions like max and min
            if (ma.Success && ma.Groups.Count > 1)
                colName = ma.Groups[1].Value;

            // Determine whether the list has permissions. Values ​​such as sum(1), sum(*), and Count(1) are directly valid.
            if (colName == "*" || int.TryParse(colName, out int colNumber) || (IsCol(subtable, colName) && _identitySvc.ColIsRole(colName, selectrole.Split(','))))
            {
                // Add quotation marks to the field name to prevent conflict with SQL keywords (backticks for mysql)
                string qm = "\"";
                if (tb.Context.CurrentConnectionConfig.DbType is SqlSugar.DbType.MySql)
                    qm = "`";

                if (ziduan.Length > 1)
                {
                    if (ziduan[1].Length > 20)
                        throw new Exception("The alias cannot exceed 20 characters");

                    str.Append(ziduan[0] + " as " + qm + ReplaceSQLChar(ziduan[1]) + qm + ",");
                }
                // Do not add `` to the function to solve the problem that sum(*), Count(1), etc. cannot be used.
                else if (ziduan[0].Contains('('))
                {
                    str.Append(ziduan[0] + ",");
                }
                else
                    str.Append(qm + ziduan[0] + qm + ",");
            }
        }
        if (string.IsNullOrEmpty(str.ToString()))
            throw new Exception($"The table {subtable} has no queryable fields!");

        tb.Select(str.ToString().TrimEnd(','));
    }

    /// <summary>
    /// Construct query conditions where
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="values"></param>
    /// <param name="tb"></param>
    /// <param name="dd"></param>
    private void ProcessWhere(string subtable, JObject values, ISugarQueryable<ExpandoObject> tb, JObject dd)
    {
        var conModels = new List<IConditionalModel>();
        if (!values["identity"].IsNullOrEmpty())
            conModels.Add(new ConditionalModel() { FieldName = values["identity"].ToString(), ConditionalType = ConditionalType.Equal, FieldValue = _identitySvc.GetUserIdentity() });

        foreach (var va in values)
        {
            string key = va.Key.Trim();
            string fieldValue = va.Value.ToString();
            if (key.StartsWith("@"))
            {
                continue;
            }
            if (key.EndsWith("$")) // fuzzy query
            {
                FuzzyQuery(subtable, conModels, va);
            }
            else if (key.EndsWith("{}")) // Logical operations
            {
                ConditionQuery(subtable, conModels, va);
            }
            else if (key.EndsWith("%")) // bwtweenquery
            {
                ConditionBetween(subtable, conModels, va, tb);
            }
            else if (key.EndsWith("@")) // Associated with the previous table
            {
                if (dd == null)
                    continue;

                var str = fieldValue.Split('/');
                var lastTableRecord = ((JObject)dd[str[^2]]);
                if (!lastTableRecord.ContainsKey(str[^1]))
                    throw new Exception($"The associated column cannot be found: {str}, please set it in {str[^2]}@column");

                var value = lastTableRecord[str[^1]].ToString();
                conModels.Add(new ConditionalModel() { FieldName = key.TrimEnd('@'), ConditionalType = ConditionalType.Equal, FieldValue = value });
            }
            else if (key.EndsWith("~")) // Not equal to (should be a regular match)
            {
                //conModels.Add(new ConditionalModel() { FieldName = key.TrimEnd('~'), ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue });
            }
            else if (IsCol(subtable, key.TrimEnd('!'))) // Other where conditions
            {
                ConditionEqual(subtable, conModels, va);
            }
        }
        if (conModels.Any())
            tb.Where(conModels);
    }

    // "@having":"function0(...)?value0;function1(...)?value1;function2(...)?value2..."，
    // SQL function conditions are generally used together with @group. Functions are generally declared in @column.
    private static void ProcessHaving(JObject values, ISugarQueryable<ExpandoObject> tb)
    {
        if (!values["@having"].IsNullOrEmpty())
        {
            var hw = new List<IConditionalModel>();
            var havingItems = new List<string>();
            if (values["@having"].HasValues)
            {
                havingItems = values["@having"].Select(p => p.ToString()).ToList();
            }
            else
            {
                havingItems.Add(values["@having"].ToString());
            }
            foreach (var item in havingItems)
            {
                var and = item.ToString();
                var model = new ConditionalModel();
                if (and.Contains(">="))
                {
                    model.FieldName = and.Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.GreaterThanOrEqual;
                    model.FieldValue = and.Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (and.Contains("<="))
                {
                    model.FieldName = and.Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.LessThanOrEqual;
                    model.FieldValue = and.Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (and.Contains('>'))
                {
                    model.FieldName = and.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.GreaterThan;
                    model.FieldValue = and.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (and.Contains('<'))
                {
                    model.FieldName = and.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.LessThan;
                    model.FieldValue = and.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (and.Contains("!="))
                {
                    model.FieldName = and.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.NoEqual;
                    model.FieldValue = and.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (and.Contains('='))
                {
                    model.FieldName = and.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    model.ConditionalType = ConditionalType.Equal;
                    model.FieldValue = and.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                hw.Add(model);
            }
            //var d = db.Context.Utilities.ConditionalModelToSql(hw);
            //tb.Having(d.Key, d.Value);
            tb.Having(string.Join(",", havingItems));
        }
    }

    // "@group":"column0,column1...", grouping method. If the Table's id is declared in @column, the id must also be declared in @group; in other cases, at least one condition must be met:
    // 1. The grouping key is declared in @column
    // 2.Table primary key is declared in @group
    private void PrccessGroup(string subtable, JObject values, ISugarQueryable<ExpandoObject> tb)
    {
        if (!values["@group"].IsNullOrEmpty())
        {
            var groupList = new List<GroupByModel>(); // Multi-library compatible writing method
            foreach (var col in values["@group"].ToString().Split(','))
            {
                if (IsCol(subtable, col))
                {
                    // str.Append(and + ",");
                    groupList.Add(new GroupByModel() { FieldName = col });
                }
            }
            if (groupList.Any())
                tb.GroupBy(groupList);
        }
    }

    // Processing sorting "@order":"name-,id" queries the User array sorted by name in descending order and id in the default order.
    private void ProcessOrder(string subtable, JObject values, ISugarQueryable<ExpandoObject> tb)
    {
        if (!values["@order"].IsNullOrEmpty())
        {
            var orderList = new List<OrderByModel>(); // Multi-library compatible writing method
            foreach (var item in values["@order"].ToString().Split(','))
            {
                string col = item.Replace("-", "").Replace("+", "").Replace(" desc", "").Replace(" asc", ""); // Add support for native sorting
                if (IsCol(subtable, col))
                {
                    orderList.Add(new OrderByModel()
                    {
                        FieldName = col,
                        OrderByType = item.EndsWith("-") || item.EndsWith(" desc") ? OrderByType.Desc : OrderByType.Asc
                    });
                }
            }

            if (orderList.Any())
                tb.OrderBy(orderList);
        }
    }

    /// <summary>
    /// Parameter "@count" (int) in the table: query the first few rows. Count and @count functions cannot be used at the same time.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="tb"></param>
    private static void ProcessLimit(JObject values, ISugarQueryable<ExpandoObject> tb)
    {
        if (!values["@count"].IsNullOrEmpty())
        {
            int c = values["@count"].ToObject<int>();
            tb.Take(c);
        }
    }

    // Conditional query "key{}":"Condition 0, Condition 1...", the condition is any SQL comparison expression string, non-Number types must use '' to include the value of the condition, such as 'a'
    // &, |, ! Logical operators, corresponding to AND, OR, NOT in database SQL.
    // Horizontal or vertical AND: The conditions within the values ​​of the same field default to | or connection, and the conditions in different fields default to & and connection.
    // ① & can be used for "key&{}":"condition", etc.
    // ② | Can be used for "key|{}":"condition", "key|{}":[], etc., generally can be omitted
    // ③ ! can be used alone, such as "key!":Object, or in conjunction with other functional symbols like &, |
    private void ConditionQuery(string subtable, List<IConditionalModel> conModels, KeyValuePair<string, JToken> va)
    {
        var vakey = va.Key.Trim();
        var field = vakey.TrimEnd("{}".ToCharArray());
        var columnName = field.TrimEnd(new char[] { '&', '|' });
        IsCol(subtable, columnName);
        var ddt = new List<KeyValuePair<WhereType, ConditionalModel>>();
        foreach (var and in va.Value.ToString().Split(','))
        {
            var model = new ConditionalModel
            {
                FieldName = columnName
            };

            if (and.StartsWith(">="))
            {
                model.ConditionalType = ConditionalType.GreaterThanOrEqual;
                model.FieldValue = and.TrimStart(">=".ToCharArray());
            }
            else if (and.StartsWith("<="))
            {
                model.ConditionalType = ConditionalType.LessThanOrEqual;
                model.FieldValue = and.TrimStart("<=".ToCharArray());
            }
            else if (and.StartsWith(">"))
            {
                model.ConditionalType = ConditionalType.GreaterThan;
                model.FieldValue = and.TrimStart('>');
            }
            else if (and.StartsWith("<"))
            {
                model.ConditionalType = ConditionalType.LessThan;
                model.FieldValue = and.TrimStart('<');
            }
            model.CSharpTypeName = FuncList.GetValueCSharpType(model.FieldValue);
            ddt.Add(new KeyValuePair<WhereType, ConditionalModel>(field.EndsWith("!") ? WhereType.Or : WhereType.And, model));
        }
        conModels.Add(new ConditionalCollections() { ConditionalList = ddt });
    }

    /// <summary>
    /// "key%":"start,end" => "key%":["start,end"], where start and end can only be one of Boolean, Number, String, such as "2017-01-01,2019-01-01", ["1,90000", "82001,100000"], which can be used for filtering in a continuous range
    /// Array form is currently not supported
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="conModels"></param>
    /// <param name="va"></param>
    /// <param name="tb"></param>
    private static void ConditionBetween(string subtable, List<IConditionalModel> conModels, KeyValuePair<string, JToken> va, ISugarQueryable<ExpandoObject> tb)
    {
        var vakey = va.Key.Trim();
        var field = vakey.TrimEnd("%".ToCharArray());
        var inValues = new List<string>();
        if (va.Value.HasValues)
        {
            foreach (var cm in va.Value)
            {
                inValues.Add(cm.ToString());
            }
        }
        else
        {
            inValues.Add(va.Value.ToString());
        }

        for (var i = 0; i < inValues.Count; i++)
        {
            var fileds = inValues[i].Split(',');
            if (fileds.Length == 2)
            {
                var type = FuncList.GetValueCSharpType(fileds[0]);
                ObjectFuncModel f = ObjectFuncModel.Create("between", field, $"{{{type}}}:{fileds[0]}", $"{{{type}}}:{fileds[1]}");
                tb.Where(f);
            }
        }
    }

    /// <summary>
    /// equal to, not equal to, in, not in
    /// </summary>
    /// <param name="subtable"></param>
    /// <param name="conModels"></param>
    /// <param name="va"></param>
    private void ConditionEqual(string subtable, List<IConditionalModel> conModels, KeyValuePair<string, JToken> va)
    {
        var key = va.Key;
        var fieldValue = va.Value.ToString();
        // in / not in
        if (va.Value is JArray)
        {
            conModels.Add(new ConditionalModel()
            {
                FieldName = key.TrimEnd('!'),
                ConditionalType = key.EndsWith("!") ? ConditionalType.NotIn : ConditionalType.In,
                FieldValue = va.Value.ToObject<string[]>().Aggregate((a, b) => a + "," + b)
            });
        }
        else
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                // is not null or ''
                if (key.EndsWith("!"))
                {
                    conModels.Add(new ConditionalModel() { FieldName = key.TrimEnd('!'), ConditionalType = ConditionalType.IsNot, FieldValue = null });
                    conModels.Add(new ConditionalModel() { FieldName = key.TrimEnd('!'), ConditionalType = ConditionalType.IsNot, FieldValue = "" });
                }
                //is null or ''
                else
                {
                    conModels.Add(new ConditionalModel() { FieldName = key.TrimEnd('!'), FieldValue = null });
                }
            }
            // = / !=
            else
            {
                conModels.Add(new ConditionalModel()
                {
                    FieldName = key.TrimEnd('!'),
                    ConditionalType = key.EndsWith("!") ? ConditionalType.NoEqual : ConditionalType.Equal,
                    FieldValue = fieldValue
                });
            }
        }
    }

    // Fuzzy search "key$":"SQL search expression" => "key$":["SQL search expression"], any SQL search expression string, such as %key% (including key), key% (starting with key), %k%e%y% (including letters k, e, y), etc., % represents any character
    private void FuzzyQuery(string subtable, List<IConditionalModel> conModels, KeyValuePair<string, JToken> va)
    {
        var vakey = va.Key.Trim();
        var fieldValue = va.Value.ToString();
        var conditionalType = ConditionalType.Like;
        if (IsCol(subtable, vakey.TrimEnd('$')))
        {
            // Supports three like queries
            if (fieldValue.StartsWith("%") && fieldValue.EndsWith("%"))
            {
                conditionalType = ConditionalType.Like;
            }
            else if (fieldValue.StartsWith("%"))
            {
                conditionalType = ConditionalType.LikeRight;
            }
            else if (fieldValue.EndsWith("%"))
            {
                conditionalType = ConditionalType.LikeLeft;
            }
            conModels.Add(new ConditionalModel() { FieldName = vakey.TrimEnd('$'), ConditionalType = conditionalType, FieldValue = fieldValue.TrimEnd("%".ToArray()).TrimStart("%".ToArray()) });
        }
    }

    // Handle sql injection
    private string ReplaceSQLChar(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        str = str.Replace("'", "");
        str = str.Replace(";", "");
        str = str.Replace(",", "");
        str = str.Replace("?", "");
        str = str.Replace("<", "");
        str = str.Replace(">", "");
        str = str.Replace("(", "");
        str = str.Replace(")", "");
        str = str.Replace("@", "");
        str = str.Replace("=", "");
        str = str.Replace("+", "");
        str = str.Replace("*", "");
        str = str.Replace("&", "");
        str = str.Replace("#", "");
        str = str.Replace("%", "");
        str = str.Replace("$", "");
        str = str.Replace("\"", "");

        // Delete database-related words
        str = Regex.Replace(str, "delete from", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "drop table", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "truncate", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "xp_cmdshell", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "exec master", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "net localgroup administrators", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "net user", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "-", "", RegexOptions.IgnoreCase);
        str = Regex.Replace(str, "truncate", "", RegexOptions.IgnoreCase);
        return str;
    }

    /// <summary>
    /// Single insert
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="cols"></param>
    /// <param name="role"></param>
    /// <returns>(various types of) ids</returns>
    public object InsertSingle(string tableName, JObject cols, APIJSON_Role role = null)
    {
        role ??= _identitySvc.GetRole();
        var dt = new Dictionary<string, object>();

        foreach (var f in cols) // Iterate over fields
        {
            if (// f.Key.ToLower() != "id" && //Do you have to pass the id?
                IsCol(tableName, f.Key) &&
                (role.Insert.Column.Contains("*") || role.Insert.Column.Contains(f.Key, StringComparer.CurrentCultureIgnoreCase)))
                dt.Add(f.Key, FuncList.TransJObjectToSugarPara(f.Value));
        }
        // If the ID is not passed externally, the backend will generate it or use the database default value. If there is no ID, an error will occur.
        object id;
        if (!dt.ContainsKey("id"))
        {
            id = YitIdHelper.NextId();// The method to generate your own ID can be passed in from outside.
            dt.Add("id", id);
        }
        else
        {
            id = dt["id"];
        }
        _db.Insertable(dt).AS(tableName).ExecuteCommand();// Return snowflakes or auto-increment according to the primary key type setting, and currently return the number of items

        return id;
    }

    /// <summary>
    /// Create udpate sql for daily records
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="record"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public int UpdateSingleRecord(string tableName, JObject record, APIJSON_Role role = null)
    {
        role ??= _identitySvc.GetRole();
        if (!record.ContainsKey("id"))
            throw Oops.Bah("Primary key id not passed");

        var dt = new Dictionary<string, object>();
        var sb = new StringBuilder(100);
        object id = null;
        foreach (var f in record)// Iterate through each field
        {
            if (f.Key.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                if (f.Value is JArray)
                {
                    sb.Append($"{f.Key} in (@{f.Key})");
                    id = FuncList.TransJArrayToSugarPara(f.Value);
                }
                else
                {
                    sb.Append($"{f.Key}=@{f.Key}");
                    id = FuncList.TransJObjectToSugarPara(f.Value);
                }
            }
            else if (IsCol(tableName, f.Key) && (role.Update.Column.Contains("*") || role.Update.Column.Contains(f.Key, StringComparer.CurrentCultureIgnoreCase)))
            {
                dt.Add(f.Key, FuncList.TransJObjectToSugarPara(f.Value));
            }
        }
        string whereSql = sb.ToString();
        int count = _db.Updateable(dt).AS(tableName).Where(whereSql, new { id }).ExecuteCommand();
        return count;
    }

    /// <summary>
    /// Update a single table and support multiple records in the same table
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="records"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public int UpdateSingleTable(string tableName, JToken records, APIJSON_Role role = null)
    {
        role ??= _identitySvc.GetRole();
        int count = 0;
        if (records is JArray)
        {
            foreach (var record in records.ToObject<JObject[]>())
            {
                count += UpdateSingleRecord(tableName, record, role);
            }
        }
        else
        {
            count = UpdateSingleRecord(tableName, records.ToObject<JObject>(), role);
        }
        return count;
    }
}