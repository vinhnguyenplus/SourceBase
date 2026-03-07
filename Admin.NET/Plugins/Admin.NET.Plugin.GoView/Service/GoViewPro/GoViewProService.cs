// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// Project Management Services 🧩
/// </summary>
[UnifyProvider("GoView")]
[ApiDescriptionSettings(GoViewConst.GroupName, Module = "goview", Name = "project", Order = 100, Description = "Project Management")]
public class GoViewProService : IDynamicApiController
{
    private readonly SqlSugarRepository<GoViewPro> _goViewProRep;
    private readonly SqlSugarRepository<GoViewProData> _goViewProDataRep;

    public GoViewProService(SqlSugarRepository<GoViewPro> goViewProjectRep,
        SqlSugarRepository<GoViewProData> goViewProjectDataRep)
    {
        _goViewProRep = goViewProjectRep;
        _goViewProDataRep = goViewProjectDataRep;
    }

    /// <summary>
    /// Get project list 🔖
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [DisplayName("Get project list")]
    public async Task<List<GoViewProItemOutput>> GetList([FromQuery] int page = 1, [FromQuery] int limit = 12)
    {
        var res = await _goViewProRep.AsQueryable()
            .Select(u => new GoViewProItemOutput(), true)
            .ToPagedListAsync(page, limit);
        return res.Items.ToList();
    }

    /// <summary>
    /// New items 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Create")]
    [DisplayName("Add new project")]
    public async Task<GoViewProCreateOutput> Create(GoViewProCreateInput input)
    {
        var project = await _goViewProRep.AsInsertable(input.Adapt<GoViewPro>()).ExecuteReturnEntityAsync();
        return new GoViewProCreateOutput
        {
            Id = project.Id
        };
    }

