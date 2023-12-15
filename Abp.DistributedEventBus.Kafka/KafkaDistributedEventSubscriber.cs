using Castle.Core.Logging;
using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus.Kafka
{
    public class KafkaDistributedEventSubscriber : IDistributedEventSubscriber
    {
        public ILogger Logger { get; set; }

        private readonly ConcurrentDictionary<string, IConsumer<Ignore, string>> _dictionary;

        private readonly IKafkaSetting _kafkaSetting;

        private bool _cancelled;

        private bool _disposed;

        public KafkaDistributedEventSubscriber(IKafkaSetting kafkaSetting)
        {
            Check.NotNullOrWhiteSpace(kafkaSetting.Properties["bootstrap.servers"] as string, "bootstrap.servers");

            _kafkaSetting = kafkaSetting;

            _dictionary = new ConcurrentDictionary<string, IConsumer<Ignore, string>>();

            Logger = NullLogger.Instance;
        }

        public void Subscribe(IEnumerable<string> topics, Action<string, string> handler)
        {
            var existsTopics = topics.ToList().Where(p => _dictionary.ContainsKey(p));
            if (existsTopics.Any())
            {
                throw new AbpException(string.Format("Topics {0} have subscribed already", string.Join(",", existsTopics)));
            }

            topics.ToList().ForEach(topic =>
            {
                var conf = new ConsumerConfig(_kafkaSetting.Properties as IDictionary<string, string>);



                // Note: If a key or value deserializer is not set (as is the case below), the 
                // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
                // will be used automatically (where available). The default deserializer for string
                // is UTF8. The default deserializer for Ignore returns null for all input data
                // (including non-null data).
                using (var consumer = new ConsumerBuilder<Ignore, string>(conf)
                    // Note: All handlers are called on the main .Consume thread.
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                    .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                    .SetPartitionsAssignedHandler((c, partitions) =>
                    {
                        // Since a cooperative assignor (CooperativeSticky) has been configured, the
                        // partition assignment is incremental (adds partitions to any existing assignment).
                        Console.WriteLine(
                            "Partitions incrementally assigned: [" +
                            string.Join(',', partitions.Select(p => p.Partition.Value)) +
                            "], all: [" +
                            string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)) +
                            "]");

                        // Possibly manually specify start offsets by returning a list of topic/partition/offsets
                        // to assign to, e.g.:
                        // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                    })
                    .SetPartitionsRevokedHandler((c, partitions) =>
                    {
                        // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                        // assignment is incremental (may remove only some partitions of the current assignment).
                        var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                        Console.WriteLine(
                            "Partitions incrementally revoked: [" +
                            string.Join(',', partitions.Select(p => p.Partition.Value)) +
                            "], remaining: [" +
                            string.Join(',', remaining.Select(p => p.Partition.Value)) +
                            "]");
                    })
                    .SetPartitionsLostHandler((c, partitions) =>
                    {
                        // The lost partitions handler is called when the consumer detects that it has lost ownership
                        // of its assignment (fallen out of the group).
                        Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
                    })
                    .Build())
                {
                    consumer.Subscribe(topics);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume();

                                if (consumeResult.IsPartitionEOF)
                                {
                                    Console.WriteLine(
                                        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                    continue;
                                }


                                Logger.Debug($"Topic: {consumeResult.Topic} Partition: {consumeResult.Partition} Offset: {consumeResult.Offset} {consumeResult.Value}");

                                try
                                {
                                    handler(consumeResult.Topic, consumeResult.Message.Value);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Consume error", ex);
                                }



                                try
                                {
                                    // Store the offset associated with consumeResult to a local cache. Stored offsets are committed to Kafka by a background thread every AutoCommitIntervalMs. 
                                    // The offset stored is actually the offset of the consumeResult + 1 since by convention, committed offsets specify the next message to consume. 
                                    // If EnableAutoOffsetStore had been set to the default value true, the .NET client would automatically store offsets immediately prior to delivering messages to the application. 
                                    // Explicitly storing offsets after processing gives at-least once semantics, the default behavior does not.
                                    consumer.StoreOffset(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                                }
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Consume error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Closing consumer.");
                        consumer.Close();
                    }
                }

            });                
    }

    public Task SubscribeAsync(IEnumerable<string> topics, Action<string, string> handler)
    {
        return Task.Factory.StartNew(() =>
         {
             Subscribe(topics, handler);
         });
    }

    public void Unsubscribe(IEnumerable<string> topics)
    {
        _dictionary.Where(p => topics.Contains(p.Key)).Select(p => p.Value).ToList().ForEach(p => p.Unsubscribe());
    }

    public Task UnsubscribeAsync(IEnumerable<string> topics)
    {
        return Task.Factory.StartNew(() => Unsubscribe(topics));
    }

    public void UnsubscribeAll()
    {
        _dictionary.Select(p => p.Value).ToList().ForEach(p => p.Unsubscribe());
    }

    public Task UnsubscribeAllAsync()
    {
        return Task.Factory.StartNew(() => UnsubscribeAll());
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cancelled = true;
            UnsubscribeAll();
            _dictionary.Select(p => p.Value).ToList().ForEach(consumer => consumer?.Dispose());

            _disposed = true;
        }
    }
}
}
