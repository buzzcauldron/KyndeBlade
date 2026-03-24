using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Hunger - personification of starvation from Piers Plowman. Multi-phase boss.</summary>
    public class HungerCharacter : PhaseBossCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Mage, "Hunger", CharacterSoundTheme.Creaking);
            SetStats(400f, 200f, 18f, 5f, 6f);
            Phase2HealthThreshold = 0.5f;
            Phase3HealthThreshold = 0.25f;
            HungerMoveset.ApplyToCharacter(this);
        }
    }
}
