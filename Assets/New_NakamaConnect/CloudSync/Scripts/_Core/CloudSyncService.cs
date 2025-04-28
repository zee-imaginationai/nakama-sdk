using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectCore.CloudService.Nakama.Internal;
using ProjectCore.Variables;
using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
    public class CloudSyncService : ScriptableObject
    {
        [SerializeField] private UserProgressService UserProgressService;
        [SerializeField] private DBEpochTime LastSyncTime;
        [SerializeField] private ServerTimeService ServerTimeService;

        protected Dictionary<string, object> _CloudData;
        
        public async Task SyncData()
        {
            // Get data from server
            
            _CloudData = await UserProgressService.GetUserData();

            var conflictData = await ResolveConflict();
            
            switch (conflictData.StorageType)
            {
                case StorageType.Cloud:
                    DBManager.LoadJsonData(conflictData.Data);
                    break;
                case StorageType.Local:
                case StorageType.None:
                default:
                    break;
            }
            await SyncComplete();
        }

        private async Task<ConflictData> ResolveConflict()
        {
            if (_CloudData == null)
                return new ConflictData()
                {
                    StorageType = StorageType.Local,
                    Data = null
                };
            
            if (EvaluateCondition())
            {
                return new ConflictData()
                {
                    StorageType = await GetConflictStorageType(),
                    Data = _CloudData
                };                
            }
            
            return new ConflictData()
            {
                StorageType = StorageType.Local,
                Data = null
            };
        }

        protected virtual bool EvaluateCondition()
        {
            var lastSyncKey = LastSyncTime.GetKey();
            var syncTimeFound = _CloudData.TryGetValue(lastSyncKey, out var lastCloudSyncTime);
            
            return syncTimeFound && (long)lastCloudSyncTime > LastSyncTime.GetValue();
        }

        protected virtual async Task SyncComplete()
        {
            var serverTime = await ServerTimeService.GetServerTimeAsync();
            LastSyncTime.SetValue(serverTime);
            await UserProgressService.SaveUserData();
        }

        protected virtual Task<StorageType> GetConflictStorageType()
        {
            return new Task<StorageType>(()=> StorageType.Cloud);
        }
    }

    public struct ConflictData
    {
        public StorageType StorageType;
        public Dictionary<string, object> Data;
    }
    
    public enum StorageType
    {
        None,
        Local,
        Cloud
    }
}
