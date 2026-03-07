// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Data operation type enumeration
/// </summary>
[Description("Data operation type enumeration")]
public enum DataOpTypeEnum
{
    /// <summary>
    /// other
    /// </summary>
    [Description("Other"), Theme("info")]
    Other,

    /// <summary>
    /// Increase
    /// </summary>
    [Description("increase")]
    Add,

    /// <summary>
    /// delete
    /// </summary>
    [Description("Delete")]
    Delete,

    /// <summary>
    /// edit
    /// </summary>
    [Description("Edit")]
    Edit,

    /// <summary>
    /// renew
    /// </summary>
    [Description("Update")]
    Update,

    /// <summary>
    /// Query
    /// </summary>
    [Description("Query")]
    Query,

    /// <summary>
    /// Details
    /// </summary>
    [Description("Details")]
    Detail,

    /// <summary>
    /// Tree
    /// </summary>
    [Description("Tree")]
    Tree,

    /// <summary>
    /// import
    /// </summary>
    [Description("import")]
    Import,

    /// <summary>
    /// Export
    /// </summary>
    [Description("Export")]
    Export,

    /// <summary>
    /// Authorize
    /// </summary>
    [Description("Authorization")]
    Grant,

    /// <summary>
    /// Forced retreat
    /// </summary>
    [Description("Force quit")]
    Force,

    /// <summary>
    /// Clear
    /// </summary>
    [Description("Clear")]
    Clean
}