// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;

namespace Admin.NET.Core;

/// <summary>
/// Signature authentication processing
/// </summary>
public sealed class SignatureAuthenticationHandler : AuthenticationHandler<SignatureAuthenticationOptions>
{
#if NET6_0

    public SignatureAuthenticationHandler(IOptionsMonitor<SignatureAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

#else

    public SignatureAuthenticationHandler(IOptionsMonitor<SignatureAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

#endif

    private readonly SysCacheService _sysCacheService = App.GetRequiredService<SysCacheService>();

    private new SignatureAuthenticationEvent Events
    {
        get => (SignatureAuthenticationEvent)base.Events;
        set => base.Events = value;
    }

    /// <summary>
    /// Make sure the Event type you create is DigestEvents
    /// </summary>
    /// <returns></returns>
    protected override Task<object> CreateEventsAsync() => throw new NotImplementedException($"{nameof(SignatureAuthenticationOptions)}.{nameof(SignatureAuthenticationOptions.Events)} requires an instance to be provided");

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessKey = Request.Headers["accessKey"].FirstOrDefault();
        var timestampStr = Request.Headers["timestamp"].FirstOrDefault(); // Accurate to the second
        var nonce = Request.Headers["nonce"].FirstOrDefault();
        var sign = Request.Headers["sign"].FirstOrDefault();

        if (string.IsNullOrEmpty(accessKey))
            return await AuthenticateResultFailAsync("accessKey cannot be empty");
        if (string.IsNullOrEmpty(timestampStr))
            return await AuthenticateResultFailAsync("timestamp cannot be empty");
        if (string.IsNullOrEmpty(nonce))
            return await AuthenticateResultFailAsync("Nonce cannot be empty");
        if (string.IsNullOrEmpty(sign))
            return await AuthenticateResultFailAsync("sign cannot be empty");

        // Verify that the requested data is within an acceptable time
        if (!long.TryParse(timestampStr, out var timestamp))
            return await AuthenticateResultFailAsync("timestamp value is illegal");

        var requestDate = DateTimeUtil.ConvertUnixTime(timestamp);

#if NET6_0
        var utcNow = Clock.UtcNow;
#else
        var utcNow = TimeProvider.GetUtcNow();
#endif
        if (requestDate > utcNow.Add(Options.AllowedDateDrift).LocalDateTime || requestDate < utcNow.Subtract(Options.AllowedDateDrift).LocalDateTime)
            return await AuthenticateResultFailAsync("The timestamp value exceeds the allowed deviation range");

        // Get accessSecret
        var getAccessSecretContext = new GetAccessSecretContext(Context, Scheme, Options) { AccessKey = accessKey };
        var accessSecret = await Events.GetAccessSecret(getAccessSecretContext);
        if (string.IsNullOrEmpty(accessSecret))
            return await AuthenticateResultFailAsync("accessKey Noneeffect");

        // Verify signature
        var appSecretByte = Encoding.UTF8.GetBytes(accessSecret);
        string serverSign = SignData(appSecretByte, GetMessageForSign(Context));

        if (serverSign != sign)
            return await AuthenticateResultFailAsync("sign Invalid signature");

        // Replay detection
        var cacheKey = $"{CacheConst.KeyOpenAccessNonce}{accessKey}|{nonce}";
        if (_sysCacheService.ExistKey(cacheKey)) return await AuthenticateResultFailAsync("Duplicate request");
        _sysCacheService.Set(cacheKey, null, Options.AllowedDateDrift * 2); // The cache expiration time is twice the deviation range time

        // Verified successfully
        var signatureValidatedContext = new SignatureValidatedContext(Context, Scheme, Options)
        {
            Principal = new ClaimsPrincipal(new ClaimsIdentity(SignatureAuthenticationDefaults.AuthenticationScheme)),
            AccessKey = accessKey
        };
        await Events.Validated(signatureValidatedContext);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (signatureValidatedContext.Result != null)
            return signatureValidatedContext.Result;

        // ReSharper disable once HeuristicUnreachableCode
        signatureValidatedContext.Success();
        return signatureValidatedContext.Result;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync();
        var challengeContext = new SignatureChallengeContext(Context, Scheme, Options, properties)
        {
            AuthenticateFailure = authResult.Failure,
        };
        await Events.Challenge(challengeContext);
        // Inquiry has been handled
        if (challengeContext.Handled) return;

        await base.HandleChallengeAsync(properties);
    }

    /// <summary>
    /// Get the message for signing
    /// </summary>
    /// <returns></returns>
    private static string GetMessageForSign(HttpContext context)
    {
        var method = context.Request.Method; // Request method (uppercase)
        var url = context.Request.Path; // Request url, remove protocol, domain name, parameters, start with /
        var accessKey = context.Request.Headers["accessKey"].FirstOrDefault(); // Identity mark
        var timestamp = context.Request.Headers["timestamp"].FirstOrDefault(); // Timestamp, accurate to seconds
        var nonce = context.Request.Headers["nonce"].FirstOrDefault(); // unique random number

        return $"{method}&{url}&{accessKey}&{timestamp}&{nonce}";
    }

    /// <summary>
    /// Sign data
    /// </summary>
    /// <param name="secret"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static string SignData(byte[] secret, string data)
    {
        if (secret == null)
            throw new ArgumentNullException(nameof(secret));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using HMAC hmac = new HMACSHA256();
        hmac.Key = secret;
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
    }

    /// <summary>
    /// Return the verification failure result, and add <see cref="SignatureAuthenticationDefaults.AuthenticateFailMsgKey"/> in Items to record the authentication failure message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private Task<AuthenticateResult> AuthenticateResultFailAsync(string message)
    {
        // Write authentication failure message
        Context.Items[SignatureAuthenticationDefaults.AuthenticateFailMsgKey] = message;
        return Task.FromResult(AuthenticateResult.Fail(message));
    }
}