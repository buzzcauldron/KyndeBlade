using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Green Knight - superboss from Sir Gawain. Wild nature, cyclical beheading. Cannot be permanently defeated.</summary>
    public class GreenKnightCharacter : PhaseBossCharacter
    {
        void Start()
        {
            SetIdentity(CharacterClass.Knight, GameWorldConstants.GreenKnightDisplayName, CharacterSoundTheme.Default);
            SetStats(600f, 280f, 26f, 14f, 8f);
            Phase2HealthThreshold = 0.5f;
            Phase3HealthThreshold = 0f; // 2-phase only
            GreenKnightMoveset.ApplyToCharacter(this);
        }
    }
}
