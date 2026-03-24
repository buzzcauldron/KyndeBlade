using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Enemy boss that has distinct phases by health ratio. Subclasses set thresholds and stats; phase logic is centralized.</summary>
    public abstract class PhaseBossCharacter : MedievalCharacter
    {
        [Header("Phase Boss")]
        [Tooltip("Phase 2 when health below this ratio (0–1).")]
        public float Phase2HealthThreshold = 0.5f;
        [Tooltip("Phase 3 when health below this ratio (0–1). Set ≤ 0 for 2-phase only.")]
        public float Phase3HealthThreshold = 0f;

        public int CurrentPhase { get; protected set; } = 1;

        protected override void Update()
        {
            base.Update();
            UpdatePhaseFromHealth();
        }

        void UpdatePhaseFromHealth()
        {
            float hpRatio = Stats.CurrentHealth / Mathf.Max(1f, Stats.MaxHealth);
            if (Phase3HealthThreshold > 0f && hpRatio <= Phase3HealthThreshold)
                CurrentPhase = 3;
            else if (hpRatio <= Phase2HealthThreshold)
                CurrentPhase = 2;
            else
                CurrentPhase = 1;
        }
    }
}
