// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Interface pressure test input parameters
/// </summary>
public class StressTestInput
{
    /// <summary>
    /// Interface request address
    /// </summary>
    /// <example>https://gitee.com/zuohuaijun/Admin.NET</example>
    [Required(ErrorMessage = "The interface request address cannot be empty")]
    public string RequestUri { get; set; }

    /// <summary>
    /// Request method
    /// </summary>
    [Required(ErrorMessage = "Request method cannot be empty")]
    public string RequestMethod { get; set; } = nameof(HttpMethod.Get);

    /// <summary>
    /// Requests per round
    /// </summary>
    /// <example>100</example>
    [Required(ErrorMessage = "The request volume per round cannot be empty")]
    [Range(1, 100000, ErrorMessage = "The number of requests per round must be 1-100000")]
    public int? NumberOfRequests { get; set; }

    /// <summary>
    /// Number of pressure test rounds
    /// </summary>
    /// <example>5</example>
    [Required(ErrorMessage = "The number of pressure test rounds cannot be empty.")]
    [Range(1, 10000, ErrorMessage = "The number of stress test rounds must be between 1 and 10,000")]
    public int? NumberOfRounds { get; set; }

    /// <summary>
    /// Maximum amount of parallelism (defaults to the current number of host logical processors)
    /// </summary>
    /// <example>500</example>
    [Range(0, 10000, ErrorMessage = "The maximum parallelism must be 0-10000")]
    public int? MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Request parameters
    /// </summary>
    public List<KeyValuePair<string, string>> RequestParameters { get; set; } = new();

    /// <summary>
    /// Request header parameters
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// path parameters
    /// </summary>
    public Dictionary<string, string> PathParameters { get; set; } = new();

    /// <summary>
    /// Query parameters
    /// </summary>
    public Dictionary<string, string> QueryParameters { get; set; } = new();
}