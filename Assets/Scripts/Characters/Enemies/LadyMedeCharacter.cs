using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Lady Mede - personification of bribery and corruption from Piers Plowman.</summary>
    public class LadyMedeCharacter : MedievalCharacter
    {
        void Start()
        {
            CharacterClassType = CharacterClass.Mage;
            CharacterName = "Lady Mede";
            Stats.MaxHealth = 95f;
            Stats.CurrentHealth = 95f;
            Stats.MaxStamina = 160f;
            Stats.CurrentStamina = 160f;
            Stats.AttackPower = 22f;
            Stats.Defense = 4f;
            Stats.Speed = 9f;
        }
    }
}
