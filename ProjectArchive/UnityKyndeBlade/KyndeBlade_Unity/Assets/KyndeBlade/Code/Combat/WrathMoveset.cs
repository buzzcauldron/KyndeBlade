using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Wrath moveset: berserker rage, storm attacks, fury mechanics.</summary>
    public static class WrathMoveset
    {
        public static CombatAction BlindFury()
        {
            var a = ScriptableObject.CreateInstance<BlindFuryAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Blind Fury",
                Damage = 28f, StaminaCost = 25f, BreakDamage = 15f
            };
            return a;
        }

        public static CombatAction BurningRage()
        {
            var a = ScriptableObject.CreateInstance<BurningRageAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Burning Rage",
                StaminaCost = 30f
            };
            return a;
        }

        public static CombatAction WrathStorm()
        {
            var a = ScriptableObject.CreateInstance<WrathStormAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Wrath's Storm",
                Damage = 14f, StaminaCost = 35f
            };
            return a;
        }

        public static CombatAction UnquenchableIre()
        {
            var a = ScriptableObject.CreateInstance<UnquenchableIreAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Counter, ActionName = "Unquenchable Ire",
                Damage = 22f, StaminaCost = 20f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                BlindFury(), BurningRage(), WrathStorm(), UnquenchableIre(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>High damage strike that ignores parry reduction.</summary>
    public class BlindFuryAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                float damage = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, target.Stats.Defense);
                target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
                target.TakeBreakDamage(ActionData.BreakDamage);
            }
        }
    }

    /// <summary>Self-buff: +50% ATK for the encounter but costs 15% max HP.</summary>
    public class BurningRageAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            float hpCost = executor.Stats.MaxHealth * 0.15f;
            executor.ApplyCustomDamage(hpCost, executor, damageAlreadyFinal: true);
            executor.Stats.AttackPower *= 1.5f;
        }
    }

    /// <summary>AoE: hits all players for moderate damage.</summary>
    public class WrathStormAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;
            float damage = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, 0f);
            foreach (var p in tm.PlayerCharacters)
            {
                if (p != null && p.IsAlive())
                    p.ApplyCustomDamage(damage * 0.7f, executor, damageAlreadyFinal: true);
            }
        }
    }

    /// <summary>Counter: immediate counterattack when Wrath is parried.</summary>
    public class UnquenchableIreAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                float damage = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, target.Stats.Defense);
                target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
            }
        }
    }
}
