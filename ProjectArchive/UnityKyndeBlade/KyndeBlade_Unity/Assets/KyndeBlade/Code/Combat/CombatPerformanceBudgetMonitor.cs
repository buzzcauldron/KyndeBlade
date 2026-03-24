using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Runtime combat frame-time guardrail. Logs warnings when sustained frame budget is exceeded.
    /// </summary>
    public class CombatPerformanceBudgetMonitor : MonoBehaviour
    {
        [Tooltip("Average frame-time budget in milliseconds.")]
        public float BudgetMs = 16.6f;
        [Tooltip("Spike frame-time budget in milliseconds.")]
        public float SpikeBudgetMs = 24f;
        [Tooltip("Number of consecutive over-budget frames before warning.")]
        public int ConsecutiveFramesForWarning = 30;

        TurnManager _turnManager;
        int _overBudgetFrames;
        int _overSpikeFrames;

        void Start()
        {
            _turnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            var settings = GameRuntime.GameManager != null ? GameRuntime.GameManager.Settings : null;
            if (settings != null)
            {
                BudgetMs = settings.CombatFrameBudgetMs;
                SpikeBudgetMs = settings.CombatFrameSpikeBudgetMs;
            }
        }

        void Update()
        {
            if (_turnManager == null)
                _turnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (_turnManager == null || _turnManager.State == CombatState.CombatEnded)
                return;

            float frameMs = Time.unscaledDeltaTime * 1000f;
            _overBudgetFrames = frameMs > BudgetMs ? _overBudgetFrames + 1 : 0;
            _overSpikeFrames = frameMs > SpikeBudgetMs ? _overSpikeFrames + 1 : 0;

            if (_overBudgetFrames == ConsecutiveFramesForWarning)
            {
                Debug.LogWarning($"[PerfBudget] Sustained over budget: {frameMs:F1}ms > {BudgetMs:F1}ms");
            }

            if (_overSpikeFrames == 3)
            {
                Debug.LogWarning($"[PerfBudget] Repeated spikes detected: {frameMs:F1}ms > {SpikeBudgetMs:F1}ms");
            }
        }
    }
}
