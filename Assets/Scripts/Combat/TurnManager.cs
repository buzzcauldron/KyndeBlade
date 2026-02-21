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
            CurrentCharacter?.OnTurnStart?.Invoke();
            OnTurnChanged?.Invoke(CurrentCharacter);
        }

        public void EndTurn()
        {
            CurrentCharacter?.OnTurnEnd?.Invoke();
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
                    CurrentCharacter?.OnTurnStart?.Invoke();
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
                var attacker = _currentTarget != null && EnemyCharacters.Contains(_currentTarget)
                    ? _currentTarget
                    : GetFirstAliveEnemy();
                StartRealTimeWindow(_currentAction.ActionData.SuccessWindow, attacker);
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

        public void StartRealTimeWindow(float duration, MedievalCharacter attacker = null)
        {
            if (Settings != null)
                duration = Settings.GetAdjustedWindow(duration);
            RealTimeWindowDuration = duration;
            RealTimeWindowRemaining = duration;
            CurrentAttackerDuringWindow = attacker ?? GetFirstAliveEnemy();
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

        void Update()
        {
            if (State == CombatState.RealTimeWindow)
            {
                RealTimeWindowRemaining -= Time.deltaTime;
                if (RealTimeWindowRemaining <= 0f)
                {
                    State = CombatState.ProcessingResults;
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
