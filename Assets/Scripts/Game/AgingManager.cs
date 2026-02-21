using System;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Aging per real-world hours. Applied as permanent status effect—slower but wiser.</summary>
    public class AgingManager : MonoBehaviour
    {
        [Header("References")]
        public SaveManager SaveManager;

        [Header("Aging per Real-World Hour")]
        [Tooltip("Speed multiplier per hour played (e.g. 0.97 = 3% slower each hour).")]
        public float SpeedModifierPerHour = 0.97f;
        [Tooltip("Defense bonus per hour (wiser = better defense).")]
        public float DefenseBonusPerHour = 0.1f;
        [Tooltip("Kynde generation multiplier per hour (wiser = more Kynde).")]
        public float KyndeGenModifierPerHour = 1.02f;
        [Tooltip("Stamina regen multiplier per hour (slightly reduced with age).")]
        public float StaminaRegenModifierPerHour = 0.98f;
        [Tooltip("Hours of play before first aging tier applies.")]
        public float HoursBeforeFirstAging = 0.5f;

        public event Action<float> OnAgingApplied;

        public float TotalPlayTimeHours => (SaveManager?.CurrentProgress?.TotalPlayTimeSeconds ?? 0f) / 3600f;
        public int AgeTier => Mathf.Max(0, Mathf.FloorToInt((TotalPlayTimeHours - HoursBeforeFirstAging) / 1f));

        void Start()
        {
            if (SaveManager == null) SaveManager = FindObjectOfType<SaveManager>();
        }

        float _saveInterval;

        void Update()
        {
            if (SaveManager?.CurrentProgress != null)
            {
                SaveManager.CurrentProgress.TotalPlayTimeSeconds += Time.deltaTime;
                _saveInterval += Time.deltaTime;
                if (_saveInterval >= 300f)
                {
                    _saveInterval = 0f;
                    SaveManager.Save();
                }
            }
        }

        /// <summary>Apply age as permanent status effect to each party member. Refreshes tier if already aged.</summary>
        public void ApplyAgeToParty(List<MedievalCharacter> party)
        {
            if (party == null) return;
            int tier = AgeTier;
            if (tier <= 0) return;

            float speedMult = Mathf.Pow(SpeedModifierPerHour, tier);
            float defenseMult = 1f + DefenseBonusPerHour * tier;
            float kyndeMult = Mathf.Pow(KyndeGenModifierPerHour, tier);
            float staminaMult = Mathf.Pow(StaminaRegenModifierPerHour, tier);

            foreach (var c in party)
            {
                if (c == null) continue;
                c.RemoveStatusEffect(StatusEffectType.Age);
                var effect = StatusEffect.CreateAgeEffect(tier, speedMult, defenseMult, kyndeMult, staminaMult);
                if (effect != null)
                    c.ApplyStatusEffect(effect);
            }

            OnAgingApplied?.Invoke(TotalPlayTimeHours);
        }

        public float GetSpeedMultiplier()
        {
            int tier = AgeTier;
            return tier <= 0 ? 1f : Mathf.Pow(SpeedModifierPerHour, tier);
        }

        public float GetKyndeGenMultiplier()
        {
            int tier = AgeTier;
            return tier <= 0 ? 1f : Mathf.Pow(KyndeGenModifierPerHour, tier);
        }
    }
}
