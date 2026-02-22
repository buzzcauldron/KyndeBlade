using System;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Dynamic adaptation AI: learns from player behavior and adapts action selection.
    /// - Tracks player dodge/parry success rates to favor actions that land
    /// - Adapts to combat state (stamina, health, broken targets)
    /// - Varies behavior to avoid predictability
    /// </summary>
    [RequireComponent(typeof(MedievalCharacter))]
    public class AdaptiveEnemyAI : MonoBehaviour
    {
        [Header("Timing")]
        [Tooltip("Assign in Inspector to avoid FindObjectOfType; otherwise cached once in Start().")]
        public TurnManager TurnManager;
        public float DecisionDelay = 0.5f;

        [Header("Adaptation")]
        [Tooltip("How quickly AI adapts (0=static, 1=instant).")]
        [Range(0f, 1f)]
        public float AdaptationSpeed = 0.4f;
        [Tooltip("Base randomness to avoid predictability (0-1).")]
        [Range(0f, 0.4f)]
        public float Unpredictability = 0.15f;

        MedievalCharacter _self;
        PlayerBehaviorProfile _profile = new PlayerBehaviorProfile();
        CombatAction _lastExecutedAction;
        MedievalCharacter _lastTarget;
        Dictionary<MedievalCharacter, (Action<MedievalCharacter, bool>, Action<MedievalCharacter, bool>, Action<MedievalCharacter, MedievalCharacter, float>)> _playerHandlers = new Dictionary<MedievalCharacter, (Action<MedievalCharacter, bool>, Action<MedievalCharacter, bool>, Action<MedievalCharacter, MedievalCharacter, float>)>();
        readonly List<CombatAction> _affordableBuffer = new List<CombatAction>();
        readonly Dictionary<CombatAction, float> _scoreBuffer = new Dictionary<CombatAction, float>();

        void Start()
        {
            _self = GetComponent<MedievalCharacter>();
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager == null)
                LogMissingTurnManagerOnce();
            else
            {
                TurnManager.OnTurnChanged += OnTurnChanged;
                TurnManager.OnActionExecuted += OnActionExecuted;
                TurnManager.OnCombatEnded += OnCombatEnded;
            }
        }

        static bool _warnedTurnManager;
        static void LogMissingTurnManagerOnce()
        {
            if (_warnedTurnManager) return;
            _warnedTurnManager = true;
            Debug.LogWarning("[AdaptiveEnemyAI] TurnManager not found. Assign in Inspector or ensure one exists in scene. Cached in Start(); no per-frame search.");
        }

        void OnDestroy()
        {
            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnActionExecuted -= OnActionExecuted;
                TurnManager.OnCombatEnded -= OnCombatEnded;
            }
            UnsubscribeFromPlayers();
        }

        void OnCombatEnded()
        {
            UnsubscribeFromPlayers();
        }

        void SubscribeToPlayers()
        {
            if (TurnManager == null) return;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || _playerHandlers.ContainsKey(p)) continue;

                Action<MedievalCharacter, bool> dodge = (ch, ok) => RecordDodgeResult(ok);
                Action<MedievalCharacter, bool> parry = (ch, ok) => RecordParryResult(ok);
                Action<MedievalCharacter, MedievalCharacter, float> dmg = (_, __, ___) => { };

                p.OnDodgeAttempted += dodge;
                p.OnParryAttempted += parry;
                p.OnDamageDealt += dmg;
                _playerHandlers[p] = (dodge, parry, dmg);
            }
        }

        void UnsubscribeFromPlayers()
        {
            foreach (var kv in _playerHandlers)
            {
                if (kv.Key == null) continue;
                kv.Key.OnDodgeAttempted -= kv.Value.Item1;
                kv.Key.OnParryAttempted -= kv.Value.Item2;
                kv.Key.OnDamageDealt -= kv.Value.Item3;
            }
            _playerHandlers.Clear();
        }

        void RecordDodgeResult(bool success)
        {
            if (_lastExecutedAction == null) return;
            if (_lastExecutedAction.ActionData.ActionType != CombatActionType.Escapade) return;

            if (success)
            {
                _profile.DodgeSuccesses++;
                _profile.LastReactionWasSuccess = true;
            }
            else
            {
                _profile.DodgeFails++;
                _profile.LastReactionWasSuccess = false;
            }
        }

        void RecordParryResult(bool success)
        {
            if (_lastExecutedAction == null) return;
            if (_lastExecutedAction.ActionData.ActionType != CombatActionType.Ward) return;

            if (success)
            {
                _profile.ParrySuccesses++;
                _profile.LastReactionWasSuccess = true;
            }
            else
            {
                _profile.ParryFails++;
                _profile.LastReactionWasSuccess = false;
            }
        }

        void OnActionExecuted(MedievalCharacter executor, MedievalCharacter target, CombatAction action)
        {
            if (executor == _self)
            {
                _lastExecutedAction = action;
                _lastTarget = target;
            }
            else if (TurnManager != null && TurnManager.PlayerCharacters.Contains(executor))
            {
                _profile.RecordPlayerAction(action?.ActionData.ActionType ?? CombatActionType.Rest);
            }
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (current != _self) return;
            if (TurnManager != null && TurnManager.IsPlayerTurn()) return;

            SubscribeToPlayers();
            Invoke(nameof(ExecuteTurn), DecisionDelay);
        }

        void ExecuteTurn()
        {
            if (TurnManager == null || TurnManager.CurrentCharacter != _self) return;
            if (!_self.IsAlive()) return;

            var target = ChooseTarget();
            if (target == null) return;

            var action = ChooseActionAdaptive(target);
            if (action != null)
                TurnManager.ExecuteAction(action, target);
            else
                TurnManager.NextTurn();
        }

        MedievalCharacter ChooseTarget()
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

        CombatAction ChooseActionAdaptive(MedievalCharacter target)
        {
            if (_self.AvailableActions == null || _self.AvailableActions.Count == 0) return null;

            _affordableBuffer.Clear();
            foreach (var a in _self.AvailableActions)
            {
                if (a == null) continue;
                if (a.ActionData.StaminaCost <= _self.GetCurrentStamina())
                    _affordableBuffer.Add(a);
            }
            if (_affordableBuffer.Count == 0) return null;

            _scoreBuffer.Clear();
            float staminaRatio = _self.GetCurrentStamina() / Mathf.Max(1f, _self.GetMaxStamina());
            float healthRatio = _self.GetCurrentHealth() / Mathf.Max(1f, _self.GetMaxHealth());
            bool targetBroken = target != null && target.IsBroken();
            bool targetLowHp = target != null && target.GetCurrentHealth() < target.GetMaxHealth() * 0.3f;

            foreach (var a in affordable)
            {
                float score = 1f;

                switch (a.ActionData.ActionType)
                {
                    case CombatActionType.Rest:
                        if (staminaRatio < 0.3f) score += 2f;
                        else if (staminaRatio < 0.5f) score += 1f;
                        else score -= 0.5f;
                        break;

                    case CombatActionType.Strike:
                        if (targetBroken) score += 1.5f;
                        if (targetLowHp) score += 1f;
                        score += 0.5f;
                        break;

                    case CombatActionType.Escapade:
                        float dodgeFailRate = _profile.GetDodgeFailRate();
                        if (dodgeFailRate > 0.5f) score += 1f;
                        else if (dodgeFailRate < 0.3f) score -= 1f;
                        score += UnityEngine.Random.Range(-Unpredictability, Unpredictability);
                        break;

                    case CombatActionType.Ward:
                        float parryFailRate = _profile.GetParryFailRate();
                        if (parryFailRate > 0.5f) score += 1f;
                        else if (parryFailRate < 0.3f) score -= 1f;
                        score += UnityEngine.Random.Range(-Unpredictability, Unpredictability);
                        break;

                    default:
                        score += 0.3f;
                        break;
                }

                _scoreBuffer[a] = Mathf.Max(0.01f, score);
            }

            return WeightedRandomChoice(_affordableBuffer, _scoreBuffer);
        }

        CombatAction WeightedRandomChoice(List<CombatAction> options, Dictionary<CombatAction, float> scores)
        {
            float total = 0f;
            foreach (var a in options)
                total += scores.TryGetValue(a, out var s) ? s : 1f;

            float r = UnityEngine.Random.Range(0f, total);
            foreach (var a in options)
            {
                float w = scores.TryGetValue(a, out var s) ? s : 1f;
                if (r <= w) return a;
                r -= w;
            }
            return options[options.Count - 1];
        }

        class PlayerBehaviorProfile
        {
            public int DodgeSuccesses, DodgeFails, ParrySuccesses, ParryFails;
            public int PlayerStrikes, PlayerEscapades, PlayerWards, PlayerRests;
            public bool LastReactionWasSuccess;

            public void RecordPlayerAction(CombatActionType type)
            {
                switch (type)
                {
                    case CombatActionType.Strike: PlayerStrikes++; break;
                    case CombatActionType.Escapade: PlayerEscapades++; break;
                    case CombatActionType.Ward: PlayerWards++; break;
                    case CombatActionType.Rest: PlayerRests++; break;
                }
            }

            public float GetDodgeFailRate()
            {
                int total = DodgeSuccesses + DodgeFails;
                if (total < 2) return 0.5f;
                return (float)DodgeFails / total;
            }

            public float GetParryFailRate()
            {
                int total = ParrySuccesses + ParryFails;
                if (total < 2) return 0.5f;
                return (float)ParryFails / total;
            }
        }
    }
}
