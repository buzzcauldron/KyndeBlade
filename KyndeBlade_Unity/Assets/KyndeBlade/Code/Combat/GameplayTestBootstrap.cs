using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Placeholder bootstrap for a short gameplay test. Creates minimal instances of major game systems (Save, Narrative, Dialogue, Music, Map, Combat UI, Game State, Illumination, Aging) so the scene initializes without NRE. Add with KyndeBladeGameManager for a playable combat/map test. Runs before GameManager (execution order -1100).</summary>
    [DefaultExecutionOrder(-1100)]
    public class GameplayTestBootstrap : MonoBehaviour
    {
        [Header("Options")]
        [Tooltip("Create a minimal Canvas with CombatUI so victory/defeat panels can attach.")]
        public bool EnsureCombatUI = true;
        [Tooltip("Create SaveManager (in-memory progress, optional persistence).")]
        public bool EnsureSaveManager = true;
        [Tooltip("Create NarrativeManager + DialogueSystem for story beats and choices.")]
        public bool EnsureNarrative = true;
        [Tooltip("Create MusicManager (no clips = silent).")]
        public bool EnsureMusicManager = true;
        [Tooltip("Create WorldMapManager (empty locations until level data exists).")]
        public bool EnsureWorldMapManager = true;
        [Tooltip("Create GameStateManager for victory/defeat panels.")]
        public bool EnsureGameStateManager = true;
        [Tooltip("Create IlluminationManager for victory/defeat flashes.")]
        public bool EnsureIlluminationManager = true;
        [Tooltip("Create AgingManager (optional for short test).")]
        public bool EnsureAgingManager = true;

        const string BootstrapRootName = "GameplayTestSystems";

        void Awake()
        {
            var root = GetOrCreateRoot();

            if (EnsureSaveManager && Object.FindFirstObjectByType<SaveManager>() == null)
            {
                var go = new GameObject("SaveManager");
                go.transform.SetParent(root);
                go.AddComponent<SaveManager>();
            }

            if (EnsureNarrative)
            {
                if (Object.FindFirstObjectByType<DialogueSystem>() == null)
                {
                    var canvasGo = new GameObject("DialogueCanvas");
                    canvasGo.transform.SetParent(root);
                    var canvas = canvasGo.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                    canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    canvasGo.AddComponent<DialogueSystem>();
                }
                if (Object.FindFirstObjectByType<NarrativeManager>() == null)
                {
                    var go = new GameObject("NarrativeManager");
                    go.transform.SetParent(root);
                    go.AddComponent<NarrativeManager>();
                }
            }

            if (EnsureMusicManager && Object.FindFirstObjectByType<MusicManager>() == null)
            {
                var go = new GameObject("MusicManager");
                go.transform.SetParent(root);
                go.AddComponent<MusicManager>();
            }

            if (EnsureWorldMapManager && Object.FindFirstObjectByType<WorldMapManager>() == null)
            {
                var go = new GameObject("WorldMapManager");
                go.transform.SetParent(root);
                go.AddComponent<WorldMapManager>();
            }

            if (EnsureCombatUI && Object.FindFirstObjectByType<CombatUI>() == null)
            {
                var canvasGo = new GameObject("CombatUICanvas");
                canvasGo.transform.SetParent(root);
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGo.AddComponent<CombatUI>();
            }

            if (EnsureGameStateManager && Object.FindFirstObjectByType<GameStateManager>() == null)
            {
                var go = new GameObject("GameStateManager");
                go.transform.SetParent(root);
                go.AddComponent<GameStateManager>();
            }

            if (EnsureIlluminationManager && Object.FindFirstObjectByType<IlluminationManager>() == null)
            {
                var go = new GameObject("IlluminationManager");
                go.transform.SetParent(root);
                go.AddComponent<IlluminationManager>();
            }

            if (EnsureAgingManager && Object.FindFirstObjectByType<AgingManager>() == null)
            {
                var go = new GameObject("AgingManager");
                go.transform.SetParent(root);
                go.AddComponent<AgingManager>();
            }
        }

        Transform GetOrCreateRoot()
        {
            var existing = transform.Find(BootstrapRootName);
            if (existing != null) return existing;
            var root = new GameObject(BootstrapRootName).transform;
            root.SetParent(transform);
            return root;
        }
    }
}
