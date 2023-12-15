using Abp.DistributedEventBus;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus;

namespace Abp.DistributedEventBus
{
    [Serializable]
    public sealed class DistributedEventData : EventData, IDistributedEventData
    {
        public Dictionary<string, object> Data { get; set; }

        public string Type { get; set; }

        private DistributedEventData()
        {
            Data = new Dictionary<string, object>();
        }

        public DistributedEventData(string type) : this()
        {
            Type = type;
        }

        public DistributedEventData(string type, Dictionary<string, object> data) : this()
        {
            Type = type;
            Data = data;
        }
    }
}
