// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class DefaultFileProvider : ICustomFileProvider, ITransient
{
    /// <summary>
    /// Full physical path to build file
    /// </summary>
    /// <param name="sysFile"></param>
    /// <returns></returns>
    private string BuildFullFilePath(SysFile sysFile)
    {
        return Path.Combine(App.WebHostEnvironment.WebRootPath, sysFile.FilePath ?? "", $"{sysFile.Id}{sysFile.Suffix}");
    }

    /// <summary>
    /// Full physical path to the build directory
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    private string BuildFullDirectoryPath(string relativePath)
    {
        return Path.Combine(App.WebHostEnvironment.WebRootPath, relativePath);
    }

    /// <summary>
    /// Make sure the directory exists
    /// </summary>
    /// <param name="directoryPath"></param>
    private void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }

    public Task DeleteFileAsync(SysFile sysFile)
    {
        var filePath = BuildFullFilePath(sysFile);
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task<string> DownloadFileBase64Async(SysFile sysFile)
    {
        var realFile = BuildFullFilePath(sysFile);
        if (!File.Exists(realFile))
        {
            Log.Error($"DownloadFileBase64: File [{realFile}] does not exist");
            throw Oops.Oh($"File [{sysFile.FilePath}] does not exist");
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(realFile);
        return Convert.ToBase64String(fileBytes);
    }

    public Task<FileStreamResult> GetFileStreamResultAsync(SysFile sysFile, string fileName)
    {
        var fullPath = BuildFullFilePath(sysFile);
        return Task.FromResult(new FileStreamResult(new FileStream(fullPath, FileMode.Open), "application/octet-stream")
        {
            FileDownloadName = fileName + sysFile.Suffix
        });
    }

    public async Task<SysFile> UploadFileAsync(IFormFile file, SysFile newFile, string path, string finalName)
    {
        newFile.Provider = ""; // Local Storage Provider appears empty

        var directoryPath = BuildFullDirectoryPath(path);
        EnsureDirectoryExists(directoryPath);

        var realFile = Path.Combine(directoryPath, finalName);
        await using var stream = File.Create(realFile);
        await file.CopyToAsync(stream);

        newFile.Url = $"{newFile.FilePath}/{newFile.Id + newFile.Suffix}";
        return newFile;
    }
}