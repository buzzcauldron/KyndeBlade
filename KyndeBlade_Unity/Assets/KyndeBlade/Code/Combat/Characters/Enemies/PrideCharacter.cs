using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Pride (Superbia) - final boss from Piers Plowman. The ultimate obstacle to Grace.</summary>
    public class PrideCharacter : PhaseBossCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, "Pride", CharacterSoundTheme.Pride);
            SetStats(500f, 250f, 22f, 12f, 7f);
            Phase2HealthThreshold = 0.6f;
            Phase3HealthThreshold = 0.3f;
            PrideMoveset.ApplyToCharacter(this);
        }
    }
}
