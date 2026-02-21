using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Hunger - personification of starvation from Piers Plowman. Multi-phase boss.</summary>
    public class HungerCharacter : MedievalCharacter
    {
        [Header("Hunger Boss")]
        [Tooltip("Phase 2 starts when health below this ratio (0-1).")]
        public float Phase2HealthThreshold = 0.5f;
        [Tooltip("Phase 3 starts when health below this ratio (0-1).")]
        public float Phase3HealthThreshold = 0.25f;

        public int CurrentPhase { get; private set; } = 1;

        void Start()
        {
            CharacterClassType = CharacterClass.Mage;
            CharacterName = "Hunger";
            SoundTheme = CharacterSoundTheme.Creaking;
            Stats.MaxHealth = 400f;
            Stats.CurrentHealth = 400f;
            Stats.MaxStamina = 200f;
            Stats.CurrentStamina = 200f;
            Stats.AttackPower = 18f;
            Stats.Defense = 5f;
            Stats.Speed = 6f;

            HungerMoveset.ApplyToCharacter(this);
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
