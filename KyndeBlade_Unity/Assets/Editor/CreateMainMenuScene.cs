using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace KyndeBlade.Editor
{
    /// <summary>Creates the MainMenu scene and adds it to build settings as index 0.</summary>
    public static class CreateMainMenuScene
    {
        [MenuItem("KyndeBlade/Create Main Menu Scene")]
        public static void Create()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(
                ManuscriptUITheme.ParchmentLight.r,
                ManuscriptUITheme.ParchmentLight.g,
                ManuscriptUITheme.ParchmentLight.b);
            camGo.AddComponent<AudioListener>();

            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            var menuGo = new GameObject("MainMenu");
            menuGo.AddComponent<MainMenuManager>();

            string scenePath = "Assets/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(scene, scenePath);

            AddSceneToBuildSettings(scenePath);
            Debug.Log("KyndeBlade: MainMenu scene created at " + scenePath);
        }

        static void AddSceneToBuildSettings(string menuScenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
                EditorBuildSettings.scenes);

            scenes.RemoveAll(s => s.path == menuScenePath);

            var menuScene = new EditorBuildSettingsScene(menuScenePath, true);
            scenes.Insert(0, menuScene);

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
