// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.K3Cloud.Service;

public class K3CloudPushResultOutput
{
    public ErpPushResultInfo Result { get; set; }
}

public class ErpPushResultInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public object? Id { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    public string? Number { get; set; }

    public ErpPushResultInfo_ResponseStatus ResponseStatus { get; set; }
}

public class ErpPushResultInfo_ResponseStatus
{
    public bool IsSuccess { get; set; }
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Error code MsgCode description
    ///0: default
    ///1: Context lost and session expired
    ///2: No permission
    ///3: The operation identifier is empty
    ///4: Abnormal
    ///5: The document ID is empty
    ///6: Database operation failed
    ///7: License error
    ///8: Parameter error
    ///9: The specified field/value does not exist
    ///10: No corresponding data found
    ///11: Verification failed
    ///12: Inoperable
    ///13: Network control conflict
    ///14: Call restrictions
    ///15: Prohibit administrator login
    /// </summary>
    public int? MsgCode { get; set; }

    /// <summary>
    /// If failed, specific reasons for failure
    /// </summary>
    public List<ErpPushResultInfo_Errors> Errors { get; set; }
}

public class ErpPushResultInfo_Errors
{
    public string FieldName { get; set; }
    public string Message { get; set; }
    public int DIndex { get; set; }
}