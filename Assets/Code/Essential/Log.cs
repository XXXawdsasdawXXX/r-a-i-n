using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Essential
{
    public static class Log
    {
        public static bool PROFILER_IS_ACTIVE = true;
        
        private static readonly Color SERVER_COLOR = new(0.3f, 0.4f, 0.6f);
        private static readonly Color CLIENT_COLOR = new(0.4f, 0.3f, 0.4f);
        
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        
        public static void Info(object context, string message)
        {
            if (context != null)
            {
                Debug.Log($"{context.GetType().Name}: {message}");
                return;
            }
            
            Debug.Log(message);
        }
        
        public static void Info(object context, string message, Color color)
        {
            if (context != null)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{context.GetType().Name}: {message}</color>");
                return;
            }
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }

        
        public static void Info(string message, object context = null)
        {
            if (context != null)
            {
                Debug.Log($"{context.GetType().Name}: {message}");
                return;
            }
            
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Info(string message, Color color, object context = null)
        {
            if (context != null)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{context.GetType().Name}: {message}</color>");
                return;
            }
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Info(object message, Color color, object context = null)
        {
            if (context != null)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{context.GetType().Name}: {message}</color>");
                return;
            }
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void ServerInfo(string message, object context = null)
        {
            if (context != null)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(SERVER_COLOR)}>|SERVER| {context.GetType().Name}: {message}</color>");
                return;
            }
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(SERVER_COLOR)}>|SERVER|" + message + "</color>");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void ClientInfo(string message, object context = null)
        {
            if (context != null)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(CLIENT_COLOR)}>|CLIENT| {context.GetType().Name}: {message}</color>");
                return;
            }
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(CLIENT_COLOR)}>|CLIENT|" + message + "</color>");
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Error(string message, object context = null)
        {
            if (context != null)
            {
                Debug.LogError($"{context}: {message}");
                return;
            }
            
            Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Warning(string message, object context = null)
        {
            if (context != null)
            {
                Debug.LogWarning($"{context}: {message}");
                return;
            }
            
            Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Exception(Exception e)
        {
            Debug.LogException(e);
        }
    }
}