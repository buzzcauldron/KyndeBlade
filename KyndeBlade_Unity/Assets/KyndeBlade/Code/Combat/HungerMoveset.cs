using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Hunger boss moveset: Hunger's Grip, Empty Belly, Feast of Want, Strike.</summary>
    public static class HungerMoveset
    {
        public static CombatAction HungerGrip()
        {
            var a = ScriptableObject.CreateInstance<HungerGripAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "Hunger's Grip", StaminaCost = 25f };
            return a;
        }

        public static CombatAction EmptyBelly()
        {
            var a = ScriptableObject.CreateInstance<EmptyBellyAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "The Empty Belly", StaminaCost = 30f };
            return a;
        }

        public static CombatAction FeastOfWant()
        {
            var a = ScriptableObject.CreateInstance<FeastOfWantAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Heal, ActionName = "The Feast of Want", Damage = 30f, StaminaCost = 40f };
            return a;
        }

        public static CombatAction UnendingNeed()
        {
            var a = ScriptableObject.CreateInstance<UnendingNeedAction>();
            a.ActionData = new CombatActionData { ActionType = CombatActionType.Special, ActionName = "The Unending Need", StaminaCost = 35f };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                HungerGrip(),
                EmptyBelly(),
                FeastOfWant(),
                UnendingNeed(),
                Expedition33Moveset.MeleeStrike(),
                Expedition33Moveset.Rest()
            };
        }
    }

    public class HungerGripAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm != null)
            {
                foreach (var p in tm.PlayerCharacters)
                {
                    if (p != null && p.IsAlive())
                        p.ApplyHunger(1, 12f);
                }
            }
        }
    }

    public class EmptyBellyAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm != null)
            {
                foreach (var p in tm.PlayerCharacters)
                {
                    if (p == null || !p.IsAlive()) continue;
                    p.ConsumeStamina(20f);
                    if (p.GetCurrentKynde() >= 2f)
                        p.ConsumeKynde(2f);
                    p.ApplyHunger(1, 10f);
                }
            }
        }
    }

    public class FeastOfWantAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            int hungryCount = 0;
            if (tm != null)
            {
                foreach (var p in tm.PlayerCharacters)
                    if (p != null && p.IsAlive() && p.IsHungry()) hungryCount++;
            }

            float heal = ActionData.Damage + hungryCount * 15f;
            executor.Heal(heal);
        }
    }

    public class UnendingNeedAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);

            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm != null)
            {
                foreach (var p in tm.PlayerCharacters)
                {
                    if (p != null && p.IsAlive())
                        p.ApplyHunger(2, 10f);
                }
            }
        }
    }
}
