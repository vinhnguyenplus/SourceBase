// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Open interface identity input parameters
/// </summary>
public class OpenAccessInput : BasePageInput
{
    /// <summary>
    /// Identity mark
    /// </summary>
    public string AccessKey { get; set; }
}

public class AddOpenAccessInput : SysOpenAccess
{
    /// <summary>
    /// Identity mark
    /// </summary>
    [Required(ErrorMessage = "Identity cannot be empty")]
    public override string AccessKey { get; set; }

    /// <summary>
    /// key
    /// </summary>
    [Required(ErrorMessage = "The key cannot be empty")]
    public override string AccessSecret { get; set; }

    /// <summary>
    /// Bind user ID
    /// </summary>
    [Required(ErrorMessage = "Binding user cannot be empty")]
    public override long BindUserId { get; set; }
}

public class UpdateOpenAccessInput : AddOpenAccessInput
{
}

public class DeleteOpenAccessInput : BaseIdInput
{
}

public class GenerateSignatureInput
{
    /// <summary>
    /// Identity mark
    /// </summary>
    [Required(ErrorMessage = "Identity cannot be empty")]
    public string AccessKey { get; set; }

    /// <summary>
    /// key
    /// </summary>
    [Required(ErrorMessage = "The key cannot be empty")]
    public string AccessSecret { get; set; }

    /// <summary>
    /// Request method
    /// </summary>
    public HttpMethodEnum Method { get; set; }

    /// <summary>
    /// Request interface address
    /// </summary>
    [Required(ErrorMessage = "The request interface address cannot be empty")]
    public string Url { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    [Required(ErrorMessage = "Timestampcannot benull")]
    public long Timestamp { get; set; }

    /// <summary>
    /// random number
    /// </summary>
    [Required(ErrorMessage = "The random number cannot be empty")]
    public string Nonce { get; set; }
}