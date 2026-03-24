using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Defines enemies and optional boss for a location encounter.</summary>
    [CreateAssetMenu(fileName = "Encounter", menuName = "KyndeBlade/Encounter Config")]
    [Serializable]
    public class EncounterConfig : ScriptableObject
    {
        [Header("Enemies")]
        [Tooltip("Enemy prefabs or types to spawn. Positions are relative to formation.")]
        public List<EnemySpawnEntry> Enemies = new List<EnemySpawnEntry>();

        [Header("Boss (optional)")]
        public MedievalCharacter BossPrefab;
        public string BossCharacterType;
        public Vector3 BossPosition = new Vector3(4f, 0f, 0f);

        [Header("Formation")]
        public Vector3 EnemyFormationOffset = new Vector3(4f, 0f, 0f);
        public float EnemySpacing = 1.5f;

        [Header("Piers Hazards (Limbo-style)")]
        [Tooltip("Environmental hazards: Exhaustion, Poverty, Labor, Hunger. Applied each turn.")]
        public List<PiersHazardConfig> Hazards = new List<PiersHazardConfig>();

        [Serializable]
        public class EnemySpawnEntry
        {
            public MedievalCharacter Prefab;
            public string CharacterTypeName;
            public Vector3 Position;
            [Tooltip("Multiply instantiated enemy lossy scale (e.g. 1.4 for imposing boss). 1 = default.")]
            public float LocalScaleMultiplier = 1f;
        }
    }
}
