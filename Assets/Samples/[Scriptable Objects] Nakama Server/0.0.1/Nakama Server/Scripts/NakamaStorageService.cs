using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ExtensionMethods;
using Nakama;
using ProjectCore.Events;
using ProjectCore.Integrations.Internal;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    [CreateAssetMenu(fileName = "NakamaStorageService", menuName = "ProjectCore/Integrations/NakamaServer/NakamaStorageService")]
    [InlineEditor]
    public class NakamaStorageService : StorageService
    {
        private const string COLLECTION_NAME = "Save";
        private const string KEY_NAME = "UserProgress";

        [SerializeField] private Queue SaveQueue;
        [SerializeField] private GameEvent QueueChangedEvent;
        [SerializeField] private GameEvent ProgressSaveCompleteEvent;
        
        public override void Initialize(IClient client, ISession session, CustomLogger logger)
        {
            _Logger = logger;
            _Provider = new NakamaStorageProvider(client, session);
            _Serializer = new NakamaSerializationStrategy();
            QueueChangedEvent.Handler += OnQueueChangedEvent;
        }

        private CancellationTokenSource _cts;
        private bool _isProcessing;

        private async void OnQueueChangedEvent()
        {
            if (_isProcessing) return;
            _isProcessing = true;
    
            try
            {
                var timeoutToken = _cts.RefreshToken();
        
                while (SaveQueue.TryDequeue(out Task task))
                {
                    try
                    {
                        // Execute with timeout protection
                        await task.WaitAsync(TimeSpan.FromSeconds(3), timeoutToken);
                
                        if (task.Status != TaskStatus.RanToCompletion)
                        {
                            _Logger.LogError($"[Save] Task failed with status: {task.Status}");
                        }
                    }
                    catch (TimeoutException)
                    {
                        _Logger.LogCritical("[Save] Operation timed out");
                        _cts.Cancel();
                        break;
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogCritical(ex.Message);
                    }
                    finally
                    {
                        task.Dispose();
                    }
                }
            }
            finally
            {
                _isProcessing = false;
                ProgressSaveCompleteEvent.Invoke();
            }
        }

        public override void SaveUserProgress()
        {
            var data = DBManager.GetJsonData();
            SaveQueue.Enqueue(SaveData(COLLECTION_NAME, KEY_NAME, data));
        }

        public override async Task<Dictionary<string, object>> LoadUserProgress()
        {
            try
            {
                return await LoadData(COLLECTION_NAME, KEY_NAME);
            }
            catch
            {
                _Logger.LogCritical("[Nakama] Failed to load user data");
                return new Dictionary<string, object>();
            }
        }

        public override async Task DeleteUserProgress()
        {
            try
            {
                await DeleteData(COLLECTION_NAME, KEY_NAME);
            }
            catch
            {
                _Logger.LogCritical("[Nakama] Failed to load user data");
            }
        }
    }
}