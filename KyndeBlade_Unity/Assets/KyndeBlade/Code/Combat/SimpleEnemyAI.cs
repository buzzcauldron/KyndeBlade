using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Simple AI for enemy turns (enables core loop to complete).</summary>
    [RequireComponent(typeof(MedievalCharacter))]
    public class SimpleEnemyAI : MonoBehaviour
    {
        public TurnManager TurnManager;
        public float DecisionDelay = 0.5f;

        MedievalCharacter _self;

        void Start()
        {
            _self = GetComponent<MedievalCharacter>();
            if (TurnManager == null) TurnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (TurnManager != null)
                TurnManager.OnTurnChanged += OnTurnChanged;
        }

        void OnDestroy()
        {
            if (TurnManager != null) TurnManager.OnTurnChanged -= OnTurnChanged;
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (current != _self) return;
            if (TurnManager.IsPlayerTurn()) return; // Player turn, human will act

            Invoke(nameof(ExecuteTurn), DecisionDelay);
        }

        void ExecuteTurn()
        {
            if (TurnManager == null || TurnManager.CurrentCharacter != _self) return;
            if (!_self.IsAlive()) return;

            var target = GetFirstPlayer();
            if (target == null) return;

            var action = ChooseAction();
            if (action != null)
                TurnManager.ExecuteAction(action, target);
            else
                TurnManager.NextTurn();
        }

        MedievalCharacter GetFirstPlayer()
        {
            foreach (var p in TurnManager.PlayerCharacters)
                if (p != null && p.IsAlive()) return p;
            return null;
        }

        CombatAction ChooseAction()
        {
            if (_self.AvailableActions == null || _self.AvailableActions.Count == 0) return null;
            foreach (var a in _self.AvailableActions)
            {
                if (a == null) continue;
                if (a.ActionData.StaminaCost <= _self.GetCurrentStamina())
                    return a;
            }
            return null;
        }
    }
}
