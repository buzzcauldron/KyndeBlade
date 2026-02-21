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

        public CombatState State { get; private set; } = CombatState.CombatEnded;
        public List<MedievalCharacter> PlayerCharacters { get; private set; } = new List<MedievalCharacter>();
        public List<MedievalCharacter> EnemyCharacters { get; private set; } = new List<MedievalCharacter>();
        public List<MedievalCharacter> TurnOrder { get; private set; } = new List<MedievalCharacter>();
        public MedievalCharacter CurrentCharacter { get; private set; }
        public int TurnNumber { get; private set; }
        public float RealTimeWindowRemaining { get; private set; }
        public float ActionCastTimeRemaining { get; private set; }
        public float ActionExecutionTimeRemaining { get; private set; }
        /// <summary>Attacker whose strike is imminent (for strike-warning sound theme).</summary>
        public MedievalCharacter CurrentAttackerDuringWindow { get; private set; }
        /// <summary>Defender (player) who used Escapade/Ward.</summary>
        public MedievalCharacter DefenderDuringWindow { get; private set; }
        /// <summary>Action type that triggered the window (Escapade = dodge, Ward = parry).</summary>
        public CombatActionType ActionTypeDuringWindow { get; private set; }

        CombatAction _currentAction;
        MedievalCharacter _currentTarget;

        public event Action<MedievalCharacter> OnTurnChanged;
        public event Action OnCombatEnded;
        /// <summary>Fired when an action is executed (executor, target, action). For AI observation.</summary>
        public event Action<MedievalCharacter, MedievalCharacter, CombatAction> OnActionExecuted;

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
            CurrentCharacter = TurnOrder[0];
            CurrentCharacter?.InvokeTurnStart();
            OnTurnChanged?.Invoke(CurrentCharacter);
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
                    return;
                }
                i = (i + 1) % TurnOrder.Count;
            } while (i != start);

            State = CombatState.CombatEnded;
            OnCombatEnded?.Invoke();
        }

        public void ExecuteAction(CombatAction action, MedievalCharacter target)
        {
            if (action == null || CurrentCharacter == null || State != CombatState.WaitingForInput) return;

            State = CombatState.ExecutingAction;
            _currentAction = action;
            _currentTarget = target;

            if (action.GetCastTime() > 0f)
                ActionCastTimeRemaining = action.GetCastTime();
            else
                ProcessActionCast();
        }

        void ProcessActionCast()
        {
            if (_currentAction == null || CurrentCharacter == null) return;

            CurrentCharacter.ExecuteCombatAction(_currentAction, _currentTarget);
            OnActionExecuted?.Invoke(CurrentCharacter, _currentTarget, _currentAction);

            if (_currentAction.GetExecutionTime() > 0f)
                ActionExecutionTimeRemaining = _currentAction.GetExecutionTime();
            else
                ProcessActionExecution();
        }

        void ProcessActionExecution()
        {
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
                Invoke(nameof(NextTurn), 0.01f);
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
            RealTimeWindowDuration = duration;
            RealTimeWindowRemaining = duration;
            CurrentAttackerDuringWindow = attacker ?? GetFirstAliveEnemy();
            DefenderDuringWindow = defender;
            ActionTypeDuringWindow = actionType;
            State = CombatState.RealTimeWindow;
        }

        MedievalCharacter GetFirstAliveEnemy()
        {
            foreach (var e in EnemyCharacters)
                if (e != null && e.IsAlive()) return e;
            return null;
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

        void ResolveDefenseWindow()
        {
            var defender = DefenderDuringWindow;
            var attacker = CurrentAttackerDuringWindow;
            var actionType = ActionTypeDuringWindow;
            DefenderDuringWindow = null;
            ActionTypeDuringWindow = CombatActionType.Rest;

            if (defender == null || !defender.IsAlive()) return;
            if (attacker == null || !attacker.IsAlive()) return;

            bool parrySucceeded = actionType == CombatActionType.Ward && defender.IsParrying && defender.AttemptParry();
            float damage = GetAttackerDamage(attacker);
            defender.ApplyCustomDamage(damage, attacker);

            if (parrySucceeded && defender.IsAlive() && attacker != null && attacker.IsAlive())
                StartCounterWindow(defender, attacker);
            else
                State = CombatState.ProcessingResults;
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

        float GetAttackerDamage(MedievalCharacter attacker)
        {
            if (attacker?.AvailableActions == null) return attacker.Stats.AttackPower * 10f;
            foreach (var a in attacker.AvailableActions)
            {
                if (a == null) continue;
                var t = a.ActionData.ActionType;
                if (t == CombatActionType.Strike || t == CombatActionType.RangedStrike)
                    return a.ActionData.Damage;
            }
            return attacker.Stats.AttackPower * 10f;
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
        }
    }
}
