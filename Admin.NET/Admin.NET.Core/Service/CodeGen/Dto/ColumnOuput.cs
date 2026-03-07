// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Database table column
/// </summary>
public class ColumnOuput
{
    /// <summary>
    /// Field name
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Property name of the entity
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Field data length
    /// </summary>
    public int ColumnLength { get; set; }

    /// <summary>
    /// database type
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Field data default value
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Is it a primary key?
    /// </summary>
    public bool IsPrimarykey { get; set; }

    /// <summary>
    /// Whether to allow empty
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// .NET field types
    /// </summary>
    public string NetType { get; set; }

    /// <summary>
    /// dictionary encoding
    /// </summary>
    public string DictTypeCode { get; set; }

    /// <summary>
    /// Field description
    /// </summary>
    public string ColumnComment { get; set; }

    /// <summary>
    /// primary foreign key
    /// </summary>
    public string ColumnKey { get; set; }
}