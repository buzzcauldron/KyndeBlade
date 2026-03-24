#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>Creates a minimal playable scene with KyndeBladeGameManager. Run via KyndeBlade > Create Main Scene.</summary>
    public static class CreateMainScene
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

        [MenuItem("KyndeBlade/Setup Project (Create Scene + Build)")]
        static void SetupProject()
        {
            EnsureMainCameraTag();
            if (!File.Exists(ScenePath))
                CreateSceneInternal();
            else
                AddSceneToBuild();
            ValidateBuildOrder();
            Debug.Log("KyndeBlade setup complete. Press Play to run.");
        }

        static void EnsureMainCameraTag()
        {
            var objs = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (objs == null || objs.Length == 0) return;
            var tagManager = new SerializedObject(objs[0]);
            var tagsProp = tagManager.FindProperty("tags");
            if (tagsProp == null) return;
            for (int i = 0; i < tagsProp.arraySize; i++)
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == "MainCamera") return;
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = "MainCamera";
            tagManager.ApplyModifiedProperties();
        }

        static void AddSceneToBuild()
        {
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = list.FindIndex(s => s.path == ScenePath);
            if (idx < 0) list.Add(new EditorBuildSettingsScene(ScenePath, true));
            else if (!list[idx].enabled) { list[idx] = new EditorBuildSettingsScene(ScenePath, true); }
            else return;
            EditorBuildSettings.scenes = list.ToArray();
        }

        static void ValidateBuildOrder()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int menuIdx = scenes.FindIndex(s => s.path == MainMenuScenePath);

            if (menuIdx < 0)
            {
                Debug.LogWarning("KyndeBlade: MainMenu scene is not in Build Settings. Run KyndeBlade > Create Main Menu Scene first.");
                return;
            }

            int mainIdx = scenes.FindIndex(s => s.path == ScenePath);
            if (menuIdx == 0 && mainIdx == 1)
                return;

            var menuScene = scenes[menuIdx];
            scenes.RemoveAt(menuIdx);

            mainIdx = scenes.FindIndex(s => s.path == ScenePath);
            bool hasMain = mainIdx >= 0;
            EditorBuildSettingsScene mainScene = default;
            if (hasMain)
            {
                mainScene = scenes[mainIdx];
                scenes.RemoveAt(mainIdx);
            }

            scenes.Insert(0, menuScene);
            if (hasMain)
                scenes.Insert(1, mainScene);

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        [MenuItem("KyndeBlade/Create Main Scene")]
        static void CreateScene()
        {
            EnsureMainCameraTag();
            CreateSceneInternal();
            ValidateBuildOrder();
            Debug.Log($"Created {ScenePath}. Press Play to run. Map UI and combat will appear at runtime.");
        }

        static void CreateSceneInternal()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var cam = UnityEngine.Object.FindFirstObjectByType<Camera>();
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

            var go = new GameObject("KyndeBladeGameManager");
            go.AddComponent<KyndeBladeGameManager>();

            if (!Directory.Exists("Assets/Scenes"))
                Directory.CreateDirectory("Assets/Scenes");

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuild();
        }
    }
}
#endif
