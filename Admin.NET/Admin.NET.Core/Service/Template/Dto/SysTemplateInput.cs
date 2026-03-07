// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class PageTemplateInput : BasePageInput
{
    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// template type
    /// </summary>
    public TemplateTypeEnum? Type { get; set; }

    /// <summary>
    /// TenantId
    /// </summary>
    public long TenantId { get; set; }
}

/// <summary>
/// Added template input parameters
/// </summary>
public class AddTemplateInput : SysTemplate
{
    /// <summary>
    /// name
    /// </summary>
    [Required(ErrorMessage = "Name cannot be empty")]
    public override string Name { get; set; }

    /// <summary>
    /// template type
    /// </summary>
    [Enum]
    public override TemplateTypeEnum Type { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    [Required(ErrorMessage = "Coding cannot be empty")]
    public override string Code { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [Required(ErrorMessage = "Group name cannot be empty")]
    public override string GroupName { get; set; }

    /// <summary>
    /// Template content
    /// </summary>
    [Required(ErrorMessage = "Content name cannot be empty")]
    public override string Content { get; set; }
}

/// <summary>
/// Update template input parameters
/// </summary>
public class UpdateTemplateInput : AddTemplateInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Id cannot be empty")]
    [DataValidation(ValidationTypes.Numeric)]
    public override long Id { get; set; }
}

/// <summary>
/// Preview template input parameters
/// </summary>
public class ProViewTemplateInput : BaseIdInput
{
    /// <summary>
    /// Rendering parameters
    /// </summary>
    [Required(ErrorMessage = "Rendering parameters cannot be empty")]
    public object Data { get; set; }
}

/// <summary>
/// Template rendering input parameters
/// </summary>
public class RenderTemplateInput
{
    /// <summary>
    /// Template content
    /// </summary>
    [Required(ErrorMessage = "Content name cannot be empty")]
    public string Content { get; set; }

    /// <summary>
    /// Rendering parameters
    /// </summary>
    [Required(ErrorMessage = "Rendering parameters cannot be empty")]
    public object Data { get; set; }
}