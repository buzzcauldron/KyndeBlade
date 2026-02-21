using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Hunger boss AI: prefers Hunger abilities, uses Feast of Want when many players hungry.</summary>
    [RequireComponent(typeof(MedievalCharacter))]
    public class HungerBossAI : MonoBehaviour
    {
        public TurnManager TurnManager;
        public float DecisionDelay = 0.5f;

        MedievalCharacter _self;
        HungerCharacter _hunger;

        void Start()
        {
            _self = GetComponent<MedievalCharacter>();
            _hunger = GetComponent<HungerCharacter>();
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
            var action = ChooseHungerAction(target);
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

        CombatAction ChooseHungerAction(MedievalCharacter target)
        {
            if (TurnManager == null || _self.AvailableActions == null || _self.AvailableActions.Count == 0) return null;

            int hungryCount = 0;
            foreach (var p in TurnManager.PlayerCharacters)
                if (p != null && p.IsAlive() && p.IsHungry()) hungryCount++;

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
                if (a.ActionData.ActionName == "The Feast of Want" && hungryCount >= 2 && hpRatio < 0.6f)
                { prefer = a; break; }
                if (a.ActionData.ActionName == "The Unending Need" && _hunger != null && _hunger.CurrentPhase >= 2)
                { prefer = a; break; }
                if (a.ActionData.ActionName == "The Empty Belly" && hungryCount < 2)
                { prefer = a; break; }
                if (a.ActionData.ActionName == "Hunger's Grip" && hungryCount == 0)
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
