// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using IPTools.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Admin.NET.Core;

/// <summary>
/// General tools
/// </summary>
public static class CommonUtil
{
    private static readonly SysCacheService SysCacheService = App.GetRequiredService<SysCacheService>();
    private static readonly SysFileService SysFileService = App.GetRequiredService<SysFileService>();
    private static readonly SqlSugarRepository<SysDictData> SysDictDataRep = App.GetRequiredService<SqlSugarRepository<SysDictData>>();

    /// <summary>
    /// Get a fixed integer hash value from a string
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startNumber"></param>
    /// <returns></returns>
    public static long GetFixedHashCode(string str, long startNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(str)) return 0;
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;
            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1) break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }
            return startNumber + Math.Abs(hash1 + (hash2 * 1566083941));
        }
    }

    /// <summary>
    /// Generate percentage
    /// </summary>
    /// <param name="passCount"></param>
    /// <param name="allCount"></param>
    /// <returns></returns>
    public static string ExecPercent(decimal passCount, decimal allCount)
    {
        string res = "";
        if (allCount > 0)
        {
            var value = (double)Math.Round(passCount / allCount * 100, 1);
            if (value < 0)
                res = Math.Round(value + 5 / Math.Pow(10, 0 + 1), 0, MidpointRounding.AwayFromZero).ToString();
            else
                res = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
        }
        if (res == "") res = "0";
        return res + "%";
    }

    /// <summary>
    /// Get service address
    /// </summary>
    /// <returns></returns>
    public static string GetLocalhost()
    {
        string result = $"{App.HttpContext.Request.Scheme}://{App.HttpContext.Request.Host.Value}";

        // Proxy mode: get the real local address
        // X-Original-Host=Original request
        // X-Forwarded-Server=Forwarded from where
        if (App.HttpContext.Request.Headers.ContainsKey("Origin")) // Configure it as a complete path (without "/" at the end), such as https://www.abc.com
            result = $"{App.HttpContext.Request.Headers["Origin"]}";
        else if (App.HttpContext.Request.Headers.ContainsKey("X-Original")) // Configure it as a complete path (without "/" at the end), such as https://www.abc.com
            result = $"{App.HttpContext.Request.Headers["X-Original"]}";
        else if (App.HttpContext.Request.Headers.ContainsKey("X-Original-Host"))
            result = $"{App.HttpContext.Request.Scheme}://{App.HttpContext.Request.Headers["X-Original-Host"]}";
        return result + (string.IsNullOrWhiteSpace(App.Settings.VirtualPath) ? "" : App.Settings.VirtualPath);
    }

    /// <summary>
    /// Object serialization XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeObjectToXml<T>(T obj)
    {
        if (obj == null) return string.Empty;

        var xs = new XmlSerializer(obj.GetType());
        var stream = new MemoryStream();
        var setting = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false), // Does not include BOM
            Indent = true // Set formatting indentation
        };
        using (var writer = XmlWriter.Create(stream, setting))
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // Remove default namespace
            xs.Serialize(writer, obj, ns);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// String to XML format
    /// </summary>
    /// <param name="xmlStr"></param>
    /// <returns></returns>
    public static XElement SerializeStringToXml(string xmlStr)
    {
        try
        {
            return XElement.Parse(xmlStr);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Export template Excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelTemplate<T>(string fileName = null) where T : class, new()
    {
        IImporter importer = new ExcelImporter();
        var res = await importer.GenerateTemplateBytes<T>();

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// Export data to excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<T>(ICollection<T> data, string fileName = null) where T : class, new()
    {
        var export = new ExcelExporter();
        var res = await export.ExportAsByteArray<T>(data);

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// Export data to excel, including dictionary conversion
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<TSource, TTarget>(ISugarQueryable<TSource> query, Func<TSource, TTarget, TTarget> action = null)
        where TSource : class, new() where TTarget : class, new()
    {
        var propMappings = GetExportPropertMap<TSource, TTarget>();
        var data = query.ToList();
        //Copy values ​​for the same attribute, convert dictionary values
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in propMappings)
            {
                var targetProp = dict.Value.Item3;
                if (targetProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                    {
                        continue;
                    }

                    var map = dict.Value.Item1;
                    if (map != null && map.TryGetValue(sourceVal, out string newVal1))
                    {
                        targetProp.SetValue(newData, newVal1);
                    }
                    else
                    {
                        if (targetProp.PropertyType.FullName == propertyInfo.PropertyType.FullName)
                        {
                            targetProp.SetValue(newData, sourceVal);
                        }
                        else
                        {
                            var newVal = sourceVal.ToString().ParseTo(targetProp.PropertyType);
                            targetProp.SetValue(newData, newVal);
                        }
                    }
                }
                if (action != null)
                {
                    newData = action(item, newData);
                }
            }
            result.Add(newData);
        }
        var export = new ExcelExporter();
        var res = await export.ExportAsByteArray(result);

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = typeof(TTarget).Name + ".xlsx" };
    }

    /// <summary>
    /// Import dataExcel
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file) where T : class, new()
    {
        IImporter importer = new ExcelImporter();
        var res = await importer.Import<T>(file.OpenReadStream());
        var message = string.Empty;

        if (!res.HasError) return res.Data;

        if (res.Exception != null)
            message += $"\r\n{res.Exception.Message}";
        foreach (DataRowErrorInfo drErrorInfo in res.RowErrors)
        {
            int rowNum = drErrorInfo.RowIndex;
            foreach (var item in drErrorInfo.FieldErrors)
                message += $"{item.Key}: {item.Value} (Row {drErrorInfo.RowIndex} of the file)";
        }
        message += "Field missing:" + string.Join("，", res.TemplateErrors.Select(m => m.RequireColumnName).ToList());
        throw Oops.Oh("Import Exception:" + message);
    }

    /// <summary>
    /// Importing Excel data and labeling errors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file"></param>
    /// <param name="importResultCallback"></param>
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        IImporter importer = new ExcelImporter();
        var resultStream = new MemoryStream();
        var res = await importer.Import<T>(file.OpenReadStream(), resultStream, importResultCallback);
        resultStream.Seek(0, SeekOrigin.Begin);
        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;

        SysCacheService.Remove(CacheConst.KeyExcelTemp + userId);
        SysCacheService.Set(CacheConst.KeyExcelTemp + userId, resultStream, TimeSpan.FromMinutes(5));

        var message = string.Empty;
        if (!res.HasError) return res.Data;

        if (res.Exception != null)
            message += $"\r\n{res.Exception.Message}";
        foreach (DataRowErrorInfo drErrorInfo in res.RowErrors)
        {
            message = drErrorInfo.FieldErrors.Aggregate(message, (current, item) => current + $"{item.Key}: {item.Value} (Row {drErrorInfo.RowIndex} of the file)");
        }
        if (res.TemplateErrors.Count > 0)
            message += "Field missing:" + string.Join("，", res.TemplateErrors.Select(m => m.RequireColumnName).ToList());

        if (message.Length > 200)
            message = message.Substring(0, 200) + "...\r\nIf there are too many errors, it is recommended to download the error marker file to view detailed error information and re-import it.";
        throw Oops.Oh("Import error:" + message);
    }

    /// <summary>
    /// Import dataExcel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<List<T>> ImportExcelDataAsync<T>([Required] IFormFile file) where T : class, new()
    {
        var newFile = await SysFileService.UploadFile(new UploadFileInput { File = file });

        await using var fileStream = await SysFileService.GetFileStream(newFile);

        IImporter importer = new ExcelImporter();
        var res = await importer.Import<T>(fileStream);

        // Delete files
        _ = SysFileService.DeleteFile(new BaseIdInput { Id = newFile.Id });

        if (res == null)
            throw Oops.Oh("Import data is empty");
        if (res.Exception != null)
            throw Oops.Oh("Import Exception:" + res.Exception);
        if (res.TemplateErrors?.Count > 0)
            throw Oops.Oh("Template Exception:" + res.TemplateErrors.Select(x => $"[{x.RequireColumnName}]{x.Message}").Join("\n"));

        return res.Data.ToList();
    }

    // Example: List<Dm_ApplyDemo> ls = CommonUtil.ParseList<Dm_ApplyDemoInport, Dm_ApplyDemo>(importResult.Data);
    /// <summary>
    /// Object conversion including dictionary conversion
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="data"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static List<TTarget> ParseList<TSource, TTarget>(IEnumerable<TSource> data, Func<TSource, TTarget, TTarget> action = null) where TTarget : new()
    {
        var propMappings = GetImportPropertMap<TSource, TTarget>();
        // Copy values ​​for the same attribute, convert dictionary values
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in propMappings)
            {
                var targeProp = dict.Value.Item3;
                if (targeProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                        continue;

                    var map = dict.Value.Item1;
                    if (map != null && map.ContainsKey(sourceVal.ToString()))
                    {
                        var newVal = map[sourceVal.ToString()];
                        targeProp.SetValue(newData, newVal);
                    }
                    else
                    {
                        if (targeProp.PropertyType.FullName == propertyInfo.PropertyType.FullName)
                        {
                            targeProp.SetValue(newData, sourceVal);
                        }
                        else
                        {
                            var newVal = sourceVal.ToString().ParseTo(targeProp.PropertyType);
                            targeProp.SetValue(newData, newVal);
                        }
                    }
                }
            }
            if (action != null)
                newData = action(item, newData);

            if (newData != null)
                result.Add(newData);
        }
        return result;
    }

    /// <summary>
    /// Get import attribute mapping
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>Organize the attribute names, dictionary data, original attribute information, and target attribute information of the imported objects </returns>
    private static Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>> GetImportPropertMap<TSource, TTarget>() where TTarget : new()
    {
        // Organize the attribute names of imported objects, <dictionary data, original attribute information, target attribute information>
        var propMappings = new Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>>();

        var dictService = App.GetRequiredService<SqlSugarRepository<SysDictData>>();
        var tSourceProps = typeof(TSource).GetProperties().ToList();
        var tTargetProps = typeof(TTarget).GetProperties().ToDictionary(u => u.Name);
        foreach (var propertyInfo in tSourceProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = tTargetProps[attrs.TargetPropName];
                var mappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((u, a) =>
                    new JoinQueryInfos(JoinType.Inner, u.Id == a.DictTypeId))
                    .Where(u => u.Code == attrs.TypeCode)
                    .Where((u, a) => u.Status == StatusEnum.Enable && a.Status == StatusEnum.Enable)
                    .Select((u, a) => new
                    {
                        Label = a.Label,
                        Value = a.Value
                    }).ToList()
                    .ToDictionary(u => u.Label, u => u.Value.ParseTo(targetProp.PropertyType));
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(mappingValues, propertyInfo, targetProp));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(
                    null, propertyInfo, tTargetProps.ContainsKey(propertyInfo.Name) ? tTargetProps[propertyInfo.Name] : null));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// Get export attribute mapping
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>Organize the attribute names, dictionary data, original attribute information, and target attribute information of the imported objects </returns>
    private static Dictionary<string, Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>> GetExportPropertMap<TSource, TTarget>() where TTarget : new()
    {
        // Organize the attribute names of imported objects, <dictionary data, original attribute information, target attribute information>
        var propMappings = new Dictionary<string, Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>>();

        var targetProps = typeof(TTarget).GetProperties().ToList();
        var sourceProps = typeof(TSource).GetProperties().ToDictionary(u => u.Name);
        foreach (var propertyInfo in targetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = sourceProps[attrs.TargetPropName];
                var mappingValues = SysDictDataRep.Context.Queryable<SysDictType, SysDictData>((u, a) =>
                    new JoinQueryInfos(JoinType.Inner, u.Id == a.DictTypeId))
                    .Where(u => u.Code == attrs.TypeCode)
                    .Where((u, a) => u.Status == StatusEnum.Enable && a.Status == StatusEnum.Enable)
                    .Select((u, a) => new
                    {
                        a.Label,
                        a.Value
                    }).ToList()
                    .ToDictionary(u => u.Value.ParseTo(targetProp.PropertyType), u => u.Label);
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(mappingValues, targetProp, propertyInfo));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(
                    null, sourceProps.TryGetValue(propertyInfo.Name, out PropertyInfo prop) ? prop : null, propertyInfo));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// Get attribute mapping
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>Organize the attribute names, dictionary data, original attribute information, and target attribute information of the imported objects </returns>
    private static Dictionary<string, Tuple<string, string>> GetExportDictMap<TTarget>() where TTarget : new()
    {
        // Organize the attribute names, target attribute names, and dictionary codes of imported objects
        var propMappings = new Dictionary<string, Tuple<string, string>>();
        var tTargetProps = typeof(TTarget).GetProperties();
        foreach (var propertyInfo in tTargetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                propMappings.Add(propertyInfo.Name, new Tuple<string, string>(attrs.TargetPropName, attrs.TypeCode));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// Resolve IP address
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static (string ipLocation, double? longitude, double? latitude) GetIpAddress(string ip)
    {
        try
        {
            var ipInfo = IpTool.SearchWithI18N(ip); // International query, default Chinese, Chinese zh-CN, English en
            var addressList = new List<string>() { ipInfo.Country, ipInfo.Province, ipInfo.City, ipInfo.NetworkOperator };
            return (string.Join(" ", addressList.Where(u => u != "0" && !string.IsNullOrWhiteSpace(u)).ToList()), ipInfo.Longitude, ipInfo.Latitude); // Remove zeros and spaces and connect with spaces
        }
        catch
        {
            // No processing
        }
        return ("unknown", 0, 0);
    }

    /// <summary>
    /// Get client device information (operating system + browser)
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static string GetClientDeviceInfo(string userAgent)
    {
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "Crawler";
                return $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}" +
                    $"|{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}";
            }
        }
        catch
        { }
        return "unknown";
    }
}