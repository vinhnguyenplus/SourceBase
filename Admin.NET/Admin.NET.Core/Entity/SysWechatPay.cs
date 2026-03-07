// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System WeChat payment form
/// </summary>
[SugarTable(null, "System WeChat Pay Table")]
[SysTable]
[SugarIndex("index_{table}_BU", nameof(BusinessId), OrderByType.Asc)]
[SugarIndex("index_{table}_TR", nameof(TradeState), OrderByType.Asc)]
[SugarIndex("index_{table}_TA", nameof(Tags), OrderByType.Asc)]
public partial class SysWechatPay : EntityBase
{
    /// <summary>
    /// WeChat merchant account
    /// </summary>
    [SugarColumn(ColumnDescription = "WeChat merchant account")]
    [Required]
    public virtual string MerchantId { get; set; }

    /// <summary>
    /// Service provider AppId
    /// </summary>
    [SugarColumn(ColumnDescription = "Service provider AppId")]
    [Required]
    public virtual string AppId { get; set; }

    /// <summary>
    /// Merchant order number
    /// </summary>
    [SugarColumn(ColumnDescription = "Merchant Order Number")]
    [Required]
    public virtual string OutTradeNumber { get; set; }

    /// <summary>
    /// Payment order number
    /// </summary>
    [SugarColumn(ColumnDescription = "Payment Order Number")]
    [Required]
    public virtual string TransactionId { get; set; }

    /// <summary>
    /// transaction type
    /// </summary>
    [SugarColumn(ColumnDescription = "transaction type")]
    public string? TradeType { get; set; }

    /// <summary>
    /// transaction status
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction Status")]
    public string? TradeState { get; set; }

    /// <summary>
    /// Transaction status description
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction status description")]
    public string? TradeStateDescription { get; set; }

    /// <summary>
    /// Payment bank type
    /// </summary>
    [SugarColumn(ColumnDescription = "Payment bank type")]
    public string? BankType { get; set; }

    /// <summary>
    /// Total order amount
    /// </summary>
    [SugarColumn(ColumnDescription = "Total Order Amount")]
    public int Total { get; set; }

    /// <summary>
    /// User payment amount
    /// </summary>
    [SugarColumn(ColumnDescription = "User Payment Amount")]
    public int? PayerTotal { get; set; }

    /// <summary>
    /// Payment completion time
    /// </summary>
    [SugarColumn(ColumnDescription = "Payment Completion Time")]
    public DateTime? SuccessTime { get; set; }

    /// <summary>
    /// transaction end time
    /// </summary>
    [SugarColumn(ColumnDescription = "Transaction End Time")]
    public DateTime? ExpireTime { get; set; }

    /// <summary>
    /// Product description
    /// </summary>
    [SugarColumn(ColumnDescription = "Product description")]
    public string? Description { get; set; }

    /// <summary>
    /// scene information
    /// </summary>
    [SugarColumn(ColumnDescription = "scene information")]
    public string? Scene { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    [SugarColumn(ColumnDescription = "AdditionalData")]
    public string? Attachment { get; set; }

    /// <summary>
    /// Offer mark
    /// </summary>
    [SugarColumn(ColumnDescription = "Discount Tag")]
    public string? GoodsTag { get; set; }

    /// <summary>
    /// Billing information
    /// </summary>
    [SugarColumn(ColumnDescription = "Billing information")]
    public string? Settlement { get; set; }

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

    /// <summary>
    /// WeChat OpenId logo
    /// </summary>
    [SugarColumn(ColumnDescription = "WeChat OpenId Identifier")]
    public string? OpenId { get; set; }

    /// <summary>
    /// Business tags, used to distinguish what business is being done
    /// </summary>
    /// <remarks>
    /// Tags are used to distinguish what business this payment record corresponds to determine the associated table name.
    /// Combined with the BusinessId to save the ID of the corresponding business data, the payment can be determined.
    /// Which piece of business data is the record associated with?
    /// </remarks>
    [SugarColumn(ColumnDescription = "Business Tag，used to dividepointsWhat business do you do?", Length = 64)]
    public string? Tags { get; set; }

    /// <summary>
    /// The primary key corresponding to the business
    /// </summary>
    [SugarColumn(ColumnDescription = "The primary key corresponding to the business")]
    public long BusinessId { get; set; }

    /// <summary>
    /// Payment QR code content
    /// </summary>
    [SugarColumn(ColumnDescription = "Payment QR code content")]
    public string? QrcodeContent { get; set; }

    /// <summary>
    /// Associated WeChat users
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(OpenId))]
    public SysWechatUser SysWechatUser { get; set; }

    /// <summary>
    /// Sub-merchant number
    /// </summary>
    [SugarColumn(ColumnDescription = "Sub-merchant number")]
    public string? SubMerchantId { get; set; }

    /// <summary>
    /// Sub-merchant AppId
    /// </summary>
    [SugarColumn(ColumnDescription = "Callback notification address")]
    public string? SubAppId { get; set; }

    /// <summary>
    /// Sub-merchant unique identifier
    /// </summary>
    [SugarColumn(ColumnDescription = "childMerchant OnlyoneLogo")]
    public string? SubOpenId { get; set; }
}