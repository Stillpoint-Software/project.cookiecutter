using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Context;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Core.Commands.Middleware;

public static partial class CommandMiddlewareExtensions
{
    //BF
    public static IPipelineBuilder<TInput, TOutput> WithTelemetry1<TInput, TOutput>(
        this IPipelineStartBuilder<TInput, TOutput> builder,
        string activityName,
        ActivityKind activityKind,
        Func<IPipelineBuilder<TInput, TOutput>, IPipelineBuilder<TInput, TOutput>> configure
    )
    {
        return configure(
            builder.HookAsync( TelemetryHookAsync )
        ).WrapAsync( TelemetryWrapAsync );

        async Task<object> TelemetryHookAsync(
            IPipelineContext context,
            object argument,
            FunctionAsync<object, object> next )
        {
            // create activity
            var telemetrySourceProvider = context.ServiceProvider.GetService<ITelemetrySourceProvider>();

            using var activity = telemetrySourceProvider
                .GetActivitySource()
                .StartActivity( context.Name, ActivityKind.Internal );

            var result = await next( context, argument ).ConfigureAwait( false );
            return result;
        }

        async Task<TOutput> TelemetryWrapAsync(
            IPipelineContext context,
            TInput argument,
            FunctionAsync<TInput, TOutput> next )
        {
            // create activity
            var telemetrySourceProvider = context.ServiceProvider.GetService<ITelemetrySourceProvider>();

            using var activity = telemetrySourceProvider
                .GetActivitySource()
                .StartActivity( activityName, activityKind );

            var result = await next( context, argument ).ConfigureAwait( false );
            return result;
        }
    }

    public static IPipelineStartBuilder<TInput, TOutput> WithTelemetry<TInput, TOutput>(
        this IPipelineStartBuilder<TInput, TOutput> builder,
        ITelemetrySourceProvider telemetryProvider,
        string activityName,
        ActivityKind activityKind = ActivityKind.Internal )
    {
        return builder.HookAsync( async ( context, argument, next ) =>
        {
            //Extracts incoming trace context on the consumer side
            PropagationContext parentContext = default;
            if ( activityKind == ActivityKind.Consumer )
            {
                parentContext = Propagator.Extract(
                    default,
                    context.Items,
                    ( items, key ) =>
                        items.TryGetValue<string>( key, out var raw ) ? [raw] : Array.Empty<string>() );
                Baggage.Current = parentContext.Baggage;
            }

            // Starts the span
            using var activity = telemetryProvider
                .GetActivitySource()
                .StartActivity(
                    activityName,
                    activityKind,
                    activityKind == ActivityKind.Consumer ? parentContext.ActivityContext : default );

            if ( activity != null )
            {
                // Producer-side
                if ( activityKind == ActivityKind.Producer
                    && context.Items.TryGetValue<ServiceBusMessage>( "ServiceBusMessage", out var sbMsg ) )
                {
                    // propagate context
                    Propagator.Inject(
                        new PropagationContext( activity.Context, Baggage.Current ),
                        sbMsg.ApplicationProperties,
                        ( props, key, value ) => props[key] = value );

                    // Enrich span with message details
                    activity.SetTag( "messaging.correlation_id", sbMsg.CorrelationId );
                    if ( !string.IsNullOrEmpty( sbMsg.MessageId ) )
                        activity.SetTag( "messaging.message_id", sbMsg.MessageId );
                    var payload = sbMsg.Body.ToString();
                    activity.SetTag( "messaging.payload_size_bytes", sbMsg.Body.ToArray().Length );
                    activity.SetTag( "messaging.message_payload", payload );
                }

                activity.SetTag( "pipeline.name", context.Name );
                activity.SetTag( "messaging.system", "azure.servicebus" );
                activity.SetTag( "messaging.destination", "topic" );
                activity.SetTag( "messaging.destination_kind", "topic" );
                activity.SetTag(
                    "messaging.operation",
                    activityKind == ActivityKind.Producer ? "publish" : "receive" );

                // Consumer-side
                if ( activityKind == ActivityKind.Consumer
                    && context.Items.TryGetValue<ServiceBusReceivedMessage>( "ServiceBusReceivedMessage", out var recMsg ) )
                {
                    activity.SetTag( "messaging.message_id", recMsg.MessageId );
                    activity.SetTag( "messaging.payload_size_bytes", recMsg.Body.ToArray().Length );
                    var recPayload = recMsg.Body.ToString();
                    activity.SetTag( "messaging.message_payload", recPayload );
                }
            }

            try
            {
                var result = await next( context, argument ).ConfigureAwait( false );
                activity?.SetTag( "otel.status_code", "OK" );
                return result;
            }
            catch ( Exception ex )
            {
                if ( activity != null )
                {
                    activity.SetStatus( ActivityStatusCode.Error, ex.Message );
                    activity.SetTag( "otel.status_code", "ERROR" );
                    activity.SetTag( "otel.status_description", ex.Message );
                    activity.AddEvent( new ActivityEvent(
                        "exception",
                        DateTimeOffset.UtcNow,
                        new ActivityTagsCollection
                        {
                        { "exception.type", ex.GetType().FullName },
                        { "exception.message", ex.Message },
                        { "exception.stacktrace", ex.StackTrace ?? string.Empty }
                        } ) );
                }
                throw;
            }
        } );
    }
}
