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
                    if (target != null)
                    {
                        float finalDamage = ActionData.Damage;
                        if (target.IsBroken()) finalDamage *= 1.5f;
                        target.ApplyCustomDamage(finalDamage, executor);
                        if (ActionData.BreakDamage > 0f)
                            target.TakeBreakDamage(ActionData.BreakDamage);
                    }
                    break;
                case CombatActionType.Counter:
                    if (target != null)
                    {
                        float counterDmg = ActionData.Damage * 1.5f;
                        if (target.IsBroken()) counterDmg *= 1.5f;
                        target.ApplyCustomDamage(counterDmg, executor);
                        if (ActionData.BreakDamage > 0f)
                            target.TakeBreakDamage(ActionData.BreakDamage);
                    }
                    break;
                case CombatActionType.RangedStrike:
                    if (target != null)
                    {
                        float rangedDmg = ActionData.Damage;
                        if (target.IsBroken()) rangedDmg *= 1.5f;
                        target.ApplyCustomDamage(rangedDmg, executor);
                        if (ActionData.BreakDamage > 0f)
                            target.TakeBreakDamage(ActionData.BreakDamage);
                    }
                    break;
                case CombatActionType.Heal:
                    var healTarget = target != null ? target : executor;
                    if (ActionData.Damage > 0f) healTarget.Heal(ActionData.Damage);
                    break;
                case CombatActionType.Rest:
                    executor.RestoreStamina(20f);
                    break;
            }

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
