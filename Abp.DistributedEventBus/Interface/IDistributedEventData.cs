using System.Collections.Generic;
using Abp.Events.Bus;

namespace Abp.DistributedEventBus
{
    public interface IDistributedEventData : IEventData
    {
        string Type { get; set; }

        Dictionary<string, object> Data { get; set; }
    }
}
