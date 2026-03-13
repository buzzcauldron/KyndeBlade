using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Frontline pressure archetype: durable melee threat with break pressure.</summary>
    public class MeleePressureEnemy : MedievalCharacter
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
                SetIdentity(CharacterClass.Knight, "Warder", CharacterSoundTheme.Wrath);
                SetStats(145f, 110f, 18f, 8f, 8f);
            }

            AvailableActions = new List<CombatAction>
            {
                Expedition33Moveset.MeleeStrike(),
                Expedition33Moveset.HeavyStrike(),
                Expedition33Moveset.BreakStrike(),
                Expedition33Moveset.Ward(),
                Expedition33Moveset.Rest()
            };
        }
    }
}
