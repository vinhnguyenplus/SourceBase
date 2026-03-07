// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Generate web page authorization URL
/// </summary>
public class GenAuthUrlInput
{
    /// <summary>
    /// RedirectUrl
    /// </summary>
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Scope
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// State
    /// </summary>
    public string State { get; set; }
}

/// <summary>
/// Get WeChat user OpenId
/// </summary>
public class WechatOAuth2Input
{
    /// <summary>
    /// Code
    /// </summary>
    [Required(ErrorMessage = "Code cannot be empty"), MinLength(10, ErrorMessage = "Codemistake")]
    public string Code { get; set; }
}

/// <summary>
/// WeChat user login
/// </summary>
public class WechatUserLogin
{
    /// <summary>
    /// OpenId
    /// </summary>
    [Required(ErrorMessage = "WeChat ID cannot be empty"), MinLength(10, ErrorMessage = "WeChat logo length error")]
    public string OpenId { get; set; }
}

/// <summary>
/// Get configuration signature
/// </summary>
public class SignatureInput
{
    /// <summary>
    /// Url
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// Get a list of message templates
/// </summary>
public class MessageTemplateSendInput
{
    /// <summary>
    /// Subscription TemplateId
    /// </summary>
    [Required(ErrorMessage = "Subscription template ID cannot be empty")]
    public string TemplateId { get; set; }

    /// <summary>
    /// Receiver's OpenId
    /// </summary>
    [Required(ErrorMessage = "The recipient's OpenId cannot be empty")]
    public string ToUserOpenId { get; set; }

    /// <summary>
    /// Template data, in the format of { "key1": { "value": any }, "key2": { "value": any } }
    /// </summary>
    [Required(ErrorMessage = "Template data cannot be empty")]
    public Dictionary<string, CgibinMessageSubscribeSendRequest.Types.DataItem> Data { get; set; }

    /// <summary>
    /// Template jump link
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The specific page path required to jump to the mini program, supports parameters, (example index?foo=bar)
    /// </summary>
    public string MiniProgramPagePath { get; set; }
}

/// <summary>
/// Delete message template
/// </summary>
public class DeleteMessageTemplateInput
{
    /// <summary>
    /// Subscription TemplateId
    /// </summary>
    [Required(ErrorMessage = "Subscription template ID cannot be empty")]
    public string TemplateId { get; set; }
}

public class UploadAvatarInput
{
    /// <summary>
    /// Mini program user identity
    /// </summary>
    [Required(ErrorMessage = "OpenId cannot be empty")]
    public string OpenId { get; set; }

    /// <summary>
    /// document
    /// </summary>
    [Required]
    public IFormFile File { get; set; }

    /// <summary>
    /// File type
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// file path
    /// </summary>
    public string Path { get; set; }
}

public class SetNickNameInput
{
    /// <summary>
    /// Mini program user identity
    /// </summary>
    [Required(ErrorMessage = "OpenId cannot be empty")]
    public string OpenId { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [Required(ErrorMessage = "Nickname cannot be empty")]
    public string NickName { get; set; }
}