// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.Extensions;
using Azure.Messaging.EventHubs;

#pragma warning disable AZC0001 // https://github.com/Azure/azure-sdk-tools/issues/213
namespace Microsoft.Extensions.Azure
#pragma warning restore AZC0001
{
    /// <summary>
    ///    The set of extensions to add the Event Hub client types to the clients builder
    /// </summary>
    public static class EventHubClientBuilderExtensions
    {
        /// <summary>
        ///   Registers a <see cref="EventHubProducerClient"/> instance with the provided <paramref name="connectionString"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubProducerClient, EventHubProducerClientOptions> AddEventHubProducerClient<TBuilder>(this TBuilder builder, string connectionString)
            where TBuilder : IAzureClientFactoryBuilder
        {
            return builder.RegisterClientFactory<EventHubProducerClient, EventHubProducerClientOptions>(options => new EventHubProducerClient(connectionString, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubProducerClient"/> instance with the provided <paramref name="connectionString"/> and <paramref name="eventHubName"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubProducerClient, EventHubProducerClientOptions> AddEventHubProducerClient<TBuilder>(this TBuilder builder, string connectionString, string eventHubName)
            where TBuilder : IAzureClientFactoryBuilder
        {
            return builder.RegisterClientFactory<EventHubProducerClient, EventHubProducerClientOptions>(options => new EventHubProducerClient(connectionString, eventHubName, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubProducerClient"/> instance with the provided <paramref name="fullyQualifiedNamespace"/> and <paramref name="eventHubName"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubProducerClient, EventHubProducerClientOptions> AddEventHubProducerClientWithNamespace<TBuilder>(this TBuilder builder, string fullyQualifiedNamespace, string eventHubName)
            where TBuilder : IAzureClientFactoryBuilderWithCredential
        {
            return builder.RegisterClientFactory<EventHubProducerClient, EventHubProducerClientOptions>((options, token) => new EventHubProducerClient(fullyQualifiedNamespace, eventHubName, token, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubProducerClient"/> instance with connection options loaded from the provided <paramref name="configuration"/> instance.
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubProducerClient, EventHubProducerClientOptions> AddEventHubProducerClient<TBuilder, TConfiguration>(this TBuilder builder, TConfiguration configuration)
            where TBuilder : IAzureClientFactoryBuilderWithConfiguration<TConfiguration>
        {
            return builder.RegisterClientFactory<EventHubProducerClient, EventHubProducerClientOptions>(configuration);
        }

        /// <summary>
        ///   Registers a <see cref="EventHubConsumerClientOptions"/> instance with the provided <paramref name="connectionString"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubConsumerClient, EventHubConsumerClientOptions> AddEventHubConsumerClient<TBuilder>(this TBuilder builder, string consumerGroup, string partitionId, EventPosition startingPosition, string connectionString)
            where TBuilder : IAzureClientFactoryBuilder
        {
            return builder.RegisterClientFactory<EventHubConsumerClient, EventHubConsumerClientOptions>(options => new EventHubConsumerClient(consumerGroup, partitionId, startingPosition, connectionString, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubConsumerClient"/> instance with the provided <paramref name="connectionString"/> and <paramref name="eventHubName"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubConsumerClient, EventHubConsumerClientOptions> AddEventHubConsumerClient<TBuilder>(this TBuilder builder, string consumerGroup, string partitionId, EventPosition startingPosition, string connectionString, string eventHubName)
            where TBuilder : IAzureClientFactoryBuilder
        {
            return builder.RegisterClientFactory<EventHubConsumerClient, EventHubConsumerClientOptions>(options => new EventHubConsumerClient(consumerGroup, partitionId, startingPosition, connectionString, eventHubName, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubConsumerClient"/> instance with the provided <paramref name="fullyQualifiedNamespace"/> and <paramref name="eventHubName"/>
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubConsumerClient, EventHubConsumerClientOptions> AddEventHubConsumerClientWithNamespace<TBuilder>(this TBuilder builder, string consumerGroup, string partitionId, EventPosition startingPosition, string fullyQualifiedNamespace, string eventHubName)
            where TBuilder : IAzureClientFactoryBuilderWithCredential
        {
            return builder.RegisterClientFactory<EventHubConsumerClient, EventHubConsumerClientOptions>((options, token) => new EventHubConsumerClient(consumerGroup, partitionId, startingPosition, fullyQualifiedNamespace, eventHubName, token, options));
        }

        /// <summary>
        ///   Registers a <see cref="EventHubConsumerClient"/> instance with connection options loaded from the provided <paramref name="configuration"/> instance.
        /// </summary>
        ///
        public static IAzureClientBuilder<EventHubConsumerClient, EventHubConsumerClientOptions> AddEventHubConsumerClient<TBuilder, TConfiguration>(this TBuilder builder, TConfiguration configuration)
            where TBuilder : IAzureClientFactoryBuilderWithConfiguration<TConfiguration>
        {
            return builder.RegisterClientFactory<EventHubConsumerClient, EventHubConsumerClientOptions>(configuration);
        }
    }
}
