using System;
using UnityEngine;

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
        public int Level = 1;

        public CharacterStats() { }
    }
}
