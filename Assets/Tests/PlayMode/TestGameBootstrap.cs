using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    /// <summary>Minimal headless scene setup for Play Mode tests. No Main Camera, no UI Canvas.</summary>
    public static class TestGameBootstrap
    {
        /// <summary>Creates TurnManager only. Use for combat-only tests.</summary>
        public static TurnManager CreateTurnManager()
        {
            var go = new GameObject("TurnManager");
            return go.AddComponent<TurnManager>();
        }

        /// <summary>Creates a test character with default combat actions. No prefab, no visuals.</summary>
        public static MedievalCharacter CreateTestCharacter(string name, CharacterClass cls, bool isPlayer)
        {
            var go = new GameObject(name);
            if (go.GetComponent<Collider>() == null)
                go.AddComponent<BoxCollider>(); // MedievalCharacter requires Collider
            var c = go.AddComponent<MedievalCharacter>();
            c.CharacterClassType = cls;
            c.CharacterName = name;
            DefaultCombatActions.AddDefaultsTo(c);
            return c;
        }

        /// <summary>Creates a minimal player + enemy setup for combat tests.</summary>
        public static void CreateCombatSetup(out List<MedievalCharacter> players, out List<MedievalCharacter> enemies)
        {
            players = new List<MedievalCharacter>
            {
                CreateTestCharacter("TestKnight", CharacterClass.Knight, true),
                CreateTestCharacter("TestMage", CharacterClass.Mage, true)
            };
            enemies = new List<MedievalCharacter>
            {
                CreateTestCharacter("TestEnemy1", CharacterClass.Knight, false),
                CreateTestCharacter("TestEnemy2", CharacterClass.Mage, false)
            };
        }

        /// <summary>Creates SaveManager. Uses PlayerPrefs - tests should clear or use isolated key if needed.</summary>
        public static SaveManager CreateSaveManager()
        {
            var go = new GameObject("SaveManager");
            return go.AddComponent<SaveManager>();
        }

        /// <summary>Creates WorldMapManager with minimal setup.</summary>
        public static WorldMapManager CreateWorldMapManager(SaveManager saveManager = null)
        {
            var go = new GameObject("WorldMapManager");
            var wm = go.AddComponent<WorldMapManager>();
            if (saveManager != null)
                wm.SaveManager = saveManager;
            return wm;
        }

        /// <summary>Creates AgingManager with SaveManager reference.</summary>
        public static AgingManager CreateAgingManager(SaveManager saveManager)
        {
            var go = new GameObject("AgingManager");
            var am = go.AddComponent<AgingManager>();
            am.SaveManager = saveManager;
            return am;
        }

        /// <summary>Creates PovertyManager with SaveManager reference.</summary>
        public static PovertyManager CreatePovertyManager(SaveManager saveManager)
        {
            var go = new GameObject("PovertyManager");
            var pm = go.AddComponent<PovertyManager>();
            pm.SaveManager = saveManager;
            return pm;
        }

        /// <summary>Destroys all GameObjects created during test. Call in teardown.</summary>
        public static void Cleanup(params Object[] objects)
        {
            foreach (var o in objects)
            {
                if (o != null)
                    Object.DestroyImmediate(o);
            }
        }
    }
}
