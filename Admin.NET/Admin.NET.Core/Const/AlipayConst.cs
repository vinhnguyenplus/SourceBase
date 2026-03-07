// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Alipay payment constants
/// </summary>
[SuppressSniffer]
public class AlipayConst
{
    /// <summary>
    /// Single unsecured transfer [Business Scenario] Fixed value
    /// </summary>
    public const string BizScene = "DIRECT_TRANSFER";

    /// <summary>
    /// Single unsecured transfer [sales product code] fixed value
    /// </summary>
    public const string ProductCode = "TRANS_ACCOUNT_NO_PWD";

    /// <summary>
    /// Transaction status parameter name
    /// </summary>
    public const string TradeStatus = "trade_status";

    /// <summary>
    /// Transaction success indicator
    /// </summary>
    public const string TradeSuccess = "TRADE_SUCCESS";

    /// <summary>
    /// Authorization type
    /// </summary>
    public const string GrantType = "authorization_code";
}