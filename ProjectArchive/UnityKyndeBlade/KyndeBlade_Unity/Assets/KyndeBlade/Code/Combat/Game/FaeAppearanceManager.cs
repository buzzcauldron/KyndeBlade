using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Randomly applies fairy transformation to one character for short periods. Higher likelihood before/after Green Knight appearances.</summary>
    public class FaeAppearanceManager : MonoBehaviour
    {
        [Header("Fairy Transformation")]
        [Tooltip("Base chance (0-1) per encounter to transform a random character into fairy form.")]
        [Range(0f, 1f)]
        public float BaseFaeChance = 0.15f;
        [Tooltip("Bonus chance when near Green Knight (in GK pool or within 2 encounters of GK appearance).")]
        [Range(0f, 1f)]
        public float NearGreenKnightFaeBonus = 0.25f;
        [Tooltip("Duration in seconds that fairy form lasts.")]
        public float FairyFormDuration = 25f;
        [Tooltip("Delay in seconds after combat start before first fae roll.")]
        public float InitialDelay = 5f;
        [Tooltip("Min seconds between fae transformation rolls.")]
        public float MinIntervalBetweenFae = 15f;

        SaveManager _save;
        TurnManager _turnManager;
        float _nextRollTime;
        MedievalCharacter _currentFairyCharacter;

        void Start()
        {
            _save = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            _turnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (_turnManager != null)
                _turnManager.OnCombatEnded += OnCombatEnded;
            _nextRollTime = Time.time + InitialDelay;
        }

        void OnDestroy()
        {
            if (_turnManager != null)
                _turnManager.OnCombatEnded -= OnCombatEnded;
            if (_currentFairyCharacter != null)
                _currentFairyCharacter.RemoveFairyForm();
        }

        void Update()
        {
            if (_turnManager == null || _turnManager.State == CombatState.CombatEnded)
                return;

            if (_currentFairyCharacter != null && !_currentFairyCharacter.IsFairyForm)
                _currentFairyCharacter = null;

            if (Time.time >= _nextRollTime)
            {
                _nextRollTime = Time.time + MinIntervalBetweenFae;
                TryApplyFairyTransformation();
            }
        }

        void OnCombatEnded()
        {
            if (_currentFairyCharacter != null)
            {
                _currentFairyCharacter.RemoveFairyForm();
                _currentFairyCharacter = null;
            }
        }

        void TryApplyFairyTransformation()
        {
            float chance = GetFaeChance();
            if (UnityEngine.Random.value > chance)
                return;

            var all = new List<MedievalCharacter>();
            if (_turnManager != null)
            {
                all.AddRange(_turnManager.PlayerCharacters ?? new List<MedievalCharacter>());
                all.AddRange(_turnManager.EnemyCharacters ?? new List<MedievalCharacter>());
            }
            all.RemoveAll(c => c == null || !c.IsAlive() || c.IsFairyForm);

            if (all.Count == 0)
                return;

            var pick = all[UnityEngine.Random.Range(0, all.Count)];
            if (_currentFairyCharacter != null)
                _currentFairyCharacter.RemoveFairyForm();
            pick.ApplyFairyForm(FairyFormDuration);
            _currentFairyCharacter = pick;
        }

        float GetFaeChance()
        {
            float chance = BaseFaeChance;
            bool nearGreenKnight = false;
            if (_save?.CurrentProgress != null)
            {
                nearGreenKnight = _save.GreenKnightWillAppearRandomly ||
                    _save.CurrentProgress.EncountersSinceLastGreenKnight <= 2;
            }
            if (nearGreenKnight)
                chance += NearGreenKnightFaeBonus;
            return Mathf.Clamp01(chance);
        }
    }
}
