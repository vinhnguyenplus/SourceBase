// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Export log data
/// </summary>
[ExcelExporter(Name = "Log data", TableStyle = OfficeOpenXml.Table.TableStyles.None, AutoFitAllColumn = true)]
public class ExportLogDto
{
    /// <summary>
    /// Logger class name
    /// </summary>
    [ExporterHeader(DisplayName = "Logger class name", IsBold = true)]
    public string LogName { get; set; }

    /// <summary>
    /// Log level
    /// </summary>
    [ExporterHeader(DisplayName = "Log level", IsBold = true)]
    public string LogLevel { get; set; }

    /// <summary>
    /// EventId
    /// </summary>
    [ExporterHeader(DisplayName = "Event ID", IsBold = true)]
    public string EventId { get; set; }

    /// <summary>
    /// log message
    /// </summary>
    [ExporterHeader(DisplayName = "Log message", IsBold = true)]
    public string Message { get; set; }

    /// <summary>
    /// exception object
    /// </summary>
    [ExporterHeader(DisplayName = "Exception object", IsBold = true)]
    public string Exception { get; set; }

    /// <summary>
    /// current status value
    /// </summary>
    [ExporterHeader(DisplayName = "Current state value", IsBold = true)]
    public string State { get; set; }

    /// <summary>
    /// Logging time
    /// </summary>
    [ExporterHeader(DisplayName = "Log Time", IsBold = true)]
    public DateTime LogDateTime { get; set; }

    /// <summary>
    /// ThreadId
    /// </summary>
    [ExporterHeader(DisplayName = "ThreadId", IsBold = true)]
    public int ThreadId { get; set; }

    /// <summary>
    /// Request Tracking ID
    /// </summary>
    [ExporterHeader(DisplayName = "Request Tracking ID", IsBold = true)]
    public string TraceId { get; set; }
}