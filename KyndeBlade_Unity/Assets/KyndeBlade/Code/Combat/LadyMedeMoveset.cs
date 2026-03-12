using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Lady Mede moveset: corruption, Kynde drain, debuffs.</summary>
    public static class LadyMedeMoveset
    {
        public static CombatAction GoldenTouch()
        {
            var a = ScriptableObject.CreateInstance<GoldenTouchAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Golden Touch",
                Damage = 18f, StaminaCost = 22f
            };
            return a;
        }

        public static CombatAction Bribe()
        {
            var a = ScriptableObject.CreateInstance<BribeAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Bribe",
                StaminaCost = 30f
            };
            return a;
        }

        public static CombatAction CorruptionsEmbrace()
        {
            var a = ScriptableObject.CreateInstance<CorruptionsEmbraceAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Corruption's Embrace",
                StaminaCost = 35f
            };
            return a;
        }

        public static CombatAction MedesDemand()
        {
            var a = ScriptableObject.CreateInstance<MedesDemandAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Mede's Demand",
                Damage = 15f, StaminaCost = 28f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                GoldenTouch(), Bribe(), CorruptionsEmbrace(), MedesDemand(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>Strike that drains target's Kynde.</summary>
    public class GoldenTouchAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target == null) return;
            float damage = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, target.Stats.Defense);
            target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
            float drain = Mathf.Min(target.GetCurrentKynde(), 2f);
            target.ConsumeKynde(drain);
            executor.GainKynde(drain);
        }
    }

    /// <summary>Drains target's stamina heavily, simulating a "bribe" that saps their will to act.</summary>
    public class BribeAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null && target.IsAlive())
                target.ConsumeStamina(40f);
        }
    }

    /// <summary>AoE debuff: reduces all player defense by 30%.</summary>
    public class CorruptionsEmbraceAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;
            foreach (var p in tm.PlayerCharacters)
            {
                if (p != null && p.IsAlive())
                    p.Stats.Defense *= 0.7f;
            }
        }
    }

    /// <summary>Damage scales with target's current Kynde.</summary>
    public class MedesDemandAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target == null) return;
            float bonus = target.GetCurrentKynde() * 4f;
            float damage = CombatCalculator.CalculateDamage(ActionData.Damage + bonus, executor.Stats.AttackPower, target.Stats.Defense);
            target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
        }
    }
}
