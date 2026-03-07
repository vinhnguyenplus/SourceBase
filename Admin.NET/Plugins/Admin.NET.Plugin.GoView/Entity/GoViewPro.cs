// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView;

/// <summary>
/// GoView project table
/// </summary>
[SugarTable(null, "GoView project table")]
[SysTable]
public class GoViewPro : EntityBaseTenant
{
    /// <summary>
    /// Project name
    /// </summary>
    [SugarColumn(ColumnDescription = "Project Name", Length = 64)]
    [Required, MaxLength(64)]
    public string ProjectName { get; set; }

    /// <summary>
    /// Project status
    /// </summary>
    [SugarColumn(ColumnDescription = "Project status")]
    public GoViewProStateEnum StateEnum { get; set; } = GoViewProStateEnum.UnPublish;

    /// <summary>
    /// Preview imageUrl
    /// </summary>
    [SugarColumn(ColumnDescription = "Preview Image URL", Length = 1024)]
    [MaxLength(1024)]
    public string? IndexImage { get; set; }

    /// <summary>
    /// Project notes
    /// </summary>
    [SugarColumn(ColumnDescription = "Project Notes", Length = 512)]
    [MaxLength(512)]
    public string? Remarks { get; set; }

    ///// <summary>
    ///// project data
    ///// </summary>
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //[Navigate(NavigateType.OneToOne, nameof(Id))]
    //public GoViewProData GoViewProData { get; set; }
}