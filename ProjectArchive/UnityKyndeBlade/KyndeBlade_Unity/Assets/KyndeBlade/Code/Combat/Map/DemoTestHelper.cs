using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Debug/test hooks for demo path: force start location, skip to location, log world state. Inspector only; no UI.</summary>
    public class DemoTestHelper : MonoBehaviour
    {
        [Header("Demo / Test Overrides")]
        [Tooltip("If set, new game or first load starts at this location instead of StartLocation/saved. e.g. tour or tower_on_toft.")]
        public string ForceStartLocationId;
        [Tooltip("Target location for Skip To Location (e.g. fayre_felde).")]
        public string SkipToLocationId;

        [Tooltip("If true, Tower on the Toft arrival story beat is skipped and the map shows immediately (faster slice QA).")]
        public bool SkipTowerIntroStory;

        /// <summary>When non-null, WorldMapManager uses this in TryLazyInit instead of saved/StartLocation for initial location.</summary>
        public static string OverrideStartLocationId { get; private set; }

        /// <summary>When true, <see cref="WorldMapManager"/> skips the tour/tower narrative on first load.</summary>
        public static bool SkipTowerIntroStoryActive { get; private set; }

        static int _skipTowerIntroRefCount;

        void Awake()
        {
            if (!string.IsNullOrEmpty(ForceStartLocationId))
                OverrideStartLocationId = ForceStartLocationId;
            else
                OverrideStartLocationId = null;
            if (SkipTowerIntroStory)
            {
                _skipTowerIntroRefCount++;
                SkipTowerIntroStoryActive = true;
            }
        }

        void OnDestroy()
        {
            if (OverrideStartLocationId == ForceStartLocationId)
                OverrideStartLocationId = null;
            if (SkipTowerIntroStory)
            {
                _skipTowerIntroRefCount = Mathf.Max(0, _skipTowerIntroRefCount - 1);
                SkipTowerIntroStoryActive = _skipTowerIntroRefCount > 0;
            }
        }

        [ContextMenu("Skip to Location")]
        public void SkipToLocation()
        {
            if (string.IsNullOrEmpty(SkipToLocationId)) return;
            var wm = GameRuntime.WorldMapManager ?? UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            if (wm == null) return;
            var loc = wm.GetLocation(SkipToLocationId);
            if (loc != null)
                wm.TransitionTo(loc);
#if UNITY_EDITOR
            else
                Debug.LogWarning($"[DemoTestHelper] Location not found: {SkipToLocationId}");
#endif
        }

        [ContextMenu("Log World State")]
        public void LogState()
        {
#if UNITY_EDITOR
            var save = GameRuntime.SaveManager ?? UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            var wm = GameRuntime.WorldMapManager ?? UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            var locId = wm?.CurrentLocation?.LocationId ?? "(none)";
            int misstep = save?.CurrentProgress?.EthicalMisstepCount ?? 0;
            bool gk = save?.GreenKnightWillAppearRandomly ?? false;
            bool hunger = save?.CurrentProgress?.HasEverHadHunger ?? false;
            Debug.Log($"[DemoTestHelper] Location={locId} | EthicalMisstep={misstep} | GreenKnightRandom={gk} | HasEverHadHunger={hunger}");
#endif
        }
    }
}
