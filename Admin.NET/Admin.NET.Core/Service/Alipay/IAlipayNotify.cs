// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Aop.Api.Response;

namespace Admin.NET.Core.Service;

/// <summary>
/// Alipay callback interface
/// </summary>
public abstract class IAlipayNotify
{
    /// <summary>
    /// Recharge callback method
    /// </summary>
    /// <param name="type">transaction type</param>
    /// <param name="tradeNo">transaction id</param>
    public abstract bool TopUpCallback(long type, long tradeNo);

    /// <summary>
    /// Scan code callback
    /// </summary>
    /// <param name="type"></param>
    /// <param name="userId"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    public abstract bool ScanCallback(long type, long userId, AlipayUserInfoShareResponse response);
}