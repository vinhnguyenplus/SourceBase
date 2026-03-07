// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Excel fileActionResult
/// </summary>
/// <typeparam name="T"></typeparam>
public class XlsxFileResult<T> : XlsxFileResultBase where T : class, new()
{
    public string FileDownloadName { get; }
    public ICollection<T> Data { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fileDownloadName"></param>
    public XlsxFileResult(ICollection<T> data, string fileDownloadName = null)
    {
        FileDownloadName = fileDownloadName;
        Data = data;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var exporter = new ExcelExporter();
        var bytes = await exporter.ExportAsByteArray(Data);
        var fs = new MemoryStream(bytes);
        await DownloadExcelFileAsync(context, fs, FileDownloadName);
    }
}

/// <summary>
///
/// </summary>
public class XlsxFileResult : XlsxFileResultBase
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileDownloadName"></param>
    public XlsxFileResult(Stream stream, string fileDownloadName = null)
    {
        Stream = stream;
        FileDownloadName = fileDownloadName;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="fileDownloadName"></param>

    public XlsxFileResult(byte[] bytes, string fileDownloadName = null)
    {
        Stream = new MemoryStream(bytes);
        FileDownloadName = fileDownloadName;
    }

    public Stream Stream { get; protected set; }
    public string FileDownloadName { get; protected set; }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        await DownloadExcelFileAsync(context, Stream, FileDownloadName);
    }
}

/// <summary>
/// base class
/// </summary>
public class XlsxFileResultBase : ActionResult
{
    /// <summary>
    /// Download Excel file
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stream"></param>
    /// <param name="downloadFileName"></param>
    /// <returns></returns>
    protected virtual async Task DownloadExcelFileAsync(ActionContext context, Stream stream, string downloadFileName)
    {
        var response = context.HttpContext.Response;
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        downloadFileName ??= Guid.NewGuid().ToString("N") + ".xlsx";

        if (string.IsNullOrEmpty(Path.GetExtension(downloadFileName))) downloadFileName += ".xlsx";

        context.HttpContext.Response.Headers.Append("Content-Disposition", new[] { "attachment; filename=" + HttpUtility.UrlEncode(downloadFileName) });
        await stream.CopyToAsync(context.HttpContext.Response.Body);
    }
}