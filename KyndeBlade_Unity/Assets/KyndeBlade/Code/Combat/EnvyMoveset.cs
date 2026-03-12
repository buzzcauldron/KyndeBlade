using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>Envy moveset: stat theft, blessing stealing, jealousy-scaled damage.</summary>
    public static class EnvyMoveset
    {
        public static CombatAction CovetousGaze()
        {
            var a = ScriptableObject.CreateInstance<CovetousGazeAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Covetous Gaze",
                Damage = 20f, StaminaCost = 20f, BreakDamage = 10f
            };
            return a;
        }

        public static CombatAction GreenEyedTheft()
        {
            var a = ScriptableObject.CreateInstance<GreenEyedTheftAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Green-Eyed Theft",
                StaminaCost = 25f
            };
            return a;
        }

        public static CombatAction JealousSpite()
        {
            var a = ScriptableObject.CreateInstance<JealousSpiteAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Jealous Spite",
                Damage = 15f, StaminaCost = 22f, BreakDamage = 8f
            };
            return a;
        }

        public static CombatAction BitterComparison()
        {
            var a = ScriptableObject.CreateInstance<BitterComparisonAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Bitter Comparison",
                StaminaCost = 30f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                CovetousGaze(), GreenEyedTheft(), JealousSpite(), BitterComparison(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>Strike that copies target's highest stat as a 2-turn ATK self-buff (+20%).</summary>
    public class CovetousGazeAction : CombatAction
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

                float highestStat = Mathf.Max(target.Stats.AttackPower, Mathf.Max(target.Stats.Defense, target.Stats.Speed));
                if (highestStat > 0f)
                {
                    var buff = new StatusEffect
                    {
                        Data = new StatusEffectData
                        {
                            EffectType = StatusEffectType.KyndeBoost,
                            Duration = 2f,
                            RemainingTime = 2f,
                            StackCount = 1,
                            EffectName = "Covetous Gaze",
                            AttackPowerModifier = 1.2f
                        }
                    };
                    executor.ApplyStatusEffect(buff);
                }
            }
        }
    }

    /// <summary>Steals one active blessing from target and temporarily adds it to self.</summary>
    public class GreenEyedTheftAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null && target.Stats.ActiveBlessings != null && target.Stats.ActiveBlessings.Count > 0)
            {
                var stolen = target.Stats.ActiveBlessings[0];
                target.Stats.ActiveBlessings.RemoveAt(0);
                if (executor.Stats.ActiveBlessings == null)
                    executor.Stats.ActiveBlessings = new List<ActiveBlessing>();
                executor.Stats.ActiveBlessings.Add(stolen);
            }
        }
    }

    /// <summary>Damage scales with the difference between target's ATK and Envy's ATK.</summary>
    public class JealousSpiteAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target != null)
            {
                float statDiff = Mathf.Max(0f, target.Stats.AttackPower - executor.Stats.AttackPower);
                float scaledDamage = ActionData.Damage + statDiff;
                float damage = CombatCalculator.CalculateDamage(scaledDamage, executor.Stats.AttackPower, target.Stats.Defense);
                target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
                target.TakeBreakDamage(ActionData.BreakDamage);
            }
        }
    }

    /// <summary>AoE: reduces all players' ATK by 15% for 2 turns via Weak effect.</summary>
    public class BitterComparisonAction : CombatAction
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
                    p.ApplyStatusEffect(StatusEffect.CreateWeakEffect(2f));
            }
        }
    }
}
