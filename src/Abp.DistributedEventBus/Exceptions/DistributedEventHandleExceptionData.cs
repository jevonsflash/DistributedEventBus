using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus.Exceptions;

namespace Abp.DistributedEventBus.Exceptions
{
    [Serializable]
    public class DistributedEventHandleExceptionData : AbpHandledExceptionData
    {
        public DistributedEventArgs EventArgs { get; set; }

        public DistributedEventHandleExceptionData(Exception exception, DistributedEventArgs eventArgs) : base(exception)
        {
            EventArgs = eventArgs;
        }
    }
}
