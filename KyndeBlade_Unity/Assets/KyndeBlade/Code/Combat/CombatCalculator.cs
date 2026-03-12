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
        /// <summary>Full damage formula: base power, aging mod, blessing mods, defense, variance.</summary>
        public static float CalculateDamage(MedievalCharacter attacker, MedievalCharacter target, CombatAction action)
        {
            if (attacker == null || target == null) return 0f;

            float basePower = action != null ? action.ActionData.Damage : attacker.Stats.AttackPower;
            float agingMod = GetAgingModifier(attacker);

            var atkMods = BlessingSystem.GetModifiers(attacker);
            var defMods = BlessingSystem.GetModifiers(target);

            float defense = target.Stats.Defense;
            float finalDamage = Mathf.Max(1f, (basePower * agingMod * atkMods.DamageMultiplier) - (defense * 0.5f));
            finalDamage *= defMods.DamageTakenMultiplier;

            finalDamage *= Random.Range(0.95f, 1.05f);
            return finalDamage;
        }

        /// <summary>Simplified damage calculation from raw values (used by custom moveset actions).</summary>
        public static float CalculateDamage(float baseDamage, float attackPower, float defense)
        {
            float power = baseDamage + attackPower * 0.3f;
            float finalDamage = Mathf.Max(1f, power - defense * 0.5f);
            finalDamage *= Random.Range(0.95f, 1.05f);
            return finalDamage;
        }

        /// <summary>
        /// Returns the aging modifier for damage output. Each Elde hit reduces
        /// output by 5%, capped at 50% reduction (10 hits).
        /// Returns 1.0f for enemies or when no aging data is available.
        /// </summary>
        public static float GetAgingModifier(MedievalCharacter character)
        {
            if (character == null) return 1f;
            var pad = character.GetComponent<PiersAppearanceData>();
            if (pad == null || !pad.IsPlayer) return 1f;
            var saveManager = Object.FindFirstObjectByType<SaveManager>();
            if (saveManager?.CurrentProgress == null) return 1f;
            int eldeHits = saveManager.CurrentProgress.EldeHitsAccrued;
            return Mathf.Max(0.5f, 1f - eldeHits * 0.05f);
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
