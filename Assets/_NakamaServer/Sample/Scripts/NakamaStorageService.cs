using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUtilities.Tools;
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

        private void OnQueueChangedEvent()
        {
            while (SaveQueue.Count > 0)
            {
                if (SaveQueue.Count > 5)
                {
                    SaveQueue.DeQueue();
                    continue;
                }
                var task = SaveQueue.DeQueue();
                task.Start();
                task.Wait(3);
                if (!task.IsCompletedSuccessfully)
                {
                    _Logger.LogCritical("[Nakama] Failed to save user data");
                }
                task.Dispose();
            }
            ProgressSaveCompleteEvent.Invoke();
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