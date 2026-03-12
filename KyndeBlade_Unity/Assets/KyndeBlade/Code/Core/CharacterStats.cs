using System;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    [Serializable]
    public class CharacterStats
    {
        public float MaxHealth = 100f;
        public float CurrentHealth = 100f;
        public float MaxStamina = 100f;
        public float CurrentStamina = 100f;
        public float AttackPower = 10f;
        public float Defense = 5f;
        public float Speed = 10f;
        public float MaxKynde = 10f;
        public float CurrentKynde = 0f;
        public float MaxBreakGauge = 100f;
        public float CurrentBreakGauge = 100f;
        public bool IsBroken;
        public float BrokenStunRemaining;

        [Tooltip("Number of blessings received this run (display only, replaces traditional level).")]
        public int BlessingCount;

        [NonSerialized]
        public List<ActiveBlessing> ActiveBlessings = new List<ActiveBlessing>();

        [Tooltip("Tracks first-hit immunity consumed this combat.")]
        [NonSerialized]
        public bool FirstHitUsed;

        public CharacterStats() { }
    }
}
