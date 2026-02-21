using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Piers-themed environmental hazards. Work, poverty, labor, exhaustion.</summary>
    public enum PiersHazardType
    {
        Exhaustion,
        Poverty,
        Labor,
        Hunger
    }

    /// <summary>Hazard config for encounters. Based on Piers Plowman themes.</summary>
    [CreateAssetMenu(fileName = "Hazard", menuName = "KyndeBlade/Piers Hazard Config")]
    public class PiersHazardConfig : ScriptableObject
    {
        public PiersHazardType Type;
        [Tooltip("Strength (e.g. stamina drain per turn, Kynde reduction multiplier).")]
        public float Strength = 1f;
        [Tooltip("Apply every N turns. 1 = every turn.")]
        public int IntervalTurns = 1;
    }
}
