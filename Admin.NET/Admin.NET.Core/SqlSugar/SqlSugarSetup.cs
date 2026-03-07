// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.Data.Sqlite;
using DbType = SqlSugar.DbType;

namespace Admin.NET.Core;

public static class SqlSugarSetup
{
    // Multi-tenant instance
    public static ITenant ITenant { get; set; }

    // Whether seed data is being processed
    private static bool _isHandlingSeedData = false;

    /// <summary>
    /// SqlSugar context initialization
    /// </summary>
    /// <param name="services"></param>
    public static void AddSqlSugar(this IServiceCollection services)
    {
        // Register Snowflake ID
        var snowIdOpt = App.GetConfig<SnowIdOptions>("SnowId", true);
        YitIdHelper.SetIdGenerator(snowIdOpt);

        // Customized SqlSugar snowflake ID algorithm
        SnowFlakeSingle.WorkId = snowIdOpt.WorkerId;
        StaticConfig.CustomSnowFlakeFunc = YitIdHelper.NextId;
        // Register MongoDB
        InstanceFactory.CustomAssemblies = [typeof(SqlSugar.MongoDb.MongoDbProvider).Assembly];
        // Dynamic expression SqlFunc support, https://www.donet5.com/Home/Doc?typeId=2569
        StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
        StaticConfig.DynamicExpressionParsingConfig = new ParsingConfig
        {
            CustomTypeProvider = new SqlSugarTypeProvider()
        };

        var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
        dbOptions.ConnectionConfigs.ForEach(SetDbConfig);

        SqlSugarScope sqlSugar = new(dbOptions.ConnectionConfigs.Adapt<List<ConnectionConfig>>(), db =>
        {
            dbOptions.ConnectionConfigs.ForEach(config =>
            {
                var dbProvider = db.GetConnectionScope(config.ConfigId);
                SetDbAop(dbProvider, dbOptions.EnableConsoleSql, dbOptions.SuperAdminIgnoreIDeletedFilter);
                SetDbDiffLog(dbProvider, config);
            });
        });
        ITenant = sqlSugar;

        services.AddSingleton<ISqlSugarClient>(sqlSugar); // Singleton registration
        services.AddScoped(typeof(SqlSugarRepository<>)); // Warehouse registration
        services.AddUnitOfWork<SqlSugarUnitOfWork>(); // Transaction and work unit registration

        // Initialize database table structure and seed data
        dbOptions.ConnectionConfigs.ForEach(config =>
        {
            InitDatabase(sqlSugar, config);
        });
    }

    /// <summary>
    /// Configure connection properties
    /// </summary>
    /// <param name="config"></param>
    public static void SetDbConfig(DbConnectionConfig config)
    {
        if (config.DbSettings.EnableConnStringEncrypt)
            config.ConnectionString = CryptogramUtil.Decrypt(config.ConnectionString);

        var configureExternalServices = new ConfigureExternalServices
        {
            EntityNameService = (type, entity) => // processing table
            {
                entity.IsDisabledDelete = true; // Disable deletion of columns not created by sqlsugar
                // Only the attribute [SugarTable] table is processed
                if (!type.GetCustomAttributes<SugarTable>().Any())
                    return;
                if (config.DbSettings.EnableUnderLine && !entity.DbTableName.Contains('_'))
                    entity.DbTableName = entity.DbTableName.ToUnderLine(); // camelback to underline
            },
            EntityService = (type, column) => // Process columns
            {
                // Only processes columns with attributes [SugarColumn] posted
                if (!type.GetCustomAttributes<SugarColumn>().Any())
                    return;
                if (new NullabilityInfoContext().Create(type).WriteState is NullabilityState.Nullable)
                    column.IsNullable = true;
                if (config.DbSettings.EnableUnderLine && !column.IsIgnore && !column.DbColumnName.Contains('_'))
                    column.DbColumnName = column.DbColumnName.ToUnderLine(); // camelback to underline
            },
            DataInfoCacheService = new SqlSugarCache(),
        };
        config.ConfigureExternalServices = configureExternalServices;
        config.InitKeyType = InitKeyType.Attribute;
        config.IsAutoCloseConnection = true;
        config.MoreSettings = new ConnMoreSettings
        {
            IsAutoRemoveDataCache = true, // Enable automatic cache deletion, all additions, deletions and changes will automatically call .RemoveDataCache()
            IsAutoDeleteQueryFilter = true, // Enable delete query filter
            IsAutoUpdateQueryFilter = true, // Enable update query filter
            SqlServerCodeFirstNvarchar = true // Using Nvarchar
        };

        // If the library type is Renmin University of Finance and Economics, the PG mode is set by default.
        if (config.DbType == DbType.Kdbndp)
            config.MoreSettings.DatabaseModel = DbType.PostgreSQL; // Configuring PG mode is mainly for compatibility with system table differences.

        // If the library type is Oracle, the default primary key name and parameter name maximum length
        if (config.DbType == DbType.Oracle)
            config.MoreSettings.MaxParameterNameLength = 30;
    }

