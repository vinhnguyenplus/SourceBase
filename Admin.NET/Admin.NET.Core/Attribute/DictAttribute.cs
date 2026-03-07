// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Dictionary value compliance checking feature
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class DictAttribute : ValidationAttribute, ITransient
{
    /// <summary>
    /// dictionary encoding
    /// </summary>
    public string DictTypeCode { get; }

    /// <summary>
    /// Whether to allow empty strings
    /// </summary>
    public bool AllowEmptyStrings { get; set; } = false;

    /// <summary>
    /// Allow empty values, only verify if there is a value, default false
    /// </summary>
    public bool AllowNullValue { get; set; } = false;

    /// <summary>
    /// Dictionary value compliance checking feature
    /// </summary>
    /// <param name="dictTypeCode"></param>
    /// <param name="errorMessage"></param>
    public DictAttribute(string dictTypeCode = "", string errorMessage = "The dictionary value is invalid!")
    {
        DictTypeCode = dictTypeCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Dictionary value compliance check
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        // Determine whether null values ​​are allowed
        if (AllowNullValue && value == null) return ValidationResult.Success;

        // Get the type of attribute
        var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
        if (property == null) return new ValidationResult($"Unknown property: {validationContext.MemberName}");

        string importHeaderName = GetImporterHeaderName(property, validationContext.MemberName);

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        // First try to get the service from the dependency injection container of ValidationContext. If it cannot be obtained or the type does not match, then get it from the global App container.
        if (validationContext.GetService(typeof(SysDictDataService)) is not SysDictDataService sysDictDataService)
            sysDictDataService = App.GetRequiredService<SysDictDataService>();

        // Get a list of dictionary values
        var dictDataList = sysDictDataService.GetDataList(DictTypeCode).GetAwaiter().GetResult();

        // Use HashSet to improve search efficiency
        var dictHash = new HashSet<string>(dictDataList.Select(u => u.Value));

        // Determine whether it is a collection type
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // If it is an empty collection and null values ​​are allowed, success will be returned directly.
            if (value == null && AllowNullValue) return ValidationResult.Success;

            // Handle the case when the collection is empty
            var collection = value as IEnumerable;
            if (collection == null) return ValidationResult.Success;

            // Get the element type of the collection
            var elementType = propertyType.GetGenericArguments()[0];
            var underlyingElementType = Nullable.GetUnderlyingType(elementType) ?? elementType;

            // If the element type is an enumeration, verify one by one
            if (underlyingElementType.IsEnum)
            {
                foreach (var item in collection)
                {
                    if (item == null && AllowNullValue) continue;

                    if (!Enum.IsDefined(underlyingElementType, item!))
                        return new ValidationResult($"Hint: {ErrorMessage} | The enum value [{item}] is not a valid [{underlyingElementType.Name}] enum type value!", [importHeaderName]);
                }
                return ValidationResult.Success;
            }

            foreach (var item in collection)
            {
                if (item == null && AllowNullValue) continue;

                var itemString = item?.ToString();
                if (!dictHash.Contains(itemString))
                    return new ValidationResult($"Prompt: {ErrorMessage} | The dictionary [{DictTypeCode}] does not contain [{itemString}]!", [importHeaderName]);
            }

            return ValidationResult.Success;
        }

        var valueAsString = value?.ToString();

        // Whether to ignore empty strings
        if (AllowEmptyStrings && string.IsNullOrEmpty(valueAsString)) return ValidationResult.Success;

        // Enumeration type validation
        if (propertyType.IsEnum)
        {
            if (!Enum.IsDefined(propertyType, value!)) return new ValidationResult($"Prompt: {ErrorMessage}|The enum value [{value}] is not a valid [{propertyType.Name}] enum type value!", [importHeaderName]);
            return ValidationResult.Success;
        }

        if (!dictHash.Contains(valueAsString))
            return new ValidationResult($"Hint: {ErrorMessage}|Dictionary [{DictTypeCode}] does not contain [{valueAsString}]!", [importHeaderName]);

        return ValidationResult.Success;
    }

    /// <summary>
    /// Get the Name in [ImporterHeader(Name = "xxx")] on this field, if not, use defaultName.
    /// Used when importing data from excel, it allows the caller to know which field failed validation instead of throwing an exception.
    /// </summary>
    private static string GetImporterHeaderName(PropertyInfo property, string defaultName)
    {
        var importerHeader = property.GetCustomAttribute<ImporterHeaderAttribute>();
        string importerHeaderName = importerHeader?.Name ?? defaultName;
        return importerHeaderName;
    }
}