namespace ProjectCore.Integrations.Internal
{
    public interface ISerializationStrategy
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string serializedData) where T : class;
    }
}