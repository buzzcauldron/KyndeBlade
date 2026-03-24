using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Placeholder bootstrap for the Visual Test scene. Ensures Main Camera with manuscript overlay, a directional light, and a simple test mesh so the scene runs without manual setup. Add to any GameObject in VisualTest.unity and press Play.</summary>
    [DefaultExecutionOrder(-2000)]
    public class VisualTestBootstrap : MonoBehaviour
    {
        [Header("Visual Test Options")]
        [Tooltip("Ensure a test quad is present for manuscript/toon shader preview.")]
        public bool EnsureTestQuad = true;
        [Tooltip("Use 16-bit pipeline on camera (SNES-style). If false, uses full-screen manuscript overlay only.")]
        public bool UseSixteenBitPipeline = false;

        void Awake()
        {
            EnsureMainCamera();
            EnsureDirectionalLight();
            if (EnsureTestQuad)
                EnsureTestContent();
        }

        void EnsureMainCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.transform.position = new Vector3(0f, 0f, -10f);
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.15f, 0.12f, 0.18f);
                camGo.AddComponent<AudioListener>();
            }

            if (cam.GetComponent<ManuscriptOverlayEffect>() == null)
                cam.gameObject.AddComponent<ManuscriptOverlayEffect>();

            var pipeline = cam.GetComponent<SixteenBitPipeline>();
            if (UseSixteenBitPipeline)
            {
                if (pipeline == null) pipeline = cam.gameObject.AddComponent<SixteenBitPipeline>();
                pipeline.enabled = true;
                pipeline.ApplyManuscriptOverlay = true;
                var overlay = cam.GetComponent<ManuscriptOverlayEffect>();
                if (overlay != null) overlay.enabled = false;
            }
            else
            {
                if (pipeline != null) pipeline.enabled = false;
            }
        }

        void EnsureDirectionalLight()
        {
            if (Object.FindFirstObjectByType<Light>() != null) return;
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        void EnsureTestContent()
        {
            if (GameObject.Find("VisualTestQuad") != null) return;
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "VisualTestQuad";
            quad.transform.position = Vector3.zero;
            var renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("KyndeBlade/Character Manuscript (Toon + Satin + Ink)");
                var mat = new Material(shader != null ? shader : Shader.Find("Unlit/Color"));
                mat.color = new Color(0.4f, 0.25f, 0.5f);
                renderer.sharedMaterial = mat;
            }
        }
    }
}
