using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ProjectCore.Integrations.NakamaServer.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.Internal
{
    public class CloudSyncService : ScriptableObject
    {
        [ShowInInspector] private const string FORCE_SYNC_KEY = "force_sync";
        
        [SerializeField] private NakamaStorageService UserProgressService;
        
        [SerializeField] private DBEpochTime LastSyncTime;
        [SerializeField] private DBBool IsForceSync;
        
        [SerializeField] private ServerTimeService ServerTimeService;
        
        [SerializeField] protected CustomLogger Logger;
        
        protected Dictionary<string, object> _CloudData;
        
        public async Task SyncData()
        {
            // Get data from server
            
            Logger.Log("Syncing data...");
            
            _CloudData = await UserProgressService.LoadUserProgress();

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
            return EvaluateForceSync() || !EvaluateLastSyncTimeExpired();
        }

        protected virtual async Task SyncComplete()
        {
            var serverTime = await ServerTimeService.GetServerTimeAsync();
            LastSyncTime.SetValue(serverTime);
            await UserProgressService.SaveUserProgress();
            ResetForceSync();
            Logger.Log("Syncing Complete...");
        }

        protected virtual Task<StorageType> GetConflictStorageType()
        {
            return Task.FromResult(StorageType.Cloud);
        }
        
        private bool EvaluateLastSyncTimeExpired()
        {
            var lastSyncKey = LastSyncTime.GetKey();
            var syncTimeFound = _CloudData.TryGetValue(lastSyncKey, out var lastCloudSyncTime);
            return syncTimeFound && (int)lastCloudSyncTime <= LastSyncTime.GetValue();
        }
        
        private bool EvaluateForceSync()
        {
            var forceSyncFound = _CloudData.TryGetValue(FORCE_SYNC_KEY, out var value);
            var isForceSync = forceSyncFound && (int)value > 0;

            IsForceSync.SetValue(isForceSync);

            return isForceSync;
        }
        
        private void ResetForceSync()
        {
            IsForceSync.SetValue(false);
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
