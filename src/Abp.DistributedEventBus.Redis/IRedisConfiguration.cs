using System;

namespace Abp.DistributedEventBus.Redis
{
    public interface IRedisConfiguration
    {
        IRedisConfiguration Configure(Action<IRedisSetting> configureAction);

        IRedisConfiguration Configure(IRedisSetting setting);
    }
}
