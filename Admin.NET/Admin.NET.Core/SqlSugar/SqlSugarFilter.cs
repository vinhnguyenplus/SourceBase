// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public static class SqlSugarFilter
{
    /// <summary>
    /// Caching global query filters (memory cache)
    /// </summary>
    private static readonly ICache Cache = NewLife.Caching.Cache.Default;

    private static readonly SysOrgService SysOrgService = App.GetRequiredService<SysOrgService>();
    private static readonly SysCacheService SysCacheService = App.GetRequiredService<SysCacheService>();

    /// <summary>
    /// Delete user organization cache
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbConfigId"></param>
    public static void DeleteUserOrgCache(long userId, string dbConfigId)
    {
        // Delete user organization collection cache
        SysCacheService.Remove($"{CacheConst.KeyUserOrg}{userId}");
        // Delete maximum data permission cache
        SysCacheService.Remove($"{CacheConst.KeyRoleMaxDataScope}{userId}");
        // User permission cache (button collection)
        SysCacheService.Remove($"{CacheConst.KeyUserButton}{userId}");
    }

    /// <summary>
    /// Delete custom filter cache
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbConfigId"></param>
    public static void DeleteCustomCache(long userId, string dbConfigId)
    {
        // Delete custom cache - filter
        Cache.Remove($"db:{dbConfigId}:custom:{userId}");
    }

    /// <summary>
    /// Configure user organization collection filters
    /// </summary>
    public static void SetOrgEntityFilter(SqlSugarScopeProvider db)
    {
        // If there is only personal data, return directly
        var maxDataScope = SetDataScopeFilter(db);
        // Get the user's maximum data range. If it is all data or only the user, skip it.
        if (maxDataScope is 0 or (int)DataScopeEnum.Self or (int)DataScopeEnum.All) return;

        // Obtain the organization to which the user belongs, ensuring the same scope
        var orgIds = new List<long>();
        Scoped.Create((factory, scope) =>
        {
            var services = scope.ServiceProvider;
            orgIds = services.GetRequiredService<SysOrgService>().GetUserOrgIdList().GetAwaiter().GetResult();
        });
        if (orgIds == null || orgIds.Count == 0) return;

        //Configure institution ID filter
        db.QueryFilter.AddTableFilter<IOrgIdFilter>(o => SqlFunc.ContainsArray(orgIds, o.OrgId));
    }

    /// <summary>
    /// Configure user only data filter
    /// </summary>
    private static int SetDataScopeFilter(SqlSugarScopeProvider db)
    {
        var maxDataScope = (int)DataScopeEnum.All;

        long.TryParse(App.HttpContext?.User.FindFirst(ClaimConst.UserId)?.Value, out var userId);
        if (userId <= 0) return maxDataScope;

        // Obtain the maximum data range of the user---only personal data
        maxDataScope = App.GetRequiredService<SysCacheService>().Get<int>(CacheConst.KeyRoleMaxDataScope + userId);
        // If it is 0, get the user organization collection and create a cache.
        if (maxDataScope == 0)
        {
            // Obtain the organization to which the user belongs, ensuring the same scope
            Scoped.Create((factory, scope) =>
            {
                SysOrgService.GetUserOrgIdList().GetAwaiter().GetResult();
                maxDataScope = SysCacheService.Get<int>(CacheConst.KeyRoleMaxDataScope + userId);
            });
        }
        if (maxDataScope != (int)DataScopeEnum.Self) return maxDataScope;

        // Configure user data range cache
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:dataScope:{userId}";
        var dataScopeFilter = Cache.Get<ConcurrentDictionary<Type, LambdaExpression>>(cacheKey);
        if (dataScopeFilter == null)
        {
            // Get business entity data table
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && (u.IsSubclassOf(typeof(EntityBaseOrg)) || u.IsSubclassOf(typeof(EntityBaseOrgDel))));
            if (!entityTypes.Any()) return maxDataScope;

            dataScopeFilter = new ConcurrentDictionary<Type, LambdaExpression>();
            foreach (var entityType in entityTypes)
            {
                // Exclude non-current database entities
                var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                if ((tAtt != null && db.CurrentConnectionConfig.ConfigId.ToString() != tAtt.configId.ToString()))
                    continue;

                //var lambda = DynamicExpressionParser.ParseLambda(new[] {
                //    Expression.Parameter(entityType, "u") }, typeof(bool), $"u.{nameof(EntityBaseData.CreateUserId)}=@0", userId);
                var lambda = entityType.GetConditionExpression<OwnerUserAttribute>(new List<long> { userId });

                db.QueryFilter.AddTableFilter(entityType, lambda);
                dataScopeFilter.TryAdd(entityType, lambda);
            }
            Cache.Add(cacheKey, dataScopeFilter);
        }
        else
        {
            foreach (var filter in dataScopeFilter)
                db.QueryFilter.AddTableFilter(filter.Key, filter.Value);
        }
        return maxDataScope;
    }

    /// <summary>
    /// Configure custom filters
    /// </summary>
    public static void SetCustomEntityFilter(SqlSugarScopeProvider db)
    {
        // Configure custom cache
        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:custom:{userId}";
        var tableFilterItemList = Cache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            // Get custom entity filter
            var entityFilterTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(IEntityFilter))));
            if (!entityFilterTypes.Any()) return;

            var tableFilterItems = new List<TableFilterItem<object>>();
            foreach (var entityFilter in entityFilterTypes)
            {
                var instance = Activator.CreateInstance(entityFilter);
                var entityFilterMethod = entityFilter.GetMethod("AddEntityFilter");
                var entityFilters = ((IList)entityFilterMethod?.Invoke(instance, null))?.Cast<object>();
                if (entityFilters == null) continue;

                foreach (var u in entityFilters)
                {
                    var tableFilterItem = (TableFilterItem<object>)u;
                    var entityType = tableFilterItem.GetType().GetProperty("type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tableFilterItem, null) as Type;
                    // Exclude non-current database entities
                    var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                    if ((tAtt != null && db.CurrentConnectionConfig.ConfigId.ToString() != tAtt.configId.ToString()) ||
                        (tAtt == null && db.CurrentConnectionConfig.ConfigId.ToString() != SqlSugarConst.MainConfigId))
                        continue;

                    tableFilterItems.Add(tableFilterItem);
                    db.QueryFilter.Add(tableFilterItem);
                }
            }
            Cache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }
}

/// <summary>
/// Custom entity filter interface
/// </summary>
public interface IEntityFilter
{
    /// <summary>
    /// Entity filter
    /// </summary>
    /// <returns></returns>
    IEnumerable<TableFilterItem<object>> AddEntityFilter();
}

///// <summary>
///// Custom business entity filter example
///// </summary>
//public class TestEntityFilter : IEntityFilter
//{
//    public IEnumerable<TableFilterItem<object>> AddEntityFilter()
//    {
//        // Construct a filter with custom conditions
//        Expression<Func<SysUser, bool>> dynamicExpression = u => u.Remark.Contains("xxx");
//        var tableFilterItem = new TableFilterItem<object>(typeof(SysUser), dynamicExpression);

//        return new[]
//        {
//            tableFilterItem
//        };
//    }
//}