// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Common interface parameter verification feature class
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CommonValidationAttribute : ValidationAttribute
{
    private readonly Dictionary<string, string> _conditions;
    private static readonly Dictionary<string, Delegate> CompiledConditions = new();

    /// <summary>
    /// </summary>
    /// <param name="conditionPairs">Conditional parameters, the length must be an even number<br/>
    /// Odd string parameters: dynamic conditions<br/>
    /// Even string parameters: prompt message
    /// </param>
    /// <example>
    /// <code lang="C">
    /// public class ModelInput {
    ///
    ///
    ///     public string A { get; set; }
    ///
    ///
    ///     [CommonValidation(
    ///         "A == 1 <value>&amp;&amp;</value> B == null", "When A == 1, B cannot be null",
    ///         "C == 2 <value>&amp;&amp;</value> B == null", "When C == 2, B cannot be null"
    ///     )]
    ///     public string B { get; set; }
    /// }
    /// </code>
    /// </example>
    public CommonValidationAttribute(params string[] conditionPairs)
    {
        if (conditionPairs.Length % 2 != 0) throw new ArgumentException("Conditions must be provided in the form of an even number of strings.");

        var conditions = new Dictionary<string, string>();
        for (int i = 0; i < conditionPairs.Length; i += 2)
            conditions.Add(conditionPairs[i], conditionPairs[i + 1]);

        _conditions = conditions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        foreach (var (expr, errorMessage) in _conditions)
        {
            var conditionKey = $"{validationContext.ObjectType.FullName}.{expr}";
            if (!CompiledConditions.TryGetValue(conditionKey, out var condition))
            {
                condition = CreateCondition(validationContext.ObjectType, expr);
                CompiledConditions[conditionKey] = condition;
            }

            if ((bool)condition.DynamicInvoke(validationContext.ObjectInstance)!)
            {
                return new ValidationResult(errorMessage ?? $"[{validationContext.MemberName}] verification failed");
            }
        }

        return ValidationResult.Success;
    }

    private Delegate CreateCondition(Type modelType, string expression)
    {
        try
        {
            // Create parameter expression
            var parameter = Expression.Parameter(typeof(object), "x");

            // Build a lambda expression
            var lambda = DynamicExpressionParser.ParseLambda(new[] { Expression.Parameter(modelType, "x") }, typeof(bool), expression);

            // Create a new Lambda expression that accepts an object parameter and invokes the compiled expression
            var invokeExpression = Expression.Invoke(lambda, Expression.Convert(parameter, modelType));
            var finalLambda = Expression.Lambda<Func<object, bool>>(invokeExpression, parameter);

            return finalLambda.Compile();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Cannot parse expression '{expression}': {ex.Message}", ex);
        }
    }
}