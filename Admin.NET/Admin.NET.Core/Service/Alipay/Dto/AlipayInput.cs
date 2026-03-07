// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Aop.Api.Domain;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Admin.NET.Core.Service;

public class AlipayFundTransUniTransferInput
{
    /// <summary>
    /// User ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// MerchantAppId
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// Merchant order number
    /// </summary>
    public string OutBizNo { get; set; }

    /// <summary>
    /// Transfer amount
    /// </summary>
    public decimal TransAmount { get; set; }

    /// <summary>
    /// business title
    /// </summary>
    public string OrderTitle { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// Whether to display the payer alias
    /// </summary>
    public bool PayerShowNameUseAlias { get; set; }

    /// <summary>
    /// Payee ID type
    /// </summary>
    public AlipayCertTypeEnum? CertType { get; set; }

    /// <summary>
    /// Payee ID number, required
    /// </summary>
    public string CertNo { get; set; }

    /// <summary>
    /// Payee ID
    /// </summary>
    public string Identity { get; set; }

    /// <summary>
    /// Payee’s real name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Payee ID type
    /// </summary>
    public AlipayIdentityTypeEnum? IdentityType { get; set; }
}

/// <summary>
///  Unified order collection and payment page interface input parameters
/// </summary>
public class AlipayTradePagePayInput
{
    /// <summary>
    /// Merchant order number
    /// </summary>
    [Required(ErrorMessage = "Merchant order number cannot be empty")]
    public string OutTradeNo { get; set; }

    /// <summary>
    /// Total order amount
    /// </summary>
    [Required(ErrorMessage = "The total order amount cannot be empty")]
    public string TotalAmount { get; set; }

    /// <summary>
    /// Order title
    /// </summary>
    [Required(ErrorMessage = "Order title cannot be empty")]
    public string Subject { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// timeout
    /// </summary>
    public string TimeoutExpress { get; set; }

    /// <summary>
    /// QR code width
    /// </summary>
    [Required(ErrorMessage = "QR code width cannot be empty")]
    public int? QrcodeWidth { get; set; }

    /// <summary>
    /// Business parameters
    /// </summary>
    public ExtendParams ExtendParams { get; set; }

    /// <summary>
    /// Merchant business data
    /// </summary>
    public Dictionary<string, object> BusinessParams { get; set; }

    /// <summary>
    /// Billing information
    /// </summary>
    public InvoiceInfo InvoiceInfo { get; set; }

    /// <summary>
    /// External buyer information
    /// </summary>
    public ExtUserInfo ExtUserInfo { get; set; }
}

public class AlipayPreCreateInput
{
    /// <summary>
    /// Merchant order number
    /// </summary>
    [Required(ErrorMessage = "Merchant order number cannot be empty")]
    public string OutTradeNo { get; set; }

    /// <summary>
    /// Total order amount
    /// </summary>
    [Required(ErrorMessage = "The total order amount cannot be empty")]
    public string TotalAmount { get; set; }

    /// <summary>
    /// Order title
    /// </summary>
    [Required(ErrorMessage = "Order title cannot be empty")]
    public string Subject { get; set; }

    /// <summary>
    /// timeout
    /// </summary>
    public string TimeoutExpress { get; set; }
}

public class AlipayAuthInfoInput
{
    /// <summary>
    /// UserId
    /// </summary>

    [JsonProperty("user_id")]
    [JsonPropertyName("user_id")]
    [FromQuery(Name = "user_id")]
    public string UserId { get; set; }

    /// <summary>
    /// Authorization code
    /// </summary>
    [JsonProperty("auth_code")]
    [JsonPropertyName("auth_code")]
    [FromQuery(Name = "auth_code")]
    public string AuthCode { get; set; }
}