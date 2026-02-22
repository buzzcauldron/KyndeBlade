using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Base for boss AI: shared setup, turn handling, and lowest-HP targeting. Subclasses implement ChooseSpecificAction only.</summary>
    [RequireComponent(typeof(MedievalCharacter))]
    public abstract class BaseBossAI : MonoBehaviour
    {
        public TurnManager TurnManager;
        public float DecisionDelay = 0.5f;

        [Header("Dialogue")]
        [Tooltip("Optional. When set, BossDialogueManager uses this for entry/midpoint/desperation lines.")]
        public BossDialogueScript BossDialogueScript;
        protected bool _entryLinePlayed;

        [Header("Narrative Pause (Pre-Raphaelite pathos)")]
        [Tooltip("When boss chooses a Special action (or one in DramaticActionNames), trigger slow-mo + DoF portrait before executing.")]
        public bool UseNarrativePause = true;
        [Tooltip("Real-time duration of the pause in seconds.")]
        public float NarrativePauseDuration = 1.5f;
        [Tooltip("Time.timeScale during the pause.")]
        [Range(0.01f, 1f)] public float NarrativePauseTimeScale = 0.2f;
        [Tooltip("If non-empty, only these action names trigger the pause. If empty, any Special action triggers it.")]
        public string[] DramaticActionNames = new[] { "The Feast of Want", "Wild Nature's Wrath", "The Green Chapel's Curse", "The Tower", "Pride's Fall" };

        /// <summary>Fired when narrative pause starts (boss, duration). Subscribe to enable DoF focused on boss.</summary>
        public static event Action<MedievalCharacter, float> OnNarrativePauseStarted;
        /// <summary>Fired when narrative pause ends. Restore DoF / timeScale.</summary>
        public static event Action OnNarrativePauseEnded;

        protected MedievalCharacter _self;
        readonly List<CombatAction> _affordableBuffer = new List<CombatAction>();

        protected virtual void Start()
        {
            _self = GetComponent<MedievalCharacter>();
            if (TurnManager == null) TurnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (TurnManager != null) TurnManager.OnTurnChanged += OnTurnChanged;
        }

        protected virtual void OnDestroy()
        {
            if (TurnManager != null) TurnManager.OnTurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(MedievalCharacter current)
        {
            if (current != _self) return;
            if (TurnManager != null && TurnManager.IsPlayerTurn()) return;
            Invoke(nameof(ExecuteTurn), DecisionDelay);
        }

        protected virtual void ExecuteTurn()
        {
            if (TurnManager == null || TurnManager.CurrentCharacter != _self || !_self.IsAlive()) return;

            var dm = BossDialogueManager.Instance;
            if (dm != null && BossDialogueScript != null)
            {
                if (!_entryLinePlayed)
                {
                    dm.TriggerEntry();
                    _entryLinePlayed = true;
                }
                float hpRatio = GetHpRatio();
                if (hpRatio < 0.2f) dm.TriggerDesperation();
                else if (hpRatio < 0.6f) dm.TriggerMidpoint();
            }

            var target = ChooseTarget();
            var action = ChooseSpecificAction(target);
            var finalTarget = target ?? ChooseTarget();

            if (action != null)
            {
                if (UseNarrativePause && WantsNarrativePause(action))
                    StartCoroutine(RunNarrativePauseThenExecute(action, finalTarget));
                else
                    TurnManager.ExecuteAction(action, finalTarget);
            }
            else
                TurnManager.NextTurn();
        }

        /// <summary>True when action is a configured "dramatic" moment (by name list or Special type).</summary>
        bool WantsNarrativePause(CombatAction action)
        {
            if (action?.ActionData == null) return false;
            bool hasNameList = DramaticActionNames != null && DramaticActionNames.Length > 0;
            if (hasNameList)
            {
                string name = action.ActionData.ActionName ?? "";
                foreach (var n in DramaticActionNames)
                    if (string.Equals(n, name, StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }
            return action.ActionData.ActionType == CombatActionType.Special;
        }

        IEnumerator RunNarrativePauseThenExecute(CombatAction action, MedievalCharacter target)
        {
            float prevScale = Time.timeScale;
            Time.timeScale = NarrativePauseTimeScale;
            OnNarrativePauseStarted?.Invoke(_self, NarrativePauseDuration);
            yield return new WaitForSecondsRealtime(NarrativePauseDuration);
            Time.timeScale = prevScale;
            OnNarrativePauseEnded?.Invoke();
            if (TurnManager != null && action != null)
                TurnManager.ExecuteAction(action, target);
        }

        /// <summary>Common targeting: find player with lowest HP. Override for different logic.</summary>
        protected virtual MedievalCharacter ChooseTarget()
        {
            if (TurnManager == null) return null;
            MedievalCharacter best = null;
            float lowestHp = float.MaxValue;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                float hp = p.GetCurrentHealth();
                if (hp < lowestHp) { lowestHp = hp; best = p; }
            }
            return best;
        }

        /// <summary>Number of alive player characters. Reused by subclasses to avoid per-turn allocation.</summary>
        protected int GetAlivePlayerCount()
        {
            if (TurnManager == null) return 0;
            int n = 0;
            foreach (var p in TurnManager.PlayerCharacters)
                if (p != null && p.IsAlive()) n++;
            return n;
        }

        /// <summary>Current health ratio (0–1). Uses cached _self.</summary>
        protected float GetHpRatio() => _self != null ? _self.GetCurrentHealth() / Mathf.Max(1f, _self.GetMaxHealth()) : 0f;

        /// <summary>Fills buffer with actions the character can afford (stamina). Returns same list; clear and reuse each turn. Caller must not hold reference.</summary>
        protected List<CombatAction> GetAffordableActions()
        {
            _affordableBuffer.Clear();
            if (_self?.AvailableActions == null) return _affordableBuffer;
            float stam = _self.GetCurrentStamina();
            foreach (var a in _self.AvailableActions)
                if (a != null && a.ActionData.StaminaCost <= stam)
                    _affordableBuffer.Add(a);
            return _affordableBuffer;
        }

        /// <summary>Boss-specific action choice. Must be implemented by each boss.</summary>
        protected abstract CombatAction ChooseSpecificAction(MedievalCharacter target);
    }
}
