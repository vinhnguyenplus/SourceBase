// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Aliyun.OSS.Util;
using Furion.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Admin.NET.Core.Service;

/// <summary>
/// System file service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 410, Description = "System files")]
public class SysFileService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly IConfiguration _configuration;
    private readonly string _imageType = ".jpeg.jpg.png.bmp.gif.tif";
    private readonly INamedServiceProvider<ICustomFileProvider> _namedServiceProvider;
    private readonly ICustomFileProvider _customFileProvider;

    public SysFileService(UserManager userManager,
        SqlSugarRepository<SysFile> sysFileRep,
        IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions,
        INamedServiceProvider<ICustomFileProvider> namedServiceProvider,
        IConfiguration configuration)
    {
        _namedServiceProvider = namedServiceProvider;
        _userManager = userManager;
        _sysFileRep = sysFileRep;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        _configuration = configuration;

        // Simplify provider selection logic
        if (_OSSProviderOptions.Enabled || _configuration["MultiOSS:Enabled"].ToBoolean())
        {
            // Unified use of MultiOSSFileProvider to handle all OSS situations
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(MultiOSSFileProvider));
        }
        else if (_configuration["SSHProvider:Enabled"].ToBoolean())
        {
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(SSHFileProvider));
        }
        else
        {
            _customFileProvider = _namedServiceProvider.GetService<ITransient>(nameof(DefaultFileProvider));
        }
    }

    /// <summary>
    /// Get file paging list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get paginated list of files")]
    public async Task<SqlSugarPagedList<SysFile>> Page(PageFileInput input)
    {
        // Get all public attachments
        var publicList = _sysFileRep.AsQueryable().ClearFilter().Where(u => u.IsPublic == true);
        // Get private attachments
        var privateList = _sysFileRep.AsQueryable().Where(u => u.IsPublic == false);
        // Merge public and private attachments and paginate
        return await _sysFileRep.Context.UnionAll(publicList, privateList)
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName), u => u.FileName.Contains(input.FileName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.FilePath), u => u.FilePath.Contains(input.FilePath.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Upload file Base64 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Upload file Base64")]
    public async Task<SysFile> UploadFileFromBase64(UploadFileFromBase64Input input)
    {
        var pattern = @"data:(?<type>.+?);base64,(?<data>[^""]+)";
        var regex = new Regex(pattern, RegexOptions.Compiled);
        var match = regex.Match(input.FileDataBase64);

        byte[] fileData = Convert.FromBase64String(match.Groups["data"].Value);
        var contentType = match.Groups["type"].Value;
        if (string.IsNullOrEmpty(input.FileName))
            input.FileName = $"{YitIdHelper.NextId()}.{contentType.AsSpan(contentType.LastIndexOf('/') + 1)}";

        using var ms = new MemoryStream();
        ms.Write(fileData);
        ms.Seek(0, SeekOrigin.Begin);
        IFormFile formFile = new FormFile(ms, 0, fileData.Length, "file", input.FileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        var uploadFileInput = input.Adapt<UploadFileInput>();
        uploadFileInput.File = formFile;
        return await UploadFile(uploadFileInput);
    }

    /// <summary>
    /// Upload multiple files 🔖
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [DisplayName("Upload Multiple Files")]
    public async Task<List<SysFile>> UploadFiles([Required] List<IFormFile> files)
    {
        var fileList = new List<SysFile>();
        foreach (var file in files)
        {
            var uploadedFile = await UploadFile(new UploadFileInput { File = file });
            fileList.Add(uploadedFile);
        }
        return fileList;
    }

    /// <summary>
    /// Download based on file Id or Url 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Download based on file Id or Url")]
    public async Task<IActionResult> DownloadFile(SysFile input)
    {
        var file = input.Id > 0 ? await GetFile(input.Id) : await _sysFileRep.CopyNew().GetFirstAsync(u => u.Url == input.Url);
        var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));
        return await GetFileStreamResult(file, fileName);
    }

    /// <summary>
    /// File preview 🔖
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("File preview")]
    public async Task<IActionResult> GetPreview([FromRoute] long id)
    {
        var file = await GetFile(id);
        //var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));
        return await GetFileStreamResult(file, file.Id + "");
    }

    /// <summary>
    /// Get file stream
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private async Task<IActionResult> GetFileStreamResult(SysFile file, string fileName)
    {
        return await _customFileProvider.GetFileStreamResultAsync(file, fileName);
    }

    /// <summary>
    /// Get file stream
    /// </summary>
    [NonAction]
    public async Task<Stream> GetFileStream(SysFile file)
    {
        var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));
        var result = await _customFileProvider.GetFileStreamResultAsync(file, fileName);
        return result.FileStream;
    }

    /// <summary>
    /// Download the specified file in Base64 format 🔖
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [DisplayName("Download the specified file in Base64 format")]
    public async Task<string> DownloadFileBase64([FromBody] string url)
    {
        var sysFile = await _sysFileRep.CopyNew().GetFirstAsync(u => u.Url == url) ?? throw Oops.Oh($"File does not exist");
        return await _customFileProvider.DownloadFileBase64Async(sysFile);
    }

    /// <summary>
    /// Delete files 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete files")]
    public async Task DeleteFile(BaseIdInput input)
    {
        var file = await _sysFileRep.GetByIdAsync(input.Id) ?? throw Oops.Oh($"File does not exist");
        await _sysFileRep.DeleteAsync(file);
        await _customFileProvider.DeleteFileAsync(file);
    }

    /// <summary>
    /// Update file 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("update file")]
    public async Task UpdateFile(SysFile input)
    {
        var isExist = await _sysFileRep.IsAnyAsync(u => u.Id == input.Id);
        if (!isExist) throw Oops.Oh(ErrorCodeEnum.D8000);

        await _sysFileRep.UpdateAsync(input);
    }

    /// <summary>
    /// Get files 🔖
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("Get file")]
    public async Task<SysFile> GetFile([FromQuery] long id)
    {
        var file = await _sysFileRep.CopyNew().GetByIdAsync(id);
        return file ?? throw Oops.Oh(ErrorCodeEnum.D8000);
    }

    /// <summary>
    /// Get files based on file ID collection 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("Get files based on the collection of file IDs")]
    public async Task<List<SysFile>> GetFileByIds([FromQuery][FlexibleArray<long>] List<long> ids)
    {
        return await _sysFileRep.AsQueryable().Where(u => ids.Contains(u.Id)).ToListAsync();
    }

    /// <summary>
    /// Get file path 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get file path")]
    public async Task<List<TreeNode>> GetFolder()
    {
        // Optimization: Obtain unique file paths directly at the database level
        var folders = await _sysFileRep.AsQueryable()
            .Select(u => u.FilePath)
            .Distinct()
            .ToListAsync();

        var pathTreeBuilder = new PathTreeBuilder();
        var tree = pathTreeBuilder.BuildTree(folders);
        return tree.Children;
    }

    /// <summary>
    /// Upload files 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <param name="targetPath">Store target path</param>
    /// <returns></returns>
    [DisplayName("Upload File")]
    public async Task<SysFile> UploadFile([FromForm] UploadFileInput input, [BindNever] string targetPath = "")
    {
        if (input.File == null || input.File.Length <= 0) throw Oops.Oh(ErrorCodeEnum.D8000);

        if (input.File.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) throw Oops.Oh(ErrorCodeEnum.D8005);

        // Determine whether uploaded files are repeated
        var sizeKb = input.File.Length / 1024; // Size KB
        var fileMd5 = string.Empty;
        if (_uploadOptions.EnableMd5)
        {
            await using (var fileStream = input.File.OpenReadStream())
            {
                fileMd5 = OssUtils.ComputeContentMd5(fileStream, fileStream.Length);
            }
            // If encoding other than utf8mb4_general_ci is used in Mysql8, an error will occur. Try to avoid using .ToString() in conditions.
            // Because Squsugar does not convert variables into strings to construct SQL statements, but constructs statements such as CAST(123 AS CHAR), so the return value is utf8mb4_general_ci, so it is prone to errors.
            var sysFile = await _sysFileRep.GetFirstAsync(u => u.FileMd5 == fileMd5 && u.SizeKb == sizeKb);
            if (sysFile != null) return sysFile;
        }

        // Verify file type
        if (!_uploadOptions.ContentType.Contains(input.File.ContentType)) throw Oops.Oh($"{ErrorCodeEnum.D8001}:{input.File.ContentType}");

        // Verify file size
        if (sizeKb > _uploadOptions.MaxSize) throw Oops.Oh($"{ErrorCodeEnum.D8002}, maximum allowed: {_uploadOptions.MaxSize}KB");

        // Get file suffix
        var suffix = Path.GetExtension(input.File.FileName).ToLower(); // suffix
        if (string.IsNullOrWhiteSpace(suffix))
            suffix = string.Concat(".", input.File.ContentType.AsSpan(input.File.ContentType.LastIndexOf('/') + 1));
        if (!string.IsNullOrWhiteSpace(suffix))
        {
            //var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
            //suffix = contentTypeProvider.Mappings.FirstOrDefault(u => u.Value == file.ContentType).Key;
            // Modify the .jpeg, jpe suffix returned by image/jpeg type
            if (suffix == ".jpeg" || suffix == ".jpe")
                suffix = ".jpg";
        }
        if (string.IsNullOrWhiteSpace(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);

        // Prevent clients from forging file types
        if (!string.IsNullOrWhiteSpace(input.AllowSuffix) && !input.AllowSuffix.Contains(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);
        //if (!VerifyFileExtensionName.IsSameType(file.OpenReadStream(), suffix)) throw Oops.Oh(ErrorCodeEnum.D8001);

        // File storage location
        var path = string.IsNullOrWhiteSpace(targetPath) ? _uploadOptions.Path : targetPath;
        path = path.ParseToDateTimeForRep();

        var newFile = input.Adapt<SysFile>();
        newFile.Id = YitIdHelper.NextId();

        // Priority is given to the user-specified bucket name. If not specified, the default configuration is used.
        if (!string.IsNullOrEmpty(input.BucketName))
        {
            newFile.BucketName = input.BucketName;
        }
        else
        {
            // MultiOSSFileProvider will automatically use the default configuration
            newFile.BucketName = _OSSProviderOptions.Enabled ? _OSSProviderOptions.Bucket : "Local";
        }

        newFile.FileName = Path.GetFileNameWithoutExtension(input.File.FileName);
        newFile.Suffix = suffix;
        newFile.SizeKb = sizeKb;
        newFile.FilePath = path;
        newFile.FileMd5 = fileMd5;
        newFile.DataId = input.DataId;

        var finalName = newFile.Id + suffix; // file final name

        newFile = await _customFileProvider.UploadFileAsync(input.File, newFile, path, finalName);
        await _sysFileRep.AsInsertable(newFile).ExecuteCommandAsync();
        return newFile;
    }

    /// <summary>
    /// Upload avatar 🔖
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [DisplayName("Upload Avatar")]
    public async Task<SysFile> UploadAvatar([Required] IFormFile file)
    {
        var sysFile = await UploadFile(new UploadFileInput { File = file, AllowSuffix = _imageType }, "upload/avatar");

        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = await sysUserRep.GetByIdAsync(_userManager.UserId);
        await sysUserRep.UpdateAsync(u => new SysUser() { Avatar = sysFile.Url }, u => u.Id == user.Id);
        // Delete existing avatar files
        if (!string.IsNullOrWhiteSpace(user.Avatar))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Avatar);
            if (long.TryParse(fileId, out var id))
            {
                try
                {
                    await DeleteFile(new BaseIdInput { Id = id });
                }
                catch
                {
                    // Ignore the error of deleting old avatar files and do not affect the upload of new avatars
                }
            }
        }

        return sysFile;
    }

    /// <summary>
    /// Upload electronic signature 🔖
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [DisplayName("Upload electronic signature")]
    public async Task<SysFile> UploadSignature([Required] IFormFile file)
    {
        var sysFile = await UploadFile(new UploadFileInput { File = file, AllowSuffix = _imageType }, "upload/signature");

        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = await sysUserRep.GetByIdAsync(_userManager.UserId);
        // Delete existing electronic signature files
        if (!string.IsNullOrWhiteSpace(user.Signature) && user.Signature.EndsWith(".png"))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Signature);
            if (long.TryParse(fileId, out var id))
            {
                try
                {
                    await DeleteFile(new BaseIdInput { Id = id });
                }
                catch
                {
                    // Ignore the error of deleting old signature files and do not affect the upload of new signatures
                }
            }
        }
        await sysUserRep.UpdateAsync(u => new SysUser() { Signature = sysFile.Url }, u => u.Id == user.Id);
        return sysFile;
    }

    #region uniteoneEntity and file associationtime，Business application entities only need to be definedonepieceSysFileCollection NavigationAttribute，BusinessincreaseandUpdate、DeletepointsJust don't call it

    /// <summary>
    /// Update the business data ID of the file
    /// </summary>
    /// <param name="dataId"></param>
    /// <param name="sysFiles"></param>
    /// <returns></returns>
    [NonAction]
    public async Task UpdateFileByDataId(long dataId, List<SysFile> sysFiles)
    {
        var newFileIds = sysFiles.Select(u => u.Id).ToList();

        // Find the file ID difference and delete it (invalid file)
        var tmpFiles = await _sysFileRep.GetListAsync(u => u.DataId == dataId);
        var tmpFileIds = tmpFiles.Select(u => u.Id).ToList();
        var deleteFileIds = tmpFileIds.Except(newFileIds);
        foreach (var fileId in deleteFileIds)
            await DeleteFile(new BaseIdInput() { Id = fileId });

        await _sysFileRep.UpdateAsync(u => new SysFile() { DataId = dataId }, u => newFileIds.Contains(u.Id));
    }

    /// <summary>
    /// Delete files corresponding to business data
    /// </summary>
    /// <param name="dataId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task DeleteFileByDataId(long dataId)
    {
        // Delete redundant and invalid physical files
        var tmpFiles = await _sysFileRep.GetListAsync(u => u.DataId == dataId);
        foreach (var file in tmpFiles)
            await _customFileProvider.DeleteFileAsync(file);
        await _sysFileRep.AsDeleteable().Where(u => u.DataId == dataId).ExecuteCommandAsync();
    }

    #endregion uniteoneEntity and file associationtime，Business application entities only need to be definedonepieceSysFileCollection NavigationAttribute，BusinessincreaseandUpdate、DeletepointsJust don't call it
}