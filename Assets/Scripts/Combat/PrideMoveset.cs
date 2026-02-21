using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Pride boss moveset: Pride's Arrogance, The Tower, Pride's Fall, Strike.</summary>
    public static class PrideMoveset
    {
        public static CombatAction PrideArrogance()
        {
            var a = ScriptableObject.CreateInstance<PrideArroganceAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Strike, ActionName = "Pride's Arrogance", Damage = 28f, StaminaCost = 30f, BreakDamage = 12f };
            return a;
        }

        public static CombatAction TheTower()
        {
            var a = ScriptableObject.CreateInstance<TheTowerAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "The Tower", StaminaCost = 25f };
            return a;
        }

        public static CombatAction PrideFall()
        {
            var a = ScriptableObject.CreateInstance<PrideFallAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "Pride's Fall", Damage = 18f, StaminaCost = 40f };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                PrideArrogance(),
                TheTower(),
                PrideFall(),
                Expedition33Moveset.MeleeStrike(),
                Expedition33Moveset.Rest()
            };
        }
    }

    public class PrideArroganceAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                target.ApplyCustomDamage(ActionData.Damage, executor);
                if (ActionData.BreakDamage > 0f) target.TakeBreakDamage(ActionData.BreakDamage);
            }
        }
    }

    public class TheTowerAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            executor.Stats.Defense += 8f;
            executor.Heal(25f);
        }
    }

    public class PrideFallAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);

            var tm = Object.FindObjectOfType<TurnManager>();
            if (tm != null)
            {
                foreach (var p in tm.PlayerCharacters)
                {
                    if (p != null && p.IsAlive())
                        p.ApplyCustomDamage(ActionData.Damage, executor);
                }
            }
        }
    }
}
