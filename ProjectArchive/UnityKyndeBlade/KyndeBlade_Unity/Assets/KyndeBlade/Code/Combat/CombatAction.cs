using System;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    public enum CombatActionType
    {
        Strike,
        Escapade,
        Ward,
        Counter,
        Special,
        Rest,
        RangedStrike,
        Heal
    }

    public enum KyndeElementType
    {
        None,
        Flamme,
        Frost,
        Thunder,
        Trewthe,
        Fals,
        Kynde
    }

    [Serializable]
    public class CombatActionData
    {
        public CombatActionType ActionType = CombatActionType.Rest;
        public float Damage;
        public float StaminaCost;
        public float SuccessWindow;
        public float CastTime;
        public float ExecutionTime;
        public float KyndeGenerated;
        public float KyndeCost;
        public float BreakDamage;
        public KyndeElementType ElementType;
        public string ActionName = "Kyndes Rest";
    }

    [CreateAssetMenu(fileName = "NewCombatAction", menuName = "KyndeBlade/Combat Action")]
    public class CombatAction : ScriptableObject
    {
        public CombatActionData ActionData = new CombatActionData();

        public virtual void ExecuteAction(MedievalCharacter executor, MedievalCharacter target)
        {
            if (!CanExecute(executor)) return;

            if (ActionData.KyndeCost > 0f && !executor.ConsumeKynde(ActionData.KyndeCost))
                return;

            executor.ConsumeStamina(ActionData.StaminaCost);

            if (ActionData.KyndeGenerated > 0f)
                executor.GainKynde(ActionData.KyndeGenerated);

            switch (ActionData.ActionType)
            {
                case CombatActionType.Strike:
                case CombatActionType.Counter:
                case CombatActionType.RangedStrike:
                    if (target != null)
                    {
                        if (executor.WillDeferHitToAnimation())
                            executor.RegisterPendingHit(this, target);
                        else
                            ApplyHitEffect(executor, target);
                    }
                    break;
                case CombatActionType.Heal:
                    var healTarget = target != null ? target : executor;
                    if (ActionData.Damage > 0f) healTarget.Heal(ActionData.Damage);
                    healTarget.RemoveHungerStack();
                    break;
                case CombatActionType.Rest:
                    executor.RestoreStamina(20f);
                    executor.RemoveHungerStack();
                    break;
            }

            // For deferred hit (Strike/Counter/RangedStrike), OnActionExecuted is called from OnAnimationHit
            if (ActionData.ActionType != CombatActionType.Strike && ActionData.ActionType != CombatActionType.Counter && ActionData.ActionType != CombatActionType.RangedStrike)
                OnActionExecuted(executor, target);
            else if (!executor.WillDeferHitToAnimation())
                OnActionExecuted(executor, target);
        }

        /// <summary>Apply damage and break for this action. Called immediately or from Animation Event (OnAnimationHit). Override in subclasses for custom hit effects.</summary>
        public virtual void ApplyHitEffect(MedievalCharacter executor, MedievalCharacter target)
        {
            if (target == null) return;
            float dmg = CombatCalculator.CalculateDamage(executor, target, this);
            if (ActionData.ActionType == CombatActionType.Counter) dmg *= 1.5f;
            if (target.IsBroken()) dmg *= GameWorldConstants.BrokenDamageMultiplier;
            var tm = GameRuntime.TurnManager;
            if (tm != null) dmg = tm.ApplyCritToDamage(dmg, out _);
            target.ApplyCustomDamage(dmg, executor, damageAlreadyFinal: true);
            if (ActionData.BreakDamage > 0f) target.TakeBreakDamage(ActionData.BreakDamage);
        }

        /// <summary>Called after hit is applied (e.g. from animation event). Fires OnActionExecuted for observers.</summary>
        public void NotifyHitExecuted(MedievalCharacter executor, MedievalCharacter target)
        {
            OnActionExecuted(executor, target);
        }

        protected virtual void OnActionExecuted(MedievalCharacter executor, MedievalCharacter target) { }

        protected virtual bool CanExecute(MedievalCharacter executor)
        {
            if (executor == null) return false;
            if (executor.GetCurrentStamina() < ActionData.StaminaCost) return false;
            if (ActionData.KyndeCost > 0f && executor.GetCurrentKynde() < ActionData.KyndeCost) return false;
            return true;
        }

        public float GetCastTime() => ActionData.CastTime;
        public float GetExecutionTime() => ActionData.ExecutionTime;
    }
}
