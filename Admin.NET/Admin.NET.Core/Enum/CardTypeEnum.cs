// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Document type enumeration
/// </summary>
[Description("ID Type Enumeration")]
public enum CardTypeEnum
{
    /// <summary>
    /// ID card
    /// </summary>
    [Description("ID card")]
    IdCard = 0,

    /// <summary>
    /// passport
    /// </summary>
    [Description("Passport")]
    PassportCard = 1,

    /// <summary>
    /// birth certificate
    /// </summary>
    [Description("Birth Certificate")]
    BirthCard = 2,

    /// <summary>
    /// Hong Kong, Macao and Taiwan Pass
    /// </summary>
    [Description("Hong Kong, Macao and Taiwan Pass")]
    GatCard = 3,

    /// <summary>
    /// Residence permit for foreigners
    /// </summary>
    [Description("Foreigner Residence Permit")]
    ForeignCard = 4,

    /// <summary>
    /// business license
    /// </summary>
    [Description("Business license")]
    License = 5,
}