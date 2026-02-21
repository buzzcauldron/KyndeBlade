using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Green Knight - superboss from Sir Gawain. Wild nature, cyclical beheading. Cannot be permanently defeated.</summary>
    public class GreenKnightCharacter : MedievalCharacter
    {
        [Header("Green Knight")]
        [Tooltip("Phase 2 starts when health below this ratio (0-1).")]
        public float Phase2HealthThreshold = 0.5f;

        public int CurrentPhase { get; private set; } = 1;

        void Start()
        {
            CharacterClassType = CharacterClass.Knight;
            CharacterName = "The Green Knight";
            SoundTheme = CharacterSoundTheme.Default;
            Stats.MaxHealth = 600f;
            Stats.CurrentHealth = 600f;
            Stats.MaxStamina = 280f;
            Stats.CurrentStamina = 280f;
            Stats.AttackPower = 26f;
            Stats.Defense = 14f;
            Stats.Speed = 8f;

            GreenKnightMoveset.ApplyToCharacter(this);
        }

        void Update()
        {
            float hpRatio = Stats.CurrentHealth / Mathf.Max(1f, Stats.MaxHealth);
            CurrentPhase = hpRatio <= Phase2HealthThreshold ? 2 : 1;
        }
    }
}
