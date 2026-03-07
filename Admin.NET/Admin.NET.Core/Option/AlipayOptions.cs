// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Aop.Api;

namespace Admin.NET.Core;

/// <summary>
/// Alipay payment configuration options
/// </summary>
public sealed class AlipayOptions : IConfigurableOptions
{
    /// <summary>
    /// Alipay gateway address
    /// </summary>
    public string ServerUrl { get; init; }

    /// <summary>
    /// Alipay authorized callback address
    /// </summary>
    public string AuthUrl { get; init; }

    /// <summary>
    /// Application authorization callback address
    /// </summary>
    public string AppAuthUrl { get; init; }

    /// <summary>
    /// Alipay websocket service address
    /// </summary>
    public string WebsocketUrl { get; init; }

    /// <summary>
    /// Application callback address
    /// </summary>
    public string NotifyUrl { get; init; }

    /// <summary>
    /// Alipay root certificate storage path
    /// </summary>
    public string RootCertPath { get; init; }

    /// <summary>
    /// Alipay merchant account list
    /// </summary>
    public List<AlipayMerchantAccount> AccountList { get; init; }

    /// <summary>
    /// Get Alipay client
    /// </summary>
    /// <param name="account"></param>
    public DefaultAopClient GetClient(AlipayMerchantAccount account)
    {
        account = account ?? throw new Exception("Alipay merchant account not found");
        string path = App.WebHostEnvironment.ContentRootPath;
        return new DefaultAopClient(new AlipayConfig
        {
            Format = "json",
            Charset = "UTF-8",
            ServerUrl = ServerUrl,
            AppId = account.AppId,
            SignType = account.SignType,
            PrivateKey = account.PrivateKey,
            EncryptKey = account.EncryptKey,
            RootCertPath = Path.Combine(path, RootCertPath),
            AppCertPath = Path.Combine(path, account.AppCertPath),
            AlipayPublicCertPath = Path.Combine(path, account.AlipayPublicCertPath)
        });
    }
}

/// <summary>
/// Alipay merchant account information
/// </summary>
public class AlipayMerchantAccount
{
    /// <summary>
    /// ConfigurationId
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Merchant name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// MerchantAppId
    /// </summary>
    public string AppId { get; init; }

    /// <summary>
    /// Apply private key
    /// </summary>
    public string PrivateKey { get; init; }

    /// <summary>
    /// Encryption key when obtaining sensitive information from Alipay (optional)
    /// </summary>
    public string EncryptKey { get; init; }

    /// <summary>
    /// encryption algorithm
    /// </summary>
    public string SignType { get; init; }

    /// <summary>
    /// Apply public key certificate path
    /// </summary>
    public string AppCertPath { get; init; }

    /// <summary>
    /// Alipay public key certificate path
    /// </summary>
    public string AlipayPublicCertPath { get; init; }
}