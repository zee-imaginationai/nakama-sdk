using System.Collections.Generic;
using UnityEngine;

namespace ProjectCore.CloudService.Internal
{
   [CreateAssetMenu(menuName = "Tools/Console Logger")]
    public class CustomLogger : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private LogLevel _currentLogLevel = LogLevel.Info;
        [SerializeField] private bool _logToUnityConsole = true;

        [HideInInspector] public List<string> LogHistory = new List<string>();
        private const int MAX_HISTORY_ENTRIES = 100;

        public void Log(string message, LogLevel level = LogLevel.Info, UnityEngine.Object context = null)
        {
            if (level < _currentLogLevel) return;

            string formatted = FormatLog(message, level);
            AddToHistory(formatted);

            if (_logToUnityConsole)
            {
                SendToUnityConsole(message, level, context);
            }
        }

        private string FormatLog(string message, LogLevel level)
        {
            return $"[{System.DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
        }

        private void AddToHistory(string logEntry)
        {
            LogHistory.Insert(0, logEntry);
            if (LogHistory.Count > MAX_HISTORY_ENTRIES)
            {
                LogHistory.RemoveAt(LogHistory.Count - 1);
            }
        }

        private void SendToUnityConsole(string message, LogLevel level, UnityEngine.Object context)
        {
    #if UNITY_EDITOR
            string richMessage = $"<b><color={GetLevelColor(level)}>[{level}]</color></b> {message}";
    #else
            string richMessage = $"[{level}] {message}";
    #endif

            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(richMessage, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(richMessage, context);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    Debug.LogError(richMessage, context);
                    break;
            }
        }

        private string GetLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => "cyan",
                LogLevel.Info => "white",
                LogLevel.Warning => "yellow",
                LogLevel.Error => "red",
                LogLevel.Critical => "magenta",
                _ => "white"
            };
        }

        public void SetLogLevel(LogLevel newLevel)
        {
            _currentLogLevel = newLevel;
            Log($"Log level changed to {newLevel}", LogLevel.Info);
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
}