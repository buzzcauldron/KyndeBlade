using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Lust - one of the Seven Deadly Sins from Piers Plowman.</summary>
    public class LustCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, "Lust", CharacterSoundTheme.Default);
            SetStats(100f, 130f, 16f, 4f, 14f);
        }
    }
}
