// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Code generation parameter class
/// </summary>
public class CodeGenOutput
{
    /// <summary>
    /// Code GeneratorId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Author name
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// Class name
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// Whether to remove table prefix
    /// </summary>
    public string TablePrefix { get; set; }

    /// <summary>
    /// Generation method
    /// </summary>
    public string GenerateType { get; set; }

    /// <summary>
    /// Database table name
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// package name
    /// </summary>
    public string PackageName { get; set; }

    /// <summary>
    /// Business name (business code package name)
    /// </summary>
    public string BusName { get; set; }

    /// <summary>
    /// Function name (database table name)
    /// </summary>
    public string TableComment { get; set; }

    /// <summary>
    /// Menu application classification (application coding)
    /// </summary>
    public string MenuApplication { get; set; }

    /// <summary>
    /// Whether to generate a menu
    /// </summary>
    public bool GenerateMenu { get; set; }

    /// <summary>
    /// Menu parent
    /// </summary>
    public long? MenuPid { get; set; }

    /// <summary>
    /// Supported printing types
    /// </summary>
    public string PrintType { get; set; }

    /// <summary>
    /// Print template name
    /// </summary>
    public string PrintName { get; set; }
}