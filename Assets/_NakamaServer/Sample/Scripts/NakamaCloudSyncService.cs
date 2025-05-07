using System;
using System.Threading.Tasks;
using ProjectCore.Integrations.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaCloudSyncService", menuName = "ProjectCore/Intergrations/NakamaServer/NakamaCloudSyncService")]
    public class NakamaCloudSyncService : CloudSyncService
    {
        [SerializeField] private DBString ConflictString;
        
        [SerializeField] private MainMenuState MainMenuState;
        
        [SerializeField] private bool ShowConflictResolutionPanel;

        protected override async Task SyncComplete()
        {
            MainMenuState.UpdateButton();
            await base.SyncComplete();
        }

        protected override bool EvaluateCondition()
        {
            var baseEvaluation = base.EvaluateCondition();
            return baseEvaluation || EvaluateConflict();
        }

        protected override async Task<StorageType> GetConflictStorageType()
        {
            if (!ShowConflictResolutionPanel) 
                return await base.GetConflictStorageType();
            
            try
            {
                Logger.Log("Resolving conflict storage type");
                return await MainMenuState.ResolveConflict();
            }
            catch
            {
                Logger.LogError("Failed to Open Conflict Resolution Panel");
            }
            
            return await base.GetConflictStorageType();
        }

        private bool EvaluateConflict()
        {
            var conflictKey = ConflictString.GetKey();
            var conflictValueFound = _CloudData.TryGetValue(conflictKey, out var conflictValue);
            var conflictCondition = conflictValueFound && !string.Equals((string)conflictValue,
                ConflictString.GetValue(), StringComparison.OrdinalIgnoreCase);
            return conflictCondition;
        }
    }

    
}