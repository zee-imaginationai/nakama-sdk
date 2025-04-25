using System.Threading.Tasks;
using Nakama;

namespace ProjectCore.SocialFeature.Internal
{
    public interface IUserProgressRepository
    {
        Task SaveAsync(IApiWriteStorageObject[] objects);
        Task<IApiStorageObjects> LoadAsync(IApiReadStorageObjectId[] objectIds);
        Task DeleteAsync(StorageObjectId[] objectIds);
    }

}