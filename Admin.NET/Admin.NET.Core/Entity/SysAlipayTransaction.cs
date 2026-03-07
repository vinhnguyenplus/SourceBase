// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Alipay transaction record form
/// </summary>
[SugarTable(null, "Alipay Transaction Records")]
[SysTable]
[SugarIndex("index_{table}_U", nameof(UserId), OrderByType.Asc)]
[SugarIndex("index_{table}_T", nameof(TradeNo), OrderByType.Asc)]
[SugarIndex("index_{table}_O", nameof(OutTradeNo), OrderByType.Asc)]
public class SysAlipayTransaction : EntityBase
{
    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId", Length = 64)]
    public long UserId { get; set; }

    /// <summary>
    /// Transaction number
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction number", Length = 64)]
    public string? TradeNo { get; set; }

    /// <summary>
    /// Merchant order number
    /// </summary>
    [SugarColumn(ColumnDescription = "Merchant Order Number", Length = 64)]
    public string OutTradeNo { get; set; }

    /// <summary>
    /// Transaction amount
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction amount", Length = 20)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// transaction status
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction Status", Length = 32)]
    public string TradeStatus { get; set; }

    /// <summary>
    /// transaction completion time
    /// </summary>
    [SugarColumn(ColumnDescription = "transaction completion time")]
    public DateTime? FinishTime { get; set; }

    /// <summary>
    /// transaction title
    /// </summary>
    [SugarColumn(ColumnDescription = "transaction title", Length = 256)]
    public string Subject { get; set; }

    /// <summary>
    /// Transaction description
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction Description", Length = 512)]
    public string? Body { get; set; }

    /// <summary>
    /// Buyer’s Alipay account
    /// </summary>
    [SugarColumn(ColumnDescription = "Buyer's Alipay account", Length = 128)]
    public string? BuyerLogonId { get; set; }

    /// <summary>
    /// Buyer’s Alipay user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Buyer's Alipay User ID", Length = 32)]
    public string? BuyerUserId { get; set; }

    /// <summary>
    /// Seller Alipay user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Seller's Alipay User ID", Length = 32)]
    public string? SellerUserId { get; set; }

    /// <summary>
    /// MerchantAppId
    /// </summary>
    [SugarColumn(ColumnDescription = "Merchant AppId", Length = 64)]
    public string? AppId { get; set; }

    /// <summary>
    /// Transaction extension information
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction extension information", Length = 1024)]
    public string? ExtendInfo { get; set; }

    /// <summary>
    /// Transaction exception information
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction extension information", Length = 1024)]
    public string? ErrorInfo { get; set; }

    /// <summary>
    /// Transaction notes
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction Note", Length = 512)]
    public string? Remark { get; set; }
}