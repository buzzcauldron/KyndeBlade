using System;
using UnityEngine;

namespace KyndeBlade
{
    public enum BlessingTier { Common, Rare, Legendary }
    public enum BlessingCategory { Virtue, Sin, Pilgrimage }

    /// <summary>
    /// A roguelike buff picked after victory. Virtues are clean bonuses;
    /// Sin Temptations are powerful but carry a drawback.
    /// Blessings persist for the current run only.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBlessing", menuName = "KyndeBlade/Blessing")]
    public class Blessing : ScriptableObject
    {
        public string BlessingId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public BlessingTier Tier = BlessingTier.Common;
        public BlessingCategory Category = BlessingCategory.Virtue;

        [Header("Stat Modifiers (additive)")]
        public float BonusMaxHealth;
        public float BonusMaxStamina;
        public float BonusAttackPower;
        public float BonusDefense;
        public float BonusSpeed;
        public float BonusMaxKynde;

        [Header("Stat Modifiers (multiplicative, 1.0 = no change)")]
        [Tooltip("Multiplier on damage dealt. 1.3 = +30%.")]
        public float DamageMultiplier = 1f;
        [Tooltip("Multiplier on damage taken. 0.85 = -15%.")]
        public float DamageTakenMultiplier = 1f;
        [Tooltip("Multiplier on stamina costs. 0.8 = -20% cost.")]
        public float StaminaCostMultiplier = 1f;
        [Tooltip("Multiplier on Kynde generated per action.")]
        public float KyndeGenerationMultiplier = 1f;
        [Tooltip("Multiplier on dodge/parry window duration.")]
        public float TimingWindowMultiplier = 1f;
        [Tooltip("Multiplier on Break gauge damage dealt.")]
        public float BreakDamageMultiplier = 1f;

        [Header("Special Effects")]
        [Tooltip("HP healed per turn (passive regen).")]
        public float HealPerTurn;
        [Tooltip("Stamina restored per turn (passive regen).")]
        public float StaminaPerTurn;
        [Tooltip("If true, the first hit each combat is nullified.")]
        public bool FirstHitImmunity;
        [Tooltip("If true, cures one stack of Hunger when picked.")]
        public bool CuresHungerOnPickup;

        [Header("Drawback (Sin Temptations)")]
        [TextArea(1, 2)]
        public string DrawbackDescription;
        public float DrawbackMaxHealth;
        public float DrawbackDefense;
        public float DrawbackSpeed;

        [Header("Availability")]
        [Tooltip("Minimum total party Kynde at victory to be offered.")]
        public float MinKyndeToOffer;
        [Tooltip("Required location visit to unlock (empty = always available).")]
        public string RequiredLocationVisited;
        [Tooltip("Can this blessing stack (be picked multiple times)?")]
        public bool Stackable;
    }

    /// <summary>Lightweight serializable reference to an active blessing on a character.</summary>
    [Serializable]
    public class ActiveBlessing
    {
        public string BlessingId;
        public int StackCount = 1;
    }
}
