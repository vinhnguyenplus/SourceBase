// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class VisualDb
{
    public string ConfigId { get; set; }
    public string DbNickName { get; set; }
}

/// <summary>
/// Library table visualization
/// </summary>
public class VisualDbTable
{
    public List<VisualTable> VisualTableList { get; set; }

    public List<VisualColumn> VisualColumnList { get; set; }

    public List<ColumnRelation> ColumnRelationList { get; set; }
}

public class VisualTable
{
    public string TableName { get; set; }

    public string TableComents { get; set; }

    public int X { get; set; }

    public int Y { get; set; }
}

public class VisualColumn
{
    public string TableName { get; set; }

    public string ColumnName { get; set; }

    public string DataType { get; set; }

    public string DataLength { get; set; }

    public string ColumnDescription { get; set; }
}

public class ColumnRelation
{
    public string SourceTableName { get; set; }

    public string SourceColumnName { get; set; }

    public string Type { get; set; }

    public string TargetTableName { get; set; }

    public string TargetColumnName { get; set; }
}