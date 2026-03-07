// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Dynamic job compilation
/// </summary>
public class DynamicJobCompiler : ISingleton
{
    /// <summary>
    /// Compiles the code and returns the type in which IJob is implemented
    /// </summary>
    /// <param name="script">Dynamically compiled job code</param>
    /// <returns></returns>
    public Type BuildJob(string script)
    {
        var jobAssembly = Schedular.CompileCSharpClassCode(script);
        var jobType = jobAssembly.GetTypes().FirstOrDefault(u => typeof(IJob).IsAssignableFrom(u));
        return jobType;
    }
}