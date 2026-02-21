using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Pride (Superbia) - final boss from Piers Plowman. The ultimate obstacle to Grace.</summary>
    public class PrideCharacter : MedievalCharacter
    {
        [Header("Pride Boss")]
        [Tooltip("Phase 2 starts when health below this ratio (0-1).")]
        public float Phase2HealthThreshold = 0.6f;
        [Tooltip("Phase 3 starts when health below this ratio (0-1).")]
        public float Phase3HealthThreshold = 0.3f;

        public int CurrentPhase { get; private set; } = 1;

        void Start()
        {
            CharacterClassType = CharacterClass.Knight;
            CharacterName = "Pride";
            SoundTheme = CharacterSoundTheme.Pride;
            Stats.MaxHealth = 500f;
            Stats.CurrentHealth = 500f;
            Stats.MaxStamina = 250f;
            Stats.CurrentStamina = 250f;
            Stats.AttackPower = 22f;
            Stats.Defense = 12f;
            Stats.Speed = 7f;

            PrideMoveset.ApplyToCharacter(this);
        }

        void Update()
        {
            float hpRatio = Stats.CurrentHealth / Mathf.Max(1f, Stats.MaxHealth);
            if (hpRatio <= Phase3HealthThreshold) CurrentPhase = 3;
            else if (hpRatio <= Phase2HealthThreshold) CurrentPhase = 2;
            else CurrentPhase = 1;
        }
    }
}
