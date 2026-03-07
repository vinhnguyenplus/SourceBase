// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using DbType = SqlSugar.DbType;

namespace Admin.NET.Core;

/// <summary>
/// Code generation helper class
/// </summary>
public static class CodeGenUtil
{
    /// <summary>
    /// Convert CamelCase naming
    /// </summary>
    /// <param name="columnName">Field name</param>
    /// <param name="dbColumnNames">EntityBase entity attribute name</param>
    /// <returns></returns>
    public static string CamelColumnName(string columnName, string[] dbColumnNames)
    {
        if (columnName.Contains('_'))
        {
            var arrColName = columnName.Split('_');
            var sb = new StringBuilder();
            foreach (var col in arrColName)
            {
                if (col.Length > 0)
                    sb.Append(col[..1].ToUpper() + col[1..].ToLower());
            }
            columnName = sb.ToString();
        }
        else
        {
            var propertyName = dbColumnNames.FirstOrDefault(c => c.ToLower() == columnName.ToLower());
            if (!string.IsNullOrEmpty(propertyName))
            {
                columnName = propertyName;
            }
            else
            {
                columnName = columnName[..1].ToUpper() + columnName[1..].ToLower();
            }
        }
        return columnName;
    }

    // Process the corresponding data field type according to the database type
    public static string ConvertDataType(DbColumnInfo dbColumnInfo, DbType dbType = DbType.Custom)
    {
        if (dbType == DbType.Custom)
            dbType = App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0].DbType;

        var dataType = dbType switch
        {
            DbType.Oracle => ConvertDataTypeOracleSql(string.IsNullOrEmpty(dbColumnInfo.OracleDataType) ? dbColumnInfo.DataType : dbColumnInfo.OracleDataType, dbColumnInfo.Length, dbColumnInfo.Scale),
            DbType.PostgreSQL => ConvertDataTypePostgresSql(dbColumnInfo.DataType),
            _ => ConvertDataTypeDefault(dbColumnInfo.DataType),
        };
        return dataType + (dbColumnInfo.IsNullable ? "?" : "");
    }

    public static string ConvertDataTypeOracleSql(string dataType, int? length, int? scale)
    {
        switch (dataType.ToLower())
        {
            case "interval year to month": return "int";

            case "interval day to second": return "TimeSpan";

            case "smallint": return "Int16";

            case "int":
            case "integer": return "int";

            case "long": return "long";

            case "float": return "float";

            case "decimal": return "decimal";

            case "number":
                if (length == null) return "decimal";
                return scale switch
                {
                    > 0 => "decimal",
                    0 or null when length is > 1 and < 12 => "int",
                    0 or null when length > 11 => "long",
                    _ => length == 1 ? "bool" : "decimal"
                };

            case "char":
            case "clob":
            case "nclob":
            case "nchar":
            case "nvarchar":
            case "varchar":
            case "nvarchar2":
            case "varchar2":
            case "rowid":
                return "string";

            case "timestamp":
            case "timestamp with time zone":
            case "timestamptz":
            case "timestamp without time zone":
            case "date":
            case "time":
            case "time with time zone":
            case "timetz":
            case "time without time zone":
                return "DateTime";

            case "bfile":
            case "blob":
            case "raw":
                return "byte[]";

            default:
                return "object";
        }
    }

    //Field types corresponding to PostgresSQL data types
    public static string ConvertDataTypePostgresSql(string dataType)
    {
        switch (dataType)
        {
            case "int2":
            case "smallint":
                return "Int16";

            case "int4":
            case "integer":
                return "int";

            case "int8":
            case "bigint":
                return "long";

            case "float4":
            case "real":
                return "float";

            case "float8":
            case "double precision":
                return "double";

            case "numeric":
            case "decimal":
            case "path":
            case "point":
            case "polygon":
            case "interval":
            case "lseg":
            case "macaddr":
            case "money":
                return "decimal";

            case "boolean":
            case "bool":
            case "box":
            case "bytea":
                return "bool";

            case "varchar":
            case "character varying":
            case "geometry":
            case "name":
            case "text":
            case "char":
            case "character":
            case "cidr":
            case "circle":
            case "tsquery":
            case "tsvector":
            case "txid_snapshot":
            case "xml":
            case "json":
                return "string";

            case "uuid":
                return "Guid";

            case "timestamp":
            case "timestamp with time zone":
            case "timestamptz":
            case "timestamp without time zone":
            case "date":
            case "time":
            case "time with time zone":
            case "timetz":
            case "time without time zone":
                return "DateTime";

            case "bit":
            case "bit varying":
                return "byte[]";

            case "varbit":
                return "byte";

            default:
                return "object";
        }
    }

    public static string ConvertDataTypeDefault(string dataType)
    {
        return dataType.ToLower() switch
        {
            "tinytext" or "mediumtext" or "longtext" or "mid" or "text" or "varchar" or "char" or "nvarchar" or "nchar" or "string" or "timestamp" => "string",
            "int" or "integer" or "int32" => "int",
            "smallint" => "Int16",
            //"tinyint" => "byte",
            "tinyint" => "bool",    // MYSQL
            "bigint" or "int64" => "long",
            "bit" or "boolean" => "bool",
            "money" or "smallmoney" or "numeric" or "decimal" => "decimal",
            "real" => "Single",
            "datetime" or "datetime2" or "smalldatetime" => "DateTime",
            "date" => "DateOnly",// MYSQL
            "time" => "TimeOnly",// MYSQL
            "float" or "double" => "double",
            "image" or "binary" or "varbinary" => "byte[]",
            "uniqueidentifier" => "Guid",
            _ => "object",
        };
    }

    /// <summary>
    /// Data type to display type
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static string DataTypeToEff(string dataType)
    {
        if (string.IsNullOrEmpty(dataType)) return "";
        return dataType.TrimEnd('?') switch
        {
            "string" => "Input",
            "int" => "InputNumber",
            "long" => "Input",
            "float" => "InputNumber",
            "double" => "InputNumber",
            "decimal" => "InputNumber",
            "bool" => "Switch",
            "Guid" => "Input",
            "DateTime" => "DatePicker",
            _ => "Input",
        };
    }

    // Is it a common field?
    public static bool IsCommonColumn(string columnName)
    {
        var columnList = new List<string>()
        {
            nameof(EntityBaseOrg.OrgId),
            nameof(EntityBaseTenant.TenantId),
            nameof(EntityBase.CreateTime),
            nameof(EntityBase.UpdateTime),
            nameof(EntityBase.CreateUserId),
            nameof(EntityBase.UpdateUserId),
            nameof(EntityBase.CreateUserName),
            nameof(EntityBase.UpdateUserName),
            nameof(EntityBaseDel.IsDelete)
        };
        return columnList.Contains(columnName);
    }

    /// <summary>
    /// Get the PropertyInfo list of the type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static PropertyInfo[] GetPropertyInfoArray(Type type)
    {
        PropertyInfo[] props = null;
        try
        {
            props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        catch
        { }
        return props;
    }
}