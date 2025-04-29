using System;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

using CustomUtilities.Tools;


namespace ProjectCore.CloudService.Nakama.Internal
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "NakamaCloudSyncService", menuName = "ProjectCore/SocialFeature/Cloud/NakamaCloudSyncService")]
    public class NakamaCloudSyncService : CloudSyncService
    {
        [SerializeField] private DBBool IsEmailLoggedIn;

        [SerializeField] private DBString Email;
        [SerializeField] private DBString Password;
     
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

        private bool OnSignIn(bool success, string email, string password)
        {
            IsEmailLoggedIn.SetValue(success);
            if (!success) return false;
            Email.SetValue(email);
            Password.SetValue(password);
            return true;
        }
        
        #region Helper Functions

        private bool UserHasData(IApiStorageObjects obj)
        {
            if (obj == null) return false;
            var data = obj.Objects.ToList();
            return data.Count != 0;
        }

        #endregion
    }

    
}