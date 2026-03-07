// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Plugin.ApprovalFlow.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Admin.NET.Plugin.ApprovalFlow;

/// <summary>
/// Extended approval flow middleware
/// </summary>
public static class ApprovalFlowMiddlewareExtensions
{
    /// <summary>
    /// Use approval flow
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseApprovalFlow(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApprovalFlowMiddleware>();
    }
}

/// <summary>
/// Approval flow middleware
/// </summary>
public class ApprovalFlowMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SysApprovalService _sysApprovalService;

    public ApprovalFlowMiddleware(RequestDelegate next)
    {
        _next = next;
        _sysApprovalService = App.GetRequiredService<SysApprovalService>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _sysApprovalService.MatchApproval(context);

        // Call next middleware
        await _next(context);
    }
}