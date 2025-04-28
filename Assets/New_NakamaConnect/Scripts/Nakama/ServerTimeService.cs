using System;
using System.Threading.Tasks;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "ServerTimeService", menuName = "ProjectCore/CloudService/NakamaServerTimeService")]
    public class ServerTimeService : ScriptableObject
    {
        // Reference to Nakama client and session (set by Server after auth)
        protected IClient _client;
        protected ISession _session;

        // Initialize with dependencies from Server
        public void Initialize(IClient client, ISession session)
        {
            _client = client;
            _session = session;
        }

        // Fetch server time via Nakama's built-in RPC
        [Button]
        public async Task<int> GetServerTimeAsync()
        {
            if (_client == null || _session == null)
            {
                Debug.LogError("ServerTimeService not initialized!");
                return -1;
            }

            try
            {
                // var response = await _client.RpcAsync(_session, "get_server_time");
                string url = $"{_client.Scheme}://{_client.Host}:{_client.Port}/v2/healthcheck";
                using var request = UnityWebRequest.Get(url);
                request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"HTTP error: {request.error}");
                }

                // Parse response (e.g., {"status":"healthy","server_time":1717000000000})
                var json = JsonUtility.FromJson<HealthCheckResponse>(request.downloadHandler.text);
                return json.server_time;
                // return int.Parse(response.Payload); // Unix timestamp in milliseconds
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch server time: {ex.Message}");
                return -1;
            }
        }
    }
    
    [Serializable]
    internal class HealthCheckResponse
    {
        public string status;
        public int server_time;
    }
}