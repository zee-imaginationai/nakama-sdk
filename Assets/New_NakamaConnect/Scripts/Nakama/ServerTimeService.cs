using System;
using System.Threading.Tasks;
using CustomUtilities.Tools;
using ExtensionMethods;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.NakamaServer.Internal
{
    [CreateAssetMenu(fileName = "ServerTimeService", menuName = "ProjectCore/CloudService/NakamaServerTimeService")]
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
    
    /*
     * Fetch Server Time Using Nakama RPC
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nakama;
    using UnityEngine;

    public abstract class ServerTimeService : ScriptableObject
    {
        [SerializeField] protected CustomLogger Logger;
        
        protected IClient _client;
        protected ISession _session;
        protected Server _server; // Reference to parent Server for session refresh

        // For cancellation/timeout
        private CancellationTokenSource _cts;
        private const int RpcTimeoutSeconds = 10;
        private const int MaxRetries = 2;

        public virtual void Initialize(IClient client, ISession session, Server server)
        {
            _client = client;
            _session = session;
            _server = server;
        }

        public async Task<long> GetServerTimeAsync()
        {
            if (!IsServiceReady())
            {
                Logger?.LogError("ServerTimeService not initialized.");
                throw new InvalidOperationException("Service not initialized.");
            }

            _cts = new CancellationTokenSource();
            _cts.CancelAfter(TimeSpan.FromSeconds(RpcTimeoutSeconds));

            int retryCount = 0;
            while (retryCount < MaxRetries)
            {
                try
                {
                    var response = await _client.RpcAsync(
                        _session, 
                        "get_server_time", 
                        cancellationToken: _cts.Token
                    );

                    if (long.TryParse(response.Payload, out long serverTime))
                    {
                        return serverTime;
                    }
                    else
                    {
                        Logger?.LogError($"Invalid server time payload: {response.Payload}");
                        throw new FormatException("Invalid server time format.");
                    }
                }
                catch (ApiResponseException ex) when (ex.StatusCode == 401) // Unauthorized
                {
                    Logger?.LogWarning("Session expired. Attempting refresh...");
                    bool refreshed = await _server.RefreshSession();
                    if (!refreshed) throw new Exception("Session refresh failed.");
                    retryCount++;
                }
                catch (TaskCanceledException)
                {
                    Logger?.LogError("Server time request timed out.");
                    throw;
                }
                catch (Exception ex)
                {
                    Logger?.LogError($"RPC failed: {ex.Message}");
                    retryCount++;
                    if (retryCount >= MaxRetries) throw;
                    await Task.Delay(1000 * retryCount); // Exponential backoff
                }
            }

            throw new Exception("Failed to fetch server time after retries.");
        }

        private bool IsServiceReady()
        {
            return _client != null && 
                   _session != null && 
                   !_session.IsExpired;
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
     */
}