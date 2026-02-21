using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Wires CombatFeedback to character events (Hodent: Perception).</summary>
    public class CombatFeedbackManager : MonoBehaviour
    {
        public TurnManager TurnManager;
        public CombatFeedback Feedback;

        Dictionary<MedievalCharacter, (Action<MedievalCharacter, bool>, Action<MedievalCharacter, bool>, Action<MedievalCharacter, MedievalCharacter, float>)> _handlers = new Dictionary<MedievalCharacter, (Action<MedievalCharacter, bool>, Action<MedievalCharacter, bool>, Action<MedievalCharacter, MedievalCharacter, float>)>();

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (Feedback == null) Feedback = FindObjectOfType<CombatFeedback>();

            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged += OnTurnChanged;
                TurnManager.OnCombatEnded += UnsubscribeAll;
            }
        }

        void OnDestroy()
        {
            UnsubscribeAll();
            if (TurnManager != null) { TurnManager.OnTurnChanged -= OnTurnChanged; TurnManager.OnCombatEnded -= UnsubscribeAll; }
        }

        void OnTurnChanged(MedievalCharacter _)
        {
            SubscribeToAllInCombat();
        }

        void SubscribeToAllInCombat()
        {
            UnsubscribeAll();
            if (TurnManager == null || Feedback == null) return;

            foreach (var c in TurnManager.PlayerCharacters)
                if (c != null) Subscribe(c);
            foreach (var c in TurnManager.EnemyCharacters)
                if (c != null) Subscribe(c);
        }

        void Subscribe(MedievalCharacter c)
        {
            if (_handlers.ContainsKey(c)) return;

            Action<MedievalCharacter, bool> dodge = (ch, ok) => { if (ok) Feedback.OnDodgeSuccess(ch); else Feedback.OnDodgeFail(ch); };
            Action<MedievalCharacter, bool> parry = (ch, ok) => { if (ok) Feedback.OnParrySuccess(ch); else Feedback.OnParryFail(ch); };
            Action<MedievalCharacter, MedievalCharacter, float> dmg = (_, t, __) => Feedback.OnDamageDealt(t);

            c.OnDodgeAttempted += dodge;
            c.OnParryAttempted += parry;
            c.OnDamageDealt += dmg;
            _handlers[c] = (dodge, parry, dmg);
        }

        void UnsubscribeAll()
        {
            foreach (var kv in _handlers)
            {
                if (kv.Key == null) continue;
                kv.Key.OnDodgeAttempted -= kv.Value.Item1;
                kv.Key.OnParryAttempted -= kv.Value.Item2;
                kv.Key.OnDamageDealt -= kv.Value.Item3;
            }
            _handlers.Clear();
        }
    }
}