    /// <summary>
    /// Modify project 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Modify project")]
    public async Task Edit(GoViewProEditInput input)
    {
        await _goViewProRep.AsUpdateable(input.Adapt<GoViewPro>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete item 🔖
    /// </summary>
    [ApiDescriptionSettings(Name = "Delete")]
    [DisplayName("Delete Project")]
    [UnitOfWork]
    public async Task Delete([FromQuery] string ids)
    {
        var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(u => Convert.ToInt64(u)).ToList();
        await _goViewProRep.AsDeleteable().Where(u => idList.Contains(u.Id)).ExecuteCommandAsync();
        await _goViewProDataRep.AsDeleteable().Where(u => idList.Contains(u.Id)).ExecuteCommandAsync();
    }

    /// <summary>
    /// Modify publishing status 🔖
    /// </summary>
    [HttpPut]
    [DisplayName("Modify Publish Status")]
    public async Task Publish(GoViewProPublishInput input)
    {
        await _goViewProRep.AsUpdateable()
            .SetColumns(u => new GoViewPro
            {
                StateEnum = input.StateEnum
            })
            .Where(u => u.Id == input.Id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// Get project data 🔖
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [ApiDescriptionSettings(Name = "GetData")]
    [DisplayName("Obtain project data")]
    public async Task<GoViewProDetailOutput> GetData([FromQuery] long projectId)
    {
        var projectData = await _goViewProDataRep.GetByIdAsync(projectId);
        if (projectData == null) return null;

        var project = await _goViewProRep.GetByIdAsync(projectId);
        var projectDetail = project.Adapt<GoViewProDetailOutput>();
        projectDetail.Content = projectData.Content;

        return projectDetail;
    }

    /// <summary>
    /// Save project data 🔖
    /// </summary>
    [ApiDescriptionSettings(Name = "save/data")]
    [DisplayName("Save project data")]
    public async Task SaveData([FromForm] GoViewProSaveDataInput input)
    {
        if (await _goViewProDataRep.IsAnyAsync(u => u.Id == input.ProjectId))
        {
            await _goViewProDataRep.AsUpdateable()
                .SetColumns(u => new GoViewProData
                {
                    Content = input.Content
                })
                .Where(u => u.Id == input.ProjectId)
                .ExecuteCommandAsync();
        }
        else
        {
            await _goViewProDataRep.InsertAsync(new GoViewProData
            {
                Id = input.ProjectId,
                Content = input.Content,
            });
        }
    }

    /// <summary>
    /// Upload preview image 🔖
    /// </summary>
    [DisplayName("Upload preview image")]
    public async Task<GoViewProUploadOutput> Upload(IFormFile @object)
    {
        /*
         * Front-end logic（useSync.hook.ts of dataSyncUpdate Method）：
         * If FileUrl Not empty，Use FileUrl
         * nothen use GetOssInfo InterfaceObtainarrived BucketUrl and FileName Perform splicing
         */

        // File name format example 13414795568325_index_preview.png
        var fileNameSplit = @object.FileName.Split('_');
        var idStr = fileNameSplit[0];
        if (!long.TryParse(idStr, out var id)) return new GoViewProUploadOutput();

        // Convert preview image to Base64
        var ms = new MemoryStream();
        await @object.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // save
        if (await _goViewProDataRep.IsAnyAsync(u => u.Id == id))
        {
            await _goViewProDataRep.AsUpdateable()
                .SetColumns(u => new GoViewProData
                {
                    IndexImageData = base64Image
                })
                .Where(u => u.Id == id)
                .ExecuteCommandAsync();
        }
        else
        {
            await _goViewProDataRep.InsertAsync(new GoViewProData
            {
                Id = id,
                IndexImageData = base64Image,
            });
        }

        var output = new GoViewProUploadOutput
        {
            Id = id,
            BucketName = null,
            CreateTime = null,
            CreateUserId = null,
            FileName = null,
            FileSize = 0,
            FileSuffix = "png",
            FileUrl = $"api/goview/project/getIndexImage/{id}",
            UpdateTime = null,
            UpdateUserId = null
        };

        #region Use SysFileService Method（Commented）

        ////Delete existing preview image
        //var uploadFileName = Path.GetFileNameWithoutExtension(@object.FileName);
        //var existFiles = await _fileRep.GetListAsync(u => u.FileName == uploadFileName);
        //foreach (var f in existFiles)
        //    await _fileService.DeleteFile(new DeleteFileInput { Id = f.Id });

        ////save preview
        //var result = await _fileService.UploadFile(@object, "");
        //var file = await _fileRep.GetByIdAsync(result.Id);
        //int.TryParse(file.SizeKb, out var size);

        ////local storage, using spliced ​​addresses
        //var fileUrl = file.BucketName == "Local" ? $"{file.FilePath}/{file.Id}{file.Suffix}" : file.Url;

        //var output = new ProjectUploadOutput
        //{
        //    Id = file.Id,
        //    BucketName = file.BucketName,
        //    CreateTime = file.CreateTime,
        //    CreateUserId = file.CreateUserId,
        //    FileName = $"{file.FileName}{file.Suffix}",
        //    FileSize = size,
        //    FileSuffix = file.Suffix?[1..],
        //    FileUrl = fileUrl,
        //    UpdateTime = null,
        //    UpdateUserId = null
        //};

        #endregion Use SysFileService Method（Commented）

        return output;
    }

    /// <summary>
    /// Get preview 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [NonUnify]
    [ApiDescriptionSettings(Name = "GetIndexImage")]
    [DisplayName("Get preview")]
    public async Task<IActionResult> GetIndexImage(long id)
    {
        var projectData = await _goViewProDataRep.AsQueryable().IgnoreColumns(u => u.Content).FirstAsync(u => u.Id == id);
        if (projectData?.IndexImageData == null)
            return new NoContentResult();

        var bytes = Convert.FromBase64String(projectData.IndexImageData);
        return new FileStreamResult(new MemoryStream(bytes), "image/png");
    }

    /// <summary>
    /// Upload background image
    /// </summary>
    [DisplayName("Upload background image")]
    public async Task<GoViewProUploadOutput> UploadBackGround(IFormFile @object)
    {
        // File name format example 13414795568325_index_preview.png
        var fileNameSplit = @object.FileName.Split('_');
        var idStr = fileNameSplit[0];
        if (!long.TryParse(idStr, out var id)) return new GoViewProUploadOutput();

        // Convert preview image to Base64
        var ms = new MemoryStream();
        await @object.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // save
        if (await _goViewProDataRep.IsAnyAsync(u => u.Id == id))
        {
            await _goViewProDataRep.AsUpdateable()
                .SetColumns(u => new GoViewProData
                {
                    BackGroundImageData = base64Image
                })
                .Where(u => u.Id == id)
                .ExecuteCommandAsync();
        }
        else
        {
            await _goViewProDataRep.InsertAsync(new GoViewProData
            {
                Id = id,
                BackGroundImageData = base64Image,
            });
        }

        var output = new GoViewProUploadOutput
        {
            Id = id,
            BucketName = null,
            CreateTime = null,
            CreateUserId = null,
            FileName = null,
            FileSize = 0,
            FileSuffix = "png",
            FileUrl = $"api/goview/project/getBackGroundImage/{id}",
            UpdateTime = null,
            UpdateUserId = null
        };

        return output;
    }

    /// <summary>
    /// Get background image
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [NonUnify]
    [ApiDescriptionSettings(Name = "GetBackGroundImage")]
    [DisplayName("Get background image")]
    public async Task<IActionResult> GetBackGroundImage(long id)
    {
        var projectData = await _goViewProDataRep.AsQueryable().IgnoreColumns(u => u.Content).FirstAsync(u => u.Id == id);
        if (projectData?.BackGroundImageData == null)
            return new NoContentResult();

        var bytes = Convert.FromBase64String(projectData.BackGroundImageData);
        return new FileStreamResult(new MemoryStream(bytes), "image/png");
    }
}