// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using OfficeOpenXml;

namespace Admin.NET.Core;

public class ExcelHelper
{
    /// <summary>
    /// Data import
    /// </summary>
    /// <param name="file"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IActionResult ImportData<IN, T>(IFormFile file, Action<List<IN>, Action<StorageableResult<T>, List<IN>, List<T>>> action) where IN : BaseImportInput, new() where T : EntityBaseId, new()
    {
        try
        {
            var result = CommonUtil.ImportExcelDataAsync<IN>(file).Result ?? throw Oops.Oh("Valid data is empty");
            result.ForEach(u => u.Id = YitIdHelper.NextId());

            var tasks = new List<Task>();
            action.Invoke(result, (storageable, pageItems, rows) =>
            {
                // Mark verification information
                tasks.Add(Task.Run(() =>
                {
                    if (!storageable.TotalList.Any()) return;

                    // Verify information by marking it with Id
                    var itemMap = pageItems.ToDictionary(u => u.Id, u => u);
                    foreach (var item in storageable.TotalList)
                    {
                        var temp = itemMap.GetValueOrDefault(item.Item.Id);
                        if (temp != null) temp.Error ??= item.StorageMessage;
                    }
                }));
            });

            // Wait for all tag verification information tasks to complete
            Task.WhenAll(tasks).GetAwaiter().GetResult();

            // Export error records only
            var errorList = result.Where(u => !string.IsNullOrWhiteSpace(u.Error)).ToList();
            if (!errorList.Any())
                return new JsonResult(AdminResultProvider.Ok("Import successful"));
            return ExportData(errorList);
        }
        catch (Exception ex)
        {
            return new JsonResult(AdminResultProvider.Error(ex.Message));
        }
    }

    /// <summary>
    /// Export Xlsx data
    /// </summary>
    /// <param name="list"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static IActionResult ExportData(dynamic list, string fileName = "Import Records")
    {
        var exporter = new ExcelExporter();
        var fs = new MemoryStream(exporter.ExportAsByteArray(list).GetAwaiter().GetResult());
        return new XlsxFileResult(stream: fs, fileDownloadName: $"{fileName}-{DateTime.Now:yyyy-MM-dd_HHmmss}");
    }

    /// <summary>
    /// Export Xlsx template based on type
    /// </summary>
    /// <param name="list"></param>
    /// <param name="filename"></param>
    /// <param name="addListValidationFun"></param>
    /// <returns></returns>
    public static IActionResult ExportTemplate<T>(IEnumerable<T> list, string filename = "Import template", Func<ExcelWorksheet, PropertyInfo, IEnumerable<string>> addListValidationFun = null)
    {
        using var package = new ExcelPackage((ExportData(list, filename) as XlsxFileResult)!.Stream);
        var worksheet = package.Workbook.Worksheets[0];

        // Create a hidden sheet for adding drop-down lists
        var dropdownSheet = package.Workbook.Worksheets.Add("Drop down data");
        dropdownSheet.Hidden = eWorkSheetHidden.Hidden;

        var sysDictTypeService = App.GetService<SysDictTypeService>();
        foreach (var prop in typeof(T).GetProperties())
        {
            var propType = prop.PropertyType;

            var headerAttr = prop.GetCustomAttribute<ExporterHeaderAttribute>();
            var isNullableEnum = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(propType).IsEnum();
            if (isNullableEnum) propType = Nullable.GetUnderlyingType(propType);
            if (headerAttr == null) continue;

            // Get column number
            var columnIndex = 0;
            foreach (var item in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                if (++columnIndex > 0 && item.Text.Equals(headerAttr.DisplayName)) break;
            if (columnIndex <= 0) continue;

            // First obtain the following list from the agent function. If it is empty and the field is an enumeration type, the enumeration item is filled in the following list. If it is a dictionary field, the dictionary value value list is filled in the following list.
            var dataList = addListValidationFun?.Invoke(worksheet, prop)?.ToList();
            if (dataList == null)
            {
                // Populate the enumeration items with the following list
                if (propType.IsEnum())
                {
                    dataList = propType.EnumToList()?.Select(it => it.Describe).ToList();
                }
                else
                {
                    // Get dictionary attributes on a field
                    var dict = prop.GetCustomAttribute<DictAttribute>();
                    if (dict != null)
                    {
                        // Populate the dictionary value with the following list
                        dataList = sysDictTypeService.GetDataList(new GetDataDictTypeInput { Code = dict.DictTypeCode })
                            .Result?.Select(x => x.Label).ToList();
                    }
                }
            }

            if (dataList != null)
            {
                // Add dropdown list
                AddListValidation(dropdownSheet, columnIndex, dataList);
                dropdownSheet.Cells[1, columnIndex, dataList.Count, columnIndex].LoadFromCollection(dataList);
            }
        }

        package.Save();
        package.Stream.Position = 0;
        return new XlsxFileResult(stream: package.Stream, fileDownloadName: $"{filename}-{DateTime.Now:yyyy-MM-dd_HHmmss}");

        void AddListValidation(ExcelWorksheet dropdownSheet, int columnIndex, List<string> dataList)
        {
            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, columnIndex, ExcelPackage.MaxRows, columnIndex].Address);
            validation!.Formula.ExcelFormula = "=" + dropdownSheet.Cells[1, columnIndex, dataList.Count, columnIndex].FullAddressAbsolute;
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "Invalid input";
            validation.Error = "Please choose a valid option from the list";
        }
    }
}