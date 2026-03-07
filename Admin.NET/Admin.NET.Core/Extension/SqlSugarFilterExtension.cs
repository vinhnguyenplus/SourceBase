// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public static class SqlSugarFilterExtension
{
    /// <summary>
    /// Get attributes based on specified Attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    private static List<string> GetPropertyNames<T>(this Type type) where T : Attribute
    {
        return type.GetProperties()
            .Where(p => p.CustomAttributes.Any(x => x.AttributeType == typeof(T)))
            .Select(x => x.Name).ToList();
    }

    /// <summary>
    /// Get filter expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="owners"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type, List<long> owners) where T : Attribute
    {
        var fieldNames = type.GetPropertyNames<T>();
        ParameterExpression parameter = Expression.Parameter(type, "c");

        Expression right = Expression.Constant(false);
        ConstantExpression ownersCollection = Expression.Constant(owners);
        foreach (var fieldName in fieldNames)
        {
            var property = type.GetProperty(fieldName);
            Expression memberExp = Expression.Property(parameter, property!);

            // If the property is a nullable type, cast to its underlying type
            var baseType = Nullable.GetUnderlyingType(property.PropertyType);
            if (baseType != null) memberExp = Expression.Convert(memberExp, baseType);

            // Call the ownersCollection.Contains method to check whether the attribute value is included
            right = Expression.OrElse(Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                new[] { memberExp.Type },
                ownersCollection,
                memberExp
            ), right);
        }
        return Expression.Lambda(right, parameter);
    }
}