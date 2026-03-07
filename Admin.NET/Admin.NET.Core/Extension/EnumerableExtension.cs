// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Data collection extension class
/// </summary>
public static class EnumerableExtension
{
    private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyCache = new();

    /// <summary>
    /// Query data sets with parent-child relationships
    /// </summary>
    /// <param name="list">Dataset</param>
    /// <param name="idExpression">Primary key ID field</param>
    /// <param name="parentIdExpression">parent field</param>
    /// <param name="topParentIdValue">Top node parent field value</param>
    /// <param name="isContainOneself">Whether to include the top-level node itself</param>
    /// <returns></returns>
    public static IEnumerable<T> ToChildList<T, P>(this IEnumerable<T> list,
        Expression<Func<T, P>> idExpression,
        Expression<Func<T, P>> parentIdExpression,
        object topParentIdValue,
        bool isContainOneself = true)
    {
        if (list == null || !list.Any()) return Enumerable.Empty<T>();

        var propId = GetPropertyInfo(idExpression);
        var propParentId = GetPropertyInfo(parentIdExpression);

        // Find all top-level nodes
        var topNodes = list.Where(item => Equals(propId.GetValue(item), topParentIdValue)).ToList();

        return TraverseHierarchy(list, propId, propParentId, topNodes, isContainOneself);
    }

    /// <summary>
    /// Query data sets with parent-child relationships
    /// </summary>
    /// <param name="list">Dataset</param>
    /// <param name="idExpression">Primary key ID field</param>
    /// <param name="parentIdExpression">parent field</param>
    /// <param name="topLevelPredicate">Selection conditions for top-level nodes</param>
    /// <param name="isContainOneself">Whether to include the top-level node itself</param>
    /// <returns></returns>
    public static IEnumerable<T> ToChildList<T, P>(this IEnumerable<T> list,
        Expression<Func<T, P>> idExpression,
        Expression<Func<T, P>> parentIdExpression,
        Expression<Func<T, bool>> topLevelPredicate,
        bool isContainOneself = true)
    {
        if (list == null || !list.Any()) return Enumerable.Empty<T>();

        // Get the top node
        var topNodes = list.Where(topLevelPredicate.Compile()).ToList();

        if (!topNodes.Any()) return Enumerable.Empty<T>();

        var idPropertyInfo = GetPropertyInfo(idExpression);
        var parentPropertyInfo = GetPropertyInfo(parentIdExpression);

        return TraverseHierarchy(list, idPropertyInfo, parentPropertyInfo, topNodes, isContainOneself);
    }

    /// <summary>
    /// Helper method to extract attribute information from expressions and use temporary cache
    /// </summary>
    private static PropertyInfo GetPropertyInfo<T, P>(Expression<Func<T, P>> expression)
    {
        // Use ConcurrentDictionary to ensure thread safety
        return PropertyCache.GetOrAdd(typeof(T).FullName + "." + ((MemberExpression)expression.Body).Member.Name, k =>
        {
            if (expression.Body is UnaryExpression { Operand: MemberExpression member }) return (PropertyInfo)member.Member;
            if (expression.Body is MemberExpression memberExpression) return (PropertyInfo)memberExpression.Member;
            throw Oops.Oh("The expression must be a property accessor: " + expression);
        });
    }

    /// <summary>
    /// Traverse the hierarchy using queues
    /// </summary>
    private static IEnumerable<T> TraverseHierarchy<T>(IEnumerable<T> list,
        PropertyInfo idPropertyInfo,
        PropertyInfo parentPropertyInfo,
        List<T> topNodes,
        bool isContainOneself)
    {
        var queue = new Queue<T>(topNodes);
        var result = new HashSet<T>(topNodes);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            var children = list.Where(item => Equals(parentPropertyInfo.GetValue(item), idPropertyInfo.GetValue(currentNode))).ToList();
            children.Where(child => result.Add(child)).ForEach(child => queue.Enqueue(child));
        }
        if (isContainOneself) return result;

        // Remove top-level nodes themselves if they are not required to be included
        topNodes.ForEach(e => result.Remove(e));

        return result;
    }
}