// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// User login parameters
/// </summary>
public class LoginInput
{
    /// <summary>
    /// account
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "Account cannot be empty"), MinLength(2, ErrorMessage = "The account cannot be less than 2 characters")]
    public string Account { get; set; }

    /// <summary>
    /// password
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "Password cannot be empty"), MinLength(3, ErrorMessage = "The password cannot be less than 3 characters")]
    public string Password { get; set; }

    /// <summary>
    /// tenant
    /// </summary>
    public long? TenantId { get; set; }

    /// <summary>
    /// Verification codeId
    /// </summary>
    public long CodeId { get; set; }

    /// <summary>
    /// Verification code
    /// </summary>
    public string Code { get; set; }
}

public class LoginPhoneInput
{
    /// <summary>
    /// phone number
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "The mobile phone number cannot be empty")]
    [DataValidation(ValidationTypes.PhoneNumber, ErrorMessage = "The phone number is incorrect")]
    public string Phone { get; set; }

    /// <summary>
    /// Verification code
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "The verification code cannot be empty"), MinLength(4, ErrorMessage = "The verification code must be at least 4 characters")]
    public string Code { get; set; }

    /// <summary>
    /// tenant
    /// </summary>
    [Required(ErrorMessage = "Tenant cannot be empty")]
    public long? TenantId { get; set; }
}

/// <summary>
/// User registration input parameters
/// </summary>
public class UserRegistrationInput
{
    /// <summary>
    /// real name
    /// </summary>
    [Required(ErrorMessage = "Real name cannot be empty"), MinLength(2, ErrorMessage = "The real name must be at least 2 characters long")]
    public string RealName { get; set; }

    /// <summary>
    /// account
    /// </summary>
    [Required(ErrorMessage = "Account cannot be empty"), MinLength(6, ErrorMessage = "Account number cannot be less than 6 characters")]
    public string Account { get; set; }

    /// <summary>
    /// phone number
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "The mobile phone number cannot be empty")]
    [DataValidation(ValidationTypes.PhoneNumber, ErrorMessage = "The phone number is incorrect")]
    public string Phone { get; set; }

    /// <summary>
    /// Verification code
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "The verification code cannot be empty")]
    public string Code { get; set; }

    /// <summary>
    /// Verification codeId
    /// </summary>
    public long CodeId { get; set; }

    /// <summary>
    /// tenant
    /// </summary>
    [Required(ErrorMessage = "Tenant cannot be empty")]
    public long TenantId { get; set; }

    /// <summary>
    /// password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Registration plan
    /// </summary>
    public long WayId { get; set; }
}