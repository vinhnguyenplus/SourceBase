// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// File paging query
/// </summary>
public class PageFileInput : BasePageInput
{
    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// file path
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// file suffix
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// start time
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// end time
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// Upload files
/// </summary>
public class UploadFileInput
{
    /// <summary>
    /// document
    /// </summary>
    [Required]
    public IFormFile File { get; set; }

    /// <summary>
    /// File category
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// Is it public?
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Allowed formats: .jpeg.jpg.png.bmp.gif.tif
    /// </summary>
    public string AllowSuffix { get; set; }

    /// <summary>
    /// Specify bucket name
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// Specify storage provider ID
    /// </summary>
    public long? ProviderId { get; set; }

    /// <summary>
    /// Business data ID
    /// </summary>
    public long? DataId { get; set; }
}

/// <summary>
/// Upload file Base64
/// </summary>
public class UploadFileFromBase64Input
{
    /// <summary>
    /// file name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// File content
    /// </summary>
    public string FileDataBase64 { get; set; }

    /// <summary>
    /// File type("image/jpeg",)
    /// </summary>
    public string ContentType { get; set; }
}

/// <summary>
/// Query related query input
/// </summary>
public class RelationQueryInput
{
    /// <summary>
    /// Associated object name
    /// </summary>
    public string RelationName { get; set; }

    /// <summary>
    /// Associated object ID
    /// </summary>
    public long? RelationId { get; set; }

    /// <summary>
    /// File type: multiple separated by ","
    /// </summary>
    public string FileTypes { get; set; }

    /// <summary>
    /// BelongingId
    /// </summary>
    public long? BelongId { get; set; }

    /// <summary>
    /// File type split
    /// </summary>
    /// <returns></returns>
    public string[] GetFileTypeBS()
    {
        return FileTypes.Split(',');
    }
}