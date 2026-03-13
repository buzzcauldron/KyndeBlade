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

    [RequireComponent(typeof(BoxCollider))]
    public class MedievalCharacter : MonoBehaviour, IKyndeUser, IBlessingCharacter
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
        public float ActionRecoveryRemaining;
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

        public void InvokeTurnStart()
        {
            if (HasStatusEffect(StatusEffectType.Stun))
            {
                TickStatusEffects();
                OnTurnStart?.Invoke();
                return;
            }
            var bm = BlessingSystem.GetModifiers(this);
            if (bm.HealPerTurn > 0f) Heal(bm.HealPerTurn);
            if (bm.StaminaPerTurn > 0f) RestoreStamina(bm.StaminaPerTurn);
            TickStatusEffects();
            OnTurnStart?.Invoke();
        }

        void TickStatusEffects()
        {
            for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                var e = ActiveStatusEffects[i];
                e.TickEffect(this);
                if (e.IsExpired()) { e.RemoveEffect(this); ActiveStatusEffects.RemoveAt(i); }
            }
        }
        public void InvokeTurnEnd() => OnTurnEnd?.Invoke();
        /// <summary>Hodent: Clear feedback. (target, success)</summary>
        public event Action<MedievalCharacter, bool> OnDodgeAttempted;
        public event Action<MedievalCharacter, bool> OnParryAttempted;
        public event Action<MedievalCharacter, MedievalCharacter, float> OnDamageDealt;

        Animator _animator;
        CombatAction _pendingHitAction;
        MedievalCharacter _pendingHitTarget;
        PiersAppearanceData _cachedPad;
        SaveManager _cachedSaveManager;
        AgingManager _cachedAgingManager;
        PovertyManager _cachedPoverty;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _cachedPad = GetComponent<PiersAppearanceData>();
        }

        void Start()
        {
            InitializeStatsByClass();
        }

        /// <summary>Trigger the Animator with the action's type (e.g. Strike, Ward). Animator Controller needs matching Trigger parameters.</summary>
        public void PlayActionAnimation(CombatAction action)
        {
            if (_animator == null || action == null) return;
            _animator.SetTrigger(action.ActionData.ActionType.ToString());
        }

        /// <summary>True when hit damage will be deferred to an Animation Event (call OnAnimationHit at the hit frame).</summary>
        public bool WillDeferHitToAnimation() => _animator != null;

        /// <summary>Register a hit to be applied when the animation calls OnAnimationHit. Used for sword-hit sync.</summary>
        public void RegisterPendingHit(CombatAction action, MedievalCharacter target)
        {
            _pendingHitAction = action;
            _pendingHitTarget = target;
        }

        /// <summary>Call from an Animation Event at the frame the sword hits. Applies pending hit damage and clears it. In Unity: add Animation Event on the attack clip and set Function to OnAnimationHit (no arguments).</summary>
        public void OnAnimationHit()
        {
            if (_pendingHitAction == null || _pendingHitTarget == null) return;
            _pendingHitAction.ApplyHitEffect(this, _pendingHitTarget);
            _pendingHitAction.NotifyHitExecuted(this, _pendingHitTarget);
            _pendingHitAction = null;
            _pendingHitTarget = null;
        }

        protected virtual void Update()
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
            if (ActionRecoveryRemaining > 0f)
                ActionRecoveryRemaining = Mathf.Max(0f, ActionRecoveryRemaining - dt);
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
                if (_cachedPad != null)
                {
                    Color color;
                    Vector3 scale;
                    PiersAppearanceRandomizer.GetAppearance(_cachedPad.Seed, _cachedPad.CharacterKey, _cachedPad.IsPlayer, out color, out scale, _cachedPad.HasHungerScar);
                    sr.color = color;
                    transform.localScale = scale;
                }
                else
                {
                    var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
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

        /// <summary>Set display identity (class, name, sound theme). Use from enemy Start() to keep distinct feel.</summary>
        public void SetIdentity(CharacterClass classType, string displayName, CharacterSoundTheme soundTheme = CharacterSoundTheme.Default)
        {
            CharacterClassType = classType;
            CharacterName = displayName;
            SoundTheme = soundTheme;
        }

        /// <summary>Set full combat stats; current health/stamina set to max. Use from enemy Start() for one-line stat block.</summary>
        public void SetStats(float maxHealth, float maxStamina, float attackPower, float defense, float speed)
        {
            Stats.MaxHealth = maxHealth;
            Stats.CurrentHealth = maxHealth;
            Stats.MaxStamina = maxStamina;
            Stats.CurrentStamina = maxStamina;
            Stats.AttackPower = attackPower;
            Stats.Defense = defense;
            Stats.Speed = speed;
        }

        /// <summary>Applies damage after defense, hunger/parry/dodge, and (for players) ethical misstep multiplier; updates health and defeat.</summary>
        public void ApplyCustomDamage(float damage, MedievalCharacter attacker)
        {
            ApplyCustomDamage(damage, attacker, damageAlreadyFinal: false);
        }

        /// <summary>Same as ApplyCustomDamage; when damageAlreadyFinal is true, damage is not reduced by Defense (use for CombatCalculator precomputed damage).</summary>
        public void ApplyCustomDamage(float damage, MedievalCharacter attacker, bool damageAlreadyFinal)
        {
            if (!IsAlive()) return;

            if (IsDodging && AttemptDodge())
            {
                float kynde = DodgeWindowRemaining < 0.3f ? 2f : 1f;
                GainKynde(kynde);
                OnDodgeAttempted?.Invoke(this, true);
                return;
            }
            if (IsDodging) { OnDodgeAttempted?.Invoke(this, false); }

            var blessMods = BlessingSystem.GetModifiers(this);
            if (blessMods.FirstHitImmunity && !Stats.FirstHitUsed)
            {
                Stats.FirstHitUsed = true;
                return;
            }

            float actualDamage = damageAlreadyFinal
                ? Mathf.Max(0f, damage)
                : Mathf.Max(0f, damage - Stats.Defense);
            if (_cachedPad != null && _cachedPad.IsPlayer)
            {
                if (_cachedSaveManager == null) _cachedSaveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
                if (_cachedSaveManager != null)
                    actualDamage *= _cachedSaveManager.GetDamageTakenMultiplier();
            }
            if (IsHungry())
            {
                var hunger = GetStatusEffect(StatusEffectType.Hunger);
                if (hunger != null)
                    actualDamage *= 1f + hunger.Data.StackCount * 0.2f;
            }

            if (IsParrying)
            {
                actualDamage *= 0.3f;
                if (AttemptParry())
                {
                    GainKynde(ParryWindowRemaining < 0.25f ? 4f : 2f);
                    OnParryAttempted?.Invoke(this, true);
                }
                else
                {
                    OnParryAttempted?.Invoke(this, false);
                }
            }

            Stats.CurrentHealth = Mathf.Max(0f, Stats.CurrentHealth - actualDamage);
            OnHealthChanged?.Invoke(Stats.CurrentHealth, Stats.MaxHealth);

            if (attacker is EldeCharacter)
            {
                if (_cachedSaveManager == null) _cachedSaveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
                if (_cachedAgingManager == null) _cachedAgingManager = UnityEngine.Object.FindFirstObjectByType<AgingManager>();
                if (_cachedSaveManager != null && _cachedAgingManager != null)
                {
                    _cachedSaveManager.IncrementEldeHitsAccrued();
                    int tier = _cachedSaveManager.CurrentProgress.EldeHitsAccrued;
                    _cachedAgingManager.ApplyAgeToCharacter(this, tier);
                }
            }

            OnDamageDealt?.Invoke(attacker, this, actualDamage);
            if (!IsAlive()) OnCharacterDefeated?.Invoke(this);
        }

        /// <summary>Increases current health; clamps to MaxHealth and fires OnHealthChanged.</summary>
        public void Heal(float amount)
        {
            Stats.CurrentHealth = Mathf.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);
            OnHealthChanged?.Invoke(Stats.CurrentHealth, Stats.MaxHealth);
        }

        /// <summary>Spends stamina for actions; blessing modifier applied. Clamps to zero.</summary>
        public void ConsumeStamina(float amount)
        {
            var blessMods = BlessingSystem.GetModifiers(this);
            amount = Mathf.Max(0f, amount * blessMods.StaminaCostMultiplier);
            Stats.CurrentStamina = Mathf.Max(0f, Stats.CurrentStamina - amount);
            OnStaminaChanged?.Invoke(Stats.CurrentStamina, Stats.MaxStamina);
        }

        /// <summary>Restores stamina; multiplied by PovertyManager.GetStaminaRegenMultiplier(). Clamps to MaxStamina.</summary>
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
            if (_cachedPoverty == null) _cachedPoverty = UnityEngine.Object.FindFirstObjectByType<PovertyManager>();
            if (_cachedPoverty != null) amount *= _cachedPoverty.GetStaminaRegenMultiplier();
            Stats.CurrentStamina = Mathf.Min(Stats.MaxStamina, Stats.CurrentStamina + amount);
            OnStaminaChanged?.Invoke(Stats.CurrentStamina, Stats.MaxStamina);
        }

        /// <summary>Adds Kynde resource; reduced by Hunger, Age, HungerScar, and PovertyManager multiplier. Clamps to MaxKynde.</summary>
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
            if (_cachedPoverty == null) _cachedPoverty = UnityEngine.Object.FindFirstObjectByType<PovertyManager>();
            if (_cachedPoverty != null) amount *= _cachedPoverty.GetKyndeGenMultiplier();
            var blessMods = BlessingSystem.GetModifiers(this);
            amount *= blessMods.KyndeGenerationMultiplier;
            Stats.CurrentKynde = Mathf.Min(Stats.MaxKynde, Stats.CurrentKynde + amount);
            OnKyndeChanged?.Invoke(Stats.CurrentKynde, Stats.MaxKynde);
        }

        /// <summary>Spends Kynde for skills. Returns false if not enough; otherwise deducts and returns true.</summary>
        public bool ConsumeKynde(float amount)
        {
            if (Stats.CurrentKynde < amount) return false;
            Stats.CurrentKynde -= amount;
            OnKyndeChanged?.Invoke(Stats.CurrentKynde, Stats.MaxKynde);
            return true;
        }

        /// <summary>Reduces break gauge; when it hits zero, calls BreakCharacter (stun).</summary>
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
        public void BeginActionRecovery(float duration) { ActionRecoveryRemaining = Mathf.Max(ActionRecoveryRemaining, duration); }
        public void ClearActionRecovery() { ActionRecoveryRemaining = 0f; }
        public bool AttemptDodge() => DodgeWindowRemaining > 0f;
        public bool AttemptParry() => ParryWindowRemaining > 0f;

        void UpdateRealTimeCombat(float dt)
        {
            if (IsDodging) { DodgeWindowRemaining -= dt; if (DodgeWindowRemaining <= 0f) EndDodge(); }
            if (IsParrying) { ParryWindowRemaining -= dt; if (ParryWindowRemaining <= 0f) EndParry(); }
        }

        /// <summary>Runs the combat action with this character as executor and the given target.</summary>
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
            var tm = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (tm != null && tm.PlayerCharacters != null && tm.PlayerCharacters.Contains(this))
            {
                var save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
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
        public string GetBlessingCharacterName() => CharacterName;
        public float GetBlessingMaxHealth() => Stats.MaxHealth;
        public void SetBlessingMaxHealth(float value) => Stats.MaxHealth = value;
        public float GetBlessingMaxStamina() => Stats.MaxStamina;
        public void SetBlessingMaxStamina(float value) => Stats.MaxStamina = value;
        public float GetBlessingAttackPower() => Stats.AttackPower;
        public void SetBlessingAttackPower(float value) => Stats.AttackPower = value;
        public float GetBlessingDefense() => Stats.Defense;
        public void SetBlessingDefense(float value) => Stats.Defense = value;
        public float GetBlessingSpeed() => Stats.Speed;
        public void SetBlessingSpeed(float value) => Stats.Speed = value;
        public float GetBlessingMaxKynde() => Stats.MaxKynde;
        public void SetBlessingMaxKynde(float value) => Stats.MaxKynde = value;
        public float GetBlessingCurrentHealth() => Stats.CurrentHealth;
        public void SetBlessingCurrentHealth(float value) => Stats.CurrentHealth = value;
        public float GetBlessingCurrentStamina() => Stats.CurrentStamina;
        public void SetBlessingCurrentStamina(float value) => Stats.CurrentStamina = value;
        public int GetBlessingCount() => Stats.BlessingCount;
        public void SetBlessingCount(int value) => Stats.BlessingCount = value;
        public List<ActiveBlessing> GetActiveBlessings() => Stats.ActiveBlessings;

        /// <summary>Damage from this character's first Strike or RangedStrike action. Delegates to CombatCalculator so damage logic lives in one place.</summary>
        public float GetPrimaryStrikeDamage() => CombatCalculator.ComputePrimaryStrikeDamage(this);
    }
}
