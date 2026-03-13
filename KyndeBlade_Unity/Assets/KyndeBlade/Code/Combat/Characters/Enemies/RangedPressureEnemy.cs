using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Backline pressure archetype: ranged chip and punish windows.</summary>
    public class RangedPressureEnemy : MedievalCharacter
    {
        public EnemyArchetypeTemplate Template;

        void Start()
        {
            if (Template != null)
            {
                Template.ApplyTo(this);
            }
            else
            {
                SetIdentity(CharacterClass.Archer, "Bowman", CharacterSoundTheme.Envy);
                SetStats(95f, 140f, 16f, 4f, 13f);
            }

            AvailableActions = new List<CombatAction>
            {
                Expedition33Moveset.RangedStrike(),
                Expedition33Moveset.PiercingShot(),
                Expedition33Moveset.Escapade(),
                Expedition33Moveset.Rest()
            };
        }
    }
}
