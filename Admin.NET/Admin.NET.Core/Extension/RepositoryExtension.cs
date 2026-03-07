// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using MapsterMapper;

namespace Admin.NET.Core;

public static class RepositoryExtension
{
    /// <summary>
    /// Entity fake delete _rep.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int FakeDelete<T>(this ISugarRepository repository, T entity) where T : EntityBaseDel, new()
    {
        return repository.Context.FakeDelete(entity);
    }

    /// <summary>
    /// Fake delete of entity db.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int FakeDelete<T>(this ISqlSugarClient db, T entity) where T : EntityBaseDel, new()
    {
        return db.Updateable(entity).AS().ReSetValue(x => { x.IsDelete = true; })
            .IgnoreColumns(ignoreAllNullColumns: true)
            .EnableDiffLogEvent()   // Record difference log
            .UpdateColumns(x => new { x.IsDelete, x.DeleteTime, x.UpdateTime, x.UpdateUserId })  // Fields allowed to be updated - AOP interception automatically sets UpdateTime, UpdateUserId
            .ExecuteCommand();
    }

    /// <summary>
    /// Batch fake deletion of entity collection _rep.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int FakeDelete<T>(this ISugarRepository repository, List<T> entity) where T : EntityBaseDel, new()
    {
        return repository.Context.FakeDelete(entity);
    }

    /// <summary>
    /// Batch fake deletion of entity collection db.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int FakeDelete<T>(this ISqlSugarClient db, List<T> entity) where T : EntityBaseDel, new()
    {
        return db.Updateable(entity).AS().ReSetValue(x => { x.IsDelete = true; })
            .IgnoreColumns(ignoreAllNullColumns: true)
            .EnableDiffLogEvent()   // Record difference log
            .UpdateColumns(x => new { x.IsDelete, x.DeleteTime, x.UpdateTime, x.UpdateUserId })  // Fields allowed to be updated - AOP interception automatically sets UpdateTime, UpdateUserId
            .ExecuteCommand();
    }

    /// <summary>
    /// Entity fake deletion asynchronous _rep.FakeDeleteAsync(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> FakeDeleteAsync<T>(this ISugarRepository repository, T entity) where T : EntityBaseDel, new()
    {
        return repository.Context.FakeDeleteAsync(entity);
    }

