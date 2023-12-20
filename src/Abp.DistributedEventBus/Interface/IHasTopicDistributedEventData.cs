using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus
{
    public interface IHasTopicDistributedEventData
    {
        string Topic { get; set; }
    }
}
