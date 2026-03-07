// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Signature authentication event
/// </summary>
public class SignatureAuthenticationEvent
{
    public SignatureAuthenticationEvent()
    {
    }

    /// <summary>
    /// Get or set the logic processing of AccessSecret to get AccessKey
    /// </summary>
    public Func<GetAccessSecretContext, Task<string>> OnGetAccessSecret { get; set; }

    /// <summary>
    /// Gets or sets the logical processing of the challenge
    /// </summary>
    public Func<SignatureChallengeContext, Task> OnChallenge { get; set; } = _ => Task.CompletedTask;

    /// <summary>
    /// Gets or sets the verified logic processing
    /// </summary>
    public Func<SignatureValidatedContext, Task> OnValidated { get; set; } = _ => Task.CompletedTask;

    /// <summary>
    /// Get the AccessSecret of AccessKey
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task<string> GetAccessSecret(GetAccessSecretContext context) => OnGetAccessSecret?.Invoke(context) ?? throw new NotImplementedException($"Need to provide {nameof(OnGetAccessSecret)} implementation");

    /// <summary>
    /// question
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task Challenge(SignatureChallengeContext context) => OnChallenge?.Invoke(context) ?? Task.CompletedTask;

    /// <summary>
    /// Verified successfully
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task Validated(SignatureValidatedContext context) => OnValidated?.Invoke(context) ?? Task.CompletedTask;
}