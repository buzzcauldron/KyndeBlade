using System;
using System.Reflection;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Provides null-safe lookups for managers that may not exist yet.
    /// Uses GameRuntime first, then FindFirstObjectByType as fallback.</summary>
    public static class SafeManagerLookup
    {
        public static T Get<T>() where T : Component
        {
            var runtimeCandidate = GetRuntimeManagerByTypeName(typeof(T).Name) as T;
            if (runtimeCandidate != null)
                return runtimeCandidate;
            return UnityEngine.Object.FindFirstObjectByType<T>();
        }

        static object GetRuntimeManagerByTypeName(string typeName)
        {
            var propertyName = typeName switch
            {
                "TurnManager" => "TurnManager",
                "KyndeBladeGameManager" => "GameManager",
                "SaveManager" => "SaveManager",
                "WorldMapManager" => "WorldMapManager",
                "NarrativeManager" => "NarrativeManager",
                "MusicManager" => "MusicManager",
                _ => null
            };

            if (string.IsNullOrEmpty(propertyName))
                return null;

            var runtimeType = FindType("KyndeBlade.GameRuntime");
            if (runtimeType == null)
                return null;

            var prop = runtimeType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            return prop?.GetValue(null);
        }

        static Type FindType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetType(fullName, false);
                if (type != null)
                    return type;
            }
            return null;
        }

        /// <summary>Load a ScriptableObject from Resources, logging a warning if missing.</summary>
        public static T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            var asset = Resources.Load<T>(path);
            if (asset == null)
                Debug.LogWarning($"[KyndeBlade] Missing resource at '{path}'. Using defaults.");
            return asset;
        }

        /// <summary>Load a ScriptableObject or return a fallback.</summary>
        public static T LoadResourceOrDefault<T>(string path, T fallback) where T : UnityEngine.Object
        {
            var asset = Resources.Load<T>(path);
            return asset != null ? asset : fallback;
        }
    }
}
