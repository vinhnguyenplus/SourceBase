// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkGetProcessInstancesOutput
{
    public ResultData Result { get; set; }
    public bool Success { get; set; }
}

public class OperationRecord
{
    public DateTime? Date { get; set; }
    public string Result { get; set; }
    public List<object> Images { get; set; } // The image may be a string URL or an object
    public string ShowName { get; set; }
    public string Type { get; set; }
    public string UserId { get; set; }
}

// Children in table rows (for parsing of TableField)
public class TableRowItem
{
    public string BizAlias { get; set; }
    public string Label { get; set; }
    public string Value { get; set; }
    public string Key { get; set; }
    public bool Mask { get; set; }
}

// A complete row of table data
public class TableRow
{
    public List<TableRowItem> RowValue { get; set; }
    public string RowNumber { get; set; }
}

public class TaskItem
{
    public string Result { get; set; }
    public string ActivityId { get; set; }
    public string PcUrl { get; set; }
    public DateTime? CreateTime { get; set; }
    public string MobileUrl { get; set; }
    public string UserId { get; set; }
    public long TaskId { get; set; }
    public string Status { get; set; }
}

public class ResultData
{
    public List<string> AttachedProcessInstanceIds { get; set; }
    public string BusinessId { get; set; }
    public string Title { get; set; }
    public string OriginatorDeptId { get; set; }
    public List<OperationRecord> OperationRecords { get; set; }
    public List<FormComponentValue> FormComponentValues { get; set; }

    /// <summary>
    /// Approval result agree: agree refuse: refuse
    /// </summary>
    public string Result { get; set; }

    public string BizAction { get; set; }
    public DateTime? CreateTime { get; set; }
    public string OriginatorUserId { get; set; }
    public List<TaskItem> Tasks { get; set; }
    public string OriginatorDeptName { get; set; }
    public string Status { get; set; }
}