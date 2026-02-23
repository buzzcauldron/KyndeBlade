using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>
    /// Robust moveset: one curated full kit for player or versatile enemy.
    /// Melee (Strike, Heavy, Break), Ranged, one elemental, Heal, Escapade, Ward, Counter, Rest,
    /// plus one signature high-break move. All damage paths use crit (via base ApplyHitEffect / ExecuteAction).
    /// </summary>
    public static class RobustMoveset
    {
        // ─── Signature: high break, medium damage, crit-enabled ─────────────────
        public static CombatAction SunderingBlow()
        {
            var a = ScriptableObject.CreateInstance<SunderingBlowAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Sundering Blow",
                Damage = 18f,
                StaminaCost = 22f,
                KyndeGenerated = 2f,
                BreakDamage = 28f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        /// <summary>Full robust set: damage variety, defense, sustain, rest.</summary>
        public static List<CombatAction> RobustSet()
        {
            return new List<CombatAction>
            {
                Expedition33Moveset.MeleeStrike(),
                Expedition33Moveset.HeavyStrike(),
                Expedition33Moveset.BreakStrike(),
                SunderingBlow(),
                Expedition33Moveset.RangedStrike(),
                Expedition33Moveset.FlammeBolt(),
                Expedition33Moveset.KyndeHeal(),
                Expedition33Moveset.Escapade(),
                Expedition33Moveset.Ward(),
                Expedition33Moveset.Counter(),
                Expedition33Moveset.Rest()
            };
        }

        /// <summary>Apply the robust moveset to a character (player or enemy).</summary>
        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = RobustSet();
        }
    }

    /// <summary>High break, medium damage; uses standard crit path.</summary>
    public class SunderingBlowAction : CombatAction
    {
        public override void ApplyHitEffect(MedievalCharacter executor, MedievalCharacter target)
        {
            if (target == null) return;
            float dmg = CombatCalculator.CalculateDamage(executor, target, this);
            if (target.IsBroken()) dmg *= GameWorldConstants.BrokenDamageMultiplier;
            var tm = GameRuntime.TurnManager;
            if (tm != null) dmg = tm.ApplyCritToDamage(dmg, out _);
            target.ApplyCustomDamage(dmg, executor, damageAlreadyFinal: true);
            if (ActionData.BreakDamage > 0f) target.TakeBreakDamage(ActionData.BreakDamage);
        }
    }
}
