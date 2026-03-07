// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// SMS configuration options
/// </summary>
public sealed class SMSOptions : IConfigurableOptions
{
    /// <summary>
    /// Verification code cache expiration time (seconds)
    /// Default: 60 seconds
    /// </summary>
    public int VerifyCodeExpireSeconds { get; set; } = 60;

    /// <summary>
    /// Aliyun
    /// </summary>
    public SMSSettings Aliyun { get; set; }

    /// <summary>
    /// Tencentyun
    /// </summary>
    public SMSSettings Tencentyun { get; set; }

    /// <summary>
    /// Custom Custom SMS interface
    /// </summary>
    public CustomSMSSettings Custom { get; set; }
}

public sealed class SMSSettings
{
    /// <summary>
    /// SdkAppId
    /// </summary>
    public string SdkAppId { get; set; }

    /// <summary>
    /// AccessKey ID
    /// </summary>
    public string AccessKeyId { get; set; }

    /// <summary>
    /// AccessKey Secret
    /// </summary>
    public string AccessKeySecret { get; set; }

    /// <summary>
    /// Templates
    /// </summary>
    public List<SmsTemplate> Templates { get; set; }

    /// <summary>
    /// GetTemplate
    /// </summary>
    public SmsTemplate GetTemplate(string id = "0")
    {
        foreach (var template in Templates)
        {
            if (template.Id == id) { return template; }
        }
        return null;
    }
}

public class SmsTemplate
{
    public string Id { get; set; } = string.Empty;
    public string SignName { get; set; }
    public string TemplateCode { get; set; }
    public string Content { get; set; }
}

/// <summary>
/// Custom SMS configuration
/// </summary>
public sealed class CustomSMSSettings
{
    /// <summary>
    /// Whether to enable custom SMS interface
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// API interface address template
    /// <para>Supported placeholders: {mobile} - mobile phone number, {content} - text message content, {code} - verification code</para>
    /// </summary>
    /// <remarks>Example: https://api.xxxx.com/sms?u=xxxx&amp;key=59e03f49c3dbb5033&amp;m={mobile}&amp;c={content}</remarks>
    public string ApiUrl { get; set; }

    /// <summary>
    /// Request method (GET/POST)
    /// </summary>
    public string Method { get; set; } = "GET";

    /// <summary>
    /// Content-Type of POST request (application/json or application/x-www-form-urlencoded)
    /// Default: application/x-www-form-urlencoded
    /// </summary>
    public string ContentType { get; set; } = "application/x-www-form-urlencoded";

    /// <summary>
    /// Data template for POST request (supports placeholders)
    /// </summary>
    /// <remarks>
    /// JSON format example: {"mobile":"{mobile}","content":"{content}","apikey":"your_key"} <br />
    /// Form format example: mobile={mobile}&amp;content={content}&amp;apikey=your_key
    /// </remarks>
    public string PostData { get; set; }

    /// <summary>
    /// Success response identifier (used to determine whether the transmission is successful)
    /// If the response content contains this string, the sending is considered successful
    /// </summary>
    public string SuccessFlag { get; set; } = "0";

    /// <summary>
    /// SMS template list
    /// </summary>
    public List<SmsTemplate> Templates { get; set; }

    /// <summary>
    /// Get template
    /// </summary>
    public SmsTemplate GetTemplate(string id = "0")
    {
        foreach (var template in Templates)
        {
            if (template.Id == id) { return template; }
        }
        return null;
    }
}