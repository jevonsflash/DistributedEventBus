using System;
using Abp.Events.Bus.Exceptions;

namespace Abp.DistributedEventBus.Exceptions
{
    [Serializable]
    public class DistributedEventMessageHandleExceptionData : AbpHandledExceptionData
    {
        public string Topic { get; set; }

        public string Message { get; set; }

        public DistributedEventMessageHandleExceptionData(Exception exception, string topic, string message) : base(exception)
        {
            Topic = topic;
            Message = message;
        }
    }
}
