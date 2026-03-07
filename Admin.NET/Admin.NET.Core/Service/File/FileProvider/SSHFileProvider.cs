// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class SSHFileProvider : ICustomFileProvider, ITransient
{
    /// <summary>
    /// Create SSH connection assistant
    /// </summary>
    /// <returns></returns>
    private SSHHelper CreateSSHHelper()
    {
        return new SSHHelper(
            App.Configuration["SSHProvider:Host"],
            App.Configuration["SSHProvider:Port"].ToInt(),
            App.Configuration["SSHProvider:Username"],
            App.Configuration["SSHProvider:Password"]);
    }

    /// <summary>
    /// Build file full path
    /// </summary>
    /// <param name="sysFile"></param>
    /// <returns></returns>
    private string BuildFilePath(SysFile sysFile)
    {
        return string.Concat(sysFile.FilePath, "/", sysFile.Id + sysFile.Suffix);
    }

    public Task DeleteFileAsync(SysFile sysFile)
    {
        var fullPath = BuildFilePath(sysFile);
        using var helper = CreateSSHHelper();
        helper.DeleteFile(fullPath);
        return Task.CompletedTask;
    }

    public Task<string> DownloadFileBase64Async(SysFile sysFile)
    {
        using var helper = CreateSSHHelper();
        return Task.FromResult(Convert.ToBase64String(helper.ReadAllBytes(sysFile.FilePath)));
    }

    public Task<FileStreamResult> GetFileStreamResultAsync(SysFile sysFile, string fileName)
    {
        var filePath = BuildFilePath(sysFile);
        using var helper = CreateSSHHelper();
        return Task.FromResult(new FileStreamResult(helper.OpenRead(filePath), "application/octet-stream")
        {
            FileDownloadName = fileName + sysFile.Suffix
        });
    }

    public Task<SysFile> UploadFileAsync(IFormFile file, SysFile sysFile, string path, string finalName)
    {
        var fullPath = string.Concat(path.StartsWith('/') ? path : "/" + path, "/", finalName);
        using var helper = CreateSSHHelper();
        helper.UploadFile(file.OpenReadStream(), fullPath);
        return Task.FromResult(sysFile);
    }
}