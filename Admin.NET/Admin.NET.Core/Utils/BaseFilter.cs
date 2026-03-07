// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Fuzzy query conditions
/// </summary>
public class Search
{
    /// <summary>
    /// Field name collection
    /// </summary>
    public List<string> Fields { get; set; }

    /// <summary>
    /// Keywords
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// Filter filters
/// </summary>
public class Filter
{
    /// <summary>
    /// filter conditions
    /// </summary>
    public FilterLogicEnum? Logic { get; set; }

    /// <summary>
    /// Filter sub-items
    /// </summary>
    public IEnumerable<Filter>? Filters { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Logical operators
    /// </summary>
    public FilterOperatorEnum? Operator { get; set; }

    /// <summary>
    /// field value
    /// </summary>
    public object? Value { get; set; }
}

/// <summary>
/// Filter base class
/// </summary>
public abstract class BaseFilter
{
    /// <summary>
    /// Fuzzy query conditions
    /// </summary>
    public Search? Search { get; set; }

    /// <summary>
    /// Fuzzy query keywords
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Filter filters
    /// </summary>
    public Filter? Filter { get; set; }
}