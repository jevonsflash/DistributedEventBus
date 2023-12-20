using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus;

namespace Abp.DistributedEventBus
{
    public class DistributedEventArgs : EventArgs, IEventData
    {
        public IDistributedEventData EventData { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool Suspended { get; set; }

        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }

        public DistributedEventArgs(IDistributedEventData eventData, string topic,string message)
        {
            EventData = eventData;
            Message = message;
            Topic = topic;
            EventTime = DateTime.Now;
        }
    }
}
