using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Wrath - one of the Seven Deadly Sins from Piers Plowman.</summary>
    public class WrathCharacter : MedievalCharacter
    {
        void Start()
        {
            CharacterClassType = CharacterClass.Knight;
            CharacterName = "Wrath";
            Stats.MaxHealth = 180f;
            Stats.CurrentHealth = 180f;
            Stats.MaxStamina = 110f;
            Stats.CurrentStamina = 110f;
            Stats.AttackPower = 18f;
            Stats.Defense = 8f;
            Stats.Speed = 7f;
        }
    }
}
