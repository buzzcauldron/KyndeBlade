using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Elde boss moveset: aging strikes. Each hit applies age to the target.</summary>
    public static class EldeMoveset
    {
        public static CombatAction EldeStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Elde's Touch",
                Damage = 18f,
                StaminaCost = 20f,
                BreakDamage = 12f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static CombatAction HeavyAge()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Years' Weight",
                Damage = 28f,
                StaminaCost = 35f,
                BreakDamage = 18f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                EldeStrike(),
                HeavyAge(),
                Expedition33Moveset.Rest()
            };
        }
    }
}
