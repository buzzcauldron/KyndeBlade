using UnityEngine;
using UnityEngine.SceneManagement;

namespace KyndeBlade
{
    /// <summary>Handles scene load failures gracefully.
    /// Catches async load errors and falls back to Main scene.</summary>
    public class SceneFailureHandler : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.logMessageReceived += OnLogMessage;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.logMessageReceived -= OnLogMessage;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (string.IsNullOrEmpty(scene.name))
            {
                Debug.LogWarning("[SceneFailureHandler] Empty scene loaded. Returning to Main.");
                TryRecoverToMain();
            }
        }

        void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;
            if (condition.Contains("Scene") && condition.Contains("not found"))
            {
                Debug.LogWarning($"[SceneFailureHandler] Scene load failure detected: {condition}");
                TryRecoverToMain();
            }
        }

        void TryRecoverToMain()
        {
            if (SceneManager.GetActiveScene().name == "Main") return;
            try
            {
                SceneManager.LoadScene("Main");
            }
            catch
            {
                Debug.LogError("[SceneFailureHandler] Cannot load Main scene.");
            }
        }
    }
}
