#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>Creates a minimal gameplay test scene with GameplayTestBootstrap + KyndeBladeGameManager so combat and map can run without manual setup.</summary>
    public static class CreateGameplayTestScene
    {
        const string ScenePath = "Assets/Scenes/GameplayTest.unity";

        [MenuItem("KyndeBlade/Create Gameplay Test Scene")]
        public static void Create()
        {
            if (!Directory.Exists("Assets/Scenes"))
                Directory.CreateDirectory("Assets/Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var cam = Object.FindFirstObjectByType<Camera>();
            if (cam != null)
            {
                cam.gameObject.tag = "MainCamera";
                if (cam.GetComponent<AudioListener>() == null)
                    cam.gameObject.AddComponent<AudioListener>();
            }
            else
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }

            var bootstrapGo = new GameObject("GameplayTestBootstrap");
            bootstrapGo.AddComponent<GameplayTestBootstrap>();

            var managerGo = new GameObject("KyndeBladeGameManager");
            managerGo.AddComponent<KyndeBladeGameManager>();

            EditorSceneManager.SaveScene(scene, ScenePath);

            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (list.FindIndex(s => s.path == ScenePath) < 0)
                list.Add(new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = list.ToArray();

            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(ScenePath);
            Debug.Log("Gameplay Test scene created. Press Play for a short combat test (auto-spawn or map depending on GameManager settings).");
        }
    }
}
#endif
