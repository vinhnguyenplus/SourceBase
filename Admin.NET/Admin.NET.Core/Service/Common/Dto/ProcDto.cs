// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Magicodes.ExporterAndImporter.Core.Filters;
using Magicodes.ExporterAndImporter.Core.Models;

namespace Admin.NET.Core.Service;

/// <summary>
/// Basic stored procedure input class
/// </summary>
public class BaseProcInput
{
    /// <summary>
    /// ProcId
    /// </summary>
    public string ProcId { get; set; }

    /// <summary>
    /// Database configuration ID
    /// </summary>
    public string ConfigId { get; set; } = SqlSugarConst.MainConfigId;

    /// <summary>
    /// Stored procedure input parameters
    /// </summary>
    /// <example>{"id":"351060822794565"}</example>
    public Dictionary<string, object> ProcParams { get; set; }
}

/// <summary>
/// Stored procedure input class with header name
/// </summary>
public class ExportProcByTMPInput : BaseProcInput
{
    /// <summary>
    /// Template name
    /// </summary>
    public string Template { get; set; }
}

/// <summary>
/// Stored procedure input class with header name
/// </summary>
public class ExportProcInput : BaseProcInput
{
    public Dictionary<string, string> EHeader { get; set; }
}

/// <summary>
/// Specify the export class name (sorted) stored procedure input class
/// </summary>
public class ExportProcInput2 : BaseProcInput
{
    public List<string> EHeader { get; set; }
}

/// <summary>
/// Front-end specified column
/// </summary>
public class ProcExporterHeaderFilter : IExporterHeaderFilter
{
    private Dictionary<string, Tuple<string, int>> _includeHeader;

    public ProcExporterHeaderFilter(Dictionary<string, Tuple<string, int>> includeHeader)
    {
        _includeHeader = includeHeader;
    }

    public ExporterHeaderInfo Filter(ExporterHeaderInfo exporterHeaderInfo)
    {
        if (_includeHeader != null && _includeHeader.Count > 0)
        {
            var key = exporterHeaderInfo.PropertyName.ToUpper();
            if (_includeHeader.ContainsKey(key))
            {
                exporterHeaderInfo.DisplayName = _includeHeader[key].Item1;
                return exporterHeaderInfo;
            }
            else
            {
                exporterHeaderInfo.ExporterHeaderAttribute.Hidden = true;
            }
        }
        return exporterHeaderInfo;
    }
}