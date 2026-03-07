// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// GoView new items
/// </summary>
public class GoViewProCreateInput
{
    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Project notes
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// Preview image url
    /// </summary>
    public string IndexImage { get; set; }
}

/// <summary>
/// GoView edit project
/// </summary>
public class GoViewProEditInput
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Preview image url
    /// </summary>
    public string IndexImage { get; set; }
}

/// <summary>
/// GoView modifies project release status
/// </summary>
public class GoViewProPublishInput
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project status
    /// </summary>
    public GoViewProStateEnum StateEnum { get; set; }
}

/// <summary>
/// GoView saves project data
/// </summary>
public class GoViewProSaveDataInput
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Project content
    /// </summary>
    public string Content { get; set; }
}