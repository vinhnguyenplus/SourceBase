// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Event executor - timeout control, failure retry circuit breaker, etc.
/// </summary>
public class RetryEventHandlerExecutor : IEventHandlerExecutor
{
    public async Task ExecuteAsync(EventHandlerExecutingContext context, Func<EventHandlerExecutingContext, Task> handler)
    {
        var eventSubscribeAttribute = context.Attribute;
        // Determine whether the retry failure callback service has been customized
        var fallbackPolicyService = eventSubscribeAttribute?.FallbackPolicy == null
            ? null
            : App.GetRequiredService(eventSubscribeAttribute.FallbackPolicy) as IEventFallbackPolicy;

        await Retry.InvokeAsync(async () =>
        {
            try
            {
                await handler(context);
            }
            catch (Exception ex)
            {
                Log.Error($"Invoke EventHandler {context.Source.EventId} Error", ex);
                throw;
            }
        }
        , eventSubscribeAttribute?.NumRetries ?? 0
        , eventSubscribeAttribute?.RetryTimeout ?? 1000
        , exceptionTypes: eventSubscribeAttribute?.ExceptionTypes
        , fallbackPolicy: fallbackPolicyService == null ? null : async (Exception ex) => { await fallbackPolicyService.CallbackAsync(context, ex); }
        , retryAction: (total, times) =>
        {
            // Output retry log
            Log.Warning($"Retrying {times}/{total} times for  EventHandler {context.Source.EventId}");
        });
    }
}