using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Stores run appearance seed/key for restoring after fairy form. Attached by KyndeBladeGameManager.</summary>
    public class PiersAppearanceData : MonoBehaviour
    {
        public int Seed;
        public string CharacterKey;
        public bool IsPlayer;
        [Tooltip("Permanent scar from having ever had hunger. Gaunt, hollowed appearance.")]
        public bool HasHungerScar;
    }

    /// <summary>Piers Plowman–inspired character appearance. Earthy, laborer tones—browns, grays, worn cloth. Randomized per run via seed.</summary>
    public static class PiersAppearanceRandomizer
    {
        // Piers-inspired palette: laborers, pilgrims, earth, poverty (muted, grounded)
        static readonly Color[] PlayerColors =
        {
            new Color(0.35f, 0.45f, 0.55f),  // worn blue-gray (pilgrim)
            new Color(0.4f, 0.5f, 0.5f),    // slate
            new Color(0.45f, 0.48f, 0.52f),  // stone gray
            new Color(0.38f, 0.42f, 0.48f),  // dusty blue
            new Color(0.42f, 0.46f, 0.44f),  // moss gray
        };
        static readonly Color[] EnemyColors =
        {
            new Color(0.55f, 0.35f, 0.3f),   // earth brown
            new Color(0.5f, 0.38f, 0.32f),    // rust
            new Color(0.45f, 0.4f, 0.35f),   // mud
            new Color(0.52f, 0.32f, 0.28f),  // dried blood / corruption
            new Color(0.4f, 0.42f, 0.38f),   // shadow gray
        };

        static readonly Vector3[] Scales =
        {
            new Vector3(0.75f, 1.15f, 1f),
            new Vector3(0.82f, 1.2f, 1f),
            new Vector3(0.78f, 1.18f, 1f),
            new Vector3(0.8f, 1.1f, 1f),
            new Vector3(0.76f, 1.22f, 1f),
        };

        /// <summary>Get Piers-inspired color and scale for a character. Deterministic per run from seed + characterKey.</summary>
        /// <param name="hasHungerScar">If true, apply gaunt tint: desaturated, darker, slight sickly shift.</param>
        public static void GetAppearance(int seed, string characterKey, bool isPlayer, out Color color, out Vector3 scale, bool hasHungerScar = false)
        {
            var rng = new System.Random(seed + (characterKey ?? "").GetHashCode());
            var colors = isPlayer ? PlayerColors : EnemyColors;
            int ci = rng.Next(colors.Length);
            int si = rng.Next(Scales.Length);
            color = colors[ci];
            scale = Scales[si];
            // Slight hue/sat variation
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            h = (h + (float)(rng.NextDouble() - 0.5) * 0.08f) % 1f;
            if (h < 0f) h += 1f;
            s = Mathf.Clamp01(s + (float)(rng.NextDouble() - 0.5) * 0.1f);
            v = Mathf.Clamp01(v + (float)(rng.NextDouble() - 0.5) * 0.08f);
            color = Color.HSVToRGB(h, s, v);
            if (hasHungerScar)
            {
                s *= 0.6f;
                v *= 0.85f;
                color = Color.HSVToRGB(h, s, v);
                color = new Color(color.r * 0.92f, color.g * 0.95f, color.b, color.a);
            }
        }
    }
}
