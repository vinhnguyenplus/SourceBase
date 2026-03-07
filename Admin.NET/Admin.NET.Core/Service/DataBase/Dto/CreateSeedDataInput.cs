// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class CreateSeedDataInput
{
    /// <summary>
    /// Library ID
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// table name
    /// </summary>
    /// <example>student</example>
    public string TableName { get; set; }

    /// <summary>
    /// Entity name
    /// </summary>
    /// <example>Student</example>
    public string EntityName { get; set; }

    /// <summary>
    /// seed name
    /// </summary>
    /// <example>Student</example>
    public string SeedDataName { get; set; }

    /// <summary>
    /// Export location
    /// </summary>
    /// <example>Web.Application</example>
    public string Position { get; set; }

    /// <summary>
    /// suffix
    /// </summary>
    /// <example>Web.Application</example>
    public string Suffix { get; set; }

    /// <summary>
    /// Filter existing data
    /// </summary>
    /// <remarks>
    /// If the data appears in other existing seed type data with different names, this data will not be generated.
    /// It is mainly used to generate menu functions. Menu functions are often bound to sub-items. If complete data is generated, multi-process duplication of menu items will occur.
    /// </remarks>
    public bool FilterExistingData { get; set; }
}