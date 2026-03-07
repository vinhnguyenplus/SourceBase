// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public interface IDingTalkApi : IHttpDeclarative
{
    /// <summary>
    /// Obtain the access_token of the internal application of the enterprise
    /// </summary>
    /// <param name="appkey">The unique identification key of the application</param>
    /// <param name="appsecret"> The application key. AppKey and AppSecret can be obtained from the application details page of DingTalk Developer Backend.</param>
    /// <returns></returns>
    [Get("https://oapi.dingtalk.com/gettoken")]
    Task<GetDingTalkTokenOutput> GetDingTalkToken([Query] string appkey, [Query] string appsecret);

    /// <summary>
    /// Get a list of current employees
    /// </summary>
    /// <param name="access_token">Application credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/smartwork/hrm/employee/queryonjob")]
    Task<
        DingTalkBaseResponse<GetDingTalkCurrentEmployeesListOutput>
    > GetDingTalkCurrentEmployeesList(
        [Query] string access_token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            GetDingTalkCurrentEmployeesListInput input
    );

    /// <summary>
    /// Get employee roster field information
    /// </summary>
    /// <param name="access_token">Application credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/smartwork/hrm/employee/v2/list")]
    Task<
        DingTalkBaseResponse<List<DingTalkEmpRosterFieldVo>>
    > GetDingTalkCurrentEmployeesRosterList(
        [Query] string access_token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            GetDingTalkCurrentEmployeesRosterListInput input
    );

    /// <summary>
    /// Send DingTalk interactive cards
    /// </summary>
    /// <param name="token">Access credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks>
    /// DingTalk’s official documentation shows that the interface no longer supports the access of new applications, and already accessed applications can continue to be called.
    /// Recommended update interface https://open.dingtalk.com/document/orgapp/create-and-deliver-cards?spm=ding_open_doc.document.0.0.67fc50988Pf0mc
    /// </remarks>
    [Post("https://api.dingtalk.com/v1.0/im/interactiveCards/send")]
    [Obsolete]
    Task<DingTalkSendInteractiveCardsOutput> DingTalkSendInteractiveCards(
        [Header("x-acs-dingtalk-access-token")] string token,
        [Body(ContentType = "application/json", UseStringContent = true)]
            DingTalkSendInteractiveCardsInput input
    );

    /// <summary>
    /// Get DingTalk card message reading status
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Get("https://api.dingtalk.com/v1.0/robot/oToMessages/readStatus")]
    Task<GetDingTalkCardMessageReadStatusOutput> GetDingTalkCardMessageReadStatus(
        [Header("x-acs-dingtalk-access-token")] string token,
        [Query] GetDingTalkCardMessageReadStatusInput input
    );

    /// <summary>
    /// Get role list
    /// </summary>
    /// <param name="access_token">Application credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/role/list")]
    Task<DingTalkBaseResponse<DingTalkRoleListOutput>> GetDingTalkRoleList(
        [Query] string access_token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            GetDingTalkCurrentRoleListInput input
    );

    /// <summary>
    /// Get a list of employees with a specified role
    /// </summary>
    /// <param name="access_token">Application credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/role/simplelist")]
    Task<DingTalkBaseResponse<DingTalkRoleSimplelistOutput>> GetDingTalkRoleSimplelist(
        [Query] string access_token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            GetDingTalkCurrentRoleSimplelistInput input
    );

    /// <summary>
    /// Create and deliver DingTalk message cards
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://api.dingtalk.com/v1.0/card/instances/createAndDeliver")]
    Task<DingTalkCreateAndDeliverOutput> DingTalkCreateAndDeliver(
        [Header("x-acs-dingtalk-access-token")] string token,
        [Body(ContentType = "application/json", UseStringContent = true)]
            DingTalkCreateAndDeliverInput input
    );

    /// <summary>
    /// Get list of department lists
    /// </summary>
    /// <param name="access_token">Application credentials for calling this interface</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/v2/department/listsub")]
    Task<DingTalkBaseResponse<List<DingTalkDeptOutput>>> GetDingTalkDept(
        [Query] string access_token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            GetDingTalkDeptInput input
    );

    /// <summary>
    /// Initiate an approval instance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://api.dingtalk.com/v1.0/workflow/processInstances")]
    Task<DingTalkWorkflowProcessInstancesOutput> DingTalkWorkflowProcessInstances(
        [Header("x-acs-dingtalk-access-token")] string token,
        [Body(ContentType = "application/json", UseStringContent = true), Required]
            DingTalkWorkflowProcessInstancesInput input
    );

    /// <summary>
    /// Query approval examples
    /// </summary>
    /// <param name="token"></param>
    /// <param name="processInstanceId"></param>
    /// <returns></returns>
    [Get("https://api.dingtalk.com/v1.0/workflow/processInstances")]
    Task<DingTalkGetProcessInstancesOutput> GetProcessInstances(
        [Header("x-acs-dingtalk-access-token")] string token,
        [Query] string processInstanceId
    );
}