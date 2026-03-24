using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KyndeBlade
{
    /// <summary>Manages world map, current location, and transitions. Phase 1: hub/level select.</summary>
    public class WorldMapManager : MonoBehaviour
    {
        [Header("Map Data")]
        public List<LocationNode> AllLocations = new List<LocationNode>();
        public LocationNode StartLocation;
        [Tooltip("MVP: load only Vision1+2 (linear). Uncheck to include Green Chapel, Orfeo Otherworld.")]
        public bool MVPLinearOnly = true;
        public LocationNode CurrentLocation { get; private set; }

        [Header("References")]
        public KyndeBladeGameManager GameManager;
        public NarrativeManager NarrativeManager;
        public SaveManager SaveManager;
        public MusicManager MusicManager;

        public event Action<LocationNode> OnLocationChanged;
        public event Action<LocationNode> OnLocationEntered;

        Dictionary<string, LocationNode> _locationById;
        bool _lazyInitDone;

        void Awake()
        {
            _locationById = new Dictionary<string, LocationNode>();
            if (AllLocations == null || AllLocations.Count == 0)
            {
                var list = new List<LocationNode>();
                var v1 = Resources.LoadAll<LocationNode>("Data/Vision1");
                if (v1 != null && v1.Length > 0) list.AddRange(v1);
                var v2 = Resources.LoadAll<LocationNode>("Data/Vision2");
                if (v2 != null && v2.Length > 0) list.AddRange(v2);
                var gc = Resources.LoadAll<LocationNode>("Data/GreenChapel");
                if (gc != null && gc.Length > 0) list.AddRange(gc);
                if (!MVPLinearOnly)
                {
                    var oo = Resources.LoadAll<LocationNode>("Data/OrfeoOtherworld");
                    if (oo != null && oo.Length > 0) list.AddRange(oo);
                }
                if (list.Count > 0) AllLocations = list;
            }
            RebuildLocationIndex();
        }

        /// <summary>Rebuilds _locationById from AllLocations. Call after modifying AllLocations at runtime.</summary>
        public void RebuildLocationIndex()
        {
            if (_locationById == null) _locationById = new Dictionary<string, LocationNode>();
            _locationById.Clear();
            foreach (var loc in AllLocations ?? new List<LocationNode>())
            {
                if (loc != null && !string.IsNullOrEmpty(loc.LocationId))
                    _locationById[loc.LocationId] = loc;
            }
            if (StartLocation == null && !string.IsNullOrEmpty(GameWorldConstants.DefaultStartLocationId) && _locationById.TryGetValue(GameWorldConstants.DefaultStartLocationId, out var defaultLoc))
                StartLocation = defaultLoc;
            if (StartLocation == null && _locationById.TryGetValue(GameWorldConstants.LocationMalvern, out var malvern))
                StartLocation = malvern;
        }

        void Start()
        {
            ResolveRefs();
            TryLazyInit();
        }

        void Update()
        {
            if (_lazyInitDone) return;
            ResolveRefs();
            TryLazyInit();
        }

        void ResolveRefs()
        {
            if (GameManager == null) GameManager = UnityEngine.Object.FindFirstObjectByType<KyndeBladeGameManager>();
            if (NarrativeManager == null) NarrativeManager = UnityEngine.Object.FindFirstObjectByType<NarrativeManager>();
            if (SaveManager == null) SaveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (MusicManager == null) MusicManager = UnityEngine.Object.FindFirstObjectByType<MusicManager>();
        }

        void TryLazyInit()
        {
            var flow = UnityEngine.Object.FindFirstObjectByType<GameFlowController>();
            if (flow != null && flow.BlocksWorldInit) return;
            if (_lazyInitDone || CurrentLocation != null) return;
            LocationNode toSet = StartLocation;
            if (!string.IsNullOrEmpty(DemoTestHelper.OverrideStartLocationId))
            {
                var force = GetLocation(DemoTestHelper.OverrideStartLocationId);
                if (force != null) toSet = force;
            }
            else if (SaveManager != null && !string.IsNullOrEmpty(SaveManager.CurrentProgress?.CurrentLocationId))
            {
                var saved = GetLocation(SaveManager.CurrentProgress.CurrentLocationId);
                if (saved != null) toSet = saved;
            }
            if (toSet != null)
            {
                SetCurrentLocation(toSet);
                if (SaveManager != null && toSet.NextLocationIds != null)
                {
                    foreach (var nextId in toSet.NextLocationIds)
                        SaveManager.UnlockLocation(nextId);
                }
                // First view at tower: show vista story beat then map (demo: confined, lovely, spooky, overlooking fair field)
                if (!DemoTestHelper.SkipTowerIntroStoryActive
                    && string.Equals(toSet.LocationId, GameWorldConstants.LocationTowerOnToft, StringComparison.OrdinalIgnoreCase)
                    && toSet.StoryBeatOnArrival != null && NarrativeManager != null)
                {
                    var mapCanvas = GameObject.Find("MapCanvas");
                    if (mapCanvas != null) mapCanvas.SetActive(false);
                    NarrativeManager.ShowStoryBeat(toSet.StoryBeatOnArrival, () => ShowMapHideCombat());
                }
                else
                    ShowMapHideCombat();
            }
            else
                ShowMapHideCombat();
            _lazyInitDone = true;
        }

        public void SetCurrentLocation(LocationNode loc)
        {
            if (loc == null) return;
            CurrentLocation = loc;
            OnLocationChanged?.Invoke(loc);
        }

        [Header("Random Destinations")]
        [Tooltip("Chance (0-1) for Green Chapel to appear as a destination when at a building.")]
        public float GreenChapelRandomAppearChance = 0.2f;

        public List<LocationNode> GetNextLocations()
        {
            if (CurrentLocation == null)
                return new List<LocationNode>();

            var result = new List<LocationNode>();
            if (CurrentLocation.NextLocationIds != null)
            {
                foreach (var id in CurrentLocation.NextLocationIds)
                {
                    if (_locationById.TryGetValue(id, out var loc))
                        result.Add(loc);
                }
            }

            // Green Chapel can appear from time to time when at a building
            if (CurrentLocation.IsBuilding && !result.Exists(l => string.Equals(l.LocationId, "green_chapel", System.StringComparison.OrdinalIgnoreCase)))
            {
                var greenChapel = GetLocation("green_chapel");
                if (greenChapel != null && UnityEngine.Random.value < GreenChapelRandomAppearChance)
                {
                    result.Add(greenChapel);
                    if (SaveManager != null)
                        SaveManager.UnlockLocation("green_chapel");
                }
            }
            return result;
        }

        public LocationNode GetLocation(string id)
        {
            return _locationById != null && _locationById.TryGetValue(id, out var loc) ? loc : null;
        }

        /// <summary>After main menu dismiss, allow <see cref="TryLazyInit"/> to run again from Update.</summary>
        public void ResetLazyInitializationFromMenu()
        {
            _lazyInitDone = false;
            CurrentLocation = null;
        }

        /// <summary>Transition to a location. Loads scene if set, or triggers encounter in current scene.</summary>
        public void TransitionTo(LocationNode loc)
        {
            if (loc == null) return;
#if UNITY_EDITOR
            Debug.Log($"[Demo] TransitionTo location={loc.LocationId}");
#endif
            if (string.Equals(loc.LocationId, "otherworld", System.StringComparison.OrdinalIgnoreCase) && SaveManager != null)
                SaveManager.IncrementOtherworldLivingCharacters();

            SetCurrentLocation(loc);
            OnLocationEntered?.Invoke(loc);

            var music = MusicManager != null ? MusicManager : UnityEngine.Object.FindFirstObjectByType<MusicManager>();
            if (music != null && !string.IsNullOrEmpty(loc.MusicThemeOnArrival))
                music.PlayTheme(loc.MusicThemeOnArrival);

            if (SaveManager != null)
            {
                SaveManager.SaveCheckpoint(loc.LocationId);
                foreach (var nextId in loc.NextLocationIds ?? new List<string>())
                    SaveManager.UnlockLocation(nextId);
                if (loc.PovertyLevelOnArrival >= 0)
                    SaveManager.SetPovertyLevel(loc.PovertyLevelOnArrival);
            }

            if (loc.PreCombatChoiceBeat != null && NarrativeManager != null)
            {
                NarrativeManager.ShowChoiceBeat(loc.PreCombatChoiceBeat, (index, isCorrect, transitionToLocationId, associatedSin) =>
                {
                    OnChoiceProceed(loc, isCorrect, transitionToLocationId, associatedSin);
                });
            }
            else if (NarrativeManager != null)
            {
                bool wodeUnlocked = SaveManager != null && SaveManager.IsWodeWoUnlocked;
                int wodeStage = SaveManager != null ? SaveManager.WodeWoArcStage : 0;
                bool hasWodeCompleteBeat = loc.StoryBeatOnArrivalWhenWodeWoComplete != null;

                // Wode-Wo positive arc (only when passcode entered): complete beat, capstone, or sequence advance
                if (wodeUnlocked && hasWodeCompleteBeat && wodeStage >= 4)
                {
                    NarrativeManager.ShowStoryBeat(loc.StoryBeatOnArrivalWhenWodeWoComplete, () => EnterLocationCombatOrScene(loc));
                }
                else if (wodeUnlocked && hasWodeCompleteBeat && wodeStage == 3)
                {
                    NarrativeManager.ShowStoryBeat(loc.StoryBeatOnArrivalWhenWodeWoComplete, () =>
                    {
                        if (SaveManager != null) SaveManager.SetWodeWoArcStage(4);
                        EnterLocationCombatOrScene(loc);
                    });
                }
                else if (wodeUnlocked && loc.AdvancesWodeWoArcOnSequenceComplete && loc.StoryBeatSequenceOnArrival != null && loc.StoryBeatSequenceOnArrival.Count > 0 && wodeStage < 3)
                {
                    NarrativeManager.ShowStoryBeatSequence(loc.StoryBeatSequenceOnArrival, () =>
                    {
                        if (SaveManager != null) SaveManager.SetWodeWoArcStage(3);
                        EnterLocationCombatOrScene(loc);
                    });
                }
                else if (loc.StoryBeatSequenceOnArrival != null && loc.StoryBeatSequenceOnArrival.Count > 0)
                {
                    NarrativeManager.ShowStoryBeatSequence(loc.StoryBeatSequenceOnArrival, () => EnterLocationCombatOrScene(loc));
                }
                else if (GetArrivalBeat(loc) != null)
                {
                    NarrativeManager.ShowStoryBeat(GetArrivalBeat(loc), () => EnterLocationCombatOrScene(loc));
                }
                else
                {
                    EnterLocationCombatOrScene(loc);
                }
            }
            else
            {
                EnterLocationCombatOrScene(loc);
            }
        }

        /// <summary>Arrival beat for location; uses permanent-flag override when set (e.g. HasEverHadHunger).</summary>
        StoryBeat GetArrivalBeat(LocationNode loc)
        {
            if (loc == null) return null;
            if (SaveManager != null && SaveManager.HasEverHadHunger && loc.StoryBeatOnArrivalWhenHasEverHadHunger != null)
                return loc.StoryBeatOnArrivalWhenHasEverHadHunger;
            return loc.StoryBeatOnArrival;
        }

        void OnChoiceProceed(LocationNode loc, bool isCorrect, string transitionToLocationId, SinType associatedSin)
        {
            if (SaveManager != null)
            {
                SaveManager.SetGreenKnightWillAppearRandomly(!isCorrect);
                if (!isCorrect)
                    SaveManager.IncrementEthicalMisstep();
            }
            ContinueAfterChoice(loc, transitionToLocationId, associatedSin, isCorrect);
        }

        void ContinueAfterChoice(LocationNode loc, string transitionToLocationId, SinType associatedSin, bool isCorrect)
        {
            if (!string.IsNullOrEmpty(transitionToLocationId))
            {
                var targetLoc = GetLocation(transitionToLocationId);
                if (targetLoc != null)
                {
                    if (SaveManager != null) SaveManager.SetOrfeoOtherworldTriggered(true);
                    var music = MusicManager != null ? MusicManager : UnityEngine.Object.FindFirstObjectByType<MusicManager>();
                    if (music != null)
                        music.PlayOmenThenOrfeo(() => TransitionTo(targetLoc));
                    else
                        TransitionTo(targetLoc);
                }
                else
                {
                    var beat = loc.StoryBeatOnArrival;
                    if (beat != null && NarrativeManager != null)
                        NarrativeManager.ShowStoryBeat(beat, () => EnterLocationCombatOrScene(loc));
                    else
                        EnterLocationCombatOrScene(loc);
                }
            }
            else if (associatedSin != SinType.None && !isCorrect && GameManager != null)
            {
                if (SaveManager != null) SaveManager.SetOrfeoOtherworldTriggered(true);
                GameManager.StartSinMinibossEncounter(associatedSin, loc);
            }
            else
            {
                var beat = GetArrivalBeat(loc);
                if (beat != null && NarrativeManager != null)
                    NarrativeManager.ShowStoryBeat(beat, () => EnterLocationCombatOrScene(loc));
                else
                    EnterLocationCombatOrScene(loc);
            }
        }

        void EnterLocationCombatOrScene(LocationNode loc)
        {
            if (!string.IsNullOrEmpty(loc.SceneName))
            {
#if UNITY_EDITOR
                Debug.Log($"[Demo] EnterLocationCombatOrScene scene={loc.SceneName}");
#endif
                SceneManager.LoadScene(loc.SceneName);
                return;
            }

            if (loc.Encounter != null && GameManager != null)
            {
#if UNITY_EDITOR
                Debug.Log($"[Demo] EnterLocationCombatOrScene combat loc={loc.LocationId}");
#endif
                GameManager.StartEncounterFromConfig(loc.Encounter, loc);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"[Demo] EnterLocationCombatOrScene map loc={loc.LocationId}");
#endif
                ShowMapHideCombat();
            }
        }

        void ShowMapHideCombat()
        {
            var mapCanvas = GameObject.Find("MapCanvas");
            var combatCanvas = GameObject.Find("CombatCanvas");
            if (mapCanvas != null) mapCanvas.SetActive(true);
            if (combatCanvas != null) combatCanvas.SetActive(false);
            var mapUI = UnityEngine.Object.FindFirstObjectByType<MapLevelSelectUI>();
            if (mapUI != null && CurrentLocation != null)
                mapUI.Refresh(CurrentLocation);
        }
    }
}
