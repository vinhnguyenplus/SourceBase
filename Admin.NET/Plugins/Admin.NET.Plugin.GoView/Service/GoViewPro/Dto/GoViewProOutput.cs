// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// GoView Item Item
/// </summary>
public class GoViewProItemOutput
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
    /// Project status
    /// </summary>
    public GoViewProStateEnum StateEnum { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Preview image url
    /// </summary>
    public string IndexImage { get; set; }

    /// <summary>
    /// background image url
    /// </summary>
    public string BackGroundImage { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// Project notes
    /// </summary>
    public string Remarks { get; set; }
}

/// <summary>
/// GoView project details
/// </summary>
public class GoViewProDetailOutput : GoViewProItemOutput
{
    /// <summary>
    /// Project content
    /// </summary>
    public string Content { get; set; }
}

/// <summary>
/// GoView new project output
/// </summary>
public class GoViewProCreateOutput
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// GoView upload project output
/// </summary>
public class GoViewProUploadOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string BucketName { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// File size KB
    /// </summary>
    public int FileSize { get; set; }

    /// <summary>
    /// file suffix
    /// </summary>
    public string FileSuffix { get; set; }

    /// <summary>
    /// File Url
    /// </summary>
    [JsonProperty("fileurl")]
    public string FileUrl { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Modifier ID
    /// </summary>
    public long? UpdateUserId { get; set; }
}