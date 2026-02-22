using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Triggers transition to a location. Attach to UI buttons or trigger colliders.</summary>
    public class SceneTransition : MonoBehaviour
    {
        public LocationNode TargetLocation;
        public WorldMapManager WorldMap;

        public void Transition()
        {
            var wm = WorldMap != null ? WorldMap : UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            if (wm != null && TargetLocation != null)
                wm.TransitionTo(TargetLocation);
        }
    }
}
