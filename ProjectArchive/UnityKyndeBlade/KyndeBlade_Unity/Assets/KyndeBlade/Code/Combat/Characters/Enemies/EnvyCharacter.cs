using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Envy - one of the Seven Deadly Sins from Piers Plowman.</summary>
    public class EnvyCharacter : MedievalCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, "Envy", CharacterSoundTheme.Envy);
            SetStats(130f, 120f, 15f, 5f, 13f);
        }
    }
}
