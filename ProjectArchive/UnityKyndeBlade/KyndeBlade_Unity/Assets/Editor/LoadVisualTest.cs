#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>Load or create a minimal scene to test manuscript overlay, 16-bit pipeline, and shaders. Menu: KyndeBlade > Load Visual Test.</summary>
    public static class LoadVisualTest
    {
        const string VisualTestScenePath = "Assets/Scenes/VisualTest.unity";

        [MenuItem("KyndeBlade/Load Visual Test")]
        public static void LoadVisualTestScene()
        {
            if (File.Exists(VisualTestScenePath))
            {
                EditorSceneManager.OpenScene(VisualTestScenePath);
                Debug.Log("Opened Visual Test scene. Press Play to see manuscript overlay. Use KyndeBlade > Add Character to Visual Test if the scene has no character.");
                return;
            }

            CreateVisualTestScene();
        }

        [MenuItem("KyndeBlade/Add Character to Visual Test")]
        public static void AddCharacterToVisualTest()
        {
            if (EditorSceneManager.GetActiveScene().path != VisualTestScenePath)
            {
                Debug.LogWarning("Open the Visual Test scene first (KyndeBlade > Load Visual Test), then run Add Character to Visual Test.");
                return;
            }
            if (GameObject.Find("VisualTestCharacter") != null)
            {
                Debug.Log("Visual Test already has a character (VisualTestCharacter).");
                return;
            }
            var charMat = new Material(Shader.Find("KyndeBlade/Character Manuscript (Toon + Satin + Ink)"));
            if (charMat.shader == null) charMat.shader = Shader.Find("Unlit/Color");
            var characterRoot = new GameObject("VisualTestCharacter");
            characterRoot.transform.position = new Vector3(0f, 0f, 0f);
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(characterRoot.transform);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(0.6f, 1f, 0.6f);
            ApplyCharacterMaterial(body.GetComponent<Renderer>(), charMat);
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(characterRoot.transform);
            head.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            head.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            ApplyCharacterMaterial(head.GetComponent<Renderer>(), charMat);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Added VisualTestCharacter (body + head) with Character Manuscript shader. Save the scene to keep it.");
        }

        static void CreateVisualTestScene()
        {
            if (!Directory.Exists("Assets/Scenes"))
                Directory.CreateDirectory("Assets/Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Main Camera with manuscript overlay (and optional 16-bit pipeline)
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.15f, 0.12f, 0.18f);
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<ManuscriptOverlayEffect>();
            var pipeline = camGo.AddComponent<SixteenBitPipeline>();
            pipeline.ApplyManuscriptOverlay = true;
            pipeline.enabled = false; // 16-bit off by default; enable on camera to test

            // Light
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Something to look at: quad with a simple material (character manuscript or default)
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "VisualTestQuad";
            quad.transform.position = new Vector3(0f, 0f, 0f);
            quad.transform.rotation = Quaternion.identity;
            var renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("KyndeBlade/Character Manuscript (Toon + Satin + Ink)"));
                if (mat.shader == null) mat.shader = Shader.Find("Unlit/Color");
                mat.color = new Color(0.4f, 0.25f, 0.5f);
                renderer.sharedMaterial = mat;
            }

            EditorSceneManager.SaveScene(scene, VisualTestScenePath);

            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (list.FindIndex(s => s.path == VisualTestScenePath) < 0)
                list.Add(new EditorBuildSettingsScene(VisualTestScenePath, true));
            EditorBuildSettings.scenes = list.ToArray();

            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(VisualTestScenePath);
            Debug.Log("Created and opened Visual Test scene. Press Play to see manuscript overlay and character (toon + satin + ink). Enable SixteenBitPipeline on Main Camera to test 16-bit.");
        }

        static void ApplyCharacterMaterial(Renderer r, Material mat)
        {
            if (r != null && mat != null)
                r.sharedMaterial = mat;
        }
    }
}
#endif
