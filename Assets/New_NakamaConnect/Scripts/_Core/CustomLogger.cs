using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtilities.Tools
{
   [CreateAssetMenu(menuName = "Tools/CustomLogger", fileName = "CustomLogger")]
    public class CustomLogger : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private LogLevel CurrentLogLevel = LogLevel.Info;
        [SerializeField] private bool LogToUnityConsole = true;

        [HideInInspector] public List<string> LogHistory = new List<string>();
        private const int MAX_HISTORY_ENTRIES = 100;

        public void LogDebug(string message, Object context = null)
        {
            LogMessage(message, LogLevel.Debug, context);
        }

        public void Log(string message, UnityEngine.Object context = null)
        {
            LogMessage(message, LogLevel.Info, context);
        }

        public void LogWarning(string message, Object context = null)
        {
            LogMessage(message, LogLevel.Warning, context);
        }

        public void LogError(string message, Object context = null)
        {
            LogMessage(message, LogLevel.Error, context);
        }

        public void LogCritical(string message, Object context = null)
        {
            LogMessage(message, LogLevel.Critical, context);
        }

        private void LogMessage(string message, LogLevel level, Object context = null)
        {
            if (level < CurrentLogLevel) return;

            string logEntry = FormatLog(message, level);
            AddToHistory(logEntry);
            if (LogToUnityConsole) 
                SendToUnityConsole(message, level, context);
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

        private void SendToUnityConsole(string message, LogLevel level, Object context)
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
            CurrentLogLevel = newLevel;
            Log($"Log level changed to {newLevel}");
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