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
        [Tooltip("If non-empty, show these beats in sequence instead of StoryBeatOnArrival (e.g. baby Wode-Wo → care → grown).")]
        public List<StoryBeat> StoryBeatSequenceOnArrival = new List<StoryBeat>();
        [Tooltip("If set and Wode-Wo is dead (install-level), show this instead of StoryBeatOnArrival. His body persists.")]
        public StoryBeat StoryBeatOnArrivalWhenWodeWoDead;
        [Tooltip("Dialogue with choices before combat (e.g. Green Chapel). Wrong choice = Green Knight appears randomly later.")]
        public DialogueChoiceBeat PreCombatChoiceBeat;

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
