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
            if (StartLocation == null && _locationById.TryGetValue("malvern", out var malvern))
                StartLocation = malvern;
        }

        void Start()
        {
            if (GameManager == null) GameManager = FindObjectOfType<KyndeBladeGameManager>();
            if (NarrativeManager == null) NarrativeManager = FindObjectOfType<NarrativeManager>();
            if (SaveManager == null) SaveManager = FindObjectOfType<SaveManager>();
            if (MusicManager == null) MusicManager = FindObjectOfType<MusicManager>();
            if (CurrentLocation == null)
            {
                LocationNode toSet = StartLocation;
                if (SaveManager != null && !string.IsNullOrEmpty(SaveManager.CurrentProgress?.CurrentLocationId))
                {
                    var saved = GetLocation(SaveManager.CurrentProgress.CurrentLocationId);
                    if (saved != null) toSet = saved;
                }
                if (toSet != null)
                {
                    SetCurrentLocation(toSet);
                    if (SaveManager != null)
                    {
                        foreach (var nextId in toSet.NextLocationIds ?? new List<string>())
                            SaveManager.UnlockLocation(nextId);
                    }
                }
            }
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

        /// <summary>Transition to a location. Loads scene if set, or triggers encounter in current scene.</summary>
        public void TransitionTo(LocationNode loc)
        {
            if (loc == null) return;

            if (string.Equals(loc.LocationId, "otherworld", System.StringComparison.OrdinalIgnoreCase) && SaveManager != null)
                SaveManager.IncrementOtherworldLivingCharacters();

            SetCurrentLocation(loc);
            OnLocationEntered?.Invoke(loc);

            var music = MusicManager != null ? MusicManager : FindObjectOfType<MusicManager>();
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
                    System.Action proceed = () => OnChoiceProceed(loc, isCorrect, transitionToLocationId, associatedSin);
                    if (!isCorrect)
                    {
                        var wodeWo = FindObjectOfType<WodeWoManager>();
                        if (wodeWo != null && wodeWo.IsWodeWoAlive)
                        {
                            StartCoroutine(AfterWodeWoDeathCutscene(proceed));
                            return;
                        }
                    }
                    proceed();
                });
            }
            else if (NarrativeManager != null)
            {
                if (InstallState.WodeWoIsDead && loc.StoryBeatOnArrivalWhenWodeWoDead != null)
                {
                    NarrativeManager.ShowStoryBeat(loc.StoryBeatOnArrivalWhenWodeWoDead, () => EnterLocationCombatOrScene(loc));
                }
                else if (loc.StoryBeatSequenceOnArrival != null && loc.StoryBeatSequenceOnArrival.Count > 0)
                {
                    NarrativeManager.ShowStoryBeatSequence(loc.StoryBeatSequenceOnArrival, () => EnterLocationCombatOrScene(loc));
                }
                else if (loc.StoryBeatOnArrival != null)
                {
                    NarrativeManager.ShowStoryBeat(loc.StoryBeatOnArrival, () => EnterLocationCombatOrScene(loc));
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

        void OnChoiceProceed(LocationNode loc, bool isCorrect, string transitionToLocationId, SinType associatedSin)
        {
            if (SaveManager != null)
                SaveManager.SetGreenKnightWillAppearRandomly(!isCorrect);
            if (!string.IsNullOrEmpty(transitionToLocationId))
            {
                var targetLoc = GetLocation(transitionToLocationId);
                if (targetLoc != null)
                {
                    if (SaveManager != null) SaveManager.SetOrfeoOtherworldTriggered(true);
                    var music = MusicManager != null ? MusicManager : FindObjectOfType<MusicManager>();
                    if (music != null)
                        music.PlayOmenThenOrfeo(() => TransitionTo(targetLoc));
                    else
                        TransitionTo(targetLoc);
                }
                else
                {
                    var beat = (InstallState.WodeWoIsDead && loc.StoryBeatOnArrivalWhenWodeWoDead != null) ? loc.StoryBeatOnArrivalWhenWodeWoDead : loc.StoryBeatOnArrival;
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
                var beat = (InstallState.WodeWoIsDead && loc.StoryBeatOnArrivalWhenWodeWoDead != null) ? loc.StoryBeatOnArrivalWhenWodeWoDead : loc.StoryBeatOnArrival;
                if (beat != null && NarrativeManager != null)
                    NarrativeManager.ShowStoryBeat(beat, () => EnterLocationCombatOrScene(loc));
                else
                    EnterLocationCombatOrScene(loc);
            }
        }

        IEnumerator AfterWodeWoDeathCutscene(System.Action onComplete)
        {
            var wodeWo = FindObjectOfType<WodeWoManager>();
            if (wodeWo != null)
            {
                bool done = false;
                wodeWo.TriggerWodeWoDeath(() => done = true);
                while (!done) yield return null;
            }
            onComplete?.Invoke();
        }

        void EnterLocationCombatOrScene(LocationNode loc)
        {
            if (!string.IsNullOrEmpty(loc.SceneName))
            {
                SceneManager.LoadScene(loc.SceneName);
                return;
            }

            if (loc.Encounter != null && GameManager != null)
            {
                GameManager.StartEncounterFromConfig(loc.Encounter, loc);
            }
            else
            {
                ShowMapHideCombat();
            }
        }

        void ShowMapHideCombat()
        {
            var mapCanvas = GameObject.Find("MapCanvas");
            var combatCanvas = GameObject.Find("CombatCanvas");
            if (mapCanvas != null) mapCanvas.SetActive(true);
            if (combatCanvas != null) combatCanvas.SetActive(false);
            var mapUI = FindObjectOfType<MapLevelSelectUI>();
            if (mapUI != null && CurrentLocation != null)
                mapUI.Refresh(CurrentLocation);
        }
    }
}
