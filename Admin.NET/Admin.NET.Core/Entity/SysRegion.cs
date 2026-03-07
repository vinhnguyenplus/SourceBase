// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System administrative area table
/// </summary>
[SugarTable(null, "System administrative area table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc, IsUnique = true)]
public partial class SysRegion : EntityBaseId
{
    /// <summary>
    /// ParentId
    /// </summary>
    [SugarColumn(ColumnDescription = "Parent ID")]
    public long Pid { get; set; }

    /// <summary>
    /// name
    /// </summary>
    [SugarColumn(ColumnDescription = "name", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string Name { get; set; }

    /// <summary>
    /// abbreviation
    /// </summary>
    [SugarColumn(ColumnDescription = "abbreviation", Length = 32)]
    [MaxLength(32)]
    public string? ShortName { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [SugarColumn(ColumnDescription = "Group name", Length = 64)]
    [MaxLength(64)]
    public string? MergerName { get; set; }

    /// <summary>
    /// administrative code
    /// </summary>
    [SugarColumn(ColumnDescription = "Administrative code", Length = 32)]
    [MaxLength(32)]
    public string? Code { get; set; }

    /// <summary>
    /// postal code
    /// </summary>
    [SugarColumn(ColumnDescription = "Postal code", Length = 6)]
    [MaxLength(6)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// area code
    /// </summary>
    [SugarColumn(ColumnDescription = "Area code", Length = 6)]
    [MaxLength(6)]
    public string? CityCode { get; set; }

    /// <summary>
    /// Hierarchy
    /// </summary>
    [SugarColumn(ColumnDescription = "Hierarchy")]
    public int Level { get; set; }

    /// <summary>
    /// Pinyin
    /// </summary>
    [SugarColumn(ColumnDescription = "Pinyin", Length = 128)]
    [MaxLength(128)]
    public string? PinYin { get; set; }

    /// <summary>
    /// longitude
    /// </summary>
    [SugarColumn(ColumnDescription = "longitude")]
    public float Lng { get; set; }

    /// <summary>
    /// Dimensions
    /// </summary>
    [SugarColumn(ColumnDescription = "Dimension")]
    public float Lat { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 128)]
    [MaxLength(128)]
    public string? Remark { get; set; }

    /// <summary>
    /// Institutional child
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRegion> Children { get; set; }
}