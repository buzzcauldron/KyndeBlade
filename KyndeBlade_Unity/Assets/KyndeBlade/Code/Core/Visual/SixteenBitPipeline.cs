using UnityEngine;

namespace KyndeBlade
{
    /// <summary>16-bit spinoff pipeline: render game at low resolution (SNES-style), point upscale, then apply manuscript overlay (parchment/grain/sepia) from the earlier full-screen code. Add to Main Camera; disable ManuscriptOverlayEffect on the same camera when using this.</summary>
    [RequireComponent(typeof(Camera))]
    public class SixteenBitPipeline : MonoBehaviour
    {
        [Header("16-Bit Resolution")]
        [Tooltip("Native resolution (SNES-style). 256×224 or 512×448 typical.")]
        public Vector2Int NativeSize = new Vector2Int(256, 224);

        [Header("Manuscript Overlay (reuses earlier code)")]
        [Tooltip("Apply parchment + grain + sepia on top of 16-bit image.")]
        public bool ApplyManuscriptOverlay = true;
        public Texture2D ParchmentTexture;
        [Range(0f, 1f)] public float ParchmentStrength = 0.45f;
        public Color SepiaTint = new Color(0.76f, 0.68f, 0.55f);
        [Range(0f, 1f)] public float SepiaStrength = 0.35f;
        [Range(0f, 0.15f)] public float Grain = 0.06f;

        Camera _cam;
        RenderTexture _lowResRT;
        Material _pointMat;
        Material _manuscriptMat;
        RenderTexture _tempRT;
        static readonly int LowResRTId = Shader.PropertyToID("_SixteenBitLowRes");

        void OnEnable()
        {
            _cam = GetComponent<Camera>();
            if (_cam == null) return;

            EnsureLowResRT();
            _cam.targetTexture = _lowResRT;

            var pointShader = Shader.Find("KyndeBlade/Point Upscale (16-Bit)");
            if (pointShader != null) _pointMat = new Material(pointShader);

            if (ApplyManuscriptOverlay)
            {
                var msShader = Shader.Find(ManuscriptOverlayParams.ShaderName);
                if (msShader != null) _manuscriptMat = new Material(msShader);
            }
        }

        void OnDisable()
        {
            if (_cam != null) _cam.targetTexture = null;
            if (_lowResRT != null)
            {
                _lowResRT.Release();
                _lowResRT = null;
            }
            if (_tempRT != null) { _tempRT.Release(); _tempRT = null; }
            if (_pointMat != null) DestroyImmediate(_pointMat);
            if (_manuscriptMat != null) DestroyImmediate(_manuscriptMat);
        }

        void OnPostRender()
        {
            if (_lowResRT == null || !_lowResRT.IsCreated()) return;

            // Point upscale: low-res RT -> backbuffer
            if (_pointMat != null)
                Graphics.Blit(_lowResRT, (RenderTexture)null, _pointMat);
            else
                Graphics.Blit(_lowResRT, (RenderTexture)null);

            // Manuscript overlay on top (reuse earlier full-screen params)
            if (ApplyManuscriptOverlay && _manuscriptMat != null)
            {
                int w = Screen.width, h = Screen.height;
                if (_tempRT == null || _tempRT.width != w || _tempRT.height != h)
                {
                    if (_tempRT != null) _tempRT.Release();
                    _tempRT = new RenderTexture(w, h, 0);
                }
                ManuscriptOverlayParams.ApplyToMaterial(_manuscriptMat, ParchmentTexture, ParchmentStrength, SepiaTint, SepiaStrength, Grain);
                Graphics.Blit((RenderTexture)null, _tempRT, _manuscriptMat);
                Graphics.Blit(_tempRT, (RenderTexture)null);
            }
        }

        void EnsureLowResRT()
        {
            int w = Mathf.Max(64, NativeSize.x);
            int h = Mathf.Max(64, NativeSize.y);
            if (_lowResRT != null && _lowResRT.width == w && _lowResRT.height == h) return;
            if (_lowResRT != null) _lowResRT.Release();
            _lowResRT = new RenderTexture(w, h, 24);
            _lowResRT.filterMode = FilterMode.Point;
            _lowResRT.Create();
        }
    }
}
