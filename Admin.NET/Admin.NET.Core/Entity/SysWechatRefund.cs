// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System WeChat payment refund form
/// </summary>
[SugarTable(null, "System WeChat payment refund form")]
[SysTable]
[SugarIndex("index_{table}_W", nameof(WechatPayId), OrderByType.Asc)]
public partial class SysWechatRefund : EntityBase
{
    /// <summary>
    /// Order primary key
    /// </summary>
    [SugarColumn(ColumnDescription = "Order Primary Key")]
    public long WechatPayId { get; set; }

    /// <summary>
    /// Merchant refund number
    /// </summary>
    [SugarColumn(ColumnDescription = "Merchant refund number")]
    [Required]
    public virtual string OutRefundNumber { get; set; }

    /// <summary>
    /// Refund order number
    /// </summary>
    [SugarColumn(ColumnDescription = "RefundOrder Number")]
    [Required]
    public virtual string TransactionId { get; set; }

    /// <summary>
    /// Reason for refund
    /// </summary>
    [SugarColumn(ColumnDescription = "Reason for refund")]
    public string? Reason { get; set; }

    /// <summary>
    /// Refund channels
    /// </summary>
    [SugarColumn(ColumnDescription = "Refund channels")]
    public string? Channel { get; set; }

    /// <summary>
    /// Refund into account
    /// </summary>
    /// <remarks>
    /// To obtain the refund account of the current refund order, there are the following situations:
    /// 1) Return bank card: {bank name}{card type}{card tail number}
    /// 2) Return the payment user’s change: Pay the user’s change
    /// 3) Return to merchant: Merchant basic account Merchant settlement bank account
    /// 4) Return payment user Lingqiantong: Payment user Lingqiantong
    /// </remarks>
    [SugarColumn(ColumnDescription = "Refund into account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>
    /// Refund status
    /// </summary>
    [SugarColumn(ColumnDescription = "Refund status")]
    public string? TradeState { get; set; }

    /// <summary>
    /// Transaction status description
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction status description")]
    public string? TradeStateDescription { get; set; }

    /// <summary>
    /// Total order amount
    /// </summary>
    [SugarColumn(ColumnDescription = "Refund amount")]
    public int Refund { get; set; }

    /// <summary>
    /// payment completion time
    /// </summary>
    [SugarColumn(ColumnDescription = "completion time")]
    public DateTime? SuccessTime { get; set; }

    /// <summary>
    /// Callback notification address
    /// </summary>
    [SugarColumn(ColumnDescription = "Callback notification address")]
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks")]
    public string? Remark { get; set; }
}