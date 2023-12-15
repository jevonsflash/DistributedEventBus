namespace Abp.DistributedEventBus
{
    public interface IDistributedEventSerializer
    {
        T Deserialize<T>(string value);

        string Serialize(object value);
    }
}