// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using MiniExcelLibs;

namespace Admin.NET.Core;

public static class MiniExcelUtil
{
    private const string SheetName = "ImportTemplate";
    private const string DirectoryName = "export";

    /// <summary>
    /// Export template Excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelTemplate<T>(string fileName = null) where T : class, new()
    {
        var values = Array.Empty<T>();
        // Create space in memory
        var memoryStream = new MemoryStream();
        // Write data to memory
        await memoryStream.SaveAsAsync(values, sheetName: SheetName);
        // Start writing from position 0
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx"
        };
    }

    /// <summary>
    /// Get imported data Excel
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> GetImportExcelData<T>([Required] IFormFile file) where T : class, new()
    {
        using MemoryStream stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var res = await stream.QueryAsync<T>(sheetName: SheetName);
        return res.ToArray();
    }

    /// <summary>
    /// Get exported data excel address
    /// </summary>
    /// <returns></returns>
    public static async Task<string> GetExportDataExcelUrl<T>(IEnumerable<T> exportData) where T : class, new()
    {
        var fileName = string.Format("{0}.xlsx", YitIdHelper.NextId());
        try
        {
            var path = Path.Combine(App.WebHostEnvironment.WebRootPath, DirectoryName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, fileName);
            await MiniExcel.SaveAsAsync(filePath, exportData, overwriteFile: true);
        }
        catch (Exception error)
        {
            throw Oops.Oh("An error occurred:" + error);
        }
        var host = CommonUtil.GetLocalhost();
        return $"{host}/{DirectoryName}/{fileName}";
    }
}