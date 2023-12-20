using Abp.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Abp.DistributedEventBus.Redis
{
    public class RedisDistributedEventProducer : IDistributedEventProducer
    {
        private readonly IDatabase _database;

        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly IDistributedEventSerializer _remoteEventSerializer;
        
        private bool _disposed;

        public RedisDistributedEventProducer(IRedisSetting redisSetting,IDistributedEventSerializer remoteEventSerializer)
        {
            Check.NotNullOrWhiteSpace(redisSetting.Server, "redisSetting.Server");

            _remoteEventSerializer = remoteEventSerializer;
            
            _connectionMultiplexer = ConnectionMultiplexer.Connect(redisSetting.Server);
            _database = _connectionMultiplexer.GetDatabase(redisSetting.DatabaseId);
        }

        public void Publish(string topic,IDistributedEventData remoteEventData)
        {
            _database.Publish(topic, _remoteEventSerializer.Serialize(remoteEventData));
        }

        public Task PublishAsync(string topic, IDistributedEventData remoteEventData)
        {
            return _database.PublishAsync(topic, remoteEventData.ToJsonString());
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connectionMultiplexer.Dispose();

                _disposed = true;
            }
        }
    }
}
