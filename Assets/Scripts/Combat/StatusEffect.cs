using System;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    public enum StatusEffectType
    {
        None,
        Age,
        Hunger,
        HungerScar,
        Frost,
        Burning,
        Poison,
        Stun,
        Slow,
        Weak,
        Vulnerable,
        Blessed,
        KyndeBoost
    }

    [Serializable]
    public class StatusEffectData
    {
        public StatusEffectType EffectType;
        public float Duration;
        public float RemainingTime;
        public int StackCount = 1;
        public float DamagePerSecond;
        public float AttackPowerModifier = 1f;
        public float DefenseModifier = 1f;
        public float SpeedModifier = 1f;
        public float StaminaRegenModifier = 1f;
        public float KyndeGenerationModifier = 1f;
        public string EffectName;
    }

    public class StatusEffect
    {
        public StatusEffectData Data;

        public StatusEffect()
        {
            Data = new StatusEffectData();
        }

        public void UpdateEffect(float deltaTime)
        {
            if (Data.Duration > 0f)
            {
                Data.RemainingTime -= deltaTime;
                Data.RemainingTime = Mathf.Max(0, Data.RemainingTime);
            }
        }

        public bool IsExpired()
        {
            if (Data.Duration <= 0f) return false;
            return Data.RemainingTime <= 0f;
        }

        public void ApplyEffect(MedievalCharacter target)
        {
            if (target == null) return;
            ApplyStatModifiers(target, true);
        }

        public void RemoveEffect(MedievalCharacter target)
        {
            if (target == null) return;
            ApplyStatModifiers(target, false);
        }

        public void StackEffect(int additionalStacks)
        {
            Data.StackCount += additionalStacks;
            if (Data.EffectType == StatusEffectType.Hunger)
            {
                Data.AttackPowerModifier = Mathf.Max(0.1f, 1f - Data.StackCount * 0.12f);
                Data.DefenseModifier = Mathf.Max(0.1f, 1f - Data.StackCount * 0.12f);
                Data.SpeedModifier = Mathf.Max(0.1f, 1f - Data.StackCount * 0.12f);
                Data.StaminaRegenModifier = Mathf.Max(0f, 1f - Data.StackCount * 0.2f);
                Data.DamagePerSecond = 1f * Data.StackCount;
                Data.KyndeGenerationModifier = Mathf.Max(0f, 1f - Data.StackCount * 0.25f);
            }
        }

        public void ApplyDamageOverTime(MedievalCharacter target, float deltaTime)
        {
            if (target == null || Data.DamagePerSecond <= 0f) return;
            float damage = Data.DamagePerSecond * deltaTime;
            target.ApplyCustomDamage(damage, null);
        }

        void ApplyStatModifiers(MedievalCharacter target, bool apply)
        {
            if (target == null) return;
            if (apply)
            {
                target.Stats.AttackPower *= Data.AttackPowerModifier;
                target.Stats.Defense *= Data.DefenseModifier;
                target.Stats.Speed *= Data.SpeedModifier;
            }
            else
            {
                if (Data.AttackPowerModifier > 0f) target.Stats.AttackPower /= Data.AttackPowerModifier;
                if (Data.DefenseModifier > 0f) target.Stats.Defense /= Data.DefenseModifier;
                if (Data.SpeedModifier > 0f) target.Stats.Speed /= Data.SpeedModifier;
            }
        }

        /// <summary>Permanent age effect. Duration=0 = never expires. Slower but wiser.</summary>
        public static StatusEffect CreateAgeEffect(int tier, float speedMult, float defenseMult, float kyndeMult, float staminaMult)
        {
            if (tier <= 0) return null;
            var effect = new StatusEffect
            {
                Data = new StatusEffectData
                {
                    EffectType = StatusEffectType.Age,
                    Duration = 0f,
                    RemainingTime = 0f,
                    StackCount = tier,
                    EffectName = "Age",
                    SpeedModifier = speedMult,
                    DefenseModifier = defenseMult,
                    KyndeGenerationModifier = kyndeMult,
                    StaminaRegenModifier = staminaMult
                }
            };
            return effect;
        }

        /// <summary>Create age effect with default modifiers (0.97 speed, +10% defense/tier, 1.02 kynde, 0.98 stamina per tier).</summary>
        public static StatusEffect CreateAgeEffect(int tier)
        {
            if (tier <= 0) return null;
            return CreateAgeEffect(tier, Mathf.Pow(0.97f, tier), 1f + tier * 0.1f, Mathf.Pow(1.02f, tier), Mathf.Pow(0.98f, tier));
        }

        public static StatusEffect CreateHungerEffect(float duration = 0f, int stacks = 1)
        {
            var effect = new StatusEffect
            {
                Data = new StatusEffectData
                {
                    EffectType = StatusEffectType.Hunger,
                    Duration = duration,
                    RemainingTime = duration,
                    StackCount = stacks,
                    EffectName = "Hunger",
                    AttackPowerModifier = Mathf.Max(0.1f, 1f - stacks * 0.12f),
                    DefenseModifier = Mathf.Max(0.1f, 1f - stacks * 0.12f),
                    SpeedModifier = Mathf.Max(0.1f, 1f - stacks * 0.12f),
                    StaminaRegenModifier = Mathf.Max(0f, 1f - stacks * 0.2f),
                    DamagePerSecond = 1f * stacks,
                    KyndeGenerationModifier = Mathf.Max(0f, 1f - stacks * 0.25f)
                }
            };
            return effect;
        }

        /// <summary>Permanent scar from having ever had hunger. Gaunt, slightly weakened—never fully recovers.</summary>
        public static StatusEffect CreateHungerScarEffect()
        {
            return new StatusEffect
            {
                Data = new StatusEffectData
                {
                    EffectType = StatusEffectType.HungerScar,
                    Duration = 0f,
                    RemainingTime = 0f,
                    StackCount = 1,
                    EffectName = "Hunger Scar",
                    AttackPowerModifier = 0.95f,
                    DefenseModifier = 0.93f,
                    SpeedModifier = 0.94f,
                    StaminaRegenModifier = 0.96f,
                    KyndeGenerationModifier = 1.03f
                }
            };
        }
    }
}
