// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Get WeChat user OpenId
/// </summary>
public class JsCode2SessionInput
{
    /// <summary>
    /// JsCode
    /// </summary>
    [Required(ErrorMessage = "JsCode cannot be empty"), MinLength(10, ErrorMessage = "JsCode Error")]
    public string JsCode { get; set; }
}

/// <summary>
/// Get WeChat user phone number
/// </summary>
public class WxPhoneInput : WxOpenIdLoginInput
{
    /// <summary>
    /// Code
    /// </summary>
    [Required(ErrorMessage = "Code cannot be empty"), MinLength(10, ErrorMessage = "Codemistake")]
    public string Code { get; set; }
}

/// <summary>
/// WeChat applet login
/// </summary>
public class WxOpenIdLoginInput
{
    /// <summary>
    /// OpenId
    /// </summary>
    [Required(ErrorMessage = "WeChat ID cannot be empty"), MinLength(10, ErrorMessage = "WeChat identification error")]
    public string OpenId { get; set; }
}

/// <summary>
/// Login with WeChat mobile number
/// </summary>
public class WxPhoneLoginInput
{
    /// <summary>
    /// telephone number
    /// </summary>
    [DataValidation(ValidationTypes.PhoneNumber, ErrorMessage = "Incorrect phone number")]
    public string PhoneNumber { get; set; }
}

/// <summary>
/// Send subscription message
/// </summary>
public class SendSubscribeMessageInput
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
    /// Template content, in the format of { "key1": { "value": any }, "key2": { "value": any } }
    /// </summary>
    [Required(ErrorMessage = "TemplateContent cannot be empty")]
    public Dictionary<string, CgibinMessageSubscribeSendRequest.Types.DataItem> Data { get; set; }

    /// <summary>
    /// Jump applet type
    /// </summary>
    public string MiniprogramState { get; set; }

    /// <summary>
    /// language type
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// The jump page after clicking the template card (only pages within this applet), supports parameters (example pages/app/index?foo=bar)
    /// </summary>
    public string MiniProgramPagePath { get; set; }
}

/// <summary>
/// Add subscription message template
/// </summary>
public class AddSubscribeMessageTemplateInput
{
    /// <summary>
    /// Template TitleId
    /// </summary>
    [Required(ErrorMessage = "Template title Id cannot be empty")]
    public string TemplateTitleId { get; set; }

    /// <summary>
    /// Template keyword list, for example [3,5,4]
    /// </summary>
    [Required(ErrorMessage = "The template keyword list cannot be empty")]
    public List<int> KeyworkIdList { get; set; }

    /// <summary>
    /// Service scenario description, within 15 words
    /// </summary>
    [Required(ErrorMessage = "Service scenario description cannot be empty")]
    public string SceneDescription { get; set; }
}

/// <summary>
/// Generate QR codes for mini programs with parameters (the total number of codes generated is limited to 100,000)
/// </summary>
public class GenerateQRImageInput
{
    /// <summary>
    /// The mini program page path entered by scanning the QR code has a maximum length of 128 characters and cannot be empty; eg: pages/index?id=0001
    /// </summary>
    public string PagePath { get; set; }

    /// <summary>
    /// The name of the file saved
    /// </summary>
    public string ImageName { get; set; }

    /// <summary>
    /// Image width default 430
    /// </summary>
    public int Width { get; set; } = 430;
}

/// <summary>
/// Generate mini program QR code with parameters (obtain unrestricted mini program code)
/// </summary>
public class GenerateQRImageUnLimitInput : GenerateQRImageInput
{
    /// <summary>
    /// Parameters carried by the QR code: eg:a=1 (maximum 32 visible characters, only supports numbers, uppercase and lowercase English and some special characters: <!-- !#$&'()*+,/:;=?@-._~ -->)
    /// </summary>
    public string Scene { get; set; }
}