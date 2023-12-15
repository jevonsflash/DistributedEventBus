using Abp.Dependency;
using Abp.DistributedEventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus
{
    public class MultipleEventTopicSelector : IDistributedEventTopicSelector
    {

        private readonly Dictionary<Type, string> _mapping;

        public MultipleEventTopicSelector()
        {
            _mapping = new Dictionary<Type, string>();
        }

        public void SetMapping<T>(string topic) where T : IDistributedEventData
        {
            _mapping[typeof(T)] = topic;
        }

        public string SelectTopic(IDistributedEventData eventData)
        {
            return eventData.Type;          
        }
    }
}
