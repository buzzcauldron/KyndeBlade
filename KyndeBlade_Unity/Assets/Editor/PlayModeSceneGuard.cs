#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// Stability guard: require entering Play Mode from the main bootstrap scene.
    /// Inspired by production samples that enforce a known-good startup path.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayModeSceneGuard
    {
        const string RequireBootstrapKey = "KyndeBlade.RequireBootstrapSceneOnPlay";
        const string BootstrapScenePath = "Assets/Scenes/Main.unity";

        static bool RequireBootstrapSceneOnPlay
        {
            get => EditorPrefs.GetBool(RequireBootstrapKey, true);
            set => EditorPrefs.SetBool(RequireBootstrapKey, value);
        }

        static PlayModeSceneGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Menu.SetChecked("KyndeBlade/Stability/Require Main Scene On Play", RequireBootstrapSceneOnPlay);
        }

        [MenuItem("KyndeBlade/Stability/Require Main Scene On Play")]
        static void ToggleRequireBootstrapSceneOnPlay()
        {
            RequireBootstrapSceneOnPlay = !RequireBootstrapSceneOnPlay;
            Menu.SetChecked("KyndeBlade/Stability/Require Main Scene On Play", RequireBootstrapSceneOnPlay);
            Debug.Log("[KyndeBlade] Require Main Scene On Play: " + (RequireBootstrapSceneOnPlay ? "ON" : "OFF"));
        }

        [MenuItem("KyndeBlade/Stability/Require Main Scene On Play", true)]
        static bool ToggleRequireBootstrapSceneOnPlayValidate()
        {
            Menu.SetChecked("KyndeBlade/Stability/Require Main Scene On Play", RequireBootstrapSceneOnPlay);
            return true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode || !RequireBootstrapSceneOnPlay)
                return;

            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.path == BootstrapScenePath)
                return;

            EditorApplication.isPlaying = false;
            Debug.LogError(
                "[KyndeBlade] Play Mode blocked for stability: start from '" + BootstrapScenePath + "'. " +
                "Current scene: '" + activeScene.path + "'. Disable the guard via KyndeBlade/Stability if needed.");
        }
    }
}
#endif
