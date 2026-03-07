// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class WechatPayOutput
{
    /// <summary>
    /// OpenId
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// Order amount
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    public string Attachment { get; set; }

    /// <summary>
    /// Offer mark
    /// </summary>
    public string GoodsTag { get; set; }
}

public class WechatPayTransactionOutput
{
    public string PrepayId { get; set; }

    public string OutTradeNumber { get; set; }

    public WechatPayParaOutput SingInfo { get; set; }
}

public class WechatPayParaOutput
{
    public string AppId { get; set; }

    public string TimeStamp { get; set; }

    public string NonceStr { get; set; }

    public string Package { get; set; }

    public string SignType { get; set; }

    public string PaySign { get; set; }
}