using Abp.Dependency;
using Abp.DistributedEventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus
{
    public class DistributedEventTopicSelector : IDistributedEventTopicSelector, ISingletonDependency
    {
        public const string TOPIC_DEFAULT = "TOPIC_DEFAULT";

        private readonly Dictionary<Type, string> _mapping;

        public DistributedEventTopicSelector()
        {
            _mapping = new Dictionary<Type, string>();
        }

        public void SetMapping<T>(string topic) where T : IDistributedEventData
        {
            _mapping[typeof(T)] = topic;
        }

        public string SelectTopic(IDistributedEventData eventData)
        {
            if (eventData is IHasTopicDistributedEventData)
            {
                return (eventData as IHasTopicDistributedEventData).Topic;
            }
            foreach (var item in _mapping)
            {
                if (item.Key.IsAssignableFrom(eventData.GetType()))
                {
                    return item.Value;
                }
            }
            return TOPIC_DEFAULT;
        }
    }
}
