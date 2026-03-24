using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Accessibility and difficulty settings (Hodent: Usability).</summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "KyndeBlade/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Timing (Accessibility)")]
        [Range(0.5f, 3f)]
        [Tooltip("Multiplier for dodge/parry windows. 1 = normal, 1.5 = easier.")]
        public float TimingWindowMultiplier = 1f;

        [Header("Player Loop Tuning")]
        [Range(0.7f, 1.5f)]
        [Tooltip("Scales player strike/ranged damage for core-loop tuning.")]
        public float PlayerDamageMultiplier = 1f;
        [Range(0.7f, 1.5f)]
        [Tooltip("Scales player stamina costs. Lower is more forgiving.")]
        public float PlayerStaminaCostMultiplier = 1f;
        [Range(0.5f, 1.5f)]
        [Tooltip("Scales cast and execution times for player actions.")]
        public float PlayerActionTimingMultiplier = 1f;

        [Header("Input Forgiveness")]
        [Range(0f, 0.3f)]
        [Tooltip("Accept action presses shortly before the game can execute them.")]
        public float InputBufferSeconds = 0.12f;
        [Range(0f, 0.2f)]
        [Tooltip("Accept dodge/parry input shortly before real-time windows open.")]
        public float DefenseCoyoteSeconds = 0.08f;

        [Header("Difficulty Scalars")]
        [Range(0.5f, 2f)]
        public float EnemyHealthMultiplierEasy = 0.85f;
        [Range(0.5f, 2f)]
        public float EnemyHealthMultiplierNormal = 1f;
        [Range(0.5f, 2f)]
        public float EnemyHealthMultiplierHard = 1.2f;
        [Range(0.5f, 2f)]
        public float EnemyHealthMultiplierExpert = 1.35f;
        [Range(0.5f, 2f)]
        public float EnemyDamageMultiplierEasy = 0.85f;
        [Range(0.5f, 2f)]
        public float EnemyDamageMultiplierNormal = 1f;
        [Range(0.5f, 2f)]
        public float EnemyDamageMultiplierHard = 1.2f;
        [Range(0.5f, 2f)]
        public float EnemyDamageMultiplierExpert = 1.35f;
        [Range(0.6f, 1.4f)]
        [Tooltip("Scales enemy decision delay (lower = more aggressive).")]
        public float EnemyAggressionMultiplierEasy = 1.1f;
        [Range(0.6f, 1.4f)]
        public float EnemyAggressionMultiplierNormal = 1f;
        [Range(0.6f, 1.4f)]
        public float EnemyAggressionMultiplierHard = 0.9f;
        [Range(0.6f, 1.4f)]
        public float EnemyAggressionMultiplierExpert = 0.8f;

        [Header("Performance Budget")]
        [Tooltip("Target average frame time during combat.")]
        public float CombatFrameBudgetMs = 16.6f;
        [Tooltip("Warn if sustained frame time exceeds this spike threshold.")]
        public float CombatFrameSpikeBudgetMs = 24f;

        [Header("Difficulty")]
        public DifficultyMode Difficulty = DifficultyMode.Normal;

        [Header("Presentation (combat)")]
        [Tooltip("Full-screen combat backdrop (SNES-style dark void). Applied by KyndeBladeGameManager.EnsureCombatBackground.")]
        public Color CombatBackdropColor = new Color(0.06f, 0.07f, 0.1f, 0.96f);
        [Tooltip("Optional orange/brown strip at bottom of combat view (hazard read).")]
        public bool ShowCombatForegroundHazardStrip;

        public float GetAdjustedWindow(float baseWindow)
        {
            float mult = TimingWindowMultiplier;
            switch (Difficulty)
            {
                case DifficultyMode.Easy: mult *= 1.5f; break;
                case DifficultyMode.Hard: mult *= 0.75f; break;
                case DifficultyMode.Expert: mult *= 0.5f; break;
            }
            return baseWindow * mult;
        }

        public float GetEnemyHealthMultiplier()
        {
            switch (Difficulty)
            {
                case DifficultyMode.Easy: return EnemyHealthMultiplierEasy;
                case DifficultyMode.Hard: return EnemyHealthMultiplierHard;
                case DifficultyMode.Expert: return EnemyHealthMultiplierExpert;
                default: return EnemyHealthMultiplierNormal;
            }
        }

        public float GetEnemyDamageMultiplier()
        {
            switch (Difficulty)
            {
                case DifficultyMode.Easy: return EnemyDamageMultiplierEasy;
                case DifficultyMode.Hard: return EnemyDamageMultiplierHard;
                case DifficultyMode.Expert: return EnemyDamageMultiplierExpert;
                default: return EnemyDamageMultiplierNormal;
            }
        }

        public float GetEnemyAggressionMultiplier()
        {
            switch (Difficulty)
            {
                case DifficultyMode.Easy: return EnemyAggressionMultiplierEasy;
                case DifficultyMode.Hard: return EnemyAggressionMultiplierHard;
                case DifficultyMode.Expert: return EnemyAggressionMultiplierExpert;
                default: return EnemyAggressionMultiplierNormal;
            }
        }
    }

    public enum DifficultyMode { Easy, Normal, Hard, Expert }
}