    /// <summary>
    /// Fake delete of entity db.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> FakeDeleteAsync<T>(this ISqlSugarClient db, T entity) where T : EntityBaseDel, new()
    {
        return db.Updateable(entity).AS().ReSetValue(x => { x.IsDelete = true; })
            .IgnoreColumns(ignoreAllNullColumns: true)
            .EnableDiffLogEvent()   // Record difference log
            .UpdateColumns(x => new { x.IsDelete, x.DeleteTime, x.UpdateTime, x.UpdateUserId })  // Fields allowed to be updated - AOP interception automatically sets UpdateTime, UpdateUserId
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Batch fake deletion of entity collection asynchronously _rep.FakeDeleteAsync(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> FakeDeleteAsync<T>(this ISugarRepository repository, List<T> entity) where T : EntityBaseDel, new()
    {
        return repository.Context.FakeDeleteAsync(entity);
    }

    /// <summary>
    /// Batch fake deletion of entity collection db.FakeDelete(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> FakeDeleteAsync<T>(this ISqlSugarClient db, List<T> entity) where T : EntityBaseDel, new()
    {
        return db.Updateable(entity).AS().ReSetValue(x => { x.IsDelete = true; })
            .IgnoreColumns(ignoreAllNullColumns: true)
            .EnableDiffLogEvent()   // Record difference log
            .UpdateColumns(x => new { x.IsDelete, x.DeleteTime, x.UpdateTime, x.UpdateUserId })  // Fields allowed to be updated - AOP interception automatically sets UpdateTime, UpdateUserId
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Sort by (default descending)
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="pageInput"> </param>
    /// <param name="prefix"> </param>
    /// <param name="defaultSortField"> Default sort field </param>
    /// <param name="descSort"> Is descending order </param>
    /// <returns> </returns>
    public static ISugarQueryable<T> OrderBuilder<T>(this ISugarQueryable<T> queryable, BasePageInput pageInput, string prefix = "", string defaultSortField = "Id", bool descSort = true)
    {
        var iSqlBuilder = InstanceFactory.GetSqlBuilderWithContext(queryable.Context);

        // It is agreed that each table is sorted by Id by default.
        var orderStr = string.IsNullOrWhiteSpace(defaultSortField) ? "" : $"{prefix}{iSqlBuilder.GetTranslationColumnName(defaultSortField)}" + (descSort ? " Desc" : " Asc");

        TypeAdapterConfig typeAdapterConfig = new();
        typeAdapterConfig.ForType<T, BasePageInput>().IgnoreNullValues(true);
        Mapper mapper = new(typeAdapterConfig); // Be sure to set the mapper to a single instance
        var nowPagerInput = mapper.Map<BasePageInput>(pageInput);
        // Whether sorting is available - sorting is enabled only when the sorting field is non-empty. The sorting order defaults to reverse order.
        if (!string.IsNullOrEmpty(nowPagerInput.Field))
        {
            nowPagerInput.Field = Regex.Replace(nowPagerInput.Field, @"[\s;()\-'@=/%]", ""); // Filter out some key characters to prevent special SQL statement injection
            var orderByDbName = queryable.Context.EntityMaintenance.GetDbColumnName<T>(nowPagerInput.Field);// To prevent injection, an error will be reported as long as the attribute name does not exist in the class.
            orderStr = $"{prefix}{iSqlBuilder.GetTranslationColumnName(orderByDbName)} {(string.IsNullOrEmpty(nowPagerInput.Order) || nowPagerInput.Order.Equals(nowPagerInput.DescStr, StringComparison.OrdinalIgnoreCase) ? "Desc" : "Asc")}";
        }
        return queryable.OrderByIF(!string.IsNullOrWhiteSpace(orderStr), orderStr);
    }

    /// <summary>
    /// Update the entity and log the difference _rep.UpdateWithDiffLog(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <param name="ignoreAllNullColumns"></param>
    /// <returns></returns>
    public static int UpdateWithDiffLog<T>(this ISugarRepository repository, T entity, bool ignoreAllNullColumns = true) where T : EntityBase, new()
    {
        return repository.Context.UpdateWithDiffLog(entity, ignoreAllNullColumns);
    }

    /// <summary>
    /// Update the entity and log the difference _rep.UpdateWithDiffLog(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <param name="ignoreAllNullColumns"></param>
    /// <returns></returns>
    public static int UpdateWithDiffLog<T>(this ISqlSugarClient db, T entity, bool ignoreAllNullColumns = true) where T : EntityBase, new()
    {
        return db.Updateable(entity).AS()
            .IgnoreColumns(ignoreAllNullColumns: ignoreAllNullColumns)
            .EnableDiffLogEvent()
            .ExecuteCommand();
    }

    /// <summary>
    /// Update entities and log differences _rep.UpdateWithDiffLogAsync(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <param name="ignoreAllNullColumns"></param>
    /// <returns></returns>
    public static Task<int> UpdateWithDiffLogAsync<T>(this ISugarRepository repository, T entity, bool ignoreAllNullColumns = true) where T : EntityBase, new()
    {
        return repository.Context.UpdateWithDiffLogAsync(entity, ignoreAllNullColumns);
    }

    /// <summary>
    /// Update entities and log differences _rep.UpdateWithDiffLogAsync(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <param name="ignoreAllNullColumns"></param>
    /// <returns></returns>
    public static Task<int> UpdateWithDiffLogAsync<T>(this ISqlSugarClient db, T entity, bool ignoreAllNullColumns = true) where T : EntityBase, new()
    {
        return db.Updateable(entity)
            .IgnoreColumns(ignoreAllNullColumns: ignoreAllNullColumns)
            .EnableDiffLogEvent()
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Add an entity and record the difference log _rep.InsertWithDiffLog(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int InsertWithDiffLog<T>(this ISugarRepository repository, T entity) where T : EntityBase, new()
    {
        return repository.Context.InsertWithDiffLog(entity);
    }

    /// <summary>
    /// Add an entity and record the difference log _rep.InsertWithDiffLog(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static int InsertWithDiffLog<T>(this ISqlSugarClient db, T entity) where T : EntityBase, new()
    {
        return db.Insertable(entity).AS().EnableDiffLogEvent().ExecuteCommand();
    }

    /// <summary>
    /// Add an entity and record the difference log _rep.InsertWithDiffLogAsync(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> InsertWithDiffLogAsync<T>(this ISugarRepository repository, T entity) where T : EntityBase, new()
    {
        return repository.Context.InsertWithDiffLogAsync(entity);
    }

    /// <summary>
    /// Add an entity and record the difference log _rep.InsertWithDiffLog(entity)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<int> InsertWithDiffLogAsync<T>(this ISqlSugarClient db, T entity) where T : EntityBase, new()
    {
        return db.Insertable(entity).AS().EnableDiffLogEvent().ExecuteCommandAsync();
    }

    /// <summary>
    /// Multi-database query
    /// </summary>
    /// <param name="queryable"></param>
    /// <returns> </returns>
    public static ISugarQueryable<T> AS<T>(this ISugarQueryable<T> queryable)
    {
        var info = GetTableInfo<T>();
        return queryable.AS<T>($"{info.Item1}.{info.Item2}");
    }

    /// <summary>
    /// Multi-database query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="queryable"></param>
    /// <returns></returns>
    public static ISugarQueryable<T, T2> AS<T, T2>(this ISugarQueryable<T, T2> queryable)
    {
        var info = GetTableInfo<T2>();
        return queryable.AS<T2>($"{info.Item1}.{info.Item2}");
    }

    /// <summary>
    /// Multiple database updates
    /// </summary>
    /// <param name="updateable"></param>
    /// <returns> </returns>
    public static IUpdateable<T> AS<T>(this IUpdateable<T> updateable) where T : EntityBase, new()
    {
        var info = GetTableInfo<T>();
        return updateable.AS($"{info.Item1}.{info.Item2}");
    }

    /// <summary>
    /// New multi-library
    /// </summary>
    /// <param name="insertable"></param>
    /// <returns> </returns>
    public static IInsertable<T> AS<T>(this IInsertable<T> insertable) where T : EntityBase, new()
    {
        var info = GetTableInfo<T>();
        return insertable.AS($"{info.Item1}.{info.Item2}");
    }

    /// <summary>
    /// Delete multiple databases
    /// </summary>
    /// <param name="deleteable"></param>
    /// <returns> </returns>
    public static IDeleteable<T> AS<T>(this IDeleteable<T> deleteable) where T : EntityBase, new()
    {
        var info = GetTableInfo<T>();
        return deleteable.AS($"{info.Item1}.{info.Item2}");
    }

    /// <summary>
    /// Get table information based on entity type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static Tuple<string, string> GetTableInfo<T>()
    {
        var entityType = typeof(T);
        var attr = entityType.GetCustomAttribute<TenantAttribute>();
        var configId = attr == null ? SqlSugarConst.MainConfigId : attr.configId.ToString();
        var tableName = entityType.GetCustomAttribute<SugarTable>().TableName;
        return new Tuple<string, string>(configId, tableName);
    }

    /// <summary>
    /// Disable filter - applies to update and delete operations (only valid for current request, disables the use of asynchronous)
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="action">Disable async</param>
    /// <returns></returns>
    public static void RunWithoutFilter(this ISugarRepository repository, Action action)
    {
        repository.Context.QueryFilter.ClearAndBackup(); // Clear and back up filters
        action.Invoke();
        repository.Context.QueryFilter.Restore(); // Restore filter

        // use case
        //_rep.RunWithoutFilter(() =>
        //{
        //    Perform updates or deletes
        //    Disable the use of asynchronous functions
        //});
    }

    /// <summary>
    /// Ignore tenant
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="ignore">Whether to ignore, default true</param>
    /// <returns> </returns>
    public static ISugarQueryable<T> IgnoreTenant<T>(this ISugarQueryable<T> queryable, bool ignore = true)
    {
        return ignore ? queryable.ClearFilter<ITenantIdFilter>() : queryable;
    }

    /// <summary>
    /// Only update certain columns
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="updateable"></param>
    /// <returns></returns>
    public static IUpdateable<T> OnlyUpdateColumn<T, R>(this IUpdateable<T> updateable) where T : EntityBase, new() where R : class, new()
    {
        if (updateable.UpdateBuilder.UpdateColumns == null)
            updateable.UpdateBuilder.UpdateColumns = new List<string>();

        foreach (PropertyInfo info in typeof(R).GetProperties())
        {
            // Determine whether they have the same attributes
            if (typeof(T).GetProperty(info.Name) != null)
                updateable.UpdateBuilder.UpdateColumns.Add(info.Name);
        }
        return updateable;
    }

    /// <summary>
    /// Navigation updates only certain columns (main table)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="t"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static UpdateNavRootOptions OnlyNavUpdateColumn<T, R>(this T t, R r)
    {
        UpdateNavRootOptions uNOption = new UpdateNavRootOptions();
        var updateColumns = new List<string>();

        foreach (PropertyInfo info in r.GetType().GetProperties())
        {
            //Determine whether they have the same attributes
            PropertyInfo pro = t.GetType().GetProperty(info.Name);
            var attr = pro.GetCustomAttribute<SugarColumn>();
            if (pro != null && attr != null && !attr.IsPrimaryKey)
                updateColumns.Add(info.Name);
        }
        uNOption.UpdateColumns = updateColumns.ToArray();
        return uNOption;
    }

    /// <summary>
    /// Batch list in query
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="exp"></param>
    /// <param name="queryList"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public static async Task<List<T1>> BulkListQuery<T1, T2>(this ISugarQueryable<T1> queryable,
            Expression<Func<T1, SingleColumnEntity<T2>, bool>> exp,
            IEnumerable<T2> queryList,
            CancellationToken stoppingToken) where T1 : class, new()
    {
        // Create a temporary table (use a real table for good compatibility, and the table name is random)
        var tableName = "Temp" + SnowFlakeSingle.Instance.NextId();
        try
        {
            var type = queryable.Context.DynamicBuilder().CreateClass(tableName, new SugarTable())
                .CreateProperty("ColumnName", typeof(string), new SugarColumn() { IsPrimaryKey = true }) // Do not increment the primary key
                .BuilderType();
            // Create table
            queryable.Context.CodeFirst.InitTables(type);
            var insertData = queryList.Select(it => new SingleColumnEntity<T2>() { ColumnName = it }).ToList();
            // Insert into temporary table
            queryable.Context.Fastest<SingleColumnEntity<T2>>()
                .AS(tableName)
                .BulkCopy(insertData);
            var queryTemp = queryable.Context.Queryable<SingleColumnEntity<T2>>()
                .AS(tableName);

            var systemData = await queryable
                .InnerJoin(queryTemp, exp)
                .ToListAsync(stoppingToken);

            queryable.Context.DbMaintenance.DropTable(tableName);
            return systemData;
        }
        catch (Exception error)
        {
            queryable.Context.DbMaintenance.DropTable(tableName);
            throw Oops.Oh(error);
        }
    }
}