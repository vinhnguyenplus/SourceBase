// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Sub-table operation warehousing interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISqlSugarRepository<T> : ISugarRepository, ISimpleClient<T> where T : class, new()
{
    /// <summary>
    /// Create data
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(T input);

    /// <summary>
    /// Create data in batches
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(List<T> input);

    /// <summary>
    /// Update data
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(T input);

    /// <summary>
    /// Update data in batches
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(List<T> input);

    /// <summary>
    /// Delete data
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(T input);

    /// <summary>
    /// Delete data in batches
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(List<T> input);

    /// <summary>
    /// Get the first article
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<T> SplitTableGetFirstAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// Determine whether it exists
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> SplitTableIsAnyAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// Get list
    /// </summary>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync();

    /// <summary>
    /// Get list
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// Get list
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="tableNames">table name</param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression, string[] tableNames);
}