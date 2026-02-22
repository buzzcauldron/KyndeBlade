using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Pure logic class for combat mathematics.
    /// Decouples damage calculation from the TurnManager state machine.
    /// Single responsibility: combat math only; testable without Unity.
    /// </summary>
    public static class CombatCalculator
    {
        /// <summary>Full damage formula: base power (action or attacker), aging mod, defense mitigation, min 1, 5–10% variance.</summary>
        public static float CalculateDamage(MedievalCharacter attacker, MedievalCharacter target, CombatAction action)
        {
            if (attacker == null || target == null) return 0f;

            float basePower = action != null ? action.ActionData.Damage : attacker.Stats.AttackPower;
            float agingMod = 1.0f; // In a full refactor, AgingManager feeds into character stats.

            float defense = target.Stats.Defense;
            float finalDamage = Mathf.Max(1f, (basePower * agingMod) - (defense * 0.5f));

            finalDamage *= Random.Range(0.95f, 1.05f);
            return finalDamage;
        }

        /// <summary>Kynde gained from performing an action (e.g. Strike 10, others 5). Wisdom/age multipliers can be applied here.</summary>
        public static float CalculateKyndeGain(MedievalCharacter character, CombatActionType actionType)
        {
            float baseGain = actionType == CombatActionType.Strike ? 10f : 5f;
            return baseGain;
        }

        /// <summary>Perfect timing = last 15% of the window (remainingTime / totalDuration &lt;= 0.15).</summary>
        public static bool IsPerfectTiming(float remainingTime, float totalDuration)
        {
            if (totalDuration <= 0f) return false;
            return (remainingTime / totalDuration) <= 0.15f;
        }

        /// <summary>Returns the attacker's first Strike or RangedStrike action for defense-window damage.</summary>
        public static CombatAction GetPrimaryStrikeAction(MedievalCharacter attacker)
        {
            if (attacker == null || attacker.AvailableActions == null) return null;
            foreach (var a in attacker.AvailableActions)
            {
                if (a == null) continue;
                var t = a.ActionData.ActionType;
                if (t == CombatActionType.Strike || t == CombatActionType.RangedStrike)
                    return a;
            }
            return null;
        }

        /// <summary>Damage from the attacker's first Strike/RangedStrike (convenience). Prefer CalculateDamage(attacker, target, GetPrimaryStrikeAction(attacker)) for full formula.</summary>
        public static float ComputePrimaryStrikeDamage(MedievalCharacter attacker)
        {
            if (attacker == null) return 0f;
            var action = GetPrimaryStrikeAction(attacker);
            return action != null ? action.ActionData.Damage : attacker.Stats.AttackPower;
        }

        /// <summary>Apply damage from defense-window resolution using full formula; defender receives precomputed final damage (no double defense).</summary>
        public static void ApplyDefenseWindowDamage(MedievalCharacter defender, MedievalCharacter attacker)
        {
            if (defender == null || !defender.IsAlive() || attacker == null) return;
            var strikeAction = GetPrimaryStrikeAction(attacker);
            float damage = CalculateDamage(attacker, defender, strikeAction);
            defender.ApplyCustomDamage(damage, attacker, damageAlreadyFinal: true);
        }
    }
}
