// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class DbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string DbColumnName { get; set; }

    public string DataType { get; set; }

    public int Length { get; set; }

    public string ColumnDescription { get; set; }

    public int IsNullable { get; set; }

    public int IsIdentity { get; set; }

    public int IsPrimarykey { get; set; }

    public int DecimalDigits { get; set; }

    public string DefaultValue { get; set; }
}

public class UpdateDbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string ColumnName { get; set; }

    public string OldColumnName { get; set; }

    public string Description { get; set; }

    public string DefaultValue { get; set; }
}

public class MoveDbColumnInput
{
    /// <summary>
    /// Database configuration ID
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// Target table name
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    ///Column name to move
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Move to the back of the column (move to the first column if empty)
    /// </summary>
    public string AfterColumnName { get; set; }
}

public class DeleteDbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string DbColumnName { get; set; }
}