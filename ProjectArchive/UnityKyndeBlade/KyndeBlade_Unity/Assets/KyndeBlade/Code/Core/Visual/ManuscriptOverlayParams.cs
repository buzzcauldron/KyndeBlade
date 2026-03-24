using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Centralized shader name and property IDs for the manuscript full-screen overlay. Used by ManuscriptOverlayEffect (built-in) and ManuscriptOverlayRendererFeature (URP).</summary>
    public static class ManuscriptOverlayParams
    {
        public const string ShaderName = "KyndeBlade/Manuscript Overlay (Full Screen)";

        public static readonly int ParchmentTexId = Shader.PropertyToID("_ParchmentTex");
        public static readonly int StrengthId = Shader.PropertyToID("_Strength");
        public static readonly int SepiaTintId = Shader.PropertyToID("_SepiaTint");
        public static readonly int SepiaStrengthId = Shader.PropertyToID("_SepiaStrength");
        public static readonly int GrainId = Shader.PropertyToID("_Grain");

        /// <summary>Apply parchment overlay settings to a material. Call before Blit.</summary>
        public static void ApplyToMaterial(Material mat, Texture2D parchmentTexture, float parchmentStrength, Color sepiaTint, float sepiaStrength, float grain)
        {
            if (mat == null) return;
            mat.SetTexture(ParchmentTexId, parchmentTexture != null ? parchmentTexture : Texture2D.grayTexture);
            mat.SetFloat(StrengthId, parchmentStrength);
            mat.SetColor(SepiaTintId, sepiaTint);
            mat.SetFloat(SepiaStrengthId, sepiaStrength);
            mat.SetFloat(GrainId, grain);
        }
    }
}
