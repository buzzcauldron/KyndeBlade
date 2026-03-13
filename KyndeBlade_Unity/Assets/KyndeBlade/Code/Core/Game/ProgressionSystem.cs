using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Roguelike blessing system. After each victory the player picks 1 of 3
    /// blessings. Blessings persist for the current run only.
    /// Higher total party Kynde at victory unlocks rarer choices.
    /// </summary>
    public static class BlessingSystem
    {
        public static event Action<IBlessingCharacter, Blessing> OnBlessingApplied;

        static List<Blessing> _pool;

        static List<Blessing> Pool
        {
            get
            {
                if (_pool == null || _pool.Count == 0)
                {
                    var asset = Resources.Load<BlessingPool>("BlessingPool");
                    _pool = (asset != null && asset.AllBlessings != null && asset.AllBlessings.Length > 0)
                        ? new List<Blessing>(asset.AllBlessings)
                        : BlessingPool.CreateDefaultBlessings();
                }
                return _pool;
            }
        }

        /// <summary>
        /// Generate 3 blessing choices weighted by total party Kynde and save state.
        /// Higher Kynde = better chance of Rare/Legendary. Sin Temptations always possible.
        /// </summary>
        public static List<Blessing> GenerateChoices(float totalPartyKynde, SaveManager save, float tierBonus = 0f)
        {
            var eligible = new List<Blessing>();
            foreach (var b in Pool)
            {
                if (b == null) continue;
                if (totalPartyKynde < b.MinKyndeToOffer) continue;
                if (!string.IsNullOrEmpty(b.RequiredLocationVisited) &&
                    (save == null || !save.HasVisited(b.RequiredLocationVisited)))
                    continue;
                eligible.Add(b);
            }

            if (eligible.Count == 0 && Pool.Count > 0)
                eligible.AddRange(Pool.GetRange(0, Mathf.Min(3, Pool.Count)));

            if (eligible.Count == 0)
                return new List<Blessing>();

            var choices = new List<Blessing>();
            var used = new HashSet<string>();
            int attempts = 0;
            float rareBias = Mathf.Clamp01((totalPartyKynde + tierBonus) / 15f);

            while (choices.Count < 3 && attempts < 100)
            {
                attempts++;
                var pick = eligible[UnityEngine.Random.Range(0, eligible.Count)];
                if (used.Contains(pick.BlessingId)) continue;

                float accept = pick.Tier switch
                {
                    BlessingTier.Legendary => rareBias * 0.4f,
                    BlessingTier.Rare => 0.3f + rareBias * 0.3f,
                    _ => 1f
                };
                if (UnityEngine.Random.value > accept) continue;

                choices.Add(pick);
                used.Add(pick.BlessingId);
            }

            while (choices.Count < 3 && eligible.Count > 0)
            {
                var pick = eligible[UnityEngine.Random.Range(0, eligible.Count)];
                if (!used.Contains(pick.BlessingId))
                {
                    choices.Add(pick);
                    used.Add(pick.BlessingId);
                }
                eligible.Remove(pick);
            }

            return choices;
        }

        /// <summary>Apply a chosen blessing to a character and persist it.</summary>
        public static void ApplyBlessing(IBlessingCharacter c, Blessing b, SaveManager save)
        {
            if (c == null || b == null) return;
            c.SetBlessingMaxHealth(c.GetBlessingMaxHealth() + b.BonusMaxHealth + b.DrawbackMaxHealth);
            c.SetBlessingMaxStamina(c.GetBlessingMaxStamina() + b.BonusMaxStamina);
            c.SetBlessingAttackPower(c.GetBlessingAttackPower() + b.BonusAttackPower);
            c.SetBlessingDefense(c.GetBlessingDefense() + b.BonusDefense + b.DrawbackDefense);
            c.SetBlessingSpeed(c.GetBlessingSpeed() + b.BonusSpeed + b.DrawbackSpeed);
            c.SetBlessingMaxKynde(c.GetBlessingMaxKynde() + b.BonusMaxKynde);

            c.SetBlessingCurrentHealth(c.GetBlessingMaxHealth());
            c.SetBlessingCurrentStamina(Mathf.Min(c.GetBlessingCurrentStamina(), c.GetBlessingMaxStamina()));

            var activeBlessings = c.GetActiveBlessings();
            if (activeBlessings == null) return;
            var existing = activeBlessings.Find(ab => ab.BlessingId == b.BlessingId);
            if (existing != null && b.Stackable)
                existing.StackCount++;
            else if (existing == null)
                activeBlessings.Add(new ActiveBlessing { BlessingId = b.BlessingId, StackCount = 1 });

            if (b.CuresHungerOnPickup)
                c.RemoveHungerStack();

            if (save?.CurrentProgress != null)
            {
                var entry = save.CurrentProgress.GetOrCreateCharacterProgress(c.GetBlessingCharacterName());
                entry.Blessings = new List<ActiveBlessing>(activeBlessings);
            }

            OnBlessingApplied?.Invoke(c, b);
        }

        /// <summary>Compute aggregate multipliers across all active blessings.</summary>
        public static BlessingModifiers GetModifiers(IBlessingCharacter c)
        {
            var mods = BlessingModifiers.Default();
            var activeBlessings = c?.GetActiveBlessings();
            if (activeBlessings == null) return mods;
            foreach (var ab in activeBlessings)
            {
                var b = FindBlessing(ab.BlessingId);
                if (b == null) continue;
                int stacks = ab.StackCount;
                mods.DamageMultiplier *= Mathf.Pow(b.DamageMultiplier, stacks);
                mods.DamageTakenMultiplier *= Mathf.Pow(b.DamageTakenMultiplier, stacks);
                mods.StaminaCostMultiplier *= Mathf.Pow(b.StaminaCostMultiplier, stacks);
                mods.KyndeGenerationMultiplier *= Mathf.Pow(b.KyndeGenerationMultiplier, stacks);
                mods.TimingWindowMultiplier *= Mathf.Pow(b.TimingWindowMultiplier, stacks);
                mods.BreakDamageMultiplier *= Mathf.Pow(b.BreakDamageMultiplier, stacks);
                mods.HealPerTurn += b.HealPerTurn * stacks;
                mods.StaminaPerTurn += b.StaminaPerTurn * stacks;
                mods.FirstHitImmunity |= b.FirstHitImmunity;
            }
            return mods;
        }

        public static Blessing FindBlessing(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var b in Pool)
                if (b != null && b.BlessingId == id) return b;
            return null;
        }

        /// <summary>Restore blessings from saved data onto a character's stats.</summary>
        public static void RestoreFromSave(IBlessingCharacter c, CharacterProgressEntry entry)
        {
            if (c == null || entry == null) return;
            var activeBlessings = c.GetActiveBlessings();
            if (activeBlessings == null) return;
            activeBlessings.Clear();
            c.SetBlessingCount(0);
            if (entry.Blessings == null || entry.Blessings.Count == 0) return;
            foreach (var ab in entry.Blessings)
            {
                if (ab == null) continue;
                var b = FindBlessing(ab.BlessingId);
                if (b == null) continue;
                for (int i = 0; i < ab.StackCount; i++)
                {
                    c.SetBlessingMaxHealth(c.GetBlessingMaxHealth() + b.BonusMaxHealth + b.DrawbackMaxHealth);
                    c.SetBlessingMaxStamina(c.GetBlessingMaxStamina() + b.BonusMaxStamina);
                    c.SetBlessingAttackPower(c.GetBlessingAttackPower() + b.BonusAttackPower);
                    c.SetBlessingDefense(c.GetBlessingDefense() + b.BonusDefense + b.DrawbackDefense);
                    c.SetBlessingSpeed(c.GetBlessingSpeed() + b.BonusSpeed + b.DrawbackSpeed);
                    c.SetBlessingMaxKynde(c.GetBlessingMaxKynde() + b.BonusMaxKynde);
                }
                activeBlessings.Add(new ActiveBlessing
                    { BlessingId = ab.BlessingId, StackCount = ab.StackCount });
                c.SetBlessingCount(c.GetBlessingCount() + ab.StackCount);
            }
            c.SetBlessingCurrentHealth(c.GetBlessingMaxHealth());
            c.SetBlessingCurrentStamina(c.GetBlessingMaxStamina());
        }
    }

    /// <summary>Aggregate of all active blessing multipliers for combat calculations.</summary>
    public struct BlessingModifiers
    {
        public float DamageMultiplier;
        public float DamageTakenMultiplier;
        public float StaminaCostMultiplier;
        public float KyndeGenerationMultiplier;
        public float TimingWindowMultiplier;
        public float BreakDamageMultiplier;
        public float HealPerTurn;
        public float StaminaPerTurn;
        public bool FirstHitImmunity;

        public static BlessingModifiers Default()
        {
            return new BlessingModifiers
            {
                DamageMultiplier = 1f,
                DamageTakenMultiplier = 1f,
                StaminaCostMultiplier = 1f,
                KyndeGenerationMultiplier = 1f,
                TimingWindowMultiplier = 1f,
                BreakDamageMultiplier = 1f,
                HealPerTurn = 0f,
                StaminaPerTurn = 0f,
                FirstHitImmunity = false
            };
        }
    }

    [Serializable]
    public class CharacterProgressEntry
    {
        public string CharacterName;
        public int Level = 1;
        public int XP;
        public List<ActiveBlessing> Blessings = new List<ActiveBlessing>();
    }
}
