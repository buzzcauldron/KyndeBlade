using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Full-screen parchment overlay: multiplies paper texture, grain, and sepia over the camera view (14th-century page). For built-in pipeline; add to Camera. For URP use ManuscriptOverlayRendererFeature.</summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class ManuscriptOverlayEffect : MonoBehaviour
    {
        [Header("Parchment")]
        public Texture2D ParchmentTexture;
        [Range(0f, 1f)]
        public float ParchmentStrength = 0.45f;

        [Header("Aged vellum")]
        public Color SepiaTint = new Color(0.76f, 0.68f, 0.55f);
        [Range(0f, 1f)]
        public float SepiaStrength = 0.35f;

        [Header("Grain")]
        [Range(0f, 0.15f)]
        public float Grain = 0.06f;

        Material _mat;

        void OnEnable()
        {
            var shader = Shader.Find(ManuscriptOverlayParams.ShaderName);
            if (shader != null)
                _mat = new Material(shader);
        }

        void OnDisable()
        {
            if (_mat != null)
            {
                if (Application.isPlaying)
                    Destroy(_mat);
                else
                    DestroyImmediate(_mat);
                _mat = null;
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_mat == null)
            {
                Graphics.Blit(src, dest);
                return;
            }
            ManuscriptOverlayParams.ApplyToMaterial(_mat, ParchmentTexture, ParchmentStrength, SepiaTint, SepiaStrength, Grain);
            Graphics.Blit(src, dest, _mat);
        }
    }
}
