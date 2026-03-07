// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Role data range enumeration
/// </summary>
[Description("Role Data Scope Enumeration")]
public enum DataScopeEnum
{
    /// <summary>
    /// All data
    /// </summary>
    [Description("All data")]
    All = 1,

    /// <summary>
    /// This department and the following data
    /// </summary>
    [Description("This department and the following data")]
    DeptChild = 2,

    /// <summary>
    /// Data of this department
    /// </summary>
    [Description("Department data")]
    Dept = 3,

    /// <summary>
    /// Only personal data
    /// </summary>
    [Description("Only personal data")]
    Self = 4,

    /// <summary>
    /// custom data
    /// </summary>
    [Description("custom data")]
    Define = 5
}