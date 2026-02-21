using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Elde - Old Age personified from Piers Plowman. When Elde hits a character, they age.</summary>
    public class EldeCharacter : MedievalCharacter
    {
        void Start()
        {
            CharacterClassType = CharacterClass.Mage;
            CharacterName = "Elde";
            SoundTheme = CharacterSoundTheme.Sloth;
            Stats.MaxHealth = 350f;
            Stats.CurrentHealth = 350f;
            Stats.MaxStamina = 180f;
            Stats.CurrentStamina = 180f;
            Stats.AttackPower = 16f;
            Stats.Defense = 6f;
            Stats.Speed = 5f;

            EldeMoveset.ApplyToCharacter(this);
        }
    }
}
