using System.Threading.Tasks;

namespace ProjectCore.CloudService.Internal
{
    public interface IStorageProvider
    {
        Task SaveDataAsync(string collection, string key, string value);
        Task<string> LoadDataAsync(string collection, string key);
        Task DeleteDataAsync(string collection, string key);
    }
}