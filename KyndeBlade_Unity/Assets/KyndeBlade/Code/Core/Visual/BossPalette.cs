using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Grimdark medieval aesthetic: sin-specific desaturated colors for bosses. Use for health bars, VFX tints, and manuscript accents.</summary>
    [CreateAssetMenu(fileName = "BossPalette", menuName = "KyndeBlade/Boss Palette")]
    public class BossPalette : ScriptableObject
    {
        [Header("Pride – Imperial Purple & Gold")]
        [Tooltip("Polished, mirror-like; velvet capes.")]
        public Color PridePrimary = new Color(0.35f, 0.2f, 0.45f);   // desaturated imperial purple
        public Color PrideSecondary = new Color(0.7f, 0.6f, 0.25f);  // gold

        [Header("Hunger – Pale Grey & Off-White")]
        [Tooltip("Bone, stretched leather, sunken; the endless void.")]
        public Color HungerPrimary = new Color(0.6f, 0.58f, 0.55f);  // pale grey
        public Color HungerSecondary = new Color(0.92f, 0.9f, 0.85f); // off-white

        [Header("Green Knight – Deep Moss & Copper")]
        [Tooltip("Overgrown iron, rusted blades, decaying leaves; eco-brutalist.")]
        public Color GreenKnightPrimary = new Color(0.22f, 0.35f, 0.2f);  // deep moss
        public Color GreenKnightSecondary = new Color(0.55f, 0.35f, 0.2f); // copper/rust

        [Header("Elde – Sloth / Age")]
        public Color EldePrimary = new Color(0.4f, 0.38f, 0.35f);
        public Color EldeSecondary = new Color(0.5f, 0.48f, 0.45f);

        [Header("Wrath / Greed / Envy – extend as needed")]
        public Color WrathPrimary = new Color(0.45f, 0.18f, 0.15f);
        public Color GreedPrimary = new Color(0.5f, 0.42f, 0.15f);

        /// <summary>Get primary color by boss character name. Falls back to a neutral if unknown.</summary>
        public Color GetPrimaryForBoss(string bossName)
        {
            if (string.IsNullOrEmpty(bossName)) return Color.grey;
            if (bossName.IndexOf("Pride", System.StringComparison.OrdinalIgnoreCase) >= 0) return PridePrimary;
            if (bossName.IndexOf("Hunger", System.StringComparison.OrdinalIgnoreCase) >= 0) return HungerPrimary;
            if (bossName.IndexOf("Green", System.StringComparison.OrdinalIgnoreCase) >= 0) return GreenKnightPrimary;
            if (bossName.IndexOf("Elde", System.StringComparison.OrdinalIgnoreCase) >= 0) return EldePrimary;
            if (bossName.IndexOf("Wrath", System.StringComparison.OrdinalIgnoreCase) >= 0) return WrathPrimary;
            if (bossName.IndexOf("Mede", System.StringComparison.OrdinalIgnoreCase) >= 0) return GreedPrimary;
            return new Color(0.4f, 0.4f, 0.4f);
        }

        public Color GetSecondaryForBoss(string bossName)
        {
            if (string.IsNullOrEmpty(bossName)) return Color.grey;
            if (bossName.IndexOf("Pride", System.StringComparison.OrdinalIgnoreCase) >= 0) return PrideSecondary;
            if (bossName.IndexOf("Hunger", System.StringComparison.OrdinalIgnoreCase) >= 0) return HungerSecondary;
            if (bossName.IndexOf("Green", System.StringComparison.OrdinalIgnoreCase) >= 0) return GreenKnightSecondary;
            if (bossName.IndexOf("Elde", System.StringComparison.OrdinalIgnoreCase) >= 0) return EldeSecondary;
            return GetPrimaryForBoss(bossName);
        }
    }
}
