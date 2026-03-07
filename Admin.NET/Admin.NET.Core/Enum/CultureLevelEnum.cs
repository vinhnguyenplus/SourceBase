// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Education level enumeration
/// </summary>
[Description("Educational Level Enumeration")]
public enum CultureLevelEnum
{
    /// <summary>
    /// other
    /// </summary>
    [Description("Other"), Theme("info")]
    Level0 = 0,

    /// <summary>
    /// illiteracy
    /// </summary>
    [Description("illiteracy")]
    Level1 = 1,

    /// <summary>
    /// primary school
    /// </summary>
    [Description("primary school")]
    Level2 = 2,

    /// <summary>
    /// junior high school
    /// </summary>
    [Description("junior high school")]
    Level3 = 3,

    /// <summary>
    /// Ordinary high school
    /// </summary>
    [Description("General high school")]
    Level4 = 4,

    /// <summary>
    /// technical school
    /// </summary>
    [Description("Technical school")]
    Level5 = 5,

    /// <summary>
    /// Vocational education
    /// </summary>
    [Description("Vocational education")]
    Level6 = 6,

    /// <summary>
    /// vocational high school
    /// </summary>
    [Description("vocational high school")]
    Level7 = 7,

    /// <summary>
    /// Secondary college
    /// </summary>
    [Description("inWaiting for a specialized subject")]
    Level8 = 8,

    /// <summary>
    /// College
    /// </summary>
    [Description("College")]
    Level9 = 9,

    /// <summary>
    /// undergraduate
    /// </summary>
    [Description("Bachelor's degree")]
    Level10 = 10,

    /// <summary>
    /// Master's degree
    /// </summary>
    [Description("Master's degree student")]
    Level11 = 11,

    /// <summary>
    /// PhD candidate
    /// </summary>
    [Description("doctoral student")]
    Level12 = 12,
}