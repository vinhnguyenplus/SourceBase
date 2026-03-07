// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView;

/// <summary>
/// GoView Project Data Sheet
/// </summary>
[SugarTable(null, "GoView Project Data Sheet")]
[SysTable]
public class GoViewProData : EntityBaseTenant
{
    /// <summary>
    /// Project content
    /// </summary>
    [SugarColumn(ColumnDescription = "Project Content", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Content { get; set; }

    /// <summary>
    /// Preview picture
    /// </summary>
    [SugarColumn(ColumnDescription = "Preview picture", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? IndexImageData { get; set; }

    /// <summary>
    /// background image
    /// </summary>
    [SugarColumn(ColumnDescription = "background image", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? BackGroundImageData { get; set; }
}