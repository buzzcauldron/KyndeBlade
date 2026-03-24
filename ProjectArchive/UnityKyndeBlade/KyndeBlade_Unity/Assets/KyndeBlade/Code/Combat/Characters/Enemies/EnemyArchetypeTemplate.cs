using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Authoring template for enemy stats/telegraph timings to keep archetypes consistent.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyArchetypeTemplate", menuName = "KyndeBlade/Enemy Archetype Template")]
    public class EnemyArchetypeTemplate : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName = "Enemy";
        public CharacterClass EnemyClass = CharacterClass.Knight;
        public CharacterSoundTheme SoundTheme = CharacterSoundTheme.Default;

        [Header("Combat Stats")]
        public float MaxHealth = 120f;
        public float MaxStamina = 120f;
        public float AttackPower = 16f;
        public float Defense = 6f;
        public float Speed = 10f;

        [Header("Readability Timing")]
        [Tooltip("Used by combat systems as a baseline telegraph hint.")]
        public float TelegraphSeconds = 0.5f;
        [Tooltip("Minimum action recovery to reduce unreadable spam.")]
        public float MinRecoverySeconds = 0.2f;

        public void ApplyTo(MedievalCharacter character)
        {
            if (character == null) return;
            character.SetIdentity(EnemyClass, string.IsNullOrWhiteSpace(DisplayName) ? character.name : DisplayName, SoundTheme);
            character.SetStats(MaxHealth, MaxStamina, AttackPower, Defense, Speed);
            character.BeginActionRecovery(Mathf.Max(0f, MinRecoverySeconds));
        }
    }
}
