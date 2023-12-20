using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DistributedEventBus.Kafka
{
    public interface IKafkaSetting
    {
        Dictionary<string, object> Properties { get; set; }
    }
}
