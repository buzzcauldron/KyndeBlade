using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Wrath - one of the Seven Deadly Sins from Piers Plowman.</summary>
    public class WrathCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, "Wrath", CharacterSoundTheme.Wrath);
            SetStats(180f, 110f, 18f, 8f, 7f);
        }
    }
}
