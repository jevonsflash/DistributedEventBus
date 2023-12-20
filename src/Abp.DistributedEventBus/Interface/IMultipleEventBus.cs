using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Events.Bus;

namespace Abp.DistributedEventBus
{
    public interface IMultipleEventBus : IDistributedEventBus, IEventBus
    {
    
    }
}