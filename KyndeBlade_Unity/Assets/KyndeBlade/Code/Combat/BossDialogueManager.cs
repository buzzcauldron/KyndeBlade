using System;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>State-driven boss dialogue: links BossDialogueScript to phase (entry/midpoint/desperation/defeat) and action barks. Subscribe to OnBossLineRequested to show speech bubble or manuscript UI.</summary>
    public class BossDialogueManager : MonoBehaviour
    {
        public static BossDialogueManager Instance { get; private set; }

        [Header("Optional")]
        [Tooltip("If set, action barks are triggered at telegraph start (wind-up).")]
        public TurnSequenceController SequenceController;

        /// <summary>Fired when a line should be shown. Args: (line text, speaker character). UI can show near speaker or in panel.</summary>
        public event Action<string, MedievalCharacter> OnBossLineRequested;

        MedievalCharacter _currentBoss;
        BossDialogueScript _currentScript;
        bool _entryPlayed;
        bool _midpointPlayed;
        bool _desperationPlayed;
        Action _unsubscribeDefeat;

        void Awake()
        {
            if (Instance == null) Instance = this;
        }

        void Start()
        {
            if (SequenceController == null) SequenceController = FindObjectOfType<TurnSequenceController>();
            if (SequenceController != null)
                SequenceController.OnTelegraphStarted += OnTelegraphStarted;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (SequenceController != null)
                SequenceController.OnTelegraphStarted -= OnTelegraphStarted;
            _unsubscribeDefeat?.Invoke();
        }

        void OnTelegraphStarted(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (actor == _currentBoss && action != null)
                TriggerActionBark(action.ActionData.ActionName);
        }

        /// <summary>Register the current boss and script for this encounter. Call when combat starts or when this boss becomes active.</summary>
        public void SetCurrentBoss(MedievalCharacter boss, BossDialogueScript script)
        {
            _unsubscribeDefeat?.Invoke();
            _unsubscribeDefeat = null;

            _currentBoss = boss;
            _currentScript = script;
            _entryPlayed = false;
            _midpointPlayed = false;
            _desperationPlayed = false;

            if (boss != null && script != null)
            {
                boss.OnCharacterDefeated += OnBossDefeated;
                _unsubscribeDefeat = () => { if (boss != null) boss.OnCharacterDefeated -= OnBossDefeated; };
            }
        }

        void OnBossDefeated(MedievalCharacter _)
        {
            if (_currentScript != null && !string.IsNullOrEmpty(_currentScript.BossDefeatedLine))
                RequestLine(_currentScript.BossDefeatedLine, _currentBoss);
            _unsubscribeDefeat?.Invoke();
            _unsubscribeDefeat = null;
        }

        /// <summary>Play entry line (greeting). Call once on first boss turn.</summary>
        public void TriggerEntry()
        {
            if (_currentScript == null || _entryPlayed || string.IsNullOrEmpty(_currentScript.EntryLine)) return;
            _entryPlayed = true;
            RequestLine(_currentScript.EntryLine, _currentBoss);
        }

        /// <summary>Play midpoint line (escalation). Call when hpRatio &lt; 0.6 or phase 2.</summary>
        public void TriggerMidpoint()
        {
            if (_currentScript == null || _midpointPlayed || string.IsNullOrEmpty(_currentScript.MidPointLine)) return;
            _midpointPlayed = true;
            RequestLine(_currentScript.MidPointLine, _currentBoss);
        }

        /// <summary>Play desperation line (climax). Call when hpRatio &lt; 0.2 or phase 3.</summary>
        public void TriggerDesperation()
        {
            if (_currentScript == null || _desperationPlayed || string.IsNullOrEmpty(_currentScript.DefeatLine)) return;
            _desperationPlayed = true;
            RequestLine(_currentScript.DefeatLine, _currentBoss);
        }

        /// <summary>Play the bark for this action name (from script's ActionBarks). Call when boss uses an action (e.g. at telegraph).</summary>
        public void TriggerActionBark(string actionName)
        {
            if (_currentScript == null || string.IsNullOrEmpty(actionName)) return;
            string line = _currentScript.GetBarkForAction(actionName);
            if (!string.IsNullOrEmpty(line))
                RequestLine(line, _currentBoss);
        }

        void RequestLine(string line, MedievalCharacter speaker)
        {
            OnBossLineRequested?.Invoke(line, speaker);
        }
    }
}
