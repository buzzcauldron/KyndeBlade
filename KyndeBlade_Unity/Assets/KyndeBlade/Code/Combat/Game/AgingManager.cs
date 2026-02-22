using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Aging per real-world hours. Applied as permanent status effect—slower but wiser.</summary>
    public class AgingManager : MonoBehaviour
    {
        [Header("References")]
        public SaveManager SaveManager;
        [Tooltip("Assign in Inspector or leave null to find on Start (cached).")]
        public WorldMapManager WorldMapManager;

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
        [Tooltip("Age tier at which player dies of old age (e.g. 10 = ~10.5 hours of play, or faster when waiting at Field of Grace).")]
        public int AgeTierDeathThreshold = 10;
        [Tooltip("Time multiplier when waiting at Field of Grace (e.g. 60 = 1 real second = 1 minute of aging).")]
        public float FieldOfGraceTimeMultiplier = 60f;

        public event Action<float> OnAgingApplied;
        /// <summary>Fired when at Field of Grace and age tier reaches death threshold. Subscribe from GameStateManager to trigger death-of-old-age flow.</summary>
        public event Action OnDeathOfOldAgeRequested;

        public float TotalPlayTimeHours => (SaveManager?.CurrentProgress?.TotalPlayTimeSeconds ?? 0f) / 3600f;
        public int AgeTier => Mathf.Max(0, Mathf.FloorToInt((TotalPlayTimeHours - HoursBeforeFirstAging) / 1f));

        void Start()
        {
            if (SaveManager == null) SaveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (WorldMapManager == null) WorldMapManager = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            if (SaveManager == null) LogMissingReferenceOnce("SaveManager");
            if (WorldMapManager == null) LogMissingReferenceOnce("WorldMapManager");
        }

        float _saveInterval;
        bool _deathOfOldAgeTriggered;
        static bool _warnedSaveManager;
        static bool _warnedWorldMapManager;

        void LogMissingReferenceOnce(string name)
        {
            if (name == "SaveManager" && !_warnedSaveManager) { _warnedSaveManager = true; Debug.LogWarning($"[AgingManager] {name} not found. Assign in Inspector or ensure one exists in scene. Cached in Start(); no per-frame search."); }
            if (name == "WorldMapManager" && !_warnedWorldMapManager) { _warnedWorldMapManager = true; Debug.LogWarning($"[AgingManager] {name} not found. Assign in Inspector or ensure one exists in scene. Cached in Start(); no per-frame search."); }
        }

        float _logSkipInterval;

        void Update()
        {
            if (SaveManager?.CurrentProgress != null)
            {
                float delta = Time.deltaTime;
                bool atFieldOfGrace = WorldMapManager != null && WorldMapManager.CurrentLocation != null &&
                    string.Equals(WorldMapManager.CurrentLocation.LocationId, "field_of_grace", System.StringComparison.OrdinalIgnoreCase);
                if (!atFieldOfGrace)
                    _deathOfOldAgeTriggered = false;
                else
                {
                    if (FieldOfGraceTimeMultiplier > 1f)
                        delta *= FieldOfGraceTimeMultiplier;
                    if (AgeTier >= AgeTierDeathThreshold && !_deathOfOldAgeTriggered)
                    {
                        _deathOfOldAgeTriggered = true;
                        OnDeathOfOldAgeRequested?.Invoke();
                    }
                }
                SaveManager.CurrentProgress.TotalPlayTimeSeconds += delta;
                _saveInterval += Time.deltaTime;
                if (_saveInterval >= 300f)
                {
                    _saveInterval = 0f;
                    SaveManager.Save();
                }
            }
            else
            {
                _logSkipInterval += Time.deltaTime;
                if (_logSkipInterval >= 10f)
                {
                    _logSkipInterval = 0f;
                    // #region agent log
                    try { var _p = "/Users/halxiii/KyndeBlade/.cursor/debug.log"; var _d = Path.GetDirectoryName(_p); if (!string.IsNullOrEmpty(_d)) Directory.CreateDirectory(_d); File.AppendAllText(_p, "{\"location\":\"AgingManager.cs:Update\",\"message\":\"skip\",\"data\":{\"saveNull\":" + (SaveManager == null ? "true" : "false") + ",\"progressNull\":" + (SaveManager != null && SaveManager.CurrentProgress == null ? "true" : "false") + "},\"timestamp\":" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + ",\"hypothesisId\":\"H5\"}\n"); } catch { }
                    // #endregion
                }
            }
        }

        /// <summary>Apply age as permanent status effect to a single character. Used when Elde hits. Refreshes tier if already aged.</summary>
        public void ApplyAgeToCharacter(MedievalCharacter target, int tier)
        {
            if (target == null || tier <= 0) return;

            float speedMult = Mathf.Pow(SpeedModifierPerHour, tier);
            float defenseMult = 1f + DefenseBonusPerHour * tier;
            float kyndeMult = Mathf.Pow(KyndeGenModifierPerHour, tier);
            float staminaMult = Mathf.Pow(StaminaRegenModifierPerHour, tier);

            target.RemoveStatusEffect(StatusEffectType.Age);
            var effect = StatusEffect.CreateAgeEffect(tier, speedMult, defenseMult, kyndeMult, staminaMult);
            if (effect != null)
                target.ApplyStatusEffect(effect);

            OnAgingApplied?.Invoke(tier);
        }

        /// <summary>Apply age as permanent status effect to each party member. Refreshes tier if already aged. Only used for tests or legacy.</summary>
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
