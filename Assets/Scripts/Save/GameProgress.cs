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
        public int RunAppearanceSeed;
        public bool HasEverHadHunger;
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
        [Tooltip("Seed for run-specific character appearance (Piers-inspired randomization). Set at NewGame.")]
        public int RunAppearanceSeed;
        [Tooltip("If any player character has ever had the hunger status effect this run. Permanent scars to abilities and appearance.")]
        public bool HasEverHadHunger;

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
            var d = new GameProgressData
            {
                CurrentLocationId = CurrentLocationId,
                VisitedLocationIds = VisitedLocationIds?.ToArray() ?? Array.Empty<string>(),
                UnlockedLocationIds = UnlockedLocationIds?.ToArray() ?? Array.Empty<string>(),
                VisionIndex = VisionIndex,
                TotalPlayTimeSeconds = TotalPlayTimeSeconds,
                SaveTimestamp = SaveTimestamp,
                GreenKnightWillAppearRandomly = GreenKnightWillAppearRandomly,
                OrfeoOtherworldTriggered = OrfeoOtherworldTriggered,
                PovertyLevel = PovertyLevel,
                EncountersSinceLastGreenKnight = EncountersSinceLastGreenKnight,
                GreenChapelBodiesAccrued = GreenChapelBodiesAccrued,
                OtherworldLivingCharactersAccrued = OtherworldLivingCharactersAccrued,
                RunAppearanceSeed = RunAppearanceSeed,
                HasEverHadHunger = HasEverHadHunger
            };
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
            p.RunAppearanceSeed = d.RunAppearanceSeed;
            p.HasEverHadHunger = d.HasEverHadHunger;
            return p;
        }
    }
}
