using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus
{
    public interface IDistributedEventTopicSelector
    {
        string SelectTopic(IDistributedEventData eventData);

        void SetMapping<T>(string topic) where T : IDistributedEventData;
    }
}
