using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Lightweight combat instrumentation for week-1 balancing.
    /// Emits structured logs for ability usage, hits, damage taken, death cause, and wave time.
    /// </summary>
    public class CombatMetricsManager : MonoBehaviour
    {
        [Header("Instrumentation")]
        [SerializeField] bool logToConsole = true;

        readonly HashSet<MedievalCharacter> _trackedCombatants = new HashSet<MedievalCharacter>();
        readonly Dictionary<MedievalCharacter, float> _lastKnownHealth = new Dictionary<MedievalCharacter, float>();
        readonly Dictionary<MedievalCharacter, string> _lastDamagerByTarget = new Dictionary<MedievalCharacter, string>();

        TurnManager _turnManager;
        bool _encounterActive;
        string _encounterId = "unknown_encounter";
        float _encounterStartTime;

        int _abilityUsedCount;
        int _hitCount;
        int _damageTakenCount;
        int _deathCount;
        float _totalDamageTaken;

        void OnEnable()
        {
            EnsureTurnManagerSubscription();
        }

        void OnDisable()
        {
            UnsubscribeTurnManager();
            UnsubscribeCombatants();
        }

        void Update()
        {
            if (_turnManager == null)
                EnsureTurnManagerSubscription();
        }

        public void BeginEncounter(string encounterId, IList<MedievalCharacter> players, IList<MedievalCharacter> enemies)
        {
            if (_encounterActive)
                EndEncounter("restart");

            EnsureTurnManagerSubscription();

            _encounterActive = true;
            _encounterId = string.IsNullOrWhiteSpace(encounterId) ? "unknown_encounter" : encounterId;
            _encounterStartTime = Time.time;

            _abilityUsedCount = 0;
            _hitCount = 0;
            _damageTakenCount = 0;
            _deathCount = 0;
            _totalDamageTaken = 0f;

            _lastKnownHealth.Clear();
            _lastDamagerByTarget.Clear();
            UnsubscribeCombatants();

            RegisterCombatants(players);
            RegisterCombatants(enemies);

            EmitMetric("encounter_start", "encounter=" + _encounterId);
        }

        public void EndEncounter(string reason = "completed")
        {
            if (!_encounterActive)
                return;

            float waveTime = Mathf.Max(0f, Time.time - _encounterStartTime);
            EmitMetric(
                "wave_time",
                "encounter=" + _encounterId +
                " seconds=" + waveTime.ToString("F2") +
                " reason=" + reason +
                " ability_used=" + _abilityUsedCount +
                " hit=" + _hitCount +
                " damage_taken=" + _damageTakenCount +
                " deaths=" + _deathCount +
                " total_damage_taken=" + _totalDamageTaken.ToString("F1"));

            _encounterActive = false;
            UnsubscribeCombatants();
        }

        void EnsureTurnManagerSubscription()
        {
            if (_turnManager == null)
                _turnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (_turnManager == null)
                return;

            _turnManager.OnActionExecuted -= HandleActionExecuted;
            _turnManager.OnCombatEnded -= HandleCombatEnded;
            _turnManager.OnActionExecuted += HandleActionExecuted;
            _turnManager.OnCombatEnded += HandleCombatEnded;
        }

        void UnsubscribeTurnManager()
        {
            if (_turnManager == null)
                return;

            _turnManager.OnActionExecuted -= HandleActionExecuted;
            _turnManager.OnCombatEnded -= HandleCombatEnded;
            _turnManager = null;
        }

        void RegisterCombatants(IList<MedievalCharacter> combatants)
        {
            if (combatants == null)
                return;

            for (int i = 0; i < combatants.Count; i++)
                RegisterCombatant(combatants[i]);
        }

        void RegisterCombatant(MedievalCharacter character)
        {
            if (character == null || _trackedCombatants.Contains(character))
                return;

            _trackedCombatants.Add(character);
            _lastKnownHealth[character] = character.GetCurrentHealth();

            character.OnDamageDealt += HandleDamageDealt;
            character.OnCharacterDefeated += HandleCharacterDefeated;
        }

        void UnsubscribeCombatants()
        {
            foreach (var character in _trackedCombatants)
            {
                if (character == null)
                    continue;
                character.OnDamageDealt -= HandleDamageDealt;
                character.OnCharacterDefeated -= HandleCharacterDefeated;
            }

            _trackedCombatants.Clear();
        }

        void HandleActionExecuted(MedievalCharacter executor, MedievalCharacter target, CombatAction action)
        {
            if (!_encounterActive || action == null)
                return;

            _abilityUsedCount++;
            string actionName = action.ActionData != null ? action.ActionData.ActionType.ToString() : "unknown_action";
            EmitMetric("ability_used", "encounter=" + _encounterId + " actor=" + GetCharacterName(executor) + " action=" + actionName);

            if (target == null)
                return;

            float previousHealth;
            if (_lastKnownHealth.TryGetValue(target, out previousHealth))
            {
                float currentHealth = target.GetCurrentHealth();
                if (currentHealth < previousHealth)
                {
                    _hitCount++;
                    float damage = previousHealth - currentHealth;
                    EmitMetric(
                        "hit",
                        "encounter=" + _encounterId +
                        " source=" + GetCharacterName(executor) +
                        " target=" + GetCharacterName(target) +
                        " damage=" + damage.ToString("F1"));
                }

                _lastKnownHealth[target] = currentHealth;
            }
        }

        void HandleDamageDealt(MedievalCharacter attacker, MedievalCharacter target, float damage)
        {
            if (!_encounterActive || target == null || damage <= 0f)
                return;

            _damageTakenCount++;
            _totalDamageTaken += damage;
            _lastDamagerByTarget[target] = GetCharacterName(attacker);

            EmitMetric(
                "damage_taken",
                "encounter=" + _encounterId +
                " target=" + GetCharacterName(target) +
                " source=" + GetCharacterName(attacker) +
                " amount=" + damage.ToString("F1"));
        }

        void HandleCharacterDefeated(MedievalCharacter defeated)
        {
            if (!_encounterActive || defeated == null)
                return;

            _deathCount++;
            string cause = "unknown";
            if (_lastDamagerByTarget.TryGetValue(defeated, out string source))
                cause = source;

            EmitMetric(
                "death_cause",
                "encounter=" + _encounterId +
                " target=" + GetCharacterName(defeated) +
                " source=" + cause);
        }

        void HandleCombatEnded()
        {
            EndEncounter("combat_ended");
        }

        static string GetCharacterName(MedievalCharacter character)
        {
            if (character == null)
                return "none";
            if (!string.IsNullOrWhiteSpace(character.CharacterName))
                return character.CharacterName;
            return character.name;
        }

        void EmitMetric(string metricName, string payload)
        {
            if (!logToConsole)
                return;

            Debug.Log("[CombatMetric] metric=" + metricName + " " + payload, this);
        }
    }
}
