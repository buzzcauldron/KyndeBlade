using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>False moveset: deception, evasion, poison tricks.</summary>
    public static class FalseMoveset
    {
        public static CombatAction HollowStrike()
        {
            var a = ScriptableObject.CreateInstance<HollowStrikeAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike, ActionName = "Hollow Strike",
                Damage = 16f, StaminaCost = 18f
            };
            return a;
        }

        public static CombatAction MirrorImage()
        {
            var a = ScriptableObject.CreateInstance<MirrorImageAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Mirror Image",
                StaminaCost = 25f
            };
            return a;
        }

        public static CombatAction FalsePromise()
        {
            var a = ScriptableObject.CreateInstance<FalsePromiseAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Heal, ActionName = "False Promise",
                Damage = 25f, StaminaCost = 30f
            };
            return a;
        }

        public static CombatAction BetrayersKiss()
        {
            var a = ScriptableObject.CreateInstance<BetrayersKissAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Special, ActionName = "Betrayer's Kiss",
                Damage = 20f, StaminaCost = 35f, KyndeCost = 3f
            };
            return a;
        }

        public static void ApplyToCharacter(MedievalCharacter c)
        {
            if (c == null) return;
            c.AvailableActions = new List<CombatAction>
            {
                HollowStrike(), MirrorImage(), FalsePromise(), BetrayersKiss(),
                Expedition33Moveset.MeleeStrike(), Expedition33Moveset.Rest()
            };
        }
    }

    /// <summary>Deceptive strike: 30% chance to deal double damage.</summary>
    public class HollowStrikeAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target == null) return;
            float mult = Random.value < 0.3f ? 2f : 1f;
            float damage = CombatCalculator.CalculateDamage(ActionData.Damage * mult, executor.Stats.AttackPower, target.Stats.Defense);
            target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
        }
    }

    /// <summary>Boosts defense temporarily by increasing evasion (reduces damage taken by 50% next hit).</summary>
    public class MirrorImageAction : CombatAction
    {
        float _storedBonus;
        MedievalCharacter _buffTarget;
        System.Action<MedievalCharacter> _revertHandler;

        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            _storedBonus = executor.Stats.Defense * 0.5f;
            executor.Stats.Defense += _storedBonus;
            _buffTarget = executor;
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm != null)
            {
                _revertHandler = OnTurnChanged;
                tm.OnTurnChanged += _revertHandler;
            }
        }

        void OnTurnChanged(MedievalCharacter next)
        {
            if (_buffTarget == null || _storedBonus <= 0f) return;
            _buffTarget.Stats.Defense -= _storedBonus;
            _storedBonus = 0f;
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm != null && _revertHandler != null)
                tm.OnTurnChanged -= _revertHandler;
            _revertHandler = null;
        }
    }

    /// <summary>Heals self and applies hunger to a random player.</summary>
    public class FalsePromiseAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            executor.Heal(ActionData.Damage);
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;
            var alive = new List<MedievalCharacter>();
            foreach (var p in tm.PlayerCharacters)
                if (p != null && p.IsAlive()) alive.Add(p);
            if (alive.Count > 0)
                alive[Random.Range(0, alive.Count)].ApplyHunger(1, 8f);
        }
    }

    /// <summary>Drains Kynde from target and deals bonus damage based on Kynde stolen.</summary>
    public class BetrayersKissAction : CombatAction
    {
        public override void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;
            executor.ConsumeStamina(ActionData.StaminaCost);
            if (target == null) return;
            float stolen = Mathf.Min(target.GetCurrentKynde(), 4f);
            target.ConsumeKynde(stolen);
            executor.GainKynde(stolen);
            float damage = ActionData.Damage + stolen * 3f;
            target.ApplyCustomDamage(damage, executor, damageAlreadyFinal: true);
        }
    }
}
