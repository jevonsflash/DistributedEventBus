using Abp.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.DistributedEventBus.Impl
{
    public class JsonDistributedEventSerializer : IDistributedEventSerializer,ISingletonDependency
    {
        private readonly JsonSerializerSettings settings;

        public JsonDistributedEventSerializer()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}