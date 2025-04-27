using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
    //(For data format flexibility)
    public interface ISerializationStrategy
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string serializedData);
    }
    
    public class JsonSerializationStrategy : ISerializationStrategy
    {
        public string Serialize<T>(T data)
        {
            return JsonUtility.ToJson(data);
        }

        public T Deserialize<T>(string serializedData)
        {
            return JsonUtility.FromJson<T>(serializedData);
        }
    }
}