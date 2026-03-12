using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Central balance tuning knobs for encounter difficulty, hazards, and timing windows.</summary>
    [CreateAssetMenu(fileName = "BalanceConfig", menuName = "KyndeBlade/Balance Config")]
    public class BalanceConfig : ScriptableObject
    {
        [Header("Enemy Scaling by Vision")]
        [Tooltip("HP multiplier per Vision index (0=Vision I, 1=Vision II).")]
        public float[] VisionHPMultiplier = { 1f, 1.4f };
        [Tooltip("ATK multiplier per Vision index.")]
        public float[] VisionATKMultiplier = { 1f, 1.25f };
        [Tooltip("DEF multiplier per Vision index.")]
        public float[] VisionDEFMultiplier = { 1f, 1.2f };

        [Header("Timing Windows")]
        [Tooltip("Base dodge window in seconds.")]
        public float BaseDodgeWindow = 1.2f;
        [Tooltip("Base parry window in seconds.")]
        public float BaseParryWindow = 0.8f;
        [Tooltip("Per-vision difficulty scaling for timing windows (smaller = harder).")]
        public float[] VisionTimingMultiplier = { 1.2f, 1f };

        [Header("Hazard Scaling")]
        [Tooltip("Hazard strength multiplier per Vision.")]
        public float[] VisionHazardMultiplier = { 1f, 1.3f };

        [Header("Boss Difficulty Curve")]
        public BossTuning[] BossTunings = new BossTuning[]
        {
            new BossTuning { BossId = "false", DifficultyTier = 1, HPOverride = 80, ATKOverride = 8, DEFOverride = 3 },
            new BossTuning { BossId = "lady_mede", DifficultyTier = 2, HPOverride = 100, ATKOverride = 10, DEFOverride = 4 },
            new BossTuning { BossId = "wrath", DifficultyTier = 3, HPOverride = 140, ATKOverride = 16, DEFOverride = 5 },
            new BossTuning { BossId = "hunger", DifficultyTier = 4, HPOverride = 160, ATKOverride = 14, DEFOverride = 8 },
            new BossTuning { BossId = "pride", DifficultyTier = 5, HPOverride = 180, ATKOverride = 18, DEFOverride = 7 },
            new BossTuning { BossId = "elde", DifficultyTier = 4, HPOverride = 170, ATKOverride = 12, DEFOverride = 10 },
            new BossTuning { BossId = "envy", DifficultyTier = 3, HPOverride = 130, ATKOverride = 15, DEFOverride = 5 },
            new BossTuning { BossId = "sloth", DifficultyTier = 3, HPOverride = 250, ATKOverride = 10, DEFOverride = 12 },
            new BossTuning { BossId = "lust", DifficultyTier = 3, HPOverride = 100, ATKOverride = 16, DEFOverride = 4 },
            new BossTuning { BossId = "green_knight", DifficultyTier = 6, HPOverride = 220, ATKOverride = 20, DEFOverride = 10 },
        };

        [Header("Status Effects")]
        [Tooltip("Burning: damage per turn.")]
        public float BurningDPT = 3f;
        [Tooltip("Burning: duration in turns.")]
        public int BurningDuration = 3;
        [Tooltip("Frost: speed reduction percentage.")]
        public float FrostSpeedReduction = 0.3f;
        [Tooltip("Frost: duration in turns.")]
        public int FrostDuration = 3;
        [Tooltip("Stun: duration in turns.")]
        public int StunDuration = 1;
        [Tooltip("Poison: damage per turn.")]
        public float PoisonDPT = 2f;
        [Tooltip("Poison: duration in turns.")]
        public int PoisonDuration = 4;
        [Tooltip("Weak: attack reduction percentage.")]
        public float WeakATKReduction = 0.2f;
        [Tooltip("Vulnerable: defense reduction percentage.")]
        public float VulnerableDEFReduction = 0.25f;

        public float GetHPMultiplier(int visionIndex) =>
            visionIndex >= 0 && visionIndex < VisionHPMultiplier.Length ? VisionHPMultiplier[visionIndex] : 1f;
        public float GetATKMultiplier(int visionIndex) =>
            visionIndex >= 0 && visionIndex < VisionATKMultiplier.Length ? VisionATKMultiplier[visionIndex] : 1f;
        public float GetTimingMultiplier(int visionIndex) =>
            visionIndex >= 0 && visionIndex < VisionTimingMultiplier.Length ? VisionTimingMultiplier[visionIndex] : 1f;

        public BossTuning FindBoss(string bossId)
        {
            if (string.IsNullOrEmpty(bossId)) return null;
            var id = bossId.ToLowerInvariant();
            foreach (var bt in BossTunings)
                if (bt != null && bt.BossId == id) return bt;
            return null;
        }
    }

    [Serializable]
    public class BossTuning
    {
        public string BossId;
        [Range(1, 10)] public int DifficultyTier;
        public float HPOverride;
        public float ATKOverride;
        public float DEFOverride;
    }
}
