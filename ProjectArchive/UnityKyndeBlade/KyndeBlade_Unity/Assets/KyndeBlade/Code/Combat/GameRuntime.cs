using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Optional central registry for bootstrap-created managers. Set by KyndeBladeGameManager.Awake(); cleared on GameManager OnDestroy so the next scene does not see stale refs.</summary>
    public static class GameRuntime
    {
        public static TurnManager TurnManager { get; set; }
        public static KyndeBladeGameManager GameManager { get; set; }
        public static SaveManager SaveManager { get; set; }
        public static CombatUI CombatUI { get; set; }
        public static GameStateManager GameStateManager { get; set; }
        public static WorldMapManager WorldMapManager { get; set; }
        public static NarrativeManager NarrativeManager { get; set; }
        public static MusicManager MusicManager { get; set; }
        public static DialogueSystem DialogueSystem { get; set; }
        public static GameFlowController GameFlowController { get; set; }

        /// <summary>Clear all refs so the next scene does not use stale managers. Called by KyndeBladeGameManager.OnDestroy.</summary>
        public static void Clear()
        {
            TurnManager = null;
            GameManager = null;
            SaveManager = null;
            CombatUI = null;
            GameStateManager = null;
            WorldMapManager = null;
            NarrativeManager = null;
            MusicManager = null;
            DialogueSystem = null;
            GameFlowController = null;
        }
    }
}
