using System;
using System.Threading.Tasks;
using ProjectCore.Integrations.Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaCloudSyncService", menuName = "ProjectCore/SocialFeature/Cloud/NakamaCloudSyncService")]
    public class NakamaCloudSyncService : CloudSyncService
    {
        [SerializeField] private CloudDBString ConflictString;
        
        [SerializeField] private MainMenuState MainMenuState;
        
        [SerializeField] private bool ShowConflictResolutionPanel;

        protected override async Task SyncComplete()
        {
            MainMenuState.UpdateButton();
            await base.SyncComplete();
        }

        protected override bool EvaluateCondition()
        {
            var timeComparison = base.EvaluateCondition();
            var conflictKey = ConflictString.GetKey();
            var conflictValueFound = _CloudData.TryGetValue(conflictKey, out var conflictValue);
            var conflictCondition = conflictValueFound && !string.Equals((string)conflictValue,
                ConflictString.GetValue(), StringComparison.OrdinalIgnoreCase);
            return timeComparison || conflictCondition;
        }

        protected override async Task<StorageType> GetConflictStorageType()
        {
            if (!ShowConflictResolutionPanel) return await base.GetConflictStorageType();
            
            try
            {
                return await MainMenuState.ResolveConflict();
            }
            catch
            {
                Logger.LogError("Failed to Open Conflict Resolution Panel");
            }
            
            return await base.GetConflictStorageType();
        }
    }

    
}