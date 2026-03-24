using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    [Serializable]
    class GameProgressData
    {
        public string CurrentLocationId;
        public string[] VisitedLocationIds;
        public string[] UnlockedLocationIds;
        public int VisionIndex;
        public float TotalPlayTimeSeconds;
        public string SaveTimestamp;
        public bool GreenKnightWillAppearRandomly;
        public bool OrfeoOtherworldTriggered;
        public int PovertyLevel;
        public int EncountersSinceLastGreenKnight;
        public int GreenChapelBodiesAccrued;
        public int OtherworldLivingCharactersAccrued;
        public int OtherworldBodiesFromDeath;
        public int EldeHitsAccrued;
        public int RunAppearanceSeed;
        public bool HasEverHadHunger;
        public int EthicalMisstepCount;
        public int WodeWoArcStage;
        public bool WodeWoUnlocked;
        public bool WodeWoDead;
        public bool HasReachedFieldOfGrace;
        public CharacterProgressEntry[] CharacterProgress;
        public bool CombatSnapshotActive;
        public string CombatEncounterId;
        public int CombatWaveIndex;
        public int CombatAliveEnemies;
        public float CombatRecoveryRemaining;
    }

    /// <summary>Serializable game progress. Phase 1: checkpoint per location.
    /// Design: No true ending—Grace is never found. The quest remains unresolved.</summary>
    public class GameProgress
    {
        public string CurrentLocationId;
        public List<string> VisitedLocationIds = new List<string>();
        public List<string> UnlockedLocationIds = new List<string>();
        public int VisionIndex;
        public float TotalPlayTimeSeconds;
        public string SaveTimestamp;
        [Tooltip("When true, Green Knight may appear randomly in encounters (wrong dialogue at Green Chapel).")]
        public bool GreenKnightWillAppearRandomly;
        [Tooltip("Wrong choice at tree - led to Orfeo's Otherworld alternate ending.")]
        public bool OrfeoOtherworldTriggered;
        public int PovertyLevel;
        [Tooltip("Incremented each encounter. Reset to 0 when Green Knight appears. Fae appearance likelihood higher when <= 2.")]
        public int EncountersSinceLastGreenKnight;
        [Tooltip("Bodies accrued at the Green Chapel from failed run attempts (deaths).")]
        public int GreenChapelBodiesAccrued;
        [Tooltip("Living characters accrued in the fairy world (Otherworld) from those who enter.")]
        public int OtherworldLivingCharactersAccrued;
        [Tooltip("Forms of the dead accrued in Orfeo's Otherworld—every violent death adds a body there.")]
        public int OtherworldBodiesFromDeath;
        [Tooltip("Hits from Elde (Old Age) boss. Each hit ages the target. Used as age tier when applying age.")]
        public int EldeHitsAccrued;
        [Tooltip("Seed for run-specific character appearance (Piers-inspired randomization). Set at NewGame.")]
        public int RunAppearanceSeed;
        [Tooltip("If any player character has ever had the hunger status effect this run. Permanent scars to abilities and appearance.")]
        public bool HasEverHadHunger;
        [Tooltip("Incremented on wrong dialogue choice (sin-aligned or Green Chapel refuse). Applies cumulative damage-taken penalty.")]
        public int EthicalMisstepCount;
        [Tooltip("Wode-Wo positive arc: 0=none, 1=Baby, 2=Care, 3=Grown, 4=Complete.")]
        public int WodeWoArcStage;
        [Tooltip("True when player has entered the W O D E passcode; enables the full Wode-Wo line.")]
        public bool WodeWoUnlocked;
        [Tooltip("True when Wode-Wo has died (fae took him). Alters arrival beats at relevant locations.")]
        public bool WodeWoDead;
        [Tooltip("True once the player has reached the Field of Grace (game complete).")]
        public bool HasReachedFieldOfGrace;
        [Tooltip("Saved combat snapshot for recovery checks while tuning core loop.")]
        public bool CombatSnapshotActive;
        public string CombatEncounterId;
        public int CombatWaveIndex;
        public int CombatAliveEnemies;
        public float CombatRecoveryRemaining;

        public List<CharacterProgressEntry> CharacterProgress = new List<CharacterProgressEntry>();

        /// <summary>Get or create a character progress entry by name.</summary>
        public CharacterProgressEntry GetOrCreateCharacterProgress(string characterName)
        {
            foreach (var e in CharacterProgress)
                if (e.CharacterName == characterName) return e;
            var entry = new CharacterProgressEntry { CharacterName = characterName };
            CharacterProgress.Add(entry);
            return entry;
        }

        public static GameProgress CreateNew(string startLocationId)
        {
            var p = new GameProgress
            {
                CurrentLocationId = startLocationId ?? "malvern",
                VisionIndex = 0,
                TotalPlayTimeSeconds = 0f,
                SaveTimestamp = DateTime.UtcNow.ToString("o"),
                RunAppearanceSeed = UnityEngine.Random.Range(1, int.MaxValue)
            };
            p.VisitedLocationIds.Add(p.CurrentLocationId);
            p.UnlockedLocationIds.Add(p.CurrentLocationId);
            return p;
        }

        public string ToJson()
        {
            var d = new GameProgressData();
            d.CurrentLocationId = CurrentLocationId;
            d.VisitedLocationIds = VisitedLocationIds?.ToArray() ?? Array.Empty<string>();
            d.UnlockedLocationIds = UnlockedLocationIds?.ToArray() ?? Array.Empty<string>();
            d.VisionIndex = VisionIndex;
            d.TotalPlayTimeSeconds = TotalPlayTimeSeconds;
            d.SaveTimestamp = SaveTimestamp;
            d.GreenKnightWillAppearRandomly = GreenKnightWillAppearRandomly;
            d.OrfeoOtherworldTriggered = OrfeoOtherworldTriggered;
            d.PovertyLevel = PovertyLevel;
            d.EncountersSinceLastGreenKnight = EncountersSinceLastGreenKnight;
            d.GreenChapelBodiesAccrued = GreenChapelBodiesAccrued;
            d.OtherworldLivingCharactersAccrued = OtherworldLivingCharactersAccrued;
            d.OtherworldBodiesFromDeath = OtherworldBodiesFromDeath;
            d.EldeHitsAccrued = EldeHitsAccrued;
            d.RunAppearanceSeed = RunAppearanceSeed;
            d.HasEverHadHunger = HasEverHadHunger;
            d.EthicalMisstepCount = EthicalMisstepCount;
            d.WodeWoArcStage = WodeWoArcStage;
            d.WodeWoUnlocked = WodeWoUnlocked;
            d.WodeWoDead = WodeWoDead;
            d.HasReachedFieldOfGrace = HasReachedFieldOfGrace;
            d.CharacterProgress = CharacterProgress?.ToArray() ?? Array.Empty<CharacterProgressEntry>();
            d.CombatSnapshotActive = CombatSnapshotActive;
            d.CombatEncounterId = CombatEncounterId;
            d.CombatWaveIndex = CombatWaveIndex;
            d.CombatAliveEnemies = CombatAliveEnemies;
            d.CombatRecoveryRemaining = CombatRecoveryRemaining;
            return JsonUtility.ToJson(d);
        }

        public static GameProgress FromJson(string json)
        {
            var d = JsonUtility.FromJson<GameProgressData>(json);
            var p = new GameProgress
            {
                CurrentLocationId = d.CurrentLocationId,
                VisionIndex = d.VisionIndex,
                TotalPlayTimeSeconds = d.TotalPlayTimeSeconds,
                SaveTimestamp = d.SaveTimestamp
            };
            p.VisitedLocationIds = d.VisitedLocationIds != null ? new List<string>(d.VisitedLocationIds) : new List<string>();
            p.UnlockedLocationIds = d.UnlockedLocationIds != null ? new List<string>(d.UnlockedLocationIds) : new List<string>();
            p.GreenKnightWillAppearRandomly = d.GreenKnightWillAppearRandomly;
            p.OrfeoOtherworldTriggered = d.OrfeoOtherworldTriggered;
            p.PovertyLevel = d.PovertyLevel;
            p.EncountersSinceLastGreenKnight = d.EncountersSinceLastGreenKnight;
            p.GreenChapelBodiesAccrued = d.GreenChapelBodiesAccrued;
            p.OtherworldLivingCharactersAccrued = d.OtherworldLivingCharactersAccrued;
            p.OtherworldBodiesFromDeath = d.OtherworldBodiesFromDeath;
            p.EldeHitsAccrued = d.EldeHitsAccrued;
            p.RunAppearanceSeed = d.RunAppearanceSeed;
            p.HasEverHadHunger = d.HasEverHadHunger;
            p.EthicalMisstepCount = d.EthicalMisstepCount;
            p.WodeWoArcStage = d.WodeWoArcStage;
            p.WodeWoUnlocked = d.WodeWoUnlocked;
            p.WodeWoDead = d.WodeWoDead;
            p.HasReachedFieldOfGrace = d.HasReachedFieldOfGrace;
            p.CharacterProgress = d.CharacterProgress != null
                ? new List<CharacterProgressEntry>(d.CharacterProgress)
                : new List<CharacterProgressEntry>();
            p.CombatSnapshotActive = d.CombatSnapshotActive;
            p.CombatEncounterId = d.CombatEncounterId;
            p.CombatWaveIndex = d.CombatWaveIndex;
            p.CombatAliveEnemies = d.CombatAliveEnemies;
            p.CombatRecoveryRemaining = d.CombatRecoveryRemaining;
            return p;
        }
    }
}
