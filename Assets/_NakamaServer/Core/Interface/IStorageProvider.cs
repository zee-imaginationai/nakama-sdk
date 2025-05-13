using System.Threading.Tasks;

namespace ProjectCore.Integrations.Internal
{
    // (Abstract interface for any backend)
    public interface IStorageProvider
    {
        Task SaveDataAsync(string collection, string key, string value);
        Task<string> LoadDataAsync(string collection, string key);
        Task DeleteDataAsync(string collection, string key);
    }
}