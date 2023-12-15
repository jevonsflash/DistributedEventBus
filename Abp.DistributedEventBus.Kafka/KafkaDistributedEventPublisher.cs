using Castle.Core.Logging;
using Confluent.Kafka;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus.Kafka
{
    public class KafkaDistributedEventPublisher : IDistributedEventPublisher
    {
        public ILogger Logger { get; set; }

        private readonly IKafkaSetting _kafkaSetting;
        private readonly IDistributedEventSerializer _remoteEventSerializer;

        private readonly IProducer<Null, string> _producer;

        private bool _disposed;

        public KafkaDistributedEventPublisher(IKafkaSetting kafkaSetting, IDistributedEventSerializer remoteEventSerializer)
        {
            Check.NotNullOrWhiteSpace(kafkaSetting.Properties["bootstrap.servers"] as string, "bootstrap.servers");

            _kafkaSetting = kafkaSetting;
            _remoteEventSerializer = remoteEventSerializer;

            Logger = NullLogger.Instance;
            var config = new ProducerConfig(_kafkaSetting.Properties as IDictionary<string, string>);

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                _producer = producer;

            }
        }

        public void Publish(string topic, IDistributedEventData remoteEventData)
        {
            PublishAsync(topic, remoteEventData);
            //_producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task PublishAsync(string topic, IDistributedEventData remoteEventData)
        {
            Logger.Debug($"{_producer.Name} producing on {topic}");

            var deliveryReport = _producer.ProduceAsync(topic, new Message<Null, string>() { Value = _remoteEventSerializer.Serialize(remoteEventData) });
            return deliveryReport.ContinueWith(task =>
            {
                Logger.Debug($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _producer?.Dispose();
                _disposed = true;
            }
        }
    }
}