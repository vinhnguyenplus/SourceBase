// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Gitee interface helper class
/// </summary>
public class GiteeHelper
{
    private const string BaseUrl = "https://gitee.com/api/v5/repos/";
    private static readonly HttpClient Client = new();

    /// <summary>
    /// Download repository zip
    /// </summary>
    /// <remarks>https://gitee.com/api/v5/swagger#/getV5ReposOwnerRepoZipball</remarks>
    /// <returns></returns>
    public static async Task<Stream> DownloadRepoZip(string owner, string repo, string accessToken = null, string @ref = null)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw Oops.Bah($"Parameter {nameof(owner)} cannot be empty");
        if (string.IsNullOrWhiteSpace(repo)) throw Oops.Bah($"Parameter {nameof(repo)} cannot be empty");
        var query = BuilderQueryString(new
        {
            access_token = accessToken,
            @ref
        });
        return await Client.GetStreamAsync($"{BaseUrl}{owner}/{repo}/zipball?{query}");
    }

    /// <summary>
    /// Build Query parameters
    /// </summary>
    /// <returns></returns>
    private static string BuilderQueryString([System.Diagnostics.CodeAnalysis.NotNull] object obj)
    {
        if (obj == null) return string.Empty;
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var prop in obj.GetType().GetProperties())
        {
            var val = prop.GetValue(obj);
            if (val == null) continue;

            // Verify parameter set as tuple
            var name = prop.Name.Trim('@');
            if (val is Tuple<object, string> { Item1: not null } tuple)
            {
                if (!tuple.Item2.Split(",").Any(x => x.Trim().Equals(tuple.Item1))) throw Oops.Oh($"The value of parameter {name} can only be: {tuple.Item2}");
                query[name] = tuple.Item1.ToString();
                continue;
            }
            query[name] = val.ToString();
        }
        return query.ToString();
    }
}