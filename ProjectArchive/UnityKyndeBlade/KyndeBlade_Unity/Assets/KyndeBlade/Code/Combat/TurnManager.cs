using System;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    public enum CombatState
    {
        WaitingForInput,
        ExecutingAction,
        RealTimeWindow,
        ProcessingResults,
        CombatEnded
    }

    public class TurnManager : MonoBehaviour
    {
        [Header("Combat")]
        public float RealTimeWindowDuration = 2f;
        public GameSettings Settings;
        [Tooltip("When set, ExecuteAction runs through telegraph → animation → impact timing. Leave empty for instant execution.")]
        public TurnSequenceController SequenceController;

        public CombatState State { get; private set; } = CombatState.CombatEnded;
        public List<MedievalCharacter> PlayerCharacters { get; private set; } = new List<MedievalCharacter>();
        public List<MedievalCharacter> EnemyCharacters { get; private set; } = new List<MedievalCharacter>();
        public List<MedievalCharacter> TurnOrder { get; private set; } = new List<MedievalCharacter>();
        public MedievalCharacter CurrentCharacter { get; private set; }
        public int TurnNumber { get; private set; }
        public float RealTimeWindowRemaining { get; private set; }

        /// <summary>Play/Edit mode tests only (see InternalsVisibleTo).</summary>
        internal void SetRealTimeWindowRemainingForTests(float seconds) => RealTimeWindowRemaining = seconds;
        public float ActionCastTimeRemaining { get; private set; }
        public float ActionExecutionTimeRemaining { get; private set; }
        public bool IsRealTimeDefenseEnabled => Settings == null || Settings.EnableRealTimeDefenseWindows;
        /// <summary>Attacker whose strike is imminent (for strike-warning sound theme).</summary>
        public MedievalCharacter CurrentAttackerDuringWindow { get; private set; }
        /// <summary>Defender (player) who used Escapade/Ward.</summary>
        public MedievalCharacter DefenderDuringWindow { get; private set; }
        /// <summary>Action type that triggered the window (Escapade = dodge, Ward = parry).</summary>
        public CombatActionType ActionTypeDuringWindow { get; private set; }

        CombatAction _currentAction;
        MedievalCharacter _currentTarget;
        BufferedAction _bufferedAction;

        struct BufferedAction
        {
            public CombatAction Action;
            public MedievalCharacter Target;
            public float ExpiresAt;
        }

        public event Action<MedievalCharacter> OnTurnChanged;
        public event Action OnCombatEnded;
        /// <summary>Fired when an action is executed (executor, target, action). For AI observation.</summary>
        public event Action<MedievalCharacter, MedievalCharacter, CombatAction> OnActionExecuted;

        [Header("Critical hit (builds across hits)")]
        [Tooltip("Added to crit pressure each time damage is dealt. Pressure resets on crit.")]
        public float CritBuildPerDeal = 0.08f;
        [Tooltip("Cap for crit pressure (0–1). Higher = sooner guaranteed crit.")]
        public float CritPressureCap = 0.4f;
        [Tooltip("Damage multiplier when a crit triggers.")]
        public float CritDamageMultiplier = 1.5f;

        float _critPressure;
        /// <summary>True for the last damage instance that was a crit (for UI/feedback).</summary>
        public bool LastDamageWasCrit { get; private set; }

        public void InitializeCombat(List<MedievalCharacter> players, List<MedievalCharacter> enemies)
        {
            PlayerCharacters = new List<MedievalCharacter>(players);
            EnemyCharacters = new List<MedievalCharacter>(enemies);
            TurnOrder = new List<MedievalCharacter>();
            TurnOrder.AddRange(PlayerCharacters);
            TurnOrder.AddRange(EnemyCharacters);
            TurnOrder.RemoveAll(c => c == null);
            CalculateTurnOrder();
        }

        public void AddCharacterToCombat(MedievalCharacter character, bool isPlayer)
        {
            if (character == null) return;
            if (isPlayer) PlayerCharacters.Add(character);
            else EnemyCharacters.Add(character);
            TurnOrder.Add(character);
            CalculateTurnOrder();
        }

        public void StartCombat()
        {
            if (TurnOrder.Count == 0) return;
            State = CombatState.WaitingForInput;
            TurnNumber = 1;
            _critPressure = 0f;
            LastDamageWasCrit = false;
            CurrentCharacter = TurnOrder[0];
            CurrentCharacter?.InvokeTurnStart();
            OnTurnChanged?.Invoke(CurrentCharacter);
        }

        /// <summary>Roll for crit using building pressure; if crit, multiply damage and reset pressure. Call before ApplyCustomDamage.</summary>
        public float ApplyCritToDamage(float damage, out bool isCrit)
        {
            isCrit = false;
            if (State == CombatState.CombatEnded) return damage;
            if (UnityEngine.Random.value < _critPressure)
            {
                isCrit = true;
                _critPressure = 0f;
                LastDamageWasCrit = true;
                return damage * CritDamageMultiplier;
            }
            LastDamageWasCrit = false;
            return damage;
        }

        /// <summary>Call after damage is actually dealt (e.g. from ApplyCustomDamage) to build crit pressure for next time.</summary>
        public void RecordDamageDealt()
        {
            if (State == CombatState.CombatEnded) return;
            _critPressure = Mathf.Min(CritPressureCap, _critPressure + CritBuildPerDeal);
        }

        public void EndTurn()
        {
            CurrentCharacter?.InvokeTurnEnd();
        }

        public void NextTurn()
        {
            EndTurn();
            CheckCombatEnd();
            if (State == CombatState.CombatEnded) return;

            int idx = TurnOrder.IndexOf(CurrentCharacter);
            int start = (idx + 1) % TurnOrder.Count;
            int i = start;

            do
            {
                var next = TurnOrder[i];
                if (next != null && next.IsAlive())
                {
                    CurrentCharacter = next;
                    TurnNumber++;
                    State = CombatState.WaitingForInput;
                    CurrentCharacter?.InvokeTurnStart();
                    OnTurnChanged?.Invoke(CurrentCharacter);
                    TryExecuteBufferedAction();
                    return;
                }
                i = (i + 1) % TurnOrder.Count;
            } while (i != start);

            State = CombatState.CombatEnded;
            OnCombatEnded?.Invoke();
        }

        public void ExecuteAction(CombatAction action, MedievalCharacter target)
        {
            if (action == null || CurrentCharacter == null) return;
            if (!CanCurrentCharacterAct())
            {
                NextTurn();
                return;
            }

            if (State != CombatState.WaitingForInput)
            {
                BufferAction(action, target);
                return;
            }

            State = CombatState.ExecutingAction;
            _currentAction = action;
            _currentTarget = target;

            if (SequenceController != null)
            {
                SequenceController.RunSequence(CurrentCharacter, action, target);
                return;
            }

            if (action.GetCastTime() > 0f)
                ActionCastTimeRemaining = action.GetCastTime();
            else
                ProcessActionCast();
        }

        /// <summary>Play animation and run combat logic (damage may be deferred to animation event). Called by TurnSequenceController or legacy flow.</summary>
        public void DoActionCast()
        {
            ProcessActionCast();
        }

        /// <summary>Start defense window or transition to next turn; clears current action. Called by TurnSequenceController or legacy flow.</summary>
        public void DoActionExecution()
        {
            ProcessActionExecution();
        }

        void ProcessActionCast()
        {
            if (_currentAction == null || CurrentCharacter == null) return;
            if (!CanCurrentCharacterAct())
            {
                _currentAction = null;
                _currentTarget = null;
                State = CombatState.ProcessingResults;
                StartCoroutine(TransitionToNextTurn());
                return;
            }

            // Turn-based reaction flow: enemy offensive actions open the window first,
            // then resolve the hit after player reaction input.
            if (IsRealTimeDefenseEnabled &&
                EnemyCharacters.Contains(CurrentCharacter) &&
                IsOffensiveAction(_currentAction.ActionData.ActionType))
            {
                var defender = _currentTarget != null && PlayerCharacters.Contains(_currentTarget) && _currentTarget.IsAlive()
                    ? _currentTarget
                    : GetFirstAlivePlayer();
                if (defender != null)
                {
                    _currentTarget = defender;
                    StartRealTimeWindow(_currentAction.ActionData.SuccessWindow, CurrentCharacter, defender, CombatActionType.Ward);
                    return;
                }
            }

            CurrentCharacter.PlayActionAnimation(_currentAction);
            CurrentCharacter.ExecuteCombatAction(_currentAction, _currentTarget);
            OnActionExecuted?.Invoke(CurrentCharacter, _currentTarget, _currentAction);

            if (_currentAction.GetExecutionTime() > 0f)
                ActionExecutionTimeRemaining = _currentAction.GetExecutionTime();
            else
                ProcessActionExecution();
        }

        void ProcessActionExecution()
        {
            if (CurrentCharacter != null && _currentAction != null)
            {
                float recovery = Mathf.Max(0f, _currentAction.GetExecutionTime() * 0.5f);
                if (Settings != null)
                    recovery *= Settings.PlayerActionTimingMultiplier;
                CurrentCharacter.BeginActionRecovery(recovery);
            }

            if (_currentAction != null &&
                (_currentAction.ActionData.ActionType == CombatActionType.Escapade ||
                 _currentAction.ActionData.ActionType == CombatActionType.Ward))
            {
                var defender = CurrentCharacter;
                var attacker = _currentTarget != null && EnemyCharacters.Contains(_currentTarget)
                    ? _currentTarget
                    : GetFirstAliveEnemy();
                StartRealTimeWindow(_currentAction.ActionData.SuccessWindow, attacker, defender, _currentAction.ActionData.ActionType);
            }
            else
            {
                State = CombatState.ProcessingResults;
                StartCoroutine(TransitionToNextTurn());
            }

            _currentAction = null;
            _currentTarget = null;
            ActionCastTimeRemaining = 0f;
            ActionExecutionTimeRemaining = 0f;
        }

        public void StartRealTimeWindow(float duration, MedievalCharacter attacker = null, MedievalCharacter defender = null, CombatActionType actionType = CombatActionType.Escapade)
        {
            if (Settings != null)
                duration = Settings.GetAdjustedWindow(duration);
            if (defender != null)
            {
                var bm = BlessingSystem.GetModifiers(defender);
                duration *= bm.TimingWindowMultiplier;
            }
            RealTimeWindowDuration = duration;
            RealTimeWindowRemaining = duration;
            CurrentAttackerDuringWindow = attacker ?? GetFirstAliveEnemy();
            DefenderDuringWindow = defender;
            ActionTypeDuringWindow = actionType;
            State = CombatState.RealTimeWindow;
        }

        void BufferAction(CombatAction action, MedievalCharacter target)
        {
            float bufferSeconds = Settings != null ? Settings.InputBufferSeconds : 0.12f;
            if (bufferSeconds <= 0f) return;
            _bufferedAction.Action = action;
            _bufferedAction.Target = target;
            _bufferedAction.ExpiresAt = Time.time + bufferSeconds;
        }

        void TryExecuteBufferedAction()
        {
            if (_bufferedAction.Action == null) return;
            if (Time.time > _bufferedAction.ExpiresAt)
            {
                _bufferedAction = default;
                return;
            }
            if (State != CombatState.WaitingForInput || CurrentCharacter == null || !CanCurrentCharacterAct())
                return;

            var action = _bufferedAction.Action;
            var target = _bufferedAction.Target;
            _bufferedAction = default;
            ExecuteAction(action, target);
        }

        bool CanCurrentCharacterAct()
        {
            if (CurrentCharacter == null || !CurrentCharacter.IsAlive()) return false;
            if (CurrentCharacter.IsBroken()) return false;
            if (CurrentCharacter.HasStatusEffect(StatusEffectType.Stun)) return false;
            if (CurrentCharacter.ActionRecoveryRemaining > 0f) return false;
            return true;
        }

        MedievalCharacter GetFirstAliveEnemy()
        {
            foreach (var e in EnemyCharacters)
                if (e != null && e.IsAlive()) return e;
            return null;
        }

        MedievalCharacter GetFirstAlivePlayer()
        {
            foreach (var p in PlayerCharacters)
                if (p != null && p.IsAlive()) return p;
            return null;
        }

        static bool IsOffensiveAction(CombatActionType actionType)
        {
            return actionType == CombatActionType.Strike ||
                   actionType == CombatActionType.RangedStrike ||
                   actionType == CombatActionType.Special ||
                   actionType == CombatActionType.Counter;
        }

        public bool IsPlayerTurn() => CurrentCharacter != null && PlayerCharacters.Contains(CurrentCharacter);

        void CalculateTurnOrder()
        {
            TurnOrder.Sort((a, b) =>
            {
                float sa = a != null ? a.Stats.Speed : 0f;
                float sb = b != null ? b.Stats.Speed : 0f;
                return sb.CompareTo(sa);
            });
        }

        void CheckCombatEnd()
        {
            if (AreAllDefeated(PlayerCharacters) || AreAllDefeated(EnemyCharacters))
            {
                State = CombatState.CombatEnded;
                OnCombatEnded?.Invoke();
            }
        }

        bool AreAllDefeated(List<MedievalCharacter> list)
        {
            foreach (var c in list)
                if (c != null && c.IsAlive()) return false;
            return true;
        }

        internal void ResolveDefenseWindow()
        {
            var defender = DefenderDuringWindow;
            var attacker = CurrentAttackerDuringWindow;
            DefenderDuringWindow = null;
            ActionTypeDuringWindow = CombatActionType.Rest;

            if (defender == null || !defender.IsAlive()) return;
            if (attacker == null || !attacker.IsAlive()) return;

            bool parrySucceeded = defender.IsParrying && defender.AttemptParry();
            bool dodgeSucceeded = defender.IsDodging && defender.AttemptDodge();
            float remainingAtSuccess = parrySucceeded ? defender.ParryWindowRemaining
                : (dodgeSucceeded ? defender.DodgeWindowRemaining : 0f);
            bool isPerfect = CombatCalculator.IsPerfectTiming(remainingAtSuccess, RealTimeWindowDuration);

            if (isPerfect)
            {
                // Trigger specific 'Perfect' feedback or rewards (e.g. extra Kynde, VFX) can be wired here.
            }

            bool pendingEnemyOffense = _currentAction != null &&
                CurrentCharacter != null &&
                EnemyCharacters.Contains(CurrentCharacter) &&
                IsOffensiveAction(_currentAction.ActionData.ActionType);

            if (pendingEnemyOffense)
            {
                // Resolve the pending enemy attack once reaction input has been captured.
                CurrentCharacter.PlayActionAnimation(_currentAction);
                CurrentCharacter.ExecuteCombatAction(_currentAction, defender);
                OnActionExecuted?.Invoke(CurrentCharacter, defender, _currentAction);
            }

            if (parrySucceeded && defender.IsAlive() && attacker != null && attacker.IsAlive())
                StartCounterWindow(defender, attacker);
            else
                State = CombatState.ProcessingResults;

            _currentAction = null;
            _currentTarget = null;
            ActionCastTimeRemaining = 0f;
            ActionExecutionTimeRemaining = 0f;
        }

        float _counterWindowRemaining;
        MedievalCharacter _counterDefender;
        MedievalCharacter _counterAttacker;

        void StartCounterWindow(MedievalCharacter defender, MedievalCharacter attacker)
        {
            _counterWindowRemaining = 0.5f;
            _counterDefender = defender;
            _counterAttacker = attacker;
            State = CombatState.ProcessingResults;
        }

        public bool IsCounterWindowActive => _counterWindowRemaining > 0f;
        public float CounterWindowRemaining => _counterWindowRemaining;

        public void ExecuteCounter()
        {
            if (!IsCounterWindowActive || _counterDefender == null || _counterAttacker == null) return;
            if (!_counterDefender.IsAlive() || !_counterAttacker.IsAlive()) { EndCounterWindow(); return; }

            var counterAction = GetCounterAction(_counterDefender);
            if (counterAction != null)
            {
                _counterDefender.ExecuteCombatAction(counterAction, _counterAttacker);
                OnActionExecuted?.Invoke(_counterDefender, _counterAttacker, counterAction);
            }
            EndCounterWindow();
            NextTurn();
        }

        void EndCounterWindow()
        {
            _counterWindowRemaining = 0f;
            _counterDefender = null;
            _counterAttacker = null;
        }

        CombatAction GetCounterAction(MedievalCharacter c)
        {
            if (c?.AvailableActions == null) return null;
            foreach (var a in c.AvailableActions)
                if (a != null && a.ActionData.ActionType == CombatActionType.Counter)
                    return a;
            return null;
        }

        /// <summary>Wait one frame then run NextTurn so previous action/anim state can finish. Prevents race conditions from hardcoded Invoke delays.</summary>
        System.Collections.IEnumerator TransitionToNextTurn()
        {
            yield return null;
            NextTurn();
        }

        void Update()
        {
            if (IsCounterWindowActive)
            {
                _counterWindowRemaining -= Time.deltaTime;
                if (_counterWindowRemaining <= 0f)
                {
                    EndCounterWindow();
                    NextTurn();
                }
                return;
            }

            if (State == CombatState.RealTimeWindow)
            {
                RealTimeWindowRemaining -= Time.deltaTime;
                if (RealTimeWindowRemaining <= 0f)
                {
                    ResolveDefenseWindow();
                    if (!IsCounterWindowActive)
                        NextTurn();
                }
                return;
            }

            if (State == CombatState.ExecutingAction)
            {
                if (ActionCastTimeRemaining > 0f)
                {
                    ActionCastTimeRemaining -= Time.deltaTime;
                    if (ActionCastTimeRemaining <= 0f) ProcessActionCast();
                }
                else if (ActionExecutionTimeRemaining > 0f)
                {
                    ActionExecutionTimeRemaining -= Time.deltaTime;
                    if (ActionExecutionTimeRemaining <= 0f) ProcessActionExecution();
                }
            }

            if (State == CombatState.WaitingForInput)
                TryExecuteBufferedAction();
        }
    }
}
