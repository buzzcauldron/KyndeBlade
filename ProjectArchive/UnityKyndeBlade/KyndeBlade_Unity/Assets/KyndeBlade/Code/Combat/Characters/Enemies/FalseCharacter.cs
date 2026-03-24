using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>False - personification of falsehood and deception from Piers Plowman.</summary>
    public class FalseCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Rogue, "False", CharacterSoundTheme.Default);
            SetStats(110f, 150f, 16f, 6f, 16f);
        }
    }
}
