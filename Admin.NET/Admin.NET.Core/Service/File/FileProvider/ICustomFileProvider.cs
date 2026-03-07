// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Custom file provider interface
/// </summary>
public interface ICustomFileProvider
{
    /// <summary>
    /// Get file stream
    /// </summary>
    /// <param name="sysFile"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public Task<FileStreamResult> GetFileStreamResultAsync(SysFile sysFile, string fileName);

    /// <summary>
    /// Download the specified file in Base64 format
    /// </summary>
    /// <param name="sysFile"></param>
    /// <returns></returns>
    public Task<string> DownloadFileBase64Async(SysFile sysFile);

    /// <summary>
    /// Delete files
    /// </summary>
    /// <param name="sysFile"></param>
    /// <returns></returns>
    public Task DeleteFileAsync(SysFile sysFile);

    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file">document</param>
    /// <param name="sysFile"></param>
    /// <param name="path">File storage location</param>
    /// <param name="finalName">file final name</param>
    /// <returns></returns>
    public Task<SysFile> UploadFileAsync(IFormFile file, SysFile sysFile, string path, string finalName);
}