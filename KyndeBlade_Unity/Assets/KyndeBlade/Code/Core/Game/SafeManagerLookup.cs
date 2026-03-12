using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Provides null-safe lookups for managers that may not exist yet.
    /// Uses GameRuntime first, then FindFirstObjectByType as fallback.</summary>
    public static class SafeManagerLookup
    {
        public static T Get<T>() where T : Component
        {
            if (typeof(T) == typeof(TurnManager))
                return (GameRuntime.TurnManager as T) ?? Object.FindFirstObjectByType<T>();
            if (typeof(T) == typeof(KyndeBladeGameManager))
                return (GameRuntime.GameManager as T) ?? Object.FindFirstObjectByType<T>();
            if (typeof(T) == typeof(SaveManager))
                return (GameRuntime.SaveManager as T) ?? Object.FindFirstObjectByType<T>();
            if (typeof(T) == typeof(WorldMapManager))
                return (GameRuntime.WorldMapManager as T) ?? Object.FindFirstObjectByType<T>();
            if (typeof(T) == typeof(NarrativeManager))
                return (GameRuntime.NarrativeManager as T) ?? Object.FindFirstObjectByType<T>();
            if (typeof(T) == typeof(MusicManager))
                return (GameRuntime.MusicManager as T) ?? Object.FindFirstObjectByType<T>();
            return Object.FindFirstObjectByType<T>();
        }

        /// <summary>Load a ScriptableObject from Resources, logging a warning if missing.</summary>
        public static T LoadResource<T>(string path) where T : Object
        {
            var asset = Resources.Load<T>(path);
            if (asset == null)
                Debug.LogWarning($"[KyndeBlade] Missing resource at '{path}'. Using defaults.");
            return asset;
        }

        /// <summary>Load a ScriptableObject or return a fallback.</summary>
        public static T LoadResourceOrDefault<T>(string path, T fallback) where T : Object
        {
            var asset = Resources.Load<T>(path);
            return asset != null ? asset : fallback;
        }
    }
}
