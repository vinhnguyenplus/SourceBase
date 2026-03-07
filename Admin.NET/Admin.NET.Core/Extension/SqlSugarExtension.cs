// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.Text.Json;

namespace Admin.NET.Core;

/// <summary>
/// Sqlsugar dynamic query extension method
/// </summary>
public static class SqlSugarExtension
{
    public static ISugarQueryable<T> SearchBy<T>(this ISugarQueryable<T> queryable, BaseFilter filter)
    {
        return queryable.SearchByKeyword(filter.Keyword)
                .AdvancedSearch(filter.Search)
                .AdvancedFilter(filter.Filter);
    }

    public static ISugarQueryable<T> SearchByKeyword<T>(this ISugarQueryable<T> queryable, string keyword)
    {
        return queryable.AdvancedSearch(new Search { Keyword = keyword });
    }

    public static ISugarQueryable<T> AdvancedSearch<T>(this ISugarQueryable<T> queryable, Search search)
    {
        if (!string.IsNullOrWhiteSpace(search?.Keyword))
        {
            var paramExpr = Expression.Parameter(typeof(T));

            Expression right = Expression.Constant(false);

            if (search.Fields?.Any() is true)
            {
                foreach (string field in search.Fields)
                {
                    MemberExpression propertyExpr = GetPropertyExpression(field, paramExpr);

                    var left = AddSearchPropertyByKeyword<T>(propertyExpr, search.Keyword);

                    right = Expression.Or(left, right);
                }
            }
            else
            {
                var properties = typeof(T).GetProperties()
                         .Where(prop => Nullable.GetUnderlyingType(prop.PropertyType) == null
                             && !prop.PropertyType.IsEnum
                             && Type.GetTypeCode(prop.PropertyType) != TypeCode.Object);

                foreach (var property in properties)
                {
                    var propertyExpr = Expression.Property(paramExpr, property);

                    var left = AddSearchPropertyByKeyword<T>(propertyExpr, search.Keyword);

                    right = Expression.Or(left, right);
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(right, paramExpr);

            return queryable.Where(lambda);
        }

        return queryable;
    }

    public static ISugarQueryable<T> AdvancedFilter<T>(this ISugarQueryable<T> queryable, Filter filter)
    {
        if (filter is not null)
        {
            var parameter = Expression.Parameter(typeof(T));

            Expression binaryExpresioFilter;

            if (filter.Logic.HasValue)
            {
                if (filter.Filters is null) throw new ArgumentException("The Filters attribute is required when declaring a logic");
                binaryExpresioFilter = CreateFilterExpression(filter.Logic.Value, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                binaryExpresioFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator.Value, filterValid.Value, parameter);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(binaryExpresioFilter, parameter);

            return queryable.Where(lambda);
        }
        return queryable;
    }

    private static Expression CombineFilter(
           FilterLogicEnum filterLogic,
           Expression bExpresionBase,
           Expression bExpresion)
    {
        return filterLogic switch
        {
            FilterLogicEnum.And => Expression.AndAlso(bExpresionBase, bExpresion),
            FilterLogicEnum.Or => Expression.OrElse(bExpresionBase, bExpresion),
            FilterLogicEnum.Xor => Expression.ExclusiveOr(bExpresionBase, bExpresion),
            _ => throw new ArgumentException("FilterLogic is not valid.", nameof(filterLogic)),
        };
    }

    private static Filter GetValidFilter(Filter filter)
    {
        if (string.IsNullOrEmpty(filter.Field)) throw new ArgumentException("The field attribute is required when declaring a filter");
        if (filter.Operator.IsNullOrEmpty()) throw new ArgumentException("The Operator attribute is required when declaring a filter");
        return filter;
    }

    private static Expression CreateFilterExpression(
        FilterLogicEnum filterLogic,
        IEnumerable<Filter> filters,
        ParameterExpression parameter)
    {
        Expression filterExpression = default!;

        foreach (var filter in filters)
        {
            Expression bExpresionFilter;

            if (filter.Logic.HasValue)
            {
                if (filter.Filters is null) throw new ArgumentException("The Filters attribute is required when declaring a logic");
                bExpresionFilter = CreateFilterExpression(filter.Logic.Value, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                bExpresionFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator.Value, filterValid.Value, parameter);
            }

            filterExpression = filterExpression is null ? bExpresionFilter : CombineFilter(filterLogic, filterExpression, bExpresionFilter);
        }

        return filterExpression;
    }

    private static Expression CreateFilterExpression(
        string field,
        FilterOperatorEnum filterOperator,
        object? value,
        ParameterExpression parameter)
    {
        var propertyExpresion = GetPropertyExpression(field, parameter);
        var valueExpresion = GeValuetExpression(field, value, propertyExpresion.Type);
        return CreateFilterExpression(propertyExpresion, valueExpresion, filterOperator);
    }

    private static Expression CreateFilterExpression(
        MemberExpression memberExpression,
        ConstantExpression constantExpression,
        FilterOperatorEnum filterOperator)
    {
        return filterOperator switch
        {
            FilterOperatorEnum.EQ => Expression.Equal(memberExpression, constantExpression),
            FilterOperatorEnum.NEQ => Expression.NotEqual(memberExpression, constantExpression),
            FilterOperatorEnum.LT => Expression.LessThan(memberExpression, constantExpression),
            FilterOperatorEnum.LTE => Expression.LessThanOrEqual(memberExpression, constantExpression),
            FilterOperatorEnum.GT => Expression.GreaterThan(memberExpression, constantExpression),
            FilterOperatorEnum.GTE => Expression.GreaterThanOrEqual(memberExpression, constantExpression),
            FilterOperatorEnum.Contains => Expression.Call(memberExpression, nameof(FilterOperatorEnum.Contains), null, constantExpression),
            FilterOperatorEnum.StartsWith => Expression.Call(memberExpression, nameof(FilterOperatorEnum.StartsWith), null, constantExpression),
            FilterOperatorEnum.EndsWith => Expression.Call(memberExpression, nameof(FilterOperatorEnum.EndsWith), null, constantExpression),
            _ => throw new ArgumentException("Filter Operator is not valid."),
        };
    }

    private static string GetStringFromJsonElement(object value)
    {
        if (value is JsonElement) return ((JsonElement)value).GetString()!;
        if (value is string) return (string)value;
        return value?.ToString();
    }

    private static ConstantExpression GeValuetExpression(
          string field,
          object? value,
          Type propertyType)
    {
        if (value == null) return Expression.Constant(null, propertyType);

        if (propertyType.IsEnum)
        {
            string? stringEnum = GetStringFromJsonElement(value);

            if (!Enum.TryParse(propertyType, stringEnum, true, out object? valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }
        if (propertyType == typeof(long) || propertyType == typeof(long?))
        {
            string? stringLong = GetStringFromJsonElement(value);

            if (!long.TryParse(stringLong, out long valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(Guid))
        {
            string? stringGuid = GetStringFromJsonElement(value);

            if (!Guid.TryParse(stringGuid, out Guid valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(string))
        {
            string? text = GetStringFromJsonElement(value);

            return Expression.Constant(text, propertyType);
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            string? text = GetStringFromJsonElement(value);
            return Expression.Constant(ChangeType(text, propertyType), propertyType);
        }

        return Expression.Constant(ChangeType(((JsonElement)value).GetRawText(), propertyType), propertyType);
    }

    private static dynamic? ChangeType(object value, Type conversion)
    {
        var t = conversion;

        if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            if (value == null)
            {
                return null;
            }

            t = Nullable.GetUnderlyingType(t);
        }

        return Convert.ChangeType(value, t!);
    }

    private static MemberExpression GetPropertyExpression(
            string propertyName,
            ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;
        foreach (string member in propertyName.Split('.'))
        {
            propertyExpression = Expression.PropertyOrField(propertyExpression, member);
        }

        return (MemberExpression)propertyExpression;
    }

    private static Expression AddSearchPropertyByKeyword<T>(
          Expression propertyExpr,
            string keyword,
            FilterOperatorEnum operatorSearch = FilterOperatorEnum.Contains)
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        ConstantExpression constant = Expression.Constant(keyword);

        MethodInfo method = operatorSearch switch
        {
            FilterOperatorEnum.Contains => typeof(string).GetMethod(nameof(FilterOperatorEnum.Contains), new Type[] { typeof(string) }),
            FilterOperatorEnum.StartsWith => typeof(string).GetMethod(nameof(FilterOperatorEnum.StartsWith), new Type[] { typeof(string) }),
            FilterOperatorEnum.EndsWith => typeof(string).GetMethod(nameof(FilterOperatorEnum.EndsWith), new Type[] { typeof(string) }),
            _ => throw new ArgumentException("Filter Operator is not valid."),
        };

        Expression selectorExpr =
               property.PropertyType == typeof(string)
                   ? propertyExpr
                   : Expression.Condition(
                       Expression.Equal(Expression.Convert(propertyExpr, typeof(object)), Expression.Constant(null, typeof(object))),
                       Expression.Constant(null, typeof(string)),
                       Expression.Call(propertyExpr, "ToString", null, null));

        return Expression.Call(selectorExpr, method, constant);
    }

    #region ViewOperation

    /// <summary>
    /// Get the mapping SQL statement used to create the view
    /// </summary>
    /// <param name="queryable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToMappedSqlString<T>(this ISugarQueryable<T> queryable) where T : class
    {
        ArgumentNullException.ThrowIfNull(queryable);

        // Get entity mapping information
        var entityInfo = queryable.Context.EntityMaintenance.GetEntityInfo(typeof(T));
        if (entityInfo?.Columns == null || entityInfo.Columns.Count == 0) return queryable.ToSqlString();

        // Build a mapping of field names that need to be replaced (only fields with actual differences are processed)
        var nameMap = entityInfo.Columns
            .Where(c => !string.Equals(c.PropertyName, c.DbColumnName, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(k => k.PropertyName.ToLower(), v => v.DbColumnName, StringComparer.OrdinalIgnoreCase);
        if (nameMap.Count == 0) return queryable.ToSqlString();

        // Precompiled regular expressions improve performance
        var sql = queryable.ToSqlString();
        foreach (var kv in nameMap)
        {
            sql = Regex.Replace(sql, $@"\b{kv.Key}\b", kv.Value ?? kv.Key, RegexOptions.IgnoreCase | RegexOptions.Compiled); // word boundary matching
        }
        return sql;
    }

    #endregion ViewOperation

    /// <summary>
    /// Convert list to tree structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">List data</param>
    /// <param name="childrenSelector">Set the list of child nodes. For example: item => item.Children</param>
    /// <param name="parentIdSelector">Sets the element's parent ID. For example: item => item.ParentId</param>
    /// <param name="rootParentId">The parent ID of the root node, default is 0 </param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<T> ToTree<T>(
       this IEnumerable<T> source,
       Func<T, List<T>> childrenSelector,
       Func<T, long> parentIdSelector,
       int rootParentId)
       where T : class
    {
        var lookup = source.ToLookup(parentIdSelector);
        List<T> BuildTree(long parentId)
        {
            return lookup[parentId].Select(item =>
            {
                var children = BuildTree(GetId(item));
                childrenSelector(item).Clear();
                childrenSelector(item).AddRange(children);
                return item;
            }).ToList();
        }

        // It is necessary to provide a method to obtain the ID. You can use reflection or pass parameters yourself.
        long GetId(T item)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) throw new Exception("Id attribute not found");
            return (long)prop.GetValue(item);
        }

        return BuildTree(rootParentId);
    }
}