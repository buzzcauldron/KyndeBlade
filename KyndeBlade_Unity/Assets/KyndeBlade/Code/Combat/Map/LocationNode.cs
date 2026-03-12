using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>ScriptableObject for a campaign location (Malvern, Fayre Felde, etc.). Per CAMPAIGN_LEVEL_DESIGN.</summary>
    [CreateAssetMenu(fileName = "Location", menuName = "KyndeBlade/Location Node")]
    public class LocationNode : ScriptableObject
    {
        [Header("Identity")]
        public string LocationId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;

        [Header("Campaign")]
        public int VisionIndex;
        public int PassusIndex;
        public string PassusTitle;

        [Header("Transition")]
        [Tooltip("Scene name to load, or empty to stay in current scene.")]
        public string SceneName;
        [Tooltip("IDs of locations reachable from this one.")]
        public List<string> NextLocationIds = new List<string>();

        [Header("Encounter")]
        [Tooltip("Encounter config for combat when entering. Null = no combat.")]
        public EncounterConfig Encounter;
        [Tooltip("Piers-themed hazards for this location. Applied if encounter has none.")]
        public List<PiersHazardConfig> CombatHazards = new List<PiersHazardConfig>();
        [Tooltip("Rewards on victory. Null = use defaults.")]
        public RewardConfig Rewards;

        [Header("Narrative")]
        [Tooltip("Story beat to show on arrival.")]
        public StoryBeat StoryBeatOnArrival;
        [Tooltip("If non-empty, show these beats in sequence instead of StoryBeatOnArrival (e.g. Wode-Wo: Baby → Care → Grown).")]
        public List<StoryBeat> StoryBeatSequenceOnArrival = new List<StoryBeat>();
        [Tooltip("If true, after showing StoryBeatSequenceOnArrival we set Wode-Wo arc to Grown (stage 3). Use for the Baby/Care/Grown sequence.")]
        public bool AdvancesWodeWoArcOnSequenceComplete;
        [Tooltip("When Wode-Wo arc is complete (stage 4), show this positive beat on arrival instead of StoryBeatOnArrival. First time at stage 3 shows it and completes the arc.")]
        public StoryBeat StoryBeatOnArrivalWhenWodeWoComplete;
        [Tooltip("When Wode-Wo has died (fae took him), show this beat on arrival instead of the Baby/Care/Grown sequence (e.g. Malvern remains).")]
        public StoryBeat StoryBeatOnArrivalWhenWodeWoDead;
        [Tooltip("Dialogue with choices before combat (e.g. Green Chapel). Wrong choice = Green Knight appears randomly later.")]
        public DialogueChoiceBeat PreCombatChoiceBeat;
        [Tooltip("Full dialogue tree to run on arrival (overrides flat beats if set). Uses DialogueTreeExecutor.")]
        public DialogueTreeDefinition DialogueTreeOnArrival;

        [Header("Music")]
        [Tooltip("Theme to play on arrival (e.g. orfeo, green_knight).")]
        public string MusicThemeOnArrival;

        [Header("Alternate Ending")]
        [Tooltip("Orfeo's Otherworld - inescapable, no return to map.")]
        public bool IsInescapable;
        [Tooltip("Alternate ending (Orfeo).")]
        public bool IsAlternateEnding;
        [Tooltip("Best outcome: wait for Grace. No next locations. She does not come.")]
        public bool IsWaitingForGrace;

        [Header("Poverty")]
        [Tooltip("Poverty level (0-5) on arrival. -1 = no change.")]
        public int PovertyLevelOnArrival = -1;

        [Header("Green Knight")]
        [Tooltip("Building location. Green Knight may randomly appear here for first appearance to start the cycle.")]
        public bool IsBuilding;

        [Header("Dream vs Real Life (Malvern)")]
        [Tooltip("If true, this is real-life Malvern, England (interstitial). If false, allegorical dream/vision.")]
        public bool IsRealLife;
        [Tooltip("Real Malvern location for interstitial nodes (e.g. worcestershire_beacon, st_annes_well, great_malvern_priory).")]
        public string RealLifeLocationId;

        public bool HasCombat => Encounter != null;
        public bool HasNextLocations => NextLocationIds != null && NextLocationIds.Count > 0;
    }
}
