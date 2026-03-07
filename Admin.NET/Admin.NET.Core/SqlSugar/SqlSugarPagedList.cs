// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Paginated generic collection
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarPagedList<TEntity>
{
    /// <summary>
    /// page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// page capacity
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// current page collection
    /// </summary>
    public IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    /// Is there a previous page?
    /// </summary>
    public bool HasPrevPage { get; set; }

    /// <summary>
    /// Is there a next page?
    /// </summary>
    public bool HasNextPage { get; set; }
}

/// <summary>
/// Pagination extension class
/// </summary>
public static class SqlSugarPagedExtensions
{
    /// <summary>
    /// Pagination expansion
    /// </summary>
    /// <param name="query"><see cref="ISugarQueryable{TEntity}"/>Object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <param name="expression">Query results Select expression</param>
    /// <returns></returns>
    public static SqlSugarPagedList<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> query, int pageIndex, int pageSize,
        Expression<Func<TEntity, TResult>> expression)
    {
        var total = 0;
        var items = query.ToPageList(pageIndex, pageSize, ref total, expression);
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Pagination expansion
    /// </summary>
    /// <param name="query"><see cref="ISugarQueryable{TEntity}"/>Object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        var total = 0;
        var items = query.ToPageList(pageIndex, pageSize, ref total);
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Pagination expansion
    /// </summary>
    /// <param name="query"><see cref="ISugarQueryable{TEntity}"/>Object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <param name="expression">Query results Select expression</param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TResult>> ToPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> query, int pageIndex, int pageSize,
        Expression<Func<TEntity, TResult>> expression)
    {
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(pageIndex, pageSize, total, expression);
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Pagination expansion
    /// </summary>
    /// <param name="query"><see cref="ISugarQueryable{TEntity}"/>Object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(pageIndex, pageSize, total);
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Desensitized paging expansion
    /// </summary>
    /// <param name="query"><see cref="ISugarQueryable{TEntity}"/>Object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListDataMaskAsync<TEntity>(this ISugarQueryable<TEntity> query, int pageIndex, int pageSize) where TEntity : class
    {
        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(pageIndex, pageSize, total);
        items.ForEach(x => x.MaskSensitiveData());
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Desensitized paging expansion
    /// </summary>
    /// <param name="list">Collection object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedListDataMask<TEntity>(this IEnumerable<TEntity> list, int pageIndex, int pageSize) where TEntity : class
    {
        var total = list.Count();
        var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        items.ForEach(x => x.MaskSensitiveData());
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Pagination expansion
    /// </summary>
    /// <param name="list">Collection object</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(this IEnumerable<TEntity> list, int pageIndex, int pageSize)
    {
        var total = list.Count();
        var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return CreateSqlSugarPagedList(items, total, pageIndex, pageSize);
    }

    /// <summary>
    /// Create a <see cref="SqlSugarPagedList{TEntity}"/> object
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="items">Collection of objects for paginated content</param>
    /// <param name="total">Total number of items</param>
    /// <param name="pageIndex">Current page number, starting from 1</param>
    /// <param name="pageSize">Page capacity</param>
    /// <returns></returns>
    private static SqlSugarPagedList<TEntity> CreateSqlSugarPagedList<TEntity>(IEnumerable<TEntity> items, int total, int pageIndex, int pageSize)
    {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0;
        return new SqlSugarPagedList<TEntity>
        {
            Page = pageIndex,
            PageSize = pageSize,
            Items = items,
            Total = total,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPrevPage = pageIndex - 1 > 0
        };
    }
}