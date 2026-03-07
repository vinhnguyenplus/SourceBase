// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Filter logical operators
/// </summary>
[Description("Filter logical operators")]
public enum FilterOperatorEnum
{
    /// <summary>
    /// equal to(=)
    /// </summary>
    [Description("equals")]
    EQ,

    /// <summary>
    /// Not equal to (!=)
    /// </summary>
    [Description("not equal to")]
    NEQ,

    /// <summary>
    /// Less than <![CDATA[ < ]]>
    /// </summary>
    [Description("less than")]
    LT,

    /// <summary>
    /// Less than or equal to <![CDATA[ <= ]]>
    /// </summary>
    [Description("less than or equal to")]
    LTE,

    /// <summary>
    /// Greater than (>)
    /// </summary>
    [Description("Greater than")]
    GT,

    /// <summary>
    /// Greater than or equal to (>=)
    /// </summary>
    [Description("greater than or equal to")]
    GTE,

    /// <summary>
    /// start containing
    /// </summary>
    [Description("start containing")]
    StartsWith,

    /// <summary>
    /// Contains at the end
    /// </summary>
    [Description("Contains at the end")]
    EndsWith,

    /// <summary>
    /// Include
    /// </summary>
    [Description("include")]
    Contains
}