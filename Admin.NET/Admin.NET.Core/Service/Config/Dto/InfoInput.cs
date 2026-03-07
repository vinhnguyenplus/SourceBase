// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System information save input parameters
/// </summary>
public class InfoSaveInput
{
    /// <summary>
    /// System icon (Data URI scheme base64 encoding)
    /// </summary>
    public string LogoBase64 { get; set; }

    /// <summary>
    /// System icon file name
    /// </summary>
    public string LogoFileName { get; set; }

    /// <summary>
    /// Watermark content
    /// </summary>
    public string Watermark { get; set; }

    /// <summary>
    /// System main title
    /// </summary>
    [Required(ErrorMessage = "System main title cannot be empty")]
    public string Title { get; set; }

    /// <summary>
    /// System subtitle
    /// </summary>
    [Required(ErrorMessage = "systemSubtitle cannot be empty")]
    public string ViceTitle { get; set; }

    /// <summary>
    /// System description
    /// </summary>
    [Required(ErrorMessage = "System description cannot be empty")]
    public string ViceDesc { get; set; }

    /// <summary>
    /// Copyright statement
    /// </summary>
    [Required(ErrorMessage = "Copyright description cannot be empty")]
    public string Copyright { get; set; }

    /// <summary>
    /// ICP registration number
    /// </summary>
    [Required(ErrorMessage = "ICP registration number cannot be empty")]
    public string Icp { get; set; }

    /// <summary>
    /// ICP address
    /// </summary>
    [Required(ErrorMessage = "ICP address cannot be empty")]
    public string IcpUrl { get; set; }

    /// <summary>
    /// Enable registration
    /// </summary>
    public YesNoEnum EnableReg { get; set; }

    /// <summary>
    /// Login two-step verification
    /// </summary>
    public YesNoEnum SecondVer { get; set; }

    /// <summary>
    /// Graphic verification code
    /// </summary>
    public YesNoEnum Captcha { get; set; }

    /// <summary>
    /// Default registration scheme ID
    /// </summary>
    public virtual long RegWayId { get; set; }
}