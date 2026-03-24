using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Sloth - one of the Seven Deadly Sins from Piers Plowman.</summary>
    public class SlothCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, "Sloth", CharacterSoundTheme.Sloth);
            SetStats(250f, 100f, 10f, 12f, 4f);
        }
    }
}
