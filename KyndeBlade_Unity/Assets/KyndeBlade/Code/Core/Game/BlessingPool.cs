using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// All available blessings for the game. Used by BlessingSystem to generate
    /// post-victory choices. Place at Resources/BlessingPool.
    /// </summary>
    [CreateAssetMenu(fileName = "BlessingPool", menuName = "KyndeBlade/Blessing Pool")]
    public class BlessingPool : ScriptableObject
    {
        public Blessing[] AllBlessings = new Blessing[0];

        /// <summary>
        /// Generate the default pool procedurally. Called by BlessingSystem
        /// when no asset is assigned, so the game always has blessings.
        /// </summary>
        public static List<Blessing> CreateDefaultBlessings()
        {
            var list = new List<Blessing>();

            // --- Virtue Blessings (clean buffs) ---

            list.Add(Make("patience", "Patience", BlessingCategory.Virtue, BlessingTier.Common,
                "Dodge and parry windows are wider.",
                timingWindow: 1.2f));

            list.Add(Make("charity", "Charity", BlessingCategory.Virtue, BlessingTier.Common,
                "Heal 3 HP each turn.",
                healPerTurn: 3f));

            list.Add(Make("temperance", "Temperance", BlessingCategory.Virtue, BlessingTier.Common,
                "Stamina costs reduced by 15%.",
                staminaCost: 0.85f));

            list.Add(Make("humility", "Humility", BlessingCategory.Virtue, BlessingTier.Common,
                "+3 Defense.",
                bonusDef: 3f));

            list.Add(Make("diligence", "Diligence", BlessingCategory.Virtue, BlessingTier.Common,
                "+2 Speed.",
                bonusSpeed: 2f));

            list.Add(Make("chastity", "Chastity", BlessingCategory.Virtue, BlessingTier.Rare,
                "Break damage increased by 25%.",
                breakMult: 1.25f));

            list.Add(Make("kindness", "Kindness", BlessingCategory.Virtue, BlessingTier.Rare,
                "Kynde generation doubled.",
                kyndeGen: 2f, minKynde: 5f));

            list.Add(Make("fortitude", "Fortitude", BlessingCategory.Virtue, BlessingTier.Common,
                "+20 max HP.",
                bonusHP: 20f));

            list.Add(Make("prudence", "Prudence", BlessingCategory.Virtue, BlessingTier.Rare,
                "+15 max Stamina and +1 Speed.",
                bonusStam: 15f, bonusSpeed: 1f, minKynde: 3f));

            list.Add(Make("trewthe", "Trewthe", BlessingCategory.Virtue, BlessingTier.Legendary,
                "All damage dealt +20%. Kynde gen +50%.",
                damageMult: 1.2f, kyndeGen: 1.5f, minKynde: 8f));

            // --- Sin Temptations (powerful with drawback) ---

            list.Add(MakeSin("wraths_edge", "Wrath's Edge",  BlessingTier.Common,
                "+25% damage dealt.", "−4 Defense. Take 10% more damage.",
                damageMult: 1.25f, drawbackDef: -4f, minKynde: 0f));

            list.Add(MakeSin("greeds_grasp", "Greed's Grasp", BlessingTier.Common,
                "Kynde generation doubled.", "−20 max HP.",
                kyndeGen: 2f, drawbackHP: -20f));

            list.Add(MakeSin("prides_crown", "Pride's Crown", BlessingTier.Rare,
                "+4 Attack and +3 Defense.", "−3 Speed.",
                bonusAtk: 4f, bonusDef: 3f, drawbackSpeed: -3f, minKynde: 3f));

            list.Add(MakeSin("sloths_embrace", "Sloth's Embrace", BlessingTier.Common,
                "Rest restores double stamina (+15 per turn).", "−3 Speed.",
                staminaPerTurn: 15f, drawbackSpeed: -3f));

            list.Add(MakeSin("envys_mirror", "Envy's Mirror", BlessingTier.Rare,
                "+5 Attack power.", "−25 max HP.",
                bonusAtk: 5f, drawbackHP: -25f, minKynde: 2f));

            list.Add(MakeSin("gluttonys_feast", "Gluttony's Feast", BlessingTier.Common,
                "+25 max HP.", "Stamina costs +25%.",
                bonusHP: 25f, staminaCost: 1.25f));

            list.Add(MakeSin("lusts_allure", "Lust's Allure", BlessingTier.Rare,
                "+3 Speed, +20% break damage.", "−3 Defense.",
                bonusSpeed: 3f, breakMult: 1.2f, drawbackDef: -3f, minKynde: 4f));

            list.Add(MakeSin("avarice_toll", "Avarice's Toll", BlessingTier.Rare,
                "+35 max HP, +2 Defense.", "−2 Attack, −1 Speed.",
                bonusHP: 35f, bonusDef: 2f, drawbackSpeed: -1f, minKynde: 5f));

            // --- Pilgrimage Blessings (location-gated, unique) ---

            list.Add(MakePilgrimage("malvern_spring", "Malvern Spring",
                "Cure one Hunger scar. +5 max Kynde.",
                bonusKynde: 5f, curesHunger: true,
                requiredLocation: "malvern"));

            list.Add(MakePilgrimage("holy_church_shield", "Holy Church Shield",
                "First hit in each combat is nullified.",
                firstHitImmune: true,
                requiredLocation: "holy_church"));

            list.Add(MakePilgrimage("plowmans_resolve", "Plowman's Resolve",
                "+25 max HP, +10 max Stamina.",
                bonusHP: 25f, bonusStam: 10f,
                requiredLocation: "plowmans_half_acre"));

            return list;
        }

        static Blessing Make(string id, string name, BlessingCategory cat, BlessingTier tier,
            string desc,
            float bonusHP = 0, float bonusStam = 0, float bonusAtk = 0, float bonusDef = 0,
            float bonusSpeed = 0, float bonusKynde = 0,
            float damageMult = 1, float staminaCost = 1, float kyndeGen = 1,
            float timingWindow = 1, float breakMult = 1,
            float healPerTurn = 0, float staminaPerTurn = 0,
            float minKynde = 0)
        {
            var b = ScriptableObject.CreateInstance<Blessing>();
            b.BlessingId = id; b.DisplayName = name; b.Description = desc;
            b.Category = cat; b.Tier = tier;
            b.BonusMaxHealth = bonusHP; b.BonusMaxStamina = bonusStam;
            b.BonusAttackPower = bonusAtk; b.BonusDefense = bonusDef;
            b.BonusSpeed = bonusSpeed; b.BonusMaxKynde = bonusKynde;
            b.DamageMultiplier = damageMult; b.StaminaCostMultiplier = staminaCost;
            b.KyndeGenerationMultiplier = kyndeGen; b.TimingWindowMultiplier = timingWindow;
            b.BreakDamageMultiplier = breakMult;
            b.HealPerTurn = healPerTurn; b.StaminaPerTurn = staminaPerTurn;
            b.DamageTakenMultiplier = 1f; b.MinKyndeToOffer = minKynde;
            return b;
        }

        static Blessing MakeSin(string id, string name, BlessingTier tier,
            string desc, string drawback,
            float bonusHP = 0, float bonusAtk = 0, float bonusDef = 0,
            float bonusSpeed = 0, float breakMult = 1,
            float damageMult = 1, float kyndeGen = 1, float staminaCost = 1,
            float staminaPerTurn = 0,
            float drawbackHP = 0, float drawbackDef = 0, float drawbackSpeed = 0,
            float minKynde = 0)
        {
            var b = Make(id, name, BlessingCategory.Sin, tier, desc,
                bonusHP: bonusHP, bonusAtk: bonusAtk, bonusDef: bonusDef,
                bonusSpeed: bonusSpeed, breakMult: breakMult,
                damageMult: damageMult, kyndeGen: kyndeGen, staminaCost: staminaCost,
                staminaPerTurn: staminaPerTurn, minKynde: minKynde);
            b.DrawbackDescription = drawback;
            b.DrawbackMaxHealth = drawbackHP;
            b.DrawbackDefense = drawbackDef;
            b.DrawbackSpeed = drawbackSpeed;
            return b;
        }

        static Blessing MakePilgrimage(string id, string name, string desc,
            float bonusHP = 0, float bonusStam = 0, float bonusKynde = 0,
            bool curesHunger = false, bool firstHitImmune = false,
            string requiredLocation = "")
        {
            var b = Make(id, name, BlessingCategory.Pilgrimage, BlessingTier.Legendary, desc,
                bonusHP: bonusHP, bonusStam: bonusStam, bonusKynde: bonusKynde);
            b.CuresHungerOnPickup = curesHunger;
            b.FirstHitImmunity = firstHitImmune;
            b.RequiredLocationVisited = requiredLocation;
            return b;
        }
    }
}
