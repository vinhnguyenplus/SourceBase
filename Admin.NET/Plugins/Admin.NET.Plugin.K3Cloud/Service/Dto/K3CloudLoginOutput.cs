// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.K3Cloud.Service;

public class K3CloudLoginOutput
{
    public string Message { get; set; }
    public string MessageCode { get; set; }
    public ErpLoginResultTypeEnum LoginResultType { get; set; }
}

public enum ErpLoginResultTypeEnum
{
    /// <summary>
    /// activation
    /// </summary>
    Activation = -7,

    /// <summary>
    /// Cloud Pass is not bound to a Cloud account
    /// </summary>
    EntryCloudUnBind = -6,

    /// <summary>
    /// Form processing required
    /// </summary>
    DealWithForm = -5,

    /// <summary>
    /// Login warning
    /// </summary>
    Wanning = -4,

    /// <summary>
    /// Password verification failed (mandatory)
    /// </summary>
    PWInvalid_Required = -3,

    /// <summary>
    /// Password verification failed (optional)
    /// </summary>
    PWInvalid_Optional = -2,

    /// <summary>
    /// Login failed
    /// </summary>
    Failure = -1,

    /// <summary>
    /// Wrong user or password
    /// </summary>
    PWError = 0,

    /// <summary>
    /// Login successful
    /// </summary>
    Success = 1
}