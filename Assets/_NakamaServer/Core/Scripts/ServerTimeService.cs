using System;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ExtensionMethods;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    [CreateAssetMenu(fileName = "ServerTimeService", menuName = "ProjectCore/Integrations/NakamaServer/ServerTimeService")]
    public class ServerTimeService : ScriptableObject
    {
        // Reference to Nakama client and session (set by Server after auth)
        private IClient _client;
        private ISession _session;
        private CustomLogger _logger;

        // Initialize with dependencies from Server
        public void Initialize(IClient client, ISession session, CustomLogger logger)
        {
            _client = client;
            _session = session;
            _logger = logger;
        }

        // Fetch server time via Nakama's built-in RPC
        [Button]
        public async Task<int> GetServerTimeAsync()
        {
            if (_client == null || _session == null)
            {
                _logger.LogError("ServerTimeService not initialized!");
                return DateTime.UtcNow.ToEpoch();
            }

            try
            {
                var response = await _client.RpcAsync(_session, "get_server_time");
                return int.Parse(response.Payload); // Unix timestamp in seconds
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch server time: {ex.Message}");
                return DateTime.UtcNow.ToEpoch();
            }
        }
    }
}