using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Save/load game progress. Phase 1: checkpoint per location.</summary>
    public class SaveManager : MonoBehaviour
    {
        const string SaveKey = "KyndeBlade_Save";

        public GameProgress CurrentProgress { get; private set; }

        public event Action<GameProgress> OnProgressLoaded;
        public event Action<GameProgress> OnProgressSaved;

        void Awake()
        {
            Load();
        }

        public void Load()
        {
            var json = PlayerPrefs.GetString(SaveKey, null);
            if (string.IsNullOrEmpty(json))
            {
                CurrentProgress = GameProgress.CreateNew("malvern");
                OnProgressLoaded?.Invoke(CurrentProgress);
                return;
            }
            try
            {
                CurrentProgress = GameProgress.FromJson(json);
                OnProgressLoaded?.Invoke(CurrentProgress);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"SaveManager: Failed to load save: {e.Message}");
                CurrentProgress = GameProgress.CreateNew("malvern");
            }
        }

        public void Save()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.SaveTimestamp = DateTime.UtcNow.ToString("o");
            var json = CurrentProgress.ToJson();
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            OnProgressSaved?.Invoke(CurrentProgress);
        }

        public void SaveCheckpoint(string locationId)
        {
            if (CurrentProgress == null)
                CurrentProgress = GameProgress.CreateNew(locationId);
            else
            {
                CurrentProgress.CurrentLocationId = locationId;
                if (!CurrentProgress.VisitedLocationIds.Contains(locationId))
                    CurrentProgress.VisitedLocationIds.Add(locationId);
                if (!CurrentProgress.UnlockedLocationIds.Contains(locationId))
                    CurrentProgress.UnlockedLocationIds.Add(locationId);
            }
            Save();
        }

        public void UnlockLocation(string locationId)
        {
            if (CurrentProgress == null) return;
            if (!CurrentProgress.UnlockedLocationIds.Contains(locationId))
                CurrentProgress.UnlockedLocationIds.Add(locationId);
        }

        public bool HasVisited(string locationId)
        {
            return CurrentProgress?.VisitedLocationIds?.Contains(locationId) ?? false;
        }

        public bool IsUnlocked(string locationId)
        {
            return CurrentProgress?.UnlockedLocationIds?.Contains(locationId) ?? false;
        }

        public void NewGame(string startLocationId = "malvern")
        {
            CurrentProgress = GameProgress.CreateNew(startLocationId);
            Save();
        }

        public void SetGreenKnightWillAppearRandomly(bool value)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.GreenKnightWillAppearRandomly = value;
            Save();
        }

        public bool GreenKnightWillAppearRandomly => CurrentProgress?.GreenKnightWillAppearRandomly ?? false;

        public void SetPovertyLevel(int level)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.PovertyLevel = Mathf.Clamp(level, 0, 5);
            Save();
        }

        public void SetOrfeoOtherworldTriggered(bool value)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.OrfeoOtherworldTriggered = value;
            Save();
        }

        public bool OrfeoOtherworldTriggered => CurrentProgress?.OrfeoOtherworldTriggered ?? false;

        public void IncrementGreenChapelBodies()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.GreenChapelBodiesAccrued++;
            Save();
        }

        public void IncrementOtherworldLivingCharacters()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.OtherworldLivingCharactersAccrued++;
            Save();
        }

        /// <summary>Every death adds a form of the body to Orfeo's Otherworld.</summary>
        public void IncrementOtherworldBodiesFromDeath()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.OtherworldBodiesFromDeath++;
            Save();
        }

        /// <summary>Elde (Old Age) hit a character. Used as age tier when applying age.</summary>
        public void IncrementEldeHitsAccrued()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.EldeHitsAccrued++;
            Save();
        }

        /// <summary>Mark that a player character has had hunger this run. Permanent scars apply.</summary>
        public void MarkHasEverHadHunger()
        {
            if (CurrentProgress == null) return;
            if (CurrentProgress.HasEverHadHunger) return;
            CurrentProgress.HasEverHadHunger = true;
            Save();
        }

        /// <summary>Increment ethical misstep count (wrong dialogue choice). Applies cumulative damage-taken penalty in combat.</summary>
        public void IncrementEthicalMisstep()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.EthicalMisstepCount++;
            Save();
        }

        /// <summary>Damage taken multiplier from ethical missteps. 1f + (count * DamageTakenPerMisstep).</summary>
        public float GetDamageTakenMultiplier()
        {
            int n = CurrentProgress?.EthicalMisstepCount ?? 0;
            if (n <= 0) return 1f;
            return 1f + n * 0.1f; // 10% more damage per misstep (1–5 scale: 10%–50%)
        }
    }
}
