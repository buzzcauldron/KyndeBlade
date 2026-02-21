using System;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    public enum CharacterClass
    {
        Knight,
        Mage,
        Archer,
        Rogue,
        Dreamer
    }

    /// <summary>Sound theme for parry/dodge zone strike warning (character-appropriate).</summary>
    public enum CharacterSoundTheme
    {
        Default,
        ChurchBell,   // Holy Church
        Creaking,    // Hunger
        Sloppy,      // Gluttony
        Wrath,       // Anger, metallic clash
        Greed,       // Lady Mede, coins
        Sloth,       // Dull, heavy
        Pride,       // Trumpet, fanfare
        Envy         // Hiss, whisper
    }

    [RequireComponent(typeof(Collider))]
    public class MedievalCharacter : MonoBehaviour
    {
        [Header("Character")]
        public CharacterClass CharacterClassType;
        public CharacterStats Stats = new CharacterStats();
        public string CharacterName = "Unknown";
        [Tooltip("Sound theme for strike warning during parry/dodge zone (e.g. ChurchBell for Holy Church).")]
        public CharacterSoundTheme SoundTheme = CharacterSoundTheme.Default;

        [Header("Combat")]
        public List<CombatAction> AvailableActions = new List<CombatAction>();
        public List<StatusEffect> ActiveStatusEffects = new List<StatusEffect>();

        [Header("Real-Time State")]
        public bool IsDodging;
        public bool IsParrying;
        public float DodgeWindowRemaining;
        public float ParryWindowRemaining;
        [Tooltip("Fairy transformation (fae appearance). Visual tint for short periods.")]
        public float FairyFormRemaining;

        public bool IsFairyForm => FairyFormRemaining > 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnStaminaChanged;
        public event Action<float, float> OnKyndeChanged;
        public event Action<float, float> OnBreakGaugeChanged;
        public event Action<MedievalCharacter> OnCharacterDefeated;
        public event Action<MedievalCharacter> OnCharacterBroken;
        public event Action OnTurnStart;
        public event Action OnTurnEnd;

        public void InvokeTurnStart() => OnTurnStart?.Invoke();
        public void InvokeTurnEnd() => OnTurnEnd?.Invoke();
        /// <summary>Hodent: Clear feedback. (target, success)</summary>
        public event Action<MedievalCharacter, bool> OnDodgeAttempted;
        public event Action<MedievalCharacter, bool> OnParryAttempted;
        public event Action<MedievalCharacter, MedievalCharacter, float> OnDamageDealt;

        void Start()
        {
            InitializeStatsByClass();
        }

        void Update()
        {
            float dt = Time.deltaTime;
            if (IsDodging || IsParrying)
                UpdateRealTimeCombat(dt);
            if (ActiveStatusEffects != null && ActiveStatusEffects.Count > 0)
                UpdateStatusEffects(dt);
            if (Stats.IsBroken && Stats.BrokenStunRemaining > 0f)
            {
                Stats.BrokenStunRemaining -= dt;
                if (Stats.BrokenStunRemaining <= 0f) RecoverFromBreak();
            }
            if (FairyFormRemaining > 0f)
            {
                FairyFormRemaining -= dt;
                if (FairyFormRemaining <= 0f) RemoveFairyForm();
            }
        }

        /// <summary>Apply fairy transformation for duration. One character at a time per encounter.</summary>
        public void ApplyFairyForm(float duration)
        {
            FairyFormRemaining = Mathf.Max(FairyFormRemaining, duration);
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(0.6f, 0.9f, 1f, 1f); // pale fae tint
        }

        /// <summary>Remove fairy transformation and restore visual.</summary>
        public void RemoveFairyForm()
        {
            FairyFormRemaining = 0f;
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var pad = GetComponent<PiersAppearanceData>();
                if (pad != null)
                {
                    Color color;
                    Vector3 scale;
                    PiersAppearanceRandomizer.GetAppearance(pad.Seed, pad.CharacterKey, pad.IsPlayer, out color, out scale, pad.HasHungerScar);
                    sr.color = color;
                    transform.localScale = scale;
                }
                else
                {
                    var tm = UnityEngine.Object.FindObjectOfType<TurnManager>();
                    bool isPlayer = tm != null && tm.PlayerCharacters != null && tm.PlayerCharacters.Contains(this);
                    sr.color = isPlayer ? new Color(0.35f, 0.45f, 0.55f) : new Color(0.55f, 0.35f, 0.3f);
                }
            }
        }

        void InitializeStatsByClass()
        {
            switch (CharacterClassType)
            {
                case CharacterClass.Knight:
                    Stats.MaxHealth = 150f; Stats.CurrentHealth = 150f;
                    Stats.MaxStamina = 120f; Stats.CurrentStamina = 120f;
                    Stats.AttackPower = 15f; Stats.Defense = 10f; Stats.Speed = 8f;
                    break;
                case CharacterClass.Mage:
                    Stats.MaxHealth = 80f; Stats.CurrentHealth = 80f;
                    Stats.MaxStamina = 150f; Stats.CurrentStamina = 150f;
                    Stats.AttackPower = 20f; Stats.Defense = 3f; Stats.Speed = 10f;
                    break;
                case CharacterClass.Archer:
                    Stats.MaxHealth = 100f; Stats.CurrentHealth = 100f;
                    Stats.MaxStamina = 130f; Stats.CurrentStamina = 130f;
                    Stats.AttackPower = 12f; Stats.Defense = 5f; Stats.Speed = 12f;
                    break;
                case CharacterClass.Rogue:
                    Stats.MaxHealth = 90f; Stats.CurrentHealth = 90f;
                    Stats.MaxStamina = 140f; Stats.CurrentStamina = 140f;
                    Stats.AttackPower = 14f; Stats.Defense = 4f; Stats.Speed = 15f;
                    break;
                case CharacterClass.Dreamer:
                    Stats.MaxHealth = 120f; Stats.CurrentHealth = 120f;
                    Stats.MaxStamina = 130f; Stats.CurrentStamina = 130f;
                    Stats.AttackPower = 14f; Stats.Defense = 6f; Stats.Speed = 9f;
                    break;
            }
        }

        public void ApplyCustomDamage(float damage, MedievalCharacter attacker)
        {
            if (!IsAlive()) return;

            float actualDamage = Mathf.Max(0f, damage - Stats.Defense);
            if (IsHungry())
            {
                var hunger = GetStatusEffect(StatusEffectType.Hunger);
                if (hunger != null)
                    actualDamage *= 1f + hunger.Data.StackCount * 0.2f;
            }

            if (IsDodging && AttemptDodge())
            {
                float kynde = DodgeWindowRemaining < 0.3f ? 2f : 1f; // Perfect in last 30%
                GainKynde(kynde);
                OnDodgeAttempted?.Invoke(this, true);
                return;
            }
            if (IsDodging) { OnDodgeAttempted?.Invoke(this, false); }

            if (IsParrying && AttemptParry())
            {
                GainKynde(ParryWindowRemaining < 0.25f ? 4f : 2f); // Perfect in last 25%
                actualDamage *= 0.3f;
                OnParryAttempted?.Invoke(this, true);
            }
            else if (IsParrying) { OnParryAttempted?.Invoke(this, false); }

            Stats.CurrentHealth = Mathf.Max(0f, Stats.CurrentHealth - actualDamage);
            OnHealthChanged?.Invoke(Stats.CurrentHealth, Stats.MaxHealth);
            OnDamageDealt?.Invoke(attacker, this, actualDamage);
            if (!IsAlive()) OnCharacterDefeated?.Invoke(this);
        }

        public void Heal(float amount)
        {
            Stats.CurrentHealth = Mathf.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);
            OnHealthChanged?.Invoke(Stats.CurrentHealth, Stats.MaxHealth);
        }

        public void ConsumeStamina(float amount)
        {
            Stats.CurrentStamina = Mathf.Max(0f, Stats.CurrentStamina - amount);
            OnStaminaChanged?.Invoke(Stats.CurrentStamina, Stats.MaxStamina);
        }

        public void RestoreStamina(float amount)
        {
            if (IsHungry())
            {
                var h = GetStatusEffect(StatusEffectType.Hunger);
                if (h != null) amount *= h.Data.StaminaRegenModifier;
            }
            var age = GetStatusEffect(StatusEffectType.Age);
            if (age != null) amount *= age.Data.StaminaRegenModifier;
            var scar = GetStatusEffect(StatusEffectType.HungerScar);
            if (scar != null) amount *= scar.Data.StaminaRegenModifier;
            var poverty = UnityEngine.Object.FindObjectOfType<PovertyManager>();
            if (poverty != null) amount *= poverty.GetStaminaRegenMultiplier();
            Stats.CurrentStamina = Mathf.Min(Stats.MaxStamina, Stats.CurrentStamina + amount);
            OnStaminaChanged?.Invoke(Stats.CurrentStamina, Stats.MaxStamina);
        }

        public void GainKynde(float amount)
        {
            if (IsHungry())
            {
                var h = GetStatusEffect(StatusEffectType.Hunger);
                if (h != null) amount *= h.Data.KyndeGenerationModifier;
            }
            var age = GetStatusEffect(StatusEffectType.Age);
            if (age != null) amount *= age.Data.KyndeGenerationModifier;
            var scar = GetStatusEffect(StatusEffectType.HungerScar);
            if (scar != null) amount *= scar.Data.KyndeGenerationModifier;
            var poverty = UnityEngine.Object.FindObjectOfType<PovertyManager>();
            if (poverty != null) amount *= poverty.GetKyndeGenMultiplier();
            Stats.CurrentKynde = Mathf.Min(Stats.MaxKynde, Stats.CurrentKynde + amount);
            OnKyndeChanged?.Invoke(Stats.CurrentKynde, Stats.MaxKynde);
        }

        public bool ConsumeKynde(float amount)
        {
            if (Stats.CurrentKynde < amount) return false;
            Stats.CurrentKynde -= amount;
            OnKyndeChanged?.Invoke(Stats.CurrentKynde, Stats.MaxKynde);
            return true;
        }

        public void TakeBreakDamage(float amount)
        {
            if (Stats.IsBroken) return;
            Stats.CurrentBreakGauge = Mathf.Max(0f, Stats.CurrentBreakGauge - amount);
            OnBreakGaugeChanged?.Invoke(Stats.CurrentBreakGauge, Stats.MaxBreakGauge);
            if (Stats.CurrentBreakGauge <= 0f) BreakCharacter();
        }

        public void BreakCharacter()
        {
            if (Stats.IsBroken) return;
            Stats.IsBroken = true;
            Stats.BrokenStunRemaining = 2f;
            OnCharacterBroken?.Invoke(this);
        }

        public void RecoverFromBreak()
        {
            Stats.IsBroken = false;
            Stats.BrokenStunRemaining = 0f;
            Stats.CurrentBreakGauge = Stats.MaxBreakGauge;
            OnBreakGaugeChanged?.Invoke(Stats.CurrentBreakGauge, Stats.MaxBreakGauge);
        }

        public void StartDodge(float windowDuration) { IsDodging = true; DodgeWindowRemaining = windowDuration; }
        public void StartParry(float windowDuration) { IsParrying = true; ParryWindowRemaining = windowDuration; }
        public void EndDodge() { IsDodging = false; DodgeWindowRemaining = 0f; }
        public void EndParry() { IsParrying = false; ParryWindowRemaining = 0f; }
        public bool AttemptDodge() => DodgeWindowRemaining > 0f;
        public bool AttemptParry() => ParryWindowRemaining > 0f;

        void UpdateRealTimeCombat(float dt)
        {
            if (IsDodging) { DodgeWindowRemaining -= dt; if (DodgeWindowRemaining <= 0f) EndDodge(); }
            if (IsParrying) { ParryWindowRemaining -= dt; if (ParryWindowRemaining <= 0f) EndParry(); }
        }

        public void ExecuteCombatAction(CombatAction action, MedievalCharacter target)
        {
            action?.ExecuteAction(this, target);
        }

        public void ApplyStatusEffect(StatusEffect effect)
        {
            if (effect == null) return;
            var existing = GetStatusEffect(effect.Data.EffectType);
            if (existing != null) { existing.StackEffect(effect.Data.StackCount); existing.ApplyEffect(this); }
            else { ActiveStatusEffects.Add(effect); effect.ApplyEffect(this); }
        }

        public void RemoveStatusEffect(StatusEffectType type)
        {
            for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                if (ActiveStatusEffects[i].Data.EffectType == type)
                {
                    ActiveStatusEffects[i].RemoveEffect(this);
                    ActiveStatusEffects.RemoveAt(i);
                    break;
                }
            }
        }

        public bool HasStatusEffect(StatusEffectType type) => GetStatusEffect(type) != null;
        public StatusEffect GetStatusEffect(StatusEffectType type)
        {
            foreach (var e in ActiveStatusEffects)
                if (e.Data.EffectType == type) return e;
            return null;
        }

        void UpdateStatusEffects(float dt)
        {
            for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                var e = ActiveStatusEffects[i];
                e.UpdateEffect(dt);
                if (e.Data.DamagePerSecond > 0f) e.ApplyDamageOverTime(this, dt);
                if (e.IsExpired()) { e.RemoveEffect(this); ActiveStatusEffects.RemoveAt(i); }
            }
        }

        public void ApplyHunger(int stacks = 1, float duration = 0f)
        {
            ApplyStatusEffect(StatusEffect.CreateHungerEffect(duration, stacks));
            var tm = UnityEngine.Object.FindObjectOfType<TurnManager>();
            if (tm != null && tm.PlayerCharacters != null && tm.PlayerCharacters.Contains(this))
            {
                var save = UnityEngine.Object.FindObjectOfType<SaveManager>();
                save?.MarkHasEverHadHunger();
            }
        }

        /// <summary>Remove one stack of hunger, or clear hunger entirely if 1 stack. Returns true if hunger was reduced.</summary>
        public bool RemoveHungerStack()
        {
            var h = GetStatusEffect(StatusEffectType.Hunger);
            if (h == null) return false;
            if (h.Data.StackCount <= 1)
            {
                RemoveStatusEffect(StatusEffectType.Hunger);
                return true;
            }
            h.RemoveEffect(this);
            h.Data.StackCount--;
            h.StackEffect(0); // Recompute modifiers for new stack count
            h.ApplyEffect(this);
            return true;
        }

        public bool IsHungry() => HasStatusEffect(StatusEffectType.Hunger);
        public bool IsAged() => HasStatusEffect(StatusEffectType.Age);
        public float GetCurrentHealth() => Stats.CurrentHealth;
        public float GetMaxHealth() => Stats.MaxHealth;
        public float GetCurrentStamina() => Stats.CurrentStamina;
        public float GetMaxStamina() => Stats.MaxStamina;
        public float GetCurrentKynde() => Stats.CurrentKynde;
        public float GetMaxKynde() => Stats.MaxKynde;
        public bool IsAlive() => Stats.CurrentHealth > 0f;
        public bool IsBroken() => Stats.IsBroken;
    }
}
