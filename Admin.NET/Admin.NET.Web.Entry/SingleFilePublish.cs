// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion;
using System.Reflection;

namespace Admin.NET.Web.Entry;

/// <summary>
/// Solve the problem of single file publishing
/// </summary>
public class SingleFilePublish : ISingleFilePublish
{
    /// <summary>
    /// Solve the problem that a single file cannot be scanned assemblies
    /// </summary>
    /// <remarks>and <see cref="IncludeAssemblyNames"/> can be configured at the same time</remarks>
    /// <returns></returns>
    public Assembly[] IncludeAssemblies()
    {
        // Just write which assemblies you need the Furion framework to scan.
        return Array.Empty<Assembly>();
    }

    /// <summary>
    /// Solve the problem of assembly name that cannot be scanned in a single file
    /// </summary>
    /// <remarks>and <see cref="IncludeAssemblies"/> can be configured at the same time</remarks>
    /// <returns></returns>
    public string[] IncludeAssemblyNames()
    {
        // Just write which assemblies you need the Furion framework to scan.
        return new[]
        {
            "Admin.NET.Application",
            "Admin.NET.Core",
            "Admin.NET.Web.Core",
        };
    }
}