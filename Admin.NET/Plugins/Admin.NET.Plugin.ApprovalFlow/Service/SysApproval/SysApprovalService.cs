// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.Http;

namespace Admin.NET.Plugin.ApprovalFlow.Service;

public class SysApprovalService : ITransient
{
    private readonly SqlSugarRepository<ApprovalFlowRecord> _approvalFlowRep;
    private readonly SqlSugarRepository<ApprovalFormRecord> _approvalFormRep;
    private readonly ApprovalFlowService _approvalFlowService;

    public SysApprovalService(SqlSugarRepository<ApprovalFlowRecord> approvalFlowRep, SqlSugarRepository<ApprovalFormRecord> approvalFormRep, ApprovalFlowService approvalFlowService)
    {
        _approvalFlowRep = approvalFlowRep;
        _approvalFormRep = approvalFormRep;
        _approvalFlowService = approvalFlowService;
    }

    /// <summary>
    /// Match the approval process
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [NonAction]
    public async Task MatchApproval(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var path = request.Path.ToString().Split("/");

        var method = request.Method;
        var qs = request.QueryString;
        var h = request.Headers;
        var b = request.Body;

        var requestHeaders = request.Headers;
        var responseHeaders = response.Headers;

        var serviceName = path[1];
        if (serviceName.StartsWith("api"))
        {
            if (path.Length > 3)
            {
                var funcName = path[2];
                var typeName = path[3];

                var list = await _approvalFlowService.FormRoutes();
                if (list.Any(u => u.Contains(funcName) && u.Contains(typeName)))
                {
                    var approvalFlow = new ApprovalFlowRecord
                    {
                        FormName = funcName,
                        CreateTime = DateTime.Now,
                    };

                    // Determine whether approval is required
                    await _approvalFlowRep.InsertAsync(approvalFlow);

                    var approvalForm = new ApprovalFormRecord
                    {
                        FlowId = approvalFlow.Id,
                        FormName = funcName,
                        FormType = typeName,
                        CreateTime = DateTime.Now,
                    };

                    // Determine whether approval is required
                    await _approvalFormRep.InsertAsync(approvalForm);
                }
            }
        }

        await Task.CompletedTask;
    }
}