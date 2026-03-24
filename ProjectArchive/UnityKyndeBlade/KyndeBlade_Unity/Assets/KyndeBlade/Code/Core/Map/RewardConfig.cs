using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Scriptable rewards for a location or encounter. Blessings, unlocks.</summary>
    [CreateAssetMenu(fileName = "Reward", menuName = "KyndeBlade/Reward Config")]
    public class RewardConfig : ScriptableObject
    {
        [Header("Blessings")]
        [Tooltip("Guaranteed blessing offered after this encounter (in addition to random choices).")]
        public Blessing GuaranteedBlessing;
        [Tooltip("Bonus blessing tier weight (higher = more rare blessings in the pool).")]
        public float BlessingTierBonus;

        [Header("Unlocks")]
        [Tooltip("Location IDs to unlock on completion.")]
        public string[] UnlockLocationIds;

        [Header("Campaign")]
        public int VisionIndex;
        public int PassusIndex;
    }
}
