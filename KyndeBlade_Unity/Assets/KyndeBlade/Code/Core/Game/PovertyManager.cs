using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Resource scarcity from poverty. Affects stamina regen, Kynde generation.</summary>
    public class PovertyManager : MonoBehaviour
    {
        [Header("References")]
        public SaveManager SaveManager;

        [Header("Poverty Level (0 = none, 1-5 = increasing)")]
        [Range(0, 5)]
        public int PovertyLevel;

        [Header("Modifiers")]
        [Tooltip("Stamina regen multiplier per poverty level (e.g. 0.8 = 20% less at level 1).")]
        public float StaminaRegenModifierPerLevel = 0.85f;
        [Tooltip("Kynde generation multiplier per poverty level.")]
        public float KyndeGenModifierPerLevel = 0.9f;

        public event Action<int> OnPovertyChanged;

        public int CurrentPovertyLevel => SaveManager?.CurrentProgress != null ? SaveManager.CurrentProgress.PovertyLevel : PovertyLevel;

        void Start()
        {
            if (SaveManager == null) SaveManager = FindObjectOfType<SaveManager>();
        }

        public void SetPovertyLevel(int level)
        {
            level = Mathf.Clamp(level, 0, 5);
            PovertyLevel = level;
            if (SaveManager?.CurrentProgress != null)
            {
                SaveManager.CurrentProgress.PovertyLevel = level;
                SaveManager.Save();
            }
            OnPovertyChanged?.Invoke(level);
        }

        public float GetStaminaRegenMultiplier()
        {
            int lvl = CurrentPovertyLevel;
            if (lvl <= 0) return 1f;
            return Mathf.Pow(StaminaRegenModifierPerLevel, lvl);
        }

        public float GetKyndeGenMultiplier()
        {
            int lvl = CurrentPovertyLevel;
            if (lvl <= 0) return 1f;
            return Mathf.Pow(KyndeGenModifierPerLevel, lvl);
        }
    }
}
