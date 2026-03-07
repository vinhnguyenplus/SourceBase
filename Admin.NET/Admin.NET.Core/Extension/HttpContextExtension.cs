// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.Authentication;

namespace Admin.NET.Core;

public static class HttpContextExtension
{
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        ArgumentNullException.ThrowIfNull(context);

        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }

    /// <summary>
    /// Get device information
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientDeviceInfo(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return CommonUtil.GetClientDeviceInfo(context.Request.Headers.UserAgent);
    }

    /// <summary>
    /// Get browser information
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientBrowser(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string userAgent = context.Request.Headers.UserAgent;
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "Crawler";
                return $"{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}";
            }
        }
        catch
        { }
        return "unknown";
    }

    /// <summary>
    /// Get operating system information
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientOs(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string userAgent = context.Request.Headers.UserAgent;
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "Crawler";
                return $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}";
            }
        }
        catch
        { }
        return "unknown";
    }
}