using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Elde - Old Age personified from Piers Plowman. When Elde hits a character, they age.</summary>
    public class EldeCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Mage, "Elde", CharacterSoundTheme.Sloth);
            SetStats(350f, 180f, 16f, 6f, 5f);
            EldeMoveset.ApplyToCharacter(this);
        }
    }
}
