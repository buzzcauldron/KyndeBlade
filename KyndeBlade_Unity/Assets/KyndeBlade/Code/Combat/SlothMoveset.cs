using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Sloth moveset: debilitating slowness, stamina drain, self-healing lethargy.</summary>
    public static class SlothMoveset
    {
        public static CombatAction Torpor()
        {
            var a = ScriptableObject.CreateInstance<TorporAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Torpor",
                StaminaCost = 20f
            };
            return a;
        }

        public static CombatAction HeavyYawn()
        {
            var a = ScriptableObject.CreateInstance<HeavyYawnAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Heavy Yawn",
                StaminaCost = 25f
            };
            return a;
        }

        public static CombatAction LethargysTouch()
        {
            var a = ScriptableObject.CreateInstance<LethargysTouchAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Lethargy's Touch",
                Damage = 12f, StaminaCost = 15f, BreakDamage = 5f
            };
            return a;
        }

        public static CombatAction Apathy()
        {
            var a = ScriptableObject.CreateInstance<ApathyAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Apathy",
                StaminaCost = 10f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                Torpor(), HeavyYawn(), LethargysTouch(), Apathy(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>Applies a 3-turn slow (50% speed) to the target.</summary>
    public class TorporAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
                target.ApplyStatusEffect(StatusEffect.CreateSlowEffect(3f, 0.5f));
        }
    }

    /// <summary>AoE: reduces all players' Speed by 30% for 2 turns.</summary>
    public class HeavyYawnAction : CombatAction
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
                    p.ApplyStatusEffect(StatusEffect.CreateSlowEffect(2f, 0.7f));
            }
        }
    }

    /// <summary>Low damage strike that drains 20 stamina from the target.</summary>
    public class LethargysTouchAction : CombatAction
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
                target.ConsumeStamina(20f);
            }
        }
    }

    /// <summary>Heals self for 40% max HP, then stuns self for 1 turn.</summary>
    public class ApathyAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            executor.Heal(executor.Stats.MaxHealth * 0.4f);
            executor.ApplyStatusEffect(StatusEffect.CreateStunEffect(1f));
        }
    }
}
