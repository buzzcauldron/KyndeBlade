using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Applies Piers-themed hazards each turn during combat. Limbo-style environmental effects.</summary>
    public class CombatHazardManager : MonoBehaviour
    {
        public TurnManager TurnManager;
        List<PiersHazardConfig> _hazards = new List<PiersHazardConfig>();
        int _turnCount;

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager != null)
                TurnManager.OnTurnChanged += OnTurnChanged;
        }

        void OnDestroy()
        {
            if (TurnManager != null)
                TurnManager.OnTurnChanged -= OnTurnChanged;
        }

        public void SetHazards(List<PiersHazardConfig> hazards)
        {
            _hazards = hazards != null ? new List<PiersHazardConfig>(hazards) : new List<PiersHazardConfig>();
            _turnCount = 0;
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (TurnManager == null) return;
            _turnCount++;
            if (!TurnManager.IsPlayerTurn()) return;
            foreach (var h in _hazards)
            {
                if (h == null) continue;
                if (_turnCount % h.IntervalTurns != 0) continue;
                ApplyHazard(h);
            }
        }

        void ApplyHazard(PiersHazardConfig h)
        {
            switch (h.Type)
            {
                case PiersHazardType.Exhaustion:
                    foreach (var p in TurnManager.PlayerCharacters)
                        if (p != null && p.IsAlive())
                            p.ConsumeStamina(h.Strength * 5f);
                    break;
                case PiersHazardType.Poverty:
                    foreach (var p in TurnManager.PlayerCharacters)
                        if (p != null && p.IsAlive() && p.GetCurrentKynde() >= 1f)
                            p.ConsumeKynde(h.Strength);
                    break;
                case PiersHazardType.Labor:
                    foreach (var p in TurnManager.PlayerCharacters)
                        if (p != null && p.IsAlive())
                            p.ConsumeStamina(h.Strength * 3f);
                    break;
                case PiersHazardType.Hunger:
                    foreach (var p in TurnManager.PlayerCharacters)
                        if (p != null && p.IsAlive())
                            p.ApplyHunger(1, 8f);
                    break;
            }
        }
    }
}
