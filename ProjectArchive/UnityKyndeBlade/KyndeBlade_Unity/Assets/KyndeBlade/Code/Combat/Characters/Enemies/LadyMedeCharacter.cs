using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Lady Mede - personification of bribery and corruption from Piers Plowman.</summary>
    public class LadyMedeCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Mage, "Lady Mede", CharacterSoundTheme.Greed);
            SetStats(95f, 160f, 22f, 4f, 9f);
        }
    }
}
