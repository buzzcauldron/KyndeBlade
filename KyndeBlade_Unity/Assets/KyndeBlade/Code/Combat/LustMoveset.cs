using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Lust moveset: charm, life-steal, temptation, escalating AoE.</summary>
    public static class LustMoveset
    {
        public static CombatAction Allure()
        {
            var a = ScriptableObject.CreateInstance<AllureAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Allure",
                StaminaCost = 25f
            };
            return a;
        }

        public static CombatAction SirensKiss()
        {
            var a = ScriptableObject.CreateInstance<SirensKissAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Siren's Kiss",
                Damage = 22f, StaminaCost = 20f, BreakDamage = 8f
            };
            return a;
        }

        public static CombatAction Temptation()
        {
            var a = ScriptableObject.CreateInstance<TemptationAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Temptation",
                StaminaCost = 22f
            };
            return a;
        }

        public static CombatAction PassionsEmbrace()
        {
            var a = ScriptableObject.CreateInstance<PassionsEmbraceAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Passion's Embrace",
                Damage = 10f, StaminaCost = 35f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                Allure(), SirensKiss(), Temptation(), PassionsEmbrace(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>Charms the target (stun for 1 turn) with a 50% chance to deal 10 damage.</summary>
    public class AllureAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                target.ApplyStatusEffect(StatusEffect.CreateStunEffect(1f));
                if (Random.value < 0.5f)
                    target.ApplyCustomDamage(10f, executor, damageAlreadyFinal: true);
            }
        }
    }

    /// <summary>High damage strike that heals self for 30% of damage dealt.</summary>
    public class SirensKissAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                float dmg = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, target.Stats.Defense);
                target.ApplyCustomDamage(dmg, executor, damageAlreadyFinal: true);
                target.TakeBreakDamage(ActionData.BreakDamage);
                executor.Heal(dmg * 0.3f);
            }
        }
    }

    /// <summary>Forces target to skip a turn but restores 20% of target's max HP.</summary>
    public class TemptationAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                target.ApplyStatusEffect(StatusEffect.CreateStunEffect(1f));
                target.Heal(target.Stats.MaxHealth * 0.2f);
            }
        }
    }

    /// <summary>AoE: hits all players with escalating damage per player index.</summary>
    public class PassionsEmbraceAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;
            float baseDmg = CombatCalculator.CalculateDamage(ActionData.Damage, executor.Stats.AttackPower, 0f);
            int idx = 0;
            foreach (var p in tm.PlayerCharacters)
            {
                if (p != null && p.IsAlive())
                {
                    float dmg = baseDmg * (1f + 0.15f * idx);
                    p.ApplyCustomDamage(dmg, executor, damageAlreadyFinal: true);
                }
                idx++;
            }
        }
    }
}
