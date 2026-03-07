// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Code generation parameter class
/// </summary>
public class CodeGenInput : BasePageInput
{
    /// <summary>
    /// Author name
    /// </summary>
    public virtual string AuthorName { get; set; }

    /// <summary>
    /// Class name
    /// </summary>
    public virtual string ClassName { get; set; }

    /// <summary>
    /// Whether to remove table prefix
    /// </summary>
    public virtual string TablePrefix { get; set; }

    /// <summary>
    /// library locator name
    /// </summary>
    public virtual string ConfigId { get; set; }

    /// <summary>
    /// Database name (reserved field)
    /// </summary>
    public virtual string DbName { get; set; }

    /// <summary>
    /// Database type
    /// </summary>
    public virtual string DbType { get; set; }

    /// <summary>
    /// Database link
    /// </summary>
    public virtual string ConnectionString { get; set; }

    /// <summary>
    /// Generation method
    /// </summary>
    public virtual string GenerateType { get; set; }

    /// <summary>
    /// Database table name
    /// </summary>
    public virtual string TableName { get; set; }

    /// <summary>
    /// namespace
    /// </summary>
    public virtual string NameSpace { get; set; }

    /// <summary>
    /// Business name (business code package name)
    /// </summary>
    public virtual string BusName { get; set; }

    /// <summary>
    /// Function name (database table name)
    /// </summary>
    public virtual string TableComment { get; set; }

    /// <summary>
    /// table unique field
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual string TableUniqueConfig { get; set; }

    /// <summary>
    /// table unique field list
    /// </summary>
    public virtual List<TableUniqueConfigItem> TableUniqueList { get; set; }

    /// <summary>
    /// Menu application classification (application coding)
    /// </summary>
    public virtual string MenuApplication { get; set; }

    /// <summary>
    /// Whether to generate a menu
    /// </summary>
    public virtual bool GenerateMenu { get; set; }

    /// <summary>
    /// Menu parent
    /// </summary>
    public virtual long? MenuPid { get; set; }

    /// <summary>
    /// menu icon
    /// </summary>
    public virtual string MenuIcon { get; set; }

    /// <summary>
    /// Page directory
    /// </summary>
    public virtual string PagePath { get; set; }

    /// <summary>
    /// Supported printing types
    /// </summary>
    public virtual string PrintType { get; set; }

    /// <summary>
    /// Print template name
    /// </summary>
    public virtual string PrintName { get; set; }
}

public class AddCodeGenInput : CodeGenInput
{
    /// <summary>
    /// Database table name
    /// </summary>
    [Required(ErrorMessage = "Database table name cannot be empty")]
    public override string TableName { get; set; }

    /// <summary>
    /// Business name (business code package name)
    /// </summary>
    [Required(ErrorMessage = "The business name cannot be empty")]
    public override string BusName { get; set; }

    /// <summary>
    /// namespace
    /// </summary>
    [Required(ErrorMessage = "Namespace cannot be empty")]
    public override string NameSpace { get; set; }

    /// <summary>
    /// Author name
    /// </summary>
    [Required(ErrorMessage = "Author's Namecannot benull")]
    public override string AuthorName { get; set; }

    ///// <summary>
    ///// class name
    ///// </summary>
    //[Required(ErrorMessage = "Class name cannot be empty")]
    //public override string ClassName { get; set; }

    ///// <summary>
    ///// Whether to remove the table prefix
    ///// </summary>
    //[Required(ErrorMessage = "Whether to remove the table prefix cannot be empty")]
    //public override string TablePrefix { get; set; }

    /// <summary>
    /// Generation method
    /// </summary>
    [Required(ErrorMessage = "Generation method cannot be empty")]
    public override string GenerateType { get; set; }

    ///// <summary>
    ///// Function name (database table name)
    ///// </summary>
    //[Required(ErrorMessage = "Database table name cannot be empty")]
    //public override string TableComment { get; set; }

    /// <summary>
    /// Whether to generate a menu
    /// </summary>
    [Required(ErrorMessage = "Whether generating the menu cannot be empty")]
    public override bool GenerateMenu { get; set; }
}

public class DeleteCodeGenInput
{
    /// <summary>
    /// Code GeneratorId
    /// </summary>
    [Required(ErrorMessage = "Code generator ID cannot be empty")]
    public long Id { get; set; }
}

public class UpdateCodeGenInput : AddCodeGenInput
{
    /// <summary>
    /// Code GeneratorId
    /// </summary>
    [Required(ErrorMessage = "Code generator ID cannot be empty")]
    public long Id { get; set; }
}

public class QueryCodeGenInput : DeleteCodeGenInput
{
}