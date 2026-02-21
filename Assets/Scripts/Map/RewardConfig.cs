using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Scriptable rewards for a location or encounter. XP, items, unlocks.</summary>
    [CreateAssetMenu(fileName = "Reward", menuName = "KyndeBlade/Reward Config")]
    public class RewardConfig : ScriptableObject
    {
        [Header("XP")]
        public int BaseXP = 50;
        public int BonusXPForVictory = 100;

        [Header("Unlocks")]
        [Tooltip("Location IDs to unlock on completion.")]
        public string[] UnlockLocationIds;

        [Header("Campaign")]
        public int VisionIndex;
        public int PassusIndex;
    }
}
