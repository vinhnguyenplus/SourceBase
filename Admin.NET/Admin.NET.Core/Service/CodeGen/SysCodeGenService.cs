// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.IO.Compression;

namespace Admin.NET.Core.Service;

/// <summary>
/// System Code Generator Service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 270)]
public class SysCodeGenService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;

    private readonly SysCodeGenConfigService _codeGenConfigService;
    private readonly DbConnectionOptions _dbConnectionOptions;
    private readonly CodeGenOptions _codeGenOptions;
    private readonly SysMenuService _sysMenuService;
    private readonly IViewEngine _viewEngine;
    private readonly UserManager _userManager;

    public SysCodeGenService(ISqlSugarClient db,
        IOptions<DbConnectionOptions> dbConnectionOptions,
        SysCodeGenConfigService codeGenConfigService,
        IOptions<CodeGenOptions> codeGenOptions,
        SysMenuService sysMenuService,
        UserManager userManager,
        IViewEngine viewEngine)
    {
        _db = db;
        _viewEngine = viewEngine;
        _userManager = userManager;
        _sysMenuService = sysMenuService;
        _codeGenOptions = codeGenOptions.Value;
        _codeGenConfigService = codeGenConfigService;
        _dbConnectionOptions = dbConnectionOptions.Value;
    }

    /// <summary>
    /// Get code generation paginated list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get code to generate paginated list")]
    public async Task<SqlSugarPagedList<SysCodeGen>> Page(CodeGenInput input)
    {
        return await _db.Queryable<SysCodeGen>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.TableName), u => u.TableName.Contains(input.TableName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.BusName), u => u.BusName.Contains(input.BusName.Trim()))
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Add code generation 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add code generation")]
    public async Task AddCodeGen(AddCodeGenInput input)
    {
        var isExist = await _db.Queryable<SysCodeGen>().Where(u => u.TableName == input.TableName).AnyAsync();
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1400);

        if (input.TableUniqueList?.Count > 0) input.TableUniqueConfig = JSON.Serialize(input.TableUniqueList);

        var codeGen = input.Adapt<SysCodeGen>();
        var newCodeGen = await _db.Insertable(codeGen).ExecuteReturnEntityAsync();

        // Add configuration table
        _codeGenConfigService.AddList(GetColumnList(input), newCodeGen);
    }

    /// <summary>
    /// Update code generation 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Updatecode generation")]
    public async Task UpdateCodeGen(UpdateCodeGenInput input)
    {
        var isExist = await _db.Queryable<SysCodeGen>().AnyAsync(u => u.TableName == input.TableName && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1400);

        var oldRecord = await _db.Queryable<SysCodeGen>().FirstAsync(u => u.Id == input.Id);
        try
        {
            // Open transaction
            _db.AsTenant().BeginTran();
            if (input.GenerateMenu)
            {
                var oldTitle = $"{oldRecord.BusName} Management";
                var newTitle = $"{input.BusName} Management";
                var updateObj = await _db.Queryable<SysMenu>().FirstAsync(u => u.Title == oldTitle);
                if (updateObj != null)
                {
                    updateObj.Title = newTitle;
                    var result = _db.Updateable(updateObj).UpdateColumns(it => new { it.Title }).ExecuteCommand();
                    _sysMenuService.DeleteMenuCache();
                }
            }
            if (input.TableUniqueList?.Count > 0) input.TableUniqueConfig = JSON.Serialize(input.TableUniqueList);
            var codeGen = input.Adapt<SysCodeGen>();
            await _db.Updateable(codeGen).ExecuteCommandAsync();

            // Only update the configuration table when the data table name changes
            //if (oldRecord.TableName != input.TableName)
            //{
            await _codeGenConfigService.DeleteCodeGenConfig(codeGen.Id);
            _codeGenConfigService.AddList(GetColumnList(input.Adapt<AddCodeGenInput>()), codeGen);
            //}
            _db.AsTenant().CommitTran();
        }
        catch (Exception ex)
        {
            _db.AsTenant().RollbackTran();
            throw Oops.Oh(ex);
        }
    }

    /// <summary>
    /// Synchronization code field (preserve historical function type) 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "SyncField"), HttpPost]
    [DisplayName("Sync code field")]
    public async Task SyncCodeFieldGen(UpdateCodeGenInput input)
    {
        var isExist = await _db.Queryable<SysCodeGen>().AnyAsync(u => u.TableName == input.TableName && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1400);
        try
        {
            // Open transaction
            _db.AsTenant().BeginTran();
            await _codeGenConfigService.UpdateList(GetColumnList(input.Adapt<AddCodeGenInput>()), input.Id);
            _db.AsTenant().CommitTran();
        }
        catch (Exception ex)
        {
            _db.AsTenant().RollbackTran();
            throw Oops.Oh(ex);
        }
    }

    /// <summary>
    /// Remove code generation 🔖
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Remove code generation")]
    public async Task DeleteCodeGen(List<DeleteCodeGenInput> inputs)
    {
        if (inputs == null || inputs.Count < 1) return;

        var codeGenConfigTaskList = new List<Task>();
        inputs.ForEach(u =>
        {
            _db.Deleteable<SysCodeGen>().In(u.Id).ExecuteCommand();

            // Delete configuration table
            codeGenConfigTaskList.Add(_codeGenConfigService.DeleteCodeGenConfig(u.Id));
        });
        await Task.WhenAll(codeGenConfigTaskList);
    }

    /// <summary>
    /// Get code generation details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Obtaincode generationDetails")]
    public async Task<SysCodeGen> GetDetail([FromQuery] QueryCodeGenInput input)
    {
        return await _db.Queryable<SysCodeGen>().SingleAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Get database library collection 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get database collection")]
    public async Task<List<DatabaseOutput>> GetDatabaseList()
    {
        var dbConfigs = _dbConnectionOptions.ConnectionConfigs;
        return await Task.FromResult(dbConfigs.Adapt<List<DatabaseOutput>>());
    }

    /// <summary>
    /// Get the database table (entity) collection 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the database table (entity) collection")]
    public async Task<List<TableOutput>> GetTableList(string configId = SqlSugarConst.MainConfigId)
    {
        var provider = _db.AsTenant().GetConnectionScope(configId);
        var dbTableInfos = provider.DbMaintenance.GetTableInfoList(false); // The cache cannot be used, otherwise the library will not work
        var config = _dbConnectionOptions.ConnectionConfigs.FirstOrDefault(u => configId.Equals(u.ConfigId));

        // var dbTableNames = dbTableInfos.Select(u => u.Name.ToLower()).ToList();
        IEnumerable<EntityInfo> entityInfos = await GetEntityInfos(configId);

        var tableOutputList = new List<TableOutput>();
        foreach (var item in entityInfos)
        {
            var tbConfigId = item.Type.GetCustomAttribute<TenantAttribute>()?.configId as string ?? SqlSugarConst.MainConfigId;
            if (item.Type.IsDefined(typeof(LogTableAttribute))) tbConfigId = SqlSugarConst.LogConfigId;
            if (tbConfigId != configId) continue;

            var table = dbTableInfos.FirstOrDefault(u => string.Equals(u.Name, (config!.DbSettings.EnableUnderLine ? item.DbTableName.ToUnderLine() : item.DbTableName), StringComparison.CurrentCultureIgnoreCase));
            if (table == null) continue;
            tableOutputList.Add(new TableOutput
            {
                ConfigId = configId,
                EntityName = item.EntityName,
                TableName = table.Name,
                TableComment = item.TableDescription
            });
        }
        return tableOutputList;
    }

    /// <summary>
    /// Get the column set based on the table name 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the column set based on the table name")]
    public List<ColumnOuput> GetColumnListByTableName([Required] string tableName, string configId = SqlSugarConst.MainConfigId)
    {
        // Qie library---used for multi-library code generation
        var provider = _db.AsTenant().GetConnectionScope(configId);
        var config = _dbConnectionOptions.ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == configId) ?? throw Oops.Oh(ErrorCodeEnum.D1401);
        if (config.DbSettings.EnableUnderLine) tableName = tableName.ToUnderLine();
        // Get entity type attributes
        var entityType = provider.DbMaintenance.GetTableInfoList(false).FirstOrDefault(u => u.Name == tableName);
        if (entityType == null) return null;
        var properties = GetEntityInfos(configId).Result.First(e => e.DbTableName.EndsWithIgnoreCase(tableName)).Type.GetProperties()
            .Where(e => e.GetCustomAttribute<SugarColumn>()?.IsIgnore == false).Select(e => new
            {
                PropertyName = e.Name,
                ColumnComment = e.GetCustomAttribute<SugarColumn>()?.ColumnDescription,
                ColumnName = e.GetCustomAttribute<SugarColumn>()?.ColumnName ?? e.Name
            }).ToList();
        // Get all entity type properties in order of original type (excluding navigation properties, null will be returned)
        var columnList = provider.DbMaintenance.GetColumnInfosByTableName(tableName).Select(u => new ColumnOuput
        {
            ColumnName = config!.DbSettings.EnableUnderLine ? u.DbColumnName.ToUnderLine() : u.DbColumnName,
            ColumnKey = u.IsPrimarykey.ToString(),
            DataType = u.DataType.ToString(),
            NetType = CodeGenUtil.ConvertDataType(u, provider.CurrentConnectionConfig.DbType),
            ColumnComment = u.ColumnDescription
        }).ToList();
        foreach (var column in columnList)
        {
            // ToLowerInvariant converts the field name to lowercase and then compares it to avoid inability to match due to inconsistency in uppercase and lowercase (pgsql will default to all lowercase when creating a table, and our entities are uppercase, so they will not match)
            var property = properties.FirstOrDefault(e => (config!.DbSettings.EnableUnderLine ? e.ColumnName.ToUnderLine() : e.ColumnName).ToLowerInvariant() == column.ColumnName.ToLowerInvariant());
            column.ColumnComment ??= property?.ColumnComment;
            column.PropertyName = property?.PropertyName;
        }
        return columnList;
    }

    /// <summary>
    /// Get the data table column (entity attribute) collection
    /// </summary>
    /// <returns></returns>
    private List<ColumnOuput> GetColumnList([FromQuery] AddCodeGenInput input)
    {
        var entityType = GetEntityInfos(input.ConfigId).GetAwaiter().GetResult().FirstOrDefault(u => u.EntityName == input.TableName);
        if (entityType == null) return null;

        var config = _dbConnectionOptions.ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == input.ConfigId);
        var dbTableName = config!.DbSettings.EnableUnderLine ? entityType.DbTableName.ToUnderLine() : entityType.DbTableName;

        // Qie library---used for multi-library code generation
        var provider = _db.AsTenant().GetConnectionScope(!string.IsNullOrEmpty(input.ConfigId) ? input.ConfigId : SqlSugarConst.MainConfigId);

        var entityBasePropertyNames = CodeGenUtil.GetPropertyInfoArray(typeof(EntityBaseTenant))?.Select(p => p.Name).ToArray();
        var columnInfos = provider.DbMaintenance.GetColumnInfosByTableName(dbTableName, false);
        var result = columnInfos.Select(u => new ColumnOuput
        {
            // The column name after the underline needs to be converted back (it will not be converted for the time being)
            //ColumnName = config.DbSettings.EnableUnderLine ? CodeGenUtil.CamelColumnName(u.DbColumnName, entityBasePropertyNames) : u.DbColumnName,
            ColumnName = u.DbColumnName,
            ColumnLength = u.Length,
            IsPrimarykey = u.IsPrimarykey,
            IsNullable = u.IsNullable,
            ColumnKey = u.IsPrimarykey.ToString(),
            NetType = CodeGenUtil.ConvertDataType(u, provider.CurrentConnectionConfig.DbType),
            DataType = CodeGenUtil.ConvertDataType(u, provider.CurrentConnectionConfig.DbType),
            ColumnComment = string.IsNullOrWhiteSpace(u.ColumnDescription) ? u.DbColumnName : u.ColumnDescription,
            DefaultValue = u.DefaultValue,
        }).ToList();

        // Get the attribute information of the entity and assign it to the PropertyName attribute (CodeFirst mode should use PropertyName as the actual name)
        var entityProperties = entityType.Type.GetProperties();

        for (int i = result.Count - 1; i >= 0; i--)
        {
            var columnOutput = result[i];
            // First look for the custom field name, and if you can't find it, look for the automatically generated field name (and filter out attributes without SugarColumn)
            var propertyInfo = entityProperties.FirstOrDefault(u => string.Equals((u.GetCustomAttribute<SugarColumn>()?.ColumnName ?? ""), columnOutput.ColumnName, StringComparison.CurrentCultureIgnoreCase)) ??
                entityProperties.FirstOrDefault(u => u.GetCustomAttribute<SugarColumn>() != null && u.Name.ToLower() == (config.DbSettings.EnableUnderLine
                ? CodeGenUtil.CamelColumnName(columnOutput.ColumnName, entityBasePropertyNames).ToLower()
                : columnOutput.ColumnName.ToLower()));
            if (propertyInfo != null)
            {
                columnOutput.PropertyName = propertyInfo.Name;
                columnOutput.ColumnComment = propertyInfo.GetCustomAttribute<SugarColumn>()!.ColumnDescription;
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                if (propertyInfo.PropertyType.IsEnum || (propertyType?.IsEnum ?? false))
                {
                    columnOutput.DictTypeCode = (propertyType ?? propertyInfo.PropertyType).Name;
                }
                else
                {
                    var dict = propertyInfo.GetCustomAttribute<DictAttribute>();
                    if (dict != null) columnOutput.DictTypeCode = dict.DictTypeCode;
                }
            }
            else
            {
                result.RemoveAt(i); // Remove fields that do not have this property defined
            }
        }
        return result;
    }

    /// <summary>
    /// Get library table information
    /// </summary>
    /// <returns></returns>
    private async Task<IEnumerable<EntityInfo>> GetEntityInfos(string configId)
    {
        var config = _dbConnectionOptions.ConnectionConfigs.FirstOrDefault(u => u.ConfigId.ToString() == configId) ?? throw Oops.Oh(ErrorCodeEnum.D1401);
        var entityInfos = new List<EntityInfo>();

        var type = typeof(SugarTable);
        var types = new List<Type>();
        if (_codeGenOptions.EntityAssemblyNames != null)
        {
            types = App.EffectiveTypes.Where(c => c.IsClass)
                .Where(c => _codeGenOptions.EntityAssemblyNames.Contains(c.Assembly.GetName().Name) || _codeGenOptions.EntityAssemblyNames.Any(name => c.Assembly.GetName().Name!.Contains(name)))
                .ToList();
        }

        Type[] cosType = types.Where(o => IsMyAttribute(Attribute.GetCustomAttributes(o, true))).ToArray();

        foreach (var ct in cosType)
        {
            var sugarAttribute = ct.GetCustomAttributes(type, true).FirstOrDefault();

            var description = "";
            var des = ct.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (des.Length > 0) description = ((DescriptionAttribute)des[0]).Description;

            var dbTableName = sugarAttribute == null || string.IsNullOrWhiteSpace(((SugarTable)sugarAttribute).TableName) ? ct.Name : ((SugarTable)sugarAttribute).TableName;
            if (config.DbSettings.EnableUnderLine) dbTableName = dbTableName.ToUnderLine();

            entityInfos.Add(new EntityInfo
            {
                EntityName = ct.Name,
                DbTableName = dbTableName,
                TableDescription = sugarAttribute == null ? description : ((SugarTable)sugarAttribute).TableDescription,
                Type = ct
            });
        }
        return await Task.FromResult(entityInfos);

        bool IsMyAttribute(Attribute[] o) => o.Any(a => a.GetType() == type);
    }

    /// <summary>
    /// Get program save location 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("ObtainProgramsavePosition")]
    public List<string> GetApplicationNamespaces()
    {
        return _codeGenOptions.BackendApplicationNamespaces;
    }

    /// <summary>
    /// Code generation to local 🔖
    /// </summary>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Generate code to local")]
    public async Task<dynamic> RunLocal(SysCodeGen input)
    {
        if (string.IsNullOrEmpty(input.GenerateType))
            input.GenerateType = "200";

        // First delete the menu list generated by the table
        List<string> targetPathList;
        var zipPath = Path.Combine(App.WebHostEnvironment.WebRootPath, "CodeGen", input.TableName!);
        if (input.GenerateType.StartsWith('1'))
        {
            targetPathList = GetZipPathList(input);
            if (Directory.Exists(zipPath)) Directory.Delete(zipPath, true);
        }
        else
            targetPathList = GetTargetPathList(input);

        var (tableFieldList, result) = await RenderTemplateAsync(input);
        var templatePathList = GetTemplatePathList(input);
        for (var i = 0; i < templatePathList.Count; i++)
        {
            var content = result.GetValueOrDefault(templatePathList[i]?.TrimEnd(".vm"));
            if (string.IsNullOrWhiteSpace(content)) continue;
            var dirPath = new DirectoryInfo(targetPathList[i]).Parent!.FullName;
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            _ = File.WriteAllTextAsync(targetPathList[i], content, Encoding.UTF8);
        }

        if (input.GenerateMenu) await AddOrUpdateMenu(input.TableName, input.BusName, input.MenuPid ?? 0, input.MenuIcon, input.PagePath, tableFieldList);

        // Non-ZIP compression returns empty
        if (!input.GenerateType.StartsWith('1')) return null;

        // Determine whether a file with the same name exists
        string downloadPath = zipPath + ".zip";
        if (File.Exists(downloadPath)) File.Delete(downloadPath);

        // Create zip file and return download address
        ZipFile.CreateFromDirectory(zipPath, downloadPath);
        return new { url = $"{App.HttpContext.Request.Scheme}://{App.HttpContext.Request.Host.Value}/codeGen/{input.TableName}.zip" };
    }

    /// <summary>
    /// Get a code generation preview 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get a code generation preview")]
    public async Task<Dictionary<string, string>> Preview(SysCodeGen input)
    {
        var (_, result) = await RenderTemplateAsync(input);
        return result;
    }

    /// <summary>
    /// render template
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<(List<CodeGenConfig> tableFieldList, Dictionary<string, string> result)> RenderTemplateAsync(SysCodeGen input)
    {
        var tableFieldList = await _codeGenConfigService.GetList(new CodeGenConfig { CodeGenId = input.Id }); // field collection
        var joinTableList = tableFieldList.Where(u => u.EffectType is "Upload" or "ForeignKey" or "ApiTreeSelector").ToList(); // Fields that require join table query

        var data = new CustomViewEngine
        {
            ConfigId = input.ConfigId,
            BusName = input.BusName,
            PagePath = input.PagePath,
            NameSpace = input.NameSpace,
            ClassName = input.TableName,
            PrintType = input.PrintType,
            PrintName = input.PrintName,
            AuthorName = input.AuthorName,
            ProjectLastName = input.NameSpace!.Split('.').Last(),
            LowerClassName = input.TableName!.ToFirstLetterLowerCase(),
            TableUniqueConfigList = input.TableUniqueList ?? new(),

            TableField = tableFieldList,
            QueryWhetherList = tableFieldList.Where(u => u.WhetherQuery == "Y").ToList(),
            ImportFieldList = tableFieldList.Where(u => u.WhetherImport == "Y").ToList(),
            UploadFieldList = tableFieldList.Where(u => u.EffectType == "Upload" || u.EffectType == "Upload_SingleFile").ToList(),
            PrimaryKeyFieldList = tableFieldList.Where(c => c.ColumnKey == "True").ToList(),
            AddUpdateFieldList = tableFieldList.Where(u => u.WhetherAddUpdate == "Y").ToList(),
            ApiTreeFieldList = tableFieldList.Where(u => u.EffectType == "ApiTreeSelector").ToList(),
            DropdownFieldList = tableFieldList.Where(u => u.EffectType is "ForeignKey" or "ApiTreeSelector").ToList(),
            DefaultValueList = tableFieldList.Where(u => u.DefaultValue != null && u.DefaultValue.Length > 0).ToList(),

            HasJoinTable = joinTableList.Count > 0,
            HasDictField = tableFieldList.Any(u => u.EffectType == "DictSelector"),
            HasEnumField = tableFieldList.Any(u => u.EffectType == "EnumSelector"),
            HasConstField = tableFieldList.Any(u => u.EffectType == "ConstSelector"),
            HasLikeQuery = tableFieldList.Any(c => c.WhetherQuery == "Y" && c.QueryType == "like")
        };

        // Get the template file and replace
        var templatePathList = GetTemplatePathList();
        var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "template");

        var result = new Dictionary<string, string>();
        foreach (var path in templatePathList)
        {
            var templateFilePath = Path.Combine(templatePath, path);
            if (!File.Exists(templateFilePath)) continue;
            var tContent = await File.ReadAllTextAsync(templateFilePath);
            var tResult = await _viewEngine.RunCompileFromCachedAsync(tContent, data, builderAction: builder =>
            {
                builder.AddAssemblyReferenceByName("System.Text.RegularExpressions");
                builder.AddAssemblyReferenceByName("System.Collections");
                builder.AddAssemblyReferenceByName("System.Linq");

                builder.AddUsing("System.Text.RegularExpressions");
                builder.AddUsing("System.Collections.Generic");
                builder.AddUsing("System.Linq");
            });
            result.Add(path?.TrimEnd(".vm"), tResult);
        }
        return (tableFieldList, result);
    }

    /// <summary>
    /// Add or update menu
    /// </summary>
    /// <param name="className"></param>
    /// <param name="busName"></param>
    /// <param name="pid"></param>
    /// <param name="menuIcon"></param>
    /// <param name="pagePath"></param>
    /// <param name="tableFieldList"></param>
    /// <returns></returns>
    private async Task AddOrUpdateMenu(string className, string busName, long pid, string menuIcon, string pagePath, List<CodeGenConfig> tableFieldList)
    {
        var title = $"{busName} Management";
        var lowerClassName = className.ToFirstLetterLowerCase();
        var menuType = pid == 0 ? MenuTypeEnum.Dir : MenuTypeEnum.Menu;

        // Check if there is a main menu
        var existingMenu = await _db.Queryable<SysMenu>()
            .Where(m => m.Title == title && m.Type == menuType && m.Pid == pid)
            .FirstAsync();

        string parentPath = "";
        if (pid != 0)
        {
            var parent = await _db.Queryable<SysMenu>().FirstAsync(m => m.Id == pid) ?? throw Oops.Oh(ErrorCodeEnum.D1505);
            parentPath = parent.Path;
        }

        long menuId;
        if (existingMenu == null)
        {
            // If it does not exist, add it
            var newMenu = new SysMenu
            {
                Pid = pid,
                Title = title,
                Type = menuType,
                Icon = menuIcon ?? "menu",
                Path = (pid == 0 ? "/" : parentPath + "/") + className.ToLower(),
                Component = pid == 0 ? "Layout" : $"/{pagePath}/{lowerClassName}/index"
            };
            menuId = await _sysMenuService.AddMenu(newMenu.Adapt<AddMenuInput>());
        }
        else
        {
            // Update if it exists
            existingMenu.Icon = menuIcon;
            existingMenu.Path = (pid == 0 ? "/" : parentPath + "/") + className.ToLower();
            existingMenu.Component = pid == 0 ? "Layout" : $"/{pagePath}/{lowerClassName}/index";
            await _sysMenuService.UpdateMenu(existingMenu.Adapt<UpdateMenuInput>());
            menuId = existingMenu.Id;
        }

        // Define the buttons that should be used
        var orderNo = 100;
        var newButtons = new List<SysMenu>
    {
        new() { Title = "Query", Permission = $"{lowerClassName}:page", OrderNo = orderNo += 10 },
        new() { Title = "Details", Permission = $"{lowerClassName}:detail", OrderNo = orderNo += 10 },
        new() { Title = "increase", Permission = $"{lowerClassName}:add", OrderNo = orderNo += 10 },
        new() { Title = "Edit", Permission = $"{lowerClassName}:update", OrderNo = orderNo += 10 },
        new() { Title = "Delete", Permission = $"{lowerClassName}:delete", OrderNo = orderNo += 10 },
        new() { Title = "Batch Delete", Permission = $"{lowerClassName}:batchDelete", OrderNo = orderNo += 10 },
        new() { Title = "Set status", Permission = $"{lowerClassName}:setStatus", OrderNo = orderNo += 10 },
        new() { Title = "Print", Permission = $"{lowerClassName}:print", OrderNo = orderNo += 10 },
        new() { Title = "import", Permission = $"{lowerClassName}:import", OrderNo = orderNo += 10 },
        new() { Title = "Export", Permission = $"{lowerClassName}:export", OrderNo = orderNo += 10 }
    };

        if (tableFieldList.Any(u => u.EffectType is "ForeignKey" or "ApiTreeSelector" && (u.WhetherAddUpdate == "Y" || u.WhetherQuery == "Y")))
        {
            newButtons.Add(new SysMenu
            {
                Title = "dropdown list data",
                Permission = $"{lowerClassName}:dropdownData",
                OrderNo = orderNo += 10
            });
        }

        foreach (var column in tableFieldList.Where(u => u.EffectType == "Upload" || u.EffectType == "Upload_SingleFile"))
        {
            newButtons.Add(new SysMenu
            {
                Title = $"Upload {column.ColumnComment}",
                Permission = $"{lowerClassName}:upload{column.PropertyName}",
                OrderNo = orderNo += 10
            });
        }

        // Get the button under the current menu
        var existingButtons = await _db.Queryable<SysMenu>()
            .Where(m => m.Pid == menuId && m.Type == MenuTypeEnum.Btn)
            .ToListAsync();

        var newPermissions = newButtons.Select(b => b.Permission).ToHashSet();

        // Add or update button
        foreach (var btn in newButtons)
        {
            var match = existingButtons.FirstOrDefault(b => b.Permission == btn.Permission);
            if (match == null)
            {
                btn.Type = MenuTypeEnum.Btn;
                btn.Pid = menuId;
                btn.Icon = "";
                await _sysMenuService.AddMenu(btn.Adapt<AddMenuInput>());
            }
            else
            {
                match.Title = btn.Title;
                match.OrderNo = btn.OrderNo;
                await _sysMenuService.UpdateMenu(match.Adapt<UpdateMenuInput>());
            }
        }

        // Remove redundant old buttons
        var toDelete = existingButtons.Where(b => !newPermissions.Contains(b.Permission)).ToList();
        foreach (var del in toDelete)
            await _sysMenuService.DeleteMenu(new DeleteMenuInput { Id = del.Id });
    }

    /// <summary>
    /// Add menu
    /// </summary>
    /// <param name="className"></param>
    /// <param name="busName"></param>
    /// <param name="pid"></param>
    /// <param name="menuIcon"></param>
    /// <param name="pagePath"></param>
    /// <param name="tableFieldList"></param>
    /// <returns></returns>
    private async Task AddMenu(string className, string busName, long pid, string menuIcon, string pagePath, List<CodeGenConfig> tableFieldList)
    {
        // Delete existing menu
        var title = $"{busName} Management";
        await DeleteMenuTree(title, pid == 0 ? MenuTypeEnum.Dir : MenuTypeEnum.Menu);

        var parentMenuPath = "";
        var lowerClassName = className!.ToFirstLetterLowerCase();
        if (pid == 0)
        {
            // Add a new directory and record the ID
            var dirMenu = new SysMenu { Pid = 0, Title = title, Type = MenuTypeEnum.Dir, Icon = "robot", Path = "/" + className.ToLower(), Component = "Layout" };
            pid = await _sysMenuService.AddMenu(dirMenu.Adapt<AddMenuInput>());
        }
        else
        {
            var parentMenu = await _db.Queryable<SysMenu>().FirstAsync(u => u.Id == pid) ?? throw Oops.Oh(ErrorCodeEnum.D1505);
            parentMenuPath = parentMenu.Path;
        }

        // Add a new menu and record the ID
        var rootMenu = new SysMenu { Pid = pid, Title = title, Type = MenuTypeEnum.Menu, Icon = menuIcon, Path = $"{parentMenuPath}/{className.ToLower()}", Component = $"/{pagePath}/{lowerClassName}/index" };
        pid = await _sysMenuService.AddMenu(rootMenu.Adapt<AddMenuInput>());

        var orderNo = 100;
        var menuList = new List<SysMenu>
        {
            new() { Title="Query", Permission=$"{lowerClassName}:page", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Details", Permission=$"{lowerClassName}:detail", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="increase", Permission=$"{lowerClassName}:add", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Edit", Permission=$"{lowerClassName}:update", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Delete", Permission=$"{lowerClassName}:delete", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Batch Delete", Permission=$"{lowerClassName}:batchDelete", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Set status", Permission=$"{lowerClassName}:setStatus", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Print", Permission=$"{lowerClassName}:print", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="import", Permission=$"{lowerClassName}:import", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10},
            new() { Title="Export", Permission=$"{lowerClassName}:export", Pid=pid, Type=MenuTypeEnum.Btn, OrderNo=orderNo+=10}
        };

        if (tableFieldList.Any(u => u.EffectType is "ForeignKey" or "ApiTreeSelector" && (u.WhetherAddUpdate == "Y" || u.WhetherQuery == "Y")))
            menuList.Add(new SysMenu { Title = "dropdown list data", Permission = $"{lowerClassName}:dropdownData", Pid = pid, Type = MenuTypeEnum.Btn, OrderNo = orderNo += 10 });

        foreach (var column in tableFieldList.Where(u => u.EffectType == "Upload"))
            menuList.Add(new SysMenu { Title = $"Upload {column.ColumnComment}", Permission = $"{lowerClassName}:upload{column.PropertyName}", Pid = pid, Type = MenuTypeEnum.Btn, OrderNo = orderNo += 10 });

        foreach (var menu in menuList) await _sysMenuService.AddMenu(menu.Adapt<AddMenuInput>());
    }

    /// <summary>
    /// Delete associated menu tree based on menu name and type
    /// </summary>
    /// <param name="title"></param>
    /// <param name="type"></param>
    private async Task DeleteMenuTree(string title, MenuTypeEnum type)
    {
        var menuList = await _db.Queryable<SysMenu>().Where(u => u.Title == title && u.Type == type).ToListAsync() ?? new();
        foreach (var menu in menuList) await _sysMenuService.DeleteMenu(new DeleteMenuInput { Id = menu.Id });
    }

    /// <summary>
    /// Get template file path collection
    /// </summary>
    /// <returns></returns>
    private static List<string> GetTemplatePathList(SysCodeGen input)
    {
        if (input.GenerateType!.Substring(1, 1).Contains('1')) return new() { "index.vue.vm", "editDialog.vue.vm", "api.ts.vm" };
        if (input.GenerateType.Substring(1, 1).Contains('2')) return new() { "Service.cs.vm", "Input.cs.vm", "Output.cs.vm", "Dto.cs.vm" };
        return new() { "Service.cs.vm", "Input.cs.vm", "Output.cs.vm", "Dto.cs.vm", "index.vue.vm", "editDialog.vue.vm", "api.ts.vm" };
    }

    /// <summary>
    /// Get template file path collection
    /// </summary>
    /// <returns></returns>
    private static List<string> GetTemplatePathList() => new() { "Service.cs.vm", "Input.cs.vm", "Output.cs.vm", "Dto.cs.vm", "index.vue.vm", "editDialog.vue.vm", "api.ts.vm" };

    /// <summary>
    /// Set the generated file path
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private List<string> GetTargetPathList(SysCodeGen input)
    {
        //var backendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent.FullName, _codeGenOptions.BackendApplicationNamespace, "Service", input.TableName);
        var backendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent!.FullName, input.NameSpace!, "Service", input.TableName!);
        var servicePath = Path.Combine(backendPath, input.TableName + "Service.cs");
        var inputPath = Path.Combine(backendPath, "Dto", input.TableName + "Input.cs");
        var outputPath = Path.Combine(backendPath, "Dto", input.TableName + "Output.cs");
        var viewPath = Path.Combine(backendPath, "Dto", input.TableName + "Dto.cs");
        var frontendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent!.Parent!.FullName, _codeGenOptions.FrontRootPath, "src", "views", input.PagePath!);
        var indexPath = Path.Combine(frontendPath, input.TableName[..1].ToLower() + input.TableName[1..], "index.vue");//
        var formModalPath = Path.Combine(frontendPath, input.TableName[..1].ToLower() + input.TableName[1..], "component", "editDialog.vue");
        var apiJsPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent!.Parent!.FullName, _codeGenOptions.FrontRootPath, "src", "api", input.PagePath, input.TableName[..1].ToLower() + input.TableName[1..] + ".ts");

        if (input.GenerateType!.Substring(1, 1).Contains('1'))
        {
            // Generate to this project (front-end)
            return new List<string>
            {
                indexPath,
                formModalPath,
                apiJsPath
            };
        }

        if (input.GenerateType.Substring(1, 1).Contains('2'))
        {
            // Generate to this project (backend)
            return new List<string>
            {
                servicePath,
                inputPath,
                outputPath,
                viewPath,
            };
        }
        // The front and back ends are generated to this project at the same time
        return new List<string>
        {
            servicePath,
            inputPath,
            outputPath,
            viewPath,
            indexPath,
            formModalPath,
            apiJsPath
        };
    }

    /// <summary>
    /// Set the generated file path
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private List<string> GetZipPathList(SysCodeGen input)
    {
        var zipPath = Path.Combine(App.WebHostEnvironment.WebRootPath, "CodeGen", input.TableName!);

        var firstLowerTableName = input.TableName!.ToFirstLetterLowerCase();
        //var backendPath = Path.Combine(zipPath, _codeGenOptions.BackendApplicationNamespace, "Service", input.TableName);
        var backendPath = Path.Combine(zipPath, input.NameSpace!, "Service", input.TableName);
        var servicePath = Path.Combine(backendPath, input.TableName + "Service.cs");
        var inputPath = Path.Combine(backendPath, "Dto", input.TableName + "Input.cs");
        var outputPath = Path.Combine(backendPath, "Dto", input.TableName + "Output.cs");
        var viewPath = Path.Combine(backendPath, "Dto", input.TableName + "Dto.cs");
        var frontendPath = Path.Combine(zipPath, _codeGenOptions.FrontRootPath, "src", "views", input.PagePath!);
        var indexPath = Path.Combine(frontendPath, firstLowerTableName, "index.vue");
        var formModalPath = Path.Combine(frontendPath, firstLowerTableName, "component", "editDialog.vue");
        var apiJsPath = Path.Combine(zipPath, _codeGenOptions.FrontRootPath, "src", "api", input.PagePath, firstLowerTableName + ".ts");
        if (input.GenerateType!.StartsWith("11"))
        {
            return new List<string>
            {
                indexPath,
                formModalPath,
                apiJsPath
            };
        }

        if (input.GenerateType.StartsWith("12"))
        {
            return new List<string>
            {
                servicePath,
                inputPath,
                outputPath,
                viewPath
            };
        }

        return new List<string>
        {
            servicePath,
            inputPath,
            outputPath,
            viewPath,
            indexPath,
            formModalPath,
            apiJsPath
        };
    }
}