    /// <summary>
    /// Configure Aop
    /// </summary>
    /// <param name="db"></param>
    /// <param name="enableConsoleSql"></param>
    /// <param name="superAdminIgnoreIDeletedFilter"></param>
    public static void SetDbAop(SqlSugarScopeProvider db, bool enableConsoleSql, bool superAdminIgnoreIDeletedFilter)
    {
        // Set timeout
        db.Ado.CommandTimeOut = 30;

        // Print SQL statement
        if (enableConsoleSql)
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //// If the parameter value exceeds 100 characters, intercept it
                //foreach (var par in pars)
                //{
                //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
                //    if (par.Value.ToString().Length > 100)
                //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
                //}

                var log = $"[{DateTime.Now}——Execute SQL]\r\n{UtilMethods.GetNativeSql(sql, pars)}\r\n";
                var originColor = Console.ForegroundColor;
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Green;
                if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(log);
                Console.ForegroundColor = originColor;
            };
        }
        db.Aop.OnError = ex =>
        {
            if (ex.Parametres == null) return;
            var log = $"[{DateTime.Now}——Error SQL]\r\n{UtilMethods.GetNativeSql(ex.Sql, (SugarParameter[])ex.Parameters)}\r\n";
            Log.Error(log, ex);
        };
        db.Aop.OnLogExecuted = (sql, pars) =>
        {
            //// If the parameter value exceeds 100 characters, intercept it
            //foreach (var par in pars)
            //{
            //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
            //    if (par.Value.ToString().Length > 100)
            //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
            //}

            // When the execution time exceeds 5 seconds
            if (!(db.Ado.SqlExecutionTime.TotalSeconds > 5)) return;

            var fileName = db.Ado.SqlStackTrace.FirstFileName; // file name
            var fileLine = db.Ado.SqlStackTrace.FirstLine; // Line number
            var firstMethodName = db.Ado.SqlStackTrace.FirstMethodName; // method name
            var log = $"[{DateTime.Now}——Timeout SQL]\r\n[File name]: {fileName}\r\n[Number of lines of code]: {fileLine}\r\n[Method name]: {firstMethodName}\r\n" + $"[SQL statement]: {UtilMethods.GetNativeSql(sql, pars)}";
            Log.Warning(log);
        };

        // Data audit
        db.Aop.DataExecuting = (_, entityInfo) =>
        {
            // If the seed data is being processed, return directly.
            if (_isHandlingSeedData) return;

            // add/insert
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                // If the primary key is a long integer and is empty, assign Snowflake Id.
                if (entityInfo.EntityColumnInfo.IsPrimarykey && !entityInfo.EntityColumnInfo.IsIdentity && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                {
                    var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                    if (id == null || (long)id == 0)
                        entityInfo.SetValue(YitIdHelper.NextId());
                }
                // If the creation time is empty, assign the current time
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateTime))
                {
                    var createTime = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue)!;
                    if (createTime == null || createTime.Equals(DateTime.MinValue))
                        entityInfo.SetValue(DateTime.Now);
                }
                // If the current user is empty (not a web thread)
                if (App.User == null) return;

                dynamic entityValue = entityInfo.EntityValue;
                if (entityInfo.PropertyName == nameof(EntityBaseTenantId.TenantId))
                {
                    var tenantId = entityValue.TenantId;
                    if (tenantId == null || tenantId == 0)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.TenantId)?.Value);
                }
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateUserId))
                {
                    var createUserId = entityValue.CreateUserId;
                    if (createUserId == 0 || createUserId == null)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.UserId)?.Value);
                }
                else if (entityInfo.PropertyName == nameof(EntityBase.CreateUserName))
                {
                    var createUserName = entityValue.CreateUserName;
                    if (string.IsNullOrEmpty(createUserName))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.RealName)?.Value);
                }
                else if (entityInfo.PropertyName == "CreateOrgId")
                {
                    var createOrgId = entityValue.CreateOrgId;
                    if (createOrgId == 0 || createOrgId == null)
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgId)?.Value);
                }
                else if (entityInfo.PropertyName == "CreateOrgName")
                {
                    var createOrgName = entityValue.CreateOrgName;
                    if (string.IsNullOrEmpty(createOrgName))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgName)?.Value);
                }
            }
            // Edit/Update
            else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                if (entityInfo.PropertyName == nameof(EntityBase.UpdateTime))
                    entityInfo.SetValue(DateTime.Now);
                else if (entityInfo.PropertyName == nameof(EntityBase.UpdateUserId))
                    entityInfo.SetValue(App.User?.FindFirst(ClaimConst.UserId)?.Value);
                else if (entityInfo.PropertyName == nameof(EntityBase.UpdateUserName))
                    entityInfo.SetValue(App.User?.FindFirst(ClaimConst.RealName)?.Value);
                else if (entityInfo.PropertyName == nameof(EntityBaseDel.DeleteTime))
                {
                    dynamic entityValue = entityInfo.EntityValue;
                    var isDelete = entityValue.IsDelete;
                    if (isDelete == true)
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                }
            }
        };

        // Is it a super administrator?
        var isSuperAdmin = App.User?.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString();

        // Configure the fake delete filter, which will not be applied if the current user is a super administrator and is allowed to ignore soft delete filters
        if (!isSuperAdmin || !superAdminIgnoreIDeletedFilter)
            db.QueryFilter.AddTableFilter<IDeletedFilter>(u => u.IsDelete == false);

        // Supertube excludes other filters
        if (isSuperAdmin) return;

        // Configure tenant filters
        var tenantId = App.User?.FindFirst(ClaimConst.TenantId)?.Value;
        if (!string.IsNullOrWhiteSpace(tenantId))
            db.QueryFilter.AddTableFilter<ITenantIdFilter>(u => u.TenantId == long.Parse(tenantId));

        // Configure user organization (data range) filter
        SqlSugarFilter.SetOrgEntityFilter(db);

        // Configure custom filters
        SqlSugarFilter.SetCustomEntityFilter(db);
    }

    /// <summary>
    /// Enable differential logs for database tables
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void SetDbDiffLog(SqlSugarScopeProvider db, DbConnectionConfig config)
    {
        if (!config.DbSettings.EnableDiffLog) return;

        async void AopOnDiffLogEvent(DiffLogModel u)
        {
            // Record differential data
            var diffData = new List<dynamic>();
            for (int i = 0; i < u.AfterData.Count; i++)
            {
                var diffColumns = new List<dynamic>();
                var afterColumns = u.AfterData[i].Columns;
                var beforeColumns = u.BeforeData[i].Columns;
                for (int j = 0; j < afterColumns.Count; j++)
                {
                    if (afterColumns[j].Value.Equals(beforeColumns[j].Value)) continue;
                    diffColumns.Add(new
                    {
                        afterColumns[j].IsPrimaryKey,
                        afterColumns[j].ColumnName,
                        afterColumns[j].ColumnDescription,
                        BeforeValue = beforeColumns[j].Value,
                        AfterValue = afterColumns[j].Value,
                    });
                }

                diffData.Add(new { u.AfterData[i].TableName, u.AfterData[i].TableDescription, Columns = diffColumns });
            }

            var logDiff = new SysLogDiff
            {
                // Difference data (field description, column name, value, table name, table description)
                DiffData = JSON.Serialize(diffData),
                // The object passed in (if the object is empty, the table name of the first data is used as the business object)
                BusinessData = u.BusinessData == null ? u.AfterData.FirstOrDefault()?.TableName : JSON.Serialize(u.BusinessData),
                // Enumeration (insert, update, delete)
                DiffType = u.DiffType.ToString(),
                Sql = u.Sql,
                Parameters = JSON.Serialize(u.Parameters.Select(e => new { e.ParameterName, e.Value, TypeName = e.DbType.ToString() })),
                Elapsed = u.Time == null ? 0 : (long)u.Time.Value.TotalMilliseconds
            };
            var logDb = ITenant.IsAnyConnection(SqlSugarConst.LogConfigId) ? ITenant.GetConnectionScope(SqlSugarConst.LogConfigId) : ITenant.GetConnectionScope(SqlSugarConst.MainConfigId);
            await logDb.CopyNew().Insertable(logDiff).ExecuteCommandAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now + $"\r\n****Start difference log*****\r\n{Environment.NewLine}{JSON.Serialize(logDiff)}{Environment.NewLine}*****End difference log*****\r\n");
        }

        db.Aop.OnDiffLogEvent = AopOnDiffLogEvent;
    }

    /// <summary>
    /// Initialize view
    /// </summary>
    /// <param name="dbProvider"></param>
    private static void InitView(SqlSugarScopeProvider dbProvider)
    {
        var totalWatch = Stopwatch.StartNew(); // Start total time
        Log.Information($"BeginningInitialize View {dbProvider.CurrentConnectionConfig.DbType} - {dbProvider.CurrentConnectionConfig.ConfigId}");
        var viewTypeList = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarView)))).ToList();

        int taskIndex = 0, size = viewTypeList.Count;
        var taskList = viewTypeList.Select(viewType => Task.Run(() =>
        {
            // Start timing
            var stopWatch = Stopwatch.StartNew();

            // Get view entity and configuration information
            var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(viewType) ?? throw new Exception("Error in obtaining view entity configuration");

            // Delete the view if it exists
            if (dbProvider.DbMaintenance.GetViewInfoList(false).Any(it => it.Name.EqualIgnoreCase(entityInfo.DbTableName)))
                dbProvider.DbMaintenance.DropView(entityInfo.DbTableName);

            // Get initialization view query SQL
            var sql = viewType.GetMethod(nameof(ISqlSugarView.GetQueryableSqlString))?.Invoke(Activator.CreateInstance(viewType), [dbProvider]) as string;
            if (string.IsNullOrWhiteSpace(sql)) throw new Exception("The SQL statement for initializing the view cannot be empty");

            // Create view
            dbProvider.Ado.ExecuteCommand($"CREATE VIEW {entityInfo.DbTableName} AS " + Environment.NewLine + " " + sql);

            // Stop timing
            stopWatch.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Initialize view {viewType.FullName,-58} ({dbProvider.CurrentConnectionConfig.ConfigId} - {Interlocked.Increment(ref taskIndex):D003}/{size:D003}, time consuming: {stopWatch.ElapsedMilliseconds:N0} ms)");
        }));
        Task.WaitAll(taskList.ToArray());

        totalWatch.Stop(); // Stop total time
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Initialize view {dbProvider.CurrentConnectionConfig.DbType} - {dbProvider.CurrentConnectionConfig.ConfigId} Total time spent: {totalWatch.ElapsedMilliseconds:N0} ms");
    }

    /// <summary>
    /// Wait for database to be ready
    /// </summary>
    /// <param name="dbProvider"></param>
    private static void WaitForDatabaseReady(SqlSugarScopeProvider dbProvider)
    {
        do
        {
            try
            {
                if (dbProvider.Ado.Connection.State != ConnectionState.Open)
                    dbProvider.Ado.Connection.Open();

                // If the connection is successful, return directly
                Log.Information("Database connection successful.");
                return;
            }
            catch (Exception ex)
            {
                Log.Warning($"The database is not ready yet, waiting... Error: {ex.Message}");
                Thread.Sleep(1000);
            }
        } while (true);
    }

    /// <summary>
    /// Initialize database
    /// </summary>
    /// <param name="db">SqlSugarScope instance</param>
    /// <param name="config">Database connection configuration</param>
    private static void InitDatabase(SqlSugarScope db, DbConnectionConfig config)
    {
        var dbProvider = db.GetConnectionScope(config.ConfigId);

        // Initialize the database. If there is no database, initialize the database first and then connect.
        if (config.DbSettings.EnableInitDb)
        {
            Log.Information($"Initialize database {config.DbType} - {config.ConfigId} - {config.ConnectionString}");
            if (config.DbType != DbType.Oracle) dbProvider.DbMaintenance.CreateDatabase();
        }

        // Wait for database connection to be ready
        WaitForDatabaseReady(dbProvider);

        // Initialize table structure
        if (config.TableSettings.EnableInitTable)
        {
            Log.Information($"Initialize table structure {config.DbType} - {config.ConfigId}");
            var entityTypes = GetEntityTypesForInit(config);
            InitializeTables(dbProvider, entityTypes, config);
        }

        // Initialize view
        if (config.DbSettings.EnableInitView) InitView(dbProvider);

        // Initialize seed data
        if (config.SeedSettings.EnableInitSeed) InitSeedData(db, config);
    }

    /// <summary>
    /// Get the entity type that needs to be initialized
    /// </summary>
    /// <param name="config">Database connection configuration</param>
    /// <returns>Entity type list</returns>
    private static List<Type> GetEntityTypesForInit(DbConnectionConfig config)
    {
        return App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false))
            .Where(u => !u.GetCustomAttributes<IgnoreTableAttribute>().Any())
            .WhereIF(config.TableSettings.EnableIncreTable, u => u.IsDefined(typeof(IncreTableAttribute), false))
            .Where(u => IsEntityForConfig(u, config))
            .ToList();
    }

    /// <summary>
    /// Determine whether the entity belongs to the current configuration
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <param name="config">Database connection configuration</param>
    /// <returns>Whether it belongs to the current configuration</returns>
    private static bool IsEntityForConfig(Type entityType, DbConnectionConfig config)
    {
        switch (config.ConfigId.ToString())
        {
            case SqlSugarConst.MainConfigId:
                return entityType.GetCustomAttributes<SysTableAttribute>().Any() ||
                       (!entityType.GetCustomAttributes<LogTableAttribute>().Any() &&
                        !entityType.GetCustomAttributes<TenantAttribute>().Any(o => o.configId.ToString() != config.ConfigId.ToString()));

            case SqlSugarConst.LogConfigId:
                return entityType.GetCustomAttributes<LogTableAttribute>().Any();

            default:
                {
                    var tenantAttribute = entityType.GetCustomAttribute<TenantAttribute>();
                    return tenantAttribute != null && tenantAttribute.configId.ToString() == config.ConfigId.ToString();
                }
        }
    }

    /// <summary>
    /// Initialize table structure
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider instance</param>
    /// <param name="entityTypes">Entity type list</param>
    /// <param name="config">Database connection configuration</param>
    private static void InitializeTables(SqlSugarScopeProvider dbProvider, List<Type> entityTypes, DbConnectionConfig config)
    {
        // Delete the view and then initialize the table structure to prevent the table structure from being unable to be synchronized due to the view.
        var viewTypeList = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarView)))).ToList();
        foreach (var viewType in viewTypeList)
        {
            var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(viewType) ?? throw new Exception("Error in obtaining view entity configuration");
            if (dbProvider.DbMaintenance.GetViewInfoList(false).Any(it => it.Name.EqualIgnoreCase(entityInfo.DbTableName)))
                dbProvider.DbMaintenance.DropView(entityInfo.DbTableName);
        }

        int count = 0, sum = entityTypes.Count;
        var tasks = entityTypes.Select(entityType => Task.Run(() =>
        {
            Console.WriteLine($"Initialization table structure {entityType.FullName,-64} ({config.ConfigId} - {Interlocked.Increment(ref count):D003}/{sum:D003})");
            UpdateNullableColumns(dbProvider, entityType);
            InitializeTable(dbProvider, entityType);
        }));

        Task.WhenAll(tasks).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Update fields in the table that do not exist in the entity to be nullable
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider instance</param>
    /// <param name="entityType">Entity type</param>
    private static void UpdateNullableColumns(SqlSugarScopeProvider dbProvider, Type entityType)
    {
        var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
        var dbColumns = dbProvider.DbMaintenance.GetColumnInfosByTableName(entityInfo.DbTableName) ?? new List<DbColumnInfo>();

        foreach (var dbColumn in dbColumns.Where(c => !c.IsPrimarykey && entityInfo.Columns.All(u => u.DbColumnName != c.DbColumnName)))
        {
            dbColumn.IsNullable = true;
            Retry(() =>
            {
                dbProvider.DbMaintenance.UpdateColumn(entityInfo.DbTableName, dbColumn);
            }, maxRetry: 3, retryIntervalMs: 1000);
        }
    }

    /// <summary>
    /// initialization table
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider instance</param>
    /// <param name="entityType">Entity type</param>
    private static void InitializeTable(SqlSugarScopeProvider dbProvider, Type entityType)
    {
        Retry(() =>
        {
            if (entityType.GetCustomAttribute<SplitTableAttribute>() == null)
            {
                dbProvider.CodeFirst.InitTables(entityType);
            }
            else
            {
                dbProvider.CodeFirst.SplitTables().InitTables(entityType);
            }
        }, maxRetry: 3, retryIntervalMs: 1000);
    }

    /// <summary>
    /// Initialize seed data
    /// </summary>
    /// <param name="db">SqlSugarScope instance</param>
    /// <param name="config">Database connection configuration</param>
    private static void InitSeedData(SqlSugarScope db, DbConnectionConfig config)
    {
        var dbProvider = db.GetConnectionScope(config.ConfigId);
        _isHandlingSeedData = true;

        Log.Information($"BeginningInitial seedchildData {config.DbType} - {config.ConfigId}");
        var seedDataTypes = GetSeedDataTypes(config);

        int count = 0, sum = seedDataTypes.Count;
        var tasks = seedDataTypes.Select(seedType => Task.Run(() =>
        {
            var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();
            if (!IsEntityForConfig(entityType, config)) return;

            var seedData = GetSeedData(seedType)?.ToList();
            if (seedData == null) return;

            AdjustSeedDataIds(seedData, config);
            InsertOrUpdateSeedData(dbProvider, seedType, entityType, seedData, config, ref count, sum);
        }));

        Task.WhenAll(tasks).GetAwaiter().GetResult();
        _isHandlingSeedData = false;
    }

    /// <summary>
    /// Get seed data type
    /// </summary>
    /// <param name="config">Database connection configuration</param>
    /// <returns>List of seed data types</returns>
    private static List<Type> GetSeedDataTypes(DbConnectionConfig config)
    {
        return App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))))
            .WhereIF(config.SeedSettings.EnableIncreSeed, u => u.IsDefined(typeof(IncreSeedAttribute), false))
            .OrderBy(u => u.GetCustomAttributes(typeof(SeedDataAttribute), false).Length > 0 ? ((SeedDataAttribute)u.GetCustomAttributes(typeof(SeedDataAttribute), false)[0]).Order : 0)
            .ToList();
    }

    /// <summary>
    /// Get seed data
    /// </summary>
    /// <param name="seedType">Seed data type</param>
    /// <returns>Seed data list</returns>
    private static IEnumerable<object> GetSeedData(Type seedType)
    {
        var instance = Activator.CreateInstance(seedType);
        var hasDataMethod = seedType.GetMethod("HasData");
        return ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
    }

    /// <summary>
    /// Adjust the ID of the seed data
    /// </summary>
    /// <param name="seedData">Seed data list</param>
    /// <param name="config">Database connection configuration</param>
    private static void AdjustSeedDataIds(IEnumerable<object> seedData, DbConnectionConfig config)
    {
        var seedId = config.ConfigId.ToLong();
        foreach (var data in seedData)
        {
            var idProperty = data.GetType().GetProperty(nameof(EntityBaseId.Id));
            if (idProperty == null || idProperty.PropertyType != typeof(Int64)) continue;

            var idValue = idProperty.GetValue(data);
            if (idValue == null || idValue.ToString() == "0" || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                idProperty.SetValue(data, ++seedId);
            }
        }
    }

    /// <summary>
    /// Insert or update seed data
    /// </summary>
    /// <param name="dbProvider">SqlSugarScopeProvider instance</param>
    /// <param name="seedType">Seed data type</param>
    /// <param name="entityType">Entity type</param>
    /// <param name="seedData">Seed data list</param>
    /// <param name="config">Database connection configuration</param>
    /// <param name="count">Quantity currently processed</param>
    /// <param name="sum">total quantity</param>
    private static void InsertOrUpdateSeedData(SqlSugarScopeProvider dbProvider, Type seedType, Type entityType, IEnumerable<object> seedData, DbConnectionConfig config, ref int count, int sum)
    {
        var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
        var dataList = seedData.ToList();

        if (entityType.GetCustomAttribute<SplitTableAttribute>(true) != null)
        {
            var initMethod = seedType.GetMethod("Init");
            initMethod?.Invoke(Activator.CreateInstance(seedType), new object[] { dbProvider });
        }
        else
        {
            int updateCount = 0, insertCount = 0;
            if (entityInfo.Columns.Any(u => u.IsPrimarykey))
            {
                var storage = dbProvider.StorageableByObject(dataList).ToStorage();
                if (seedType.GetCustomAttribute<IgnoreUpdateSeedAttribute>() == null)
                {
                    updateCount = storage.AsUpdateable
                        .IgnoreColumns(entityInfo.Columns
                            .Where(u => u.PropertyInfo.GetCustomAttribute<IgnoreUpdateSeedColumnAttribute>() != null)
                            .Select(u => u.PropertyName).ToArray())
                        .ExecuteCommand();
                }
                insertCount = storage.AsInsertable.ExecuteCommand();
            }
            else
            {
                if (!dbProvider.Queryable(entityInfo.DbTableName, entityInfo.DbTableName).Any())
                {
                    insertCount = dataList.Count;
                    dbProvider.InsertableByObject(dataList).ExecuteCommand();
                }
            }
            Console.WriteLine($"Add data {entityInfo.DbTableName,-32} ({config.ConfigId} - {Interlocked.Increment(ref count):D003}/{sum:D003}, data volume: {dataList.Count:D003}, inserted {insertCount:D003} records, updated {updateCount:D003} records)");
        }
    }

    /// <summary>
    /// Initialize tenant business database
    /// </summary>
    /// <param name="iTenant"></param>
    /// <param name="config"></param>
    public static void InitTenantDatabase(ITenant iTenant, DbConnectionConfig config)
    {
        SetDbConfig(config);

        if (!iTenant.IsAnyConnection(config.ConfigId.ToString()))
            iTenant.AddConnection(config);
        var db = iTenant.GetConnectionScope(config.ConfigId.ToString());
        db.DbMaintenance.CreateDatabase();

        // Get all business tables - initialize the tenant library table structure (exclude system tables, log tables, specific library tables)
        var entityTypes = App.EffectiveTypes
            .Where(u => !u.GetCustomAttributes<IgnoreTableAttribute>().Any())
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) &&
            !u.IsDefined(typeof(SysTableAttribute), false) && !u.IsDefined(typeof(LogTableAttribute), false) && !u.IsDefined(typeof(TenantAttribute), false)).ToList();
        if (entityTypes.Count == 0) return;

        foreach (var entityType in entityTypes)
        {
            var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTable == null)
                db.CodeFirst.InitTables(entityType);
            else
                db.CodeFirst.SplitTables().InitTables(entityType);
        }
    }

    /// <summary>
    /// Simple retry mechanism
    /// </summary>
    /// <param name="action"></param>
    /// <param name="maxRetry"></param>
    /// <param name="retryIntervalMs"></param>
    private static void Retry(Action action, int maxRetry, int retryIntervalMs)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                action();
                return;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 5) // SQLITE_BUSY
            {
                if (++attempt >= maxRetry)
                {
                    Log.Error($"Simple retry mechanism: {ex.Message}"); throw;
                }
                Log.Information($"Database is busy, retrying... (Attempt {attempt}/{maxRetry})");
                Thread.Sleep(retryIntervalMs);
            }
        }
    }
}