// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Global paging query input parameters
/// </summary>
public class BasePageInput : BaseFilter
{
    /// <summary>
    /// Current page number
    /// </summary>
    [DataValidation(ValidationTypes.Numeric)]
    public virtual int Page { get; set; } = 1;

    /// <summary>
    /// Page capacity
    /// </summary>
    //[Range(0, 100, ErrorMessage = "Page capacity exceeds the maximum limit")]
    [DataValidation(ValidationTypes.Numeric)]
    public virtual int PageSize { get; set; } = 20;

    /// <summary>
    /// sort field
    /// </summary>
    public virtual string Field { get; set; }

    /// <summary>
    /// Sorting direction
    /// </summary>
    public virtual string Order { get; set; }

    /// <summary>
    /// Sort descending
    /// </summary>
    public virtual string DescStr { get; set; } = "descending";
}

/// <summary>
/// Global paging query input parameters (with time)
/// </summary>
public class BasePageTimeInput : BasePageInput
{
    /// <summary>
    /// start time
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// end time
    /// </summary>
    public DateTime? EndTime { get; set; }
}