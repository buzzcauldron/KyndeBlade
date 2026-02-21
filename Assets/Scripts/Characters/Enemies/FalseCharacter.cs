using UnityEngine;

namespace KyndeBlade
{
    /// <summary>False - personification of falsehood and deception from Piers Plowman.</summary>
    public class FalseCharacter : MedievalCharacter
    {
        void Start()
        {
            CharacterClassType = CharacterClass.Rogue;
            CharacterName = "False";
            Stats.MaxHealth = 110f;
            Stats.CurrentHealth = 110f;
            Stats.MaxStamina = 150f;
            Stats.CurrentStamina = 150f;
            Stats.AttackPower = 16f;
            Stats.Defense = 6f;
            Stats.Speed = 16f;
        }
    }
}
