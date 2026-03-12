#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Editor playtest utilities: start from any location, skip to Vision II, debug state.</summary>
    public static class PlaytestTools
    {
        [MenuItem("KyndeBlade/Playtest/Start from Malvern")]
        static void StartMalvern() => StartFromLocation("malvern");

        [MenuItem("KyndeBlade/Playtest/Start from Fayre Felde")]
        static void StartFayre() => StartFromLocation("fayre_felde");

        [MenuItem("KyndeBlade/Playtest/Start from Seven Sins")]
        static void StartSins() => StartFromLocation("seven_sins");

        [MenuItem("KyndeBlade/Playtest/Start from Quest Do-Wel (Vision II)")]
        static void StartVision2() => StartFromLocation("quest_do_wel", visionIndex: 1);

        [MenuItem("KyndeBlade/Playtest/Start from Dongeoun Depths")]
        static void StartDepths() => StartFromLocation("dongeoun_depths", visionIndex: 1);

        [MenuItem("KyndeBlade/Playtest/Start from Years Pass")]
        static void StartYears() => StartFromLocation("years_pass", visionIndex: 1);

        [MenuItem("KyndeBlade/Playtest/Start from Green Chapel")]
        static void StartGreenChapel() => StartFromLocation("green_chapel");

        [MenuItem("KyndeBlade/Playtest/Start from Field of Grace")]
        static void StartFieldOfGrace() => StartFromLocation("field_of_grace", visionIndex: 1);

        static void StartFromLocation(string locationId, int visionIndex = 0)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[Playtest] Enter Play mode first, then use this command.");
                return;
            }

            var save = Object.FindFirstObjectByType<SaveManager>();
            if (save == null)
            {
                Debug.LogError("[Playtest] SaveManager not found.");
                return;
            }

            save.NewGame(locationId);
            if (save.CurrentProgress != null)
            {
                save.CurrentProgress.VisionIndex = visionIndex;
                save.CurrentProgress.VisitedLocationIds.Add(locationId);
                save.CurrentProgress.UnlockedLocationIds.Add(locationId);
            }
            save.Save();

            var wm = Object.FindFirstObjectByType<WorldMapManager>();
            if (wm != null)
            {
                var loc = wm.GetLocation(locationId);
                if (loc != null)
                {
                    wm.TransitionTo(loc);
                    Debug.Log($"[Playtest] Started at {locationId} (Vision {visionIndex}).");
                }
                else
                {
                    Debug.LogWarning($"[Playtest] Location '{locationId}' not found in world map.");
                }
            }
        }

        [MenuItem("KyndeBlade/Playtest/Log Game State")]
        static void LogGameState()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("[Playtest] Not in Play mode.");
                return;
            }

            var save = Object.FindFirstObjectByType<SaveManager>();
            if (save == null || save.CurrentProgress == null)
            {
                Debug.Log("[Playtest] No save data.");
                return;
            }

            var p = save.CurrentProgress;
            Debug.Log($"[Playtest State]\n" +
                $"  Location: {p.CurrentLocationId}\n" +
                $"  Vision: {p.VisionIndex}\n" +
                $"  PlayTime: {p.TotalPlayTimeSeconds:F0}s\n" +
                $"  Visited: {string.Join(", ", p.VisitedLocationIds)}\n" +
                $"  Unlocked: {string.Join(", ", p.UnlockedLocationIds)}\n" +
                $"  Missteps: {p.EthicalMisstepCount}\n" +
                $"  Elde Hits: {p.EldeHitsAccrued}\n" +
                $"  WodeWo: stage={p.WodeWoArcStage} unlocked={p.WodeWoUnlocked} dead={p.WodeWoDead}\n" +
                $"  GreenKnight Random: {p.GreenKnightWillAppearRandomly}\n" +
                $"  Hunger Scar: {p.HasEverHadHunger}\n" +
                $"  Field of Grace: {p.HasReachedFieldOfGrace}\n" +
                $"  Blessings: {p.CharacterProgress?.Count ?? 0} character entries");

            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm != null && tm.PlayerCharacters != null)
            {
                foreach (var c in tm.PlayerCharacters)
                {
                    if (c == null) continue;
                    string statuses = "";
                    foreach (var se in c.ActiveStatusEffects)
                        statuses += $"{se.Data.Type}({se.Data.RemainingTime:F0}t) ";
                    Debug.Log($"  {c.CharacterName}: HP={c.Stats.CurrentHealth:F0}/{c.Stats.MaxHealth:F0} " +
                        $"ATK={c.Stats.AttackPower:F0} DEF={c.Stats.Defense:F0} SPD={c.Stats.Speed:F0} " +
                        $"Blessings={c.Stats.ActiveBlessings?.Count ?? 0} " +
                        $"Status=[{statuses.Trim()}]");
                }
            }
        }

        [MenuItem("KyndeBlade/Playtest/Grant All Blessings")]
        static void GrantAllBlessings()
        {
            if (!Application.isPlaying) return;
            var tm = Object.FindFirstObjectByType<TurnManager>();
            var save = Object.FindFirstObjectByType<SaveManager>();
            if (tm == null || save == null) return;

            var pool = BlessingPool.CreateDefaultBlessings();
            foreach (var c in tm.PlayerCharacters)
            {
                if (c == null) continue;
                foreach (var b in pool)
                    BlessingSystem.ApplyBlessing(c, b, save);
            }
            save.Save();
            Debug.Log($"[Playtest] Granted {pool.Count} blessings to all party members.");
        }

        [MenuItem("KyndeBlade/Playtest/Heal Party")]
        static void HealParty()
        {
            if (!Application.isPlaying) return;
            var tm = Object.FindFirstObjectByType<TurnManager>();
            if (tm == null) return;
            foreach (var c in tm.PlayerCharacters)
            {
                if (c == null) continue;
                c.Heal(c.Stats.MaxHealth);
                c.RestoreStamina(c.Stats.MaxStamina);
            }
            Debug.Log("[Playtest] Party healed.");
        }
    }
}
#endif
