// Requires: Universal RP package (com.unity.render-pipelines.universal).
// Add to URP Renderer: Renderer Features -> Add -> Manuscript Overlay.
#if UNITY_2020_1_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KyndeBlade
{
    /// <summary>URP Scriptable Renderer Feature: full-screen manuscript overlay (parchment × scene, grain, sepia). Add to URP Renderer's Renderer Features.</summary>
    public class ManuscriptOverlayRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
            public Texture2D parchmentTexture;
            [Range(0f, 1f)] public float parchmentStrength = 0.45f;
            public Color sepiaTint = new Color(0.76f, 0.68f, 0.55f);
            [Range(0f, 1f)] public float sepiaStrength = 0.35f;
            [Range(0f, 0.15f)] public float grain = 0.06f;
        }

        public Settings settings = new Settings();
        ManuscriptOverlayPass _pass;

        public override void Create()
        {
            _pass = new ManuscriptOverlayPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.parchmentStrength <= 0f && settings.sepiaStrength <= 0f && settings.grain <= 0f)
                return;
            renderer.EnqueuePass(_pass);
        }
    }

    public class ManuscriptOverlayPass : ScriptableRenderPass
    {
        readonly ManuscriptOverlayRendererFeature.Settings _settings;
        Material _mat;
        static readonly int TempTargetId = Shader.PropertyToID("_ManuscriptOverlayTemp");

        public ManuscriptOverlayPass(ManuscriptOverlayRendererFeature.Settings settings)
        {
            _settings = settings;
            renderPassEvent = settings.passEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cam = renderingData.cameraData.camera;
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView)
                return;

            var shader = Shader.Find(ManuscriptOverlayParams.ShaderName);
            if (shader == null) return;
            if (_mat == null) _mat = new Material(shader);

            var renderer = renderingData.cameraData.renderer;
            if (renderer == null) return;

            ManuscriptOverlayParams.ApplyToMaterial(_mat, _settings.parchmentTexture, _settings.parchmentStrength, _settings.sepiaTint, _settings.sepiaStrength, _settings.grain);

            CommandBuffer cmd = CommandBufferPool.Get("Manuscript Overlay");
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            cmd.GetTemporaryRT(TempTargetId, desc);

            var colorTarget = renderer.cameraColorTargetHandle;
            cmd.Blit(colorTarget, TempTargetId, _mat);
            cmd.Blit(TempTargetId, colorTarget);
            cmd.ReleaseTemporaryRT(TempTargetId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif
