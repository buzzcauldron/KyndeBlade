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

        [Header("Difficulty")]
        public DifficultyMode Difficulty = DifficultyMode.Normal;

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
    }

    public enum DifficultyMode { Easy, Normal, Hard, Expert }
}
