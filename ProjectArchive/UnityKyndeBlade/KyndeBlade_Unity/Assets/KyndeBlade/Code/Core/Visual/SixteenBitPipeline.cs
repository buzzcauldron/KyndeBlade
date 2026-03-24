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
        RenderTexture _manuscriptScratch;

        void OnEnable()
        {
            _cam = GetComponent<Camera>();
            if (_cam == null) return;

            EnsureLowResRT();
            // Only redirect to RT when it's ready; otherwise camera renders to screen (avoids black screen if RT fails)
            if (_lowResRT != null && _lowResRT.IsCreated())
                _cam.targetTexture = _lowResRT;

            var pointShader = Shader.Find("KyndeBlade/Point Upscale (16-Bit)");
            if (pointShader != null) _pointMat = new Material(pointShader);

            if (ApplyManuscriptOverlay)
            {
                var msShader = Shader.Find(ManuscriptOverlayParams.ShaderName);
                if (msShader != null) _manuscriptMat = new Material(msShader);
            }
        }

        void LateUpdate()
        {
            // Apply targetTexture once RT is ready (e.g. if Create() was deferred)
            if (_cam != null && _cam.targetTexture == null && _lowResRT != null && _lowResRT.IsCreated())
                _cam.targetTexture = _lowResRT;
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
            if (_manuscriptScratch != null) { _manuscriptScratch.Release(); _manuscriptScratch = null; }
            if (_pointMat != null) { Destroy(_pointMat); _pointMat = null; }
            if (_manuscriptMat != null) { Destroy(_manuscriptMat); _manuscriptMat = null; }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_lowResRT == null || !_lowResRT.IsCreated())
            {
                Graphics.Blit(src, dest);
                return;
            }

            int w = Screen.width;
            int h = Screen.height;
            if (w <= 0 || h <= 0)
            {
                // Still satisfy Unity: non-null dest must receive a write.
                if (dest != null)
                    Graphics.Blit(src, dest);
                else if (_pointMat != null)
                    Graphics.Blit(_lowResRT, null as RenderTexture, _pointMat);
                else
                    Graphics.Blit(_lowResRT, null as RenderTexture);
                return;
            }

            EnsureScreenSizedRT(ref _tempRT, w, h);
            if (_pointMat != null)
                Graphics.Blit(_lowResRT, _tempRT, _pointMat);
            else
                Graphics.Blit(_lowResRT, _tempRT);

            RenderTexture composite;
            if (ApplyManuscriptOverlay && _manuscriptMat != null)
            {
                EnsureScreenSizedRT(ref _manuscriptScratch, w, h);
                ManuscriptOverlayParams.ApplyToMaterial(_manuscriptMat, ParchmentTexture, ParchmentStrength, SepiaTint, SepiaStrength, Grain);
                Graphics.Blit(_tempRT, _manuscriptScratch, _manuscriptMat);
                composite = _manuscriptScratch;
            }
            else
                composite = _tempRT;

            // Unity requires a Blit into `dest` whenever dest is non-null ("possibly didn't write to destination").
            // When dest is the camera's low-res target, that write does not reach the screen — also blit composite to null.
            if (dest == null)
                Graphics.Blit(composite, dest);
            else if (dest == _lowResRT)
            {
                Graphics.Blit(src, dest);
                Graphics.Blit(composite, null as RenderTexture);
            }
            else
                Graphics.Blit(composite, dest);
        }

        static void EnsureScreenSizedRT(ref RenderTexture rt, int w, int h)
        {
            if (rt != null && rt.width == w && rt.height == h) return;
            if (rt != null) rt.Release();
            rt = new RenderTexture(w, h, 0) { filterMode = FilterMode.Point };
            rt.Create();
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
