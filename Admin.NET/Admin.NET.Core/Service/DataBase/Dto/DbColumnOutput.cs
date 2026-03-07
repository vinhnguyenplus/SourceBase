// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class DbColumnOutput
{
    public string TableName { get; set; }

    public int TableId { get; set; }

    public string DbColumnName { get; set; }

    public string PropertyName { get; set; }

    public string DataType { get; set; }

    public object PropertyType { get; set; }

    public int Length { get; set; }

    public string ColumnDescription { get; set; }

    public string DefaultValue { get; set; }

    public bool IsNullable { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsPrimarykey { get; set; }

    public object Value { get; set; }

    public int DecimalDigits { get; set; }

    public int Scale { get; set; }

    public bool IsArray { get; set; }

    public bool IsJson { get; set; }

    public bool? IsUnsigned { get; set; }

    public int CreateTableFieldSort { get; set; }

    internal object SqlParameterDbType { get; set; }
}