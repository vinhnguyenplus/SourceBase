// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Enumeration value compliance checking feature
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
public class EnumAttribute : ValidationAttribute, ITransient
{
    /// <summary>
    /// Enumeration value compliance checking feature
    /// </summary>
    /// <param name="errorMessage"></param>
    public EnumAttribute(string errorMessage = "The enumerated value is invalid!")
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Enum value compliance check
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Get the type of attribute
        var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
        if (property == null)
            return new ValidationResult($"Unknown property: {validationContext.MemberName}");

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        // Check if the property type is an enum or a nullable enum type
        if (!propertyType.IsEnum)
            return new ValidationResult($"The attribute type '{validationContext.MemberName}' is not a valid enumeration type!");

        // Check if enumeration value is valid
        if (value == null && Nullable.GetUnderlyingType(property.PropertyType) == null)
            return new ValidationResult($"Tip: {ErrorMessage}|The enumeration value cannot be null!");

        if (value != null && !Enum.IsDefined(propertyType, value))
            return new ValidationResult($"Prompt: {ErrorMessage}|The enum value [{value}] is not a valid [{propertyType.Name}] enum type value!");

        return ValidationResult.Success;
    }
}