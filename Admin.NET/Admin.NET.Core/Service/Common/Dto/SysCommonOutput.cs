// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Interface pressure test output parameters
/// </summary>
public class StressTestOutput
{
    /// <summary>
    /// Total number of requests
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// Total time (seconds)
    /// </summary>
    public double TotalTimeInSeconds { get; set; }

    /// <summary>
    /// Number of successful requests
    /// </summary>
    public long SuccessfulRequests { get; set; }

    /// <summary>
    /// Number of failed requests
    /// </summary>
    public long FailedRequests { get; set; }

    /// <summary>
    /// Query rate per second (QPS)
    /// </summary>
    public double QueriesPerSecond { get; set; }

    /// <summary>
    /// Minimum response time (milliseconds)
    /// </summary>
    public double MinResponseTime { get; set; }

    /// <summary>
    /// Maximum response time (milliseconds)
    /// </summary>
    public double MaxResponseTime { get; set; }

    /// <summary>
    /// Average response time (ms)
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// P10 response time (milliseconds)
    /// </summary>
    public double Percentile10ResponseTime { get; set; }

    /// <summary>
    /// P25 response time (milliseconds)
    /// </summary>
    public double Percentile25ResponseTime { get; set; }

    /// <summary>
    /// P50 response time (milliseconds)
    /// </summary>
    public double Percentile50ResponseTime { get; set; }

    /// <summary>
    /// P75 response time (milliseconds)
    /// </summary>
    public double Percentile75ResponseTime { get; set; }

    /// <summary>
    /// P90 response time (milliseconds)
    /// </summary>
    public double Percentile90ResponseTime { get; set; }

    /// <summary>
    /// P99 response time (milliseconds)
    /// </summary>
    public double Percentile99ResponseTime { get; set; }

    /// <summary>
    /// P999 response time (milliseconds)
    /// </summary>
    public double Percentile999ResponseTime { get; set; }
}