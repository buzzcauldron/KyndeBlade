using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Pride boss AI: prefers Pride's Fall when multiple targets, The Tower when low health.</summary>
    [RequireComponent(typeof(MedievalCharacter))]
    public class PrideBossAI : MonoBehaviour
    {
        public TurnManager TurnManager;
        public float DecisionDelay = 0.5f;

        MedievalCharacter _self;
        PrideCharacter _pride;

        void Start()
        {
            _self = GetComponent<MedievalCharacter>();
            _pride = GetComponent<PrideCharacter>();
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager != null)
                TurnManager.OnTurnChanged += OnTurnChanged;
        }

        void OnDestroy()
        {
            if (TurnManager != null)
                TurnManager.OnTurnChanged -= OnTurnChanged;
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (current != _self) return;
            if (TurnManager != null && TurnManager.IsPlayerTurn()) return;
            Invoke(nameof(ExecuteTurn), DecisionDelay);
        }

        void ExecuteTurn()
        {
            if (TurnManager == null || TurnManager.CurrentCharacter != _self) return;
            if (!_self.IsAlive()) return;

            var target = ChooseTarget();
            var action = ChoosePrideAction(target);
            if (action != null)
                TurnManager.ExecuteAction(action, target ?? ChooseTarget());
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

        CombatAction ChoosePrideAction(MedievalCharacter target)
        {
            if (_self.AvailableActions == null || _self.AvailableActions.Count == 0) return null;

            int aliveCount = 0;
            foreach (var p in TurnManager.PlayerCharacters)
                if (p != null && p.IsAlive()) aliveCount++;

            float hpRatio = _self.GetCurrentHealth() / Mathf.Max(1f, _self.GetMaxHealth());
            var affordable = new List<CombatAction>();
            foreach (var a in _self.AvailableActions)
            {
                if (a == null) continue;
                if (a.ActionData.StaminaCost <= _self.GetCurrentStamina())
                    affordable.Add(a);
            }
            if (affordable.Count == 0) return null;

            CombatAction prefer = null;
            foreach (var a in affordable)
            {
                if (a.ActionData.ActionName == "The Tower" && hpRatio < 0.4f)
                { prefer = a; break; }
                if (a.ActionData.ActionName == "Pride's Fall" && aliveCount >= 2)
                { prefer = a; break; }
                if (a.ActionData.ActionName == "Pride's Arrogance" && target != null)
                { prefer = a; break; }
            }

            if (prefer != null) return prefer;

            foreach (var a in affordable)
            {
                if (a.ActionData.ActionType == CombatActionType.Strike && target != null)
                    return a;
                if (a.ActionData.ActionType == CombatActionType.Special)
                    return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest)
                    return a;

            return affordable.Count > 0 ? affordable[0] : null;
        }
    }
}
