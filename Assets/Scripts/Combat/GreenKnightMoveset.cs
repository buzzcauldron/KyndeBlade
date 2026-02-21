using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Green Knight moveset: Beheading Blow, Wild Nature's Wrath, The Green Chapel's Curse.</summary>
    public static class GreenKnightMoveset
    {
        public static CombatAction BeheadingBlow()
        {
            var a = ScriptableObject.CreateInstance<BeheadingBlowAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Strike, ActionName = "Beheading Blow", Damage = 35f, StaminaCost = 35f, BreakDamage = 20f };
            return a;
        }

        public static CombatAction WildNaturesWrath()
        {
            var a = ScriptableObject.CreateInstance<WildNaturesWrathAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "Wild Nature's Wrath", Damage = 22f, StaminaCost = 45f };
            return a;
        }

        public static CombatAction GreenChapelsCurse()
        {
            var a = ScriptableObject.CreateInstance<GreenChapelsCurseAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "The Green Chapel's Curse", StaminaCost = 50f };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                BeheadingBlow(),
                WildNaturesWrath(),
                GreenChapelsCurse(),
                Expedition33Moveset.MeleeStrike(),
                Expedition33Moveset.Rest()
            };
        }
    }

    public class BeheadingBlowAction : CombatAction
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

    public class WildNaturesWrathAction : CombatAction
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

    public class GreenChapelsCurseAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            executor.Heal(60f);
            executor.Stats.Defense += 6f;
        }
    }
}
