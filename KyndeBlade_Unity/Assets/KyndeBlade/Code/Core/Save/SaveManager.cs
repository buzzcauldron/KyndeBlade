using System;
using System.IO;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Save/load game progress using file-based JSON.
    /// Supports 3 save slots stored at Application.persistentDataPath/saves/.</summary>
    public class SaveManager : MonoBehaviour
    {
        const string SaveDir = "saves";
        const string LegacyKey = "KyndeBlade_Save";
        const int MaxSlots = 3;

        public int ActiveSlot { get; private set; }
        public GameProgress CurrentProgress { get; private set; }

        public event Action<GameProgress> OnProgressLoaded;
        public event Action<GameProgress> OnProgressSaved;

        void Awake()
        {
            MigrateLegacySave();
            Load();
        }

        string GetSavePath(int slot) =>
            Path.Combine(Application.persistentDataPath, SaveDir, $"slot_{slot}.json");

        string GetBackupPath(int slot) =>
            Path.Combine(Application.persistentDataPath, SaveDir, $"slot_{slot}.bak");

        void EnsureSaveDir()
        {
            var dir = Path.Combine(Application.persistentDataPath, SaveDir);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        void MigrateLegacySave()
        {
            if (!PlayerPrefs.HasKey(LegacyKey)) return;
            var json = PlayerPrefs.GetString(LegacyKey, "");
            if (string.IsNullOrEmpty(json)) return;
            EnsureSaveDir();
            var path = GetSavePath(0);
            if (!File.Exists(path))
            {
                try
                {
                    File.WriteAllText(path, json);
                    Debug.Log("[SaveManager] Migrated PlayerPrefs save to file slot 0.");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SaveManager] Migration failed: {e.Message}");
                }
            }
            PlayerPrefs.DeleteKey(LegacyKey);
            PlayerPrefs.Save();
        }

        public void SetActiveSlot(int slot)
        {
            ActiveSlot = Mathf.Clamp(slot, 0, MaxSlots - 1);
        }

        public void Load() => Load(ActiveSlot);

        public void Load(int slot)
        {
            ActiveSlot = Mathf.Clamp(slot, 0, MaxSlots - 1);
            var path = GetSavePath(ActiveSlot);

            if (!File.Exists(path))
            {
                CurrentProgress = GameProgress.CreateNew("malvern");
                OnProgressLoaded?.Invoke(CurrentProgress);
                return;
            }
            try
            {
                var json = File.ReadAllText(path);
                CurrentProgress = GameProgress.FromJson(json);
                OnProgressLoaded?.Invoke(CurrentProgress);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Failed to load slot {ActiveSlot}: {e.Message}");
                if (TryRecoverFromBackup(ActiveSlot)) return;
                CurrentProgress = GameProgress.CreateNew("malvern");
            }
        }

        bool TryRecoverFromBackup(int slot)
        {
            var backupPath = GetBackupPath(slot);
            if (!File.Exists(backupPath)) return false;
            try
            {
                var json = File.ReadAllText(backupPath);
                CurrentProgress = GameProgress.FromJson(json);
                Debug.Log($"[SaveManager] Recovered slot {slot} from backup.");
                Save();
                OnProgressLoaded?.Invoke(CurrentProgress);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Backup recovery failed: {e.Message}");
                return false;
            }
        }

        public void Save()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.SaveTimestamp = DateTime.UtcNow.ToString("o");
            var json = CurrentProgress.ToJson();
            EnsureSaveDir();
            var path = GetSavePath(ActiveSlot);
            try
            {
                if (File.Exists(path))
                    File.Copy(path, GetBackupPath(ActiveSlot), overwrite: true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Save failed: {e.Message}");
            }
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

        public bool HasVisited(string locationId) =>
            CurrentProgress?.VisitedLocationIds?.Contains(locationId) ?? false;

        public bool IsUnlocked(string locationId) =>
            CurrentProgress?.UnlockedLocationIds?.Contains(locationId) ?? false;

        public void NewGame(string startLocationId = "malvern")
        {
            CurrentProgress = GameProgress.CreateNew(startLocationId);
            Save();
        }

        public void DeleteSlot(int slot)
        {
            slot = Mathf.Clamp(slot, 0, MaxSlots - 1);
            var path = GetSavePath(slot);
            var backup = GetBackupPath(slot);
            try
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(backup)) File.Delete(backup);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Delete slot {slot} failed: {e.Message}");
            }
        }

        public bool SlotExists(int slot)
        {
            slot = Mathf.Clamp(slot, 0, MaxSlots - 1);
            return File.Exists(GetSavePath(slot));
        }

        public void SaveCombatSnapshot(string encounterId, int waveIndex, int aliveEnemies, float recoveryRemaining)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.CombatSnapshotActive = true;
            CurrentProgress.CombatEncounterId = encounterId;
            CurrentProgress.CombatWaveIndex = Mathf.Max(0, waveIndex);
            CurrentProgress.CombatAliveEnemies = Mathf.Max(0, aliveEnemies);
            CurrentProgress.CombatRecoveryRemaining = Mathf.Max(0f, recoveryRemaining);
            Save();
        }

        public void ClearCombatSnapshot()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.CombatSnapshotActive = false;
            CurrentProgress.CombatEncounterId = string.Empty;
            CurrentProgress.CombatWaveIndex = 0;
            CurrentProgress.CombatAliveEnemies = 0;
            CurrentProgress.CombatRecoveryRemaining = 0f;
            Save();
        }

        /// <summary>Returns a summary for the save slot UI, or null if empty.</summary>
        public SaveSlotSummary GetSlotSummary(int slot)
        {
            slot = Mathf.Clamp(slot, 0, MaxSlots - 1);
            var path = GetSavePath(slot);
            if (!File.Exists(path)) return null;
            try
            {
                var json = File.ReadAllText(path);
                var p = GameProgress.FromJson(json);
                return new SaveSlotSummary
                {
                    Slot = slot,
                    LocationId = p.CurrentLocationId,
                    VisionIndex = p.VisionIndex,
                    PlayTimeSeconds = p.TotalPlayTimeSeconds,
                    Timestamp = p.SaveTimestamp,
                    HasReachedFieldOfGrace = p.HasReachedFieldOfGrace
                };
            }
            catch
            {
                return null;
            }
        }

        // --- Convenience properties (unchanged API) ---

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

        public void IncrementOtherworldBodiesFromDeath()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.OtherworldBodiesFromDeath++;
            Save();
        }

        public void IncrementEldeHitsAccrued()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.EldeHitsAccrued++;
            Save();
        }

        public void MarkHasEverHadHunger()
        {
            if (CurrentProgress == null) return;
            if (CurrentProgress.HasEverHadHunger) return;
            CurrentProgress.HasEverHadHunger = true;
            Save();
        }

        public void IncrementEthicalMisstep()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.EthicalMisstepCount++;
            Save();
        }

        public float GetDamageTakenMultiplier()
        {
            int n = CurrentProgress?.EthicalMisstepCount ?? 0;
            if (n <= 0) return 1f;
            return 1f + n * 0.1f;
        }

        public int WodeWoArcStage => CurrentProgress?.WodeWoArcStage ?? 0;
        public bool IsWodeWoArcComplete => WodeWoArcStage >= 4;

        public void SetWodeWoArcStage(int stage)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.WodeWoArcStage = Mathf.Clamp(stage, 0, 4);
            Save();
        }

        public void AdvanceWodeWoArc() => SetWodeWoArcStage(WodeWoArcStage + 1);

        public bool IsWodeWoUnlocked => CurrentProgress?.WodeWoUnlocked ?? false;

        public void SetWodeWoUnlocked(bool unlocked)
        {
            if (CurrentProgress == null) return;
            CurrentProgress.WodeWoUnlocked = unlocked;
            Save();
        }

        public bool IsWodeWoDead => CurrentProgress?.WodeWoDead ?? false;

        public void SetWodeWoDead()
        {
            if (CurrentProgress == null) return;
            CurrentProgress.WodeWoDead = true;
            Save();
        }

        public bool HasReachedFieldOfGrace => CurrentProgress?.HasReachedFieldOfGrace ?? false;

        public void SetReachedFieldOfGrace()
        {
            if (CurrentProgress == null) return;
            if (CurrentProgress.HasReachedFieldOfGrace) return;
            CurrentProgress.HasReachedFieldOfGrace = true;
            Save();
        }
    }

    public class SaveSlotSummary
    {
        public int Slot;
        public string LocationId;
        public int VisionIndex;
        public float PlayTimeSeconds;
        public string Timestamp;
        public bool HasReachedFieldOfGrace;
    }
}
