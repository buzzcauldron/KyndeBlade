using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class Phase5GameSystemsTests
    {
        [UnityTest]
        public IEnumerator Save_MarkHunger_IncrementCounters_PersistFields()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var go = new GameObject("Save");
            var save = go.AddComponent<SaveManager>();
            yield return null;
            save.MarkHasEverHadHunger();
            Assert.IsTrue(save.HasEverHadHunger);
            save.IncrementEldeHitsAccrued();
            Assert.AreEqual(1, save.CurrentProgress.EldeHitsAccrued);
            save.IncrementEthicalMisstep();
            Assert.Greater(save.GetDamageTakenMultiplier(), 1f);
            Object.DestroyImmediate(go);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator Save_Meta_LifetimeRunCount_PersistsAcrossNewGame()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var go = new GameObject("Save");
            var save = go.AddComponent<SaveManager>();
            yield return null;
            save.IncrementLifetimeRunCount();
            save.IncrementLifetimeRunCount();
            Assert.AreEqual(2, save.LifetimeRunCount);
            save.NewGame(GameWorldConstants.LocationMalvern);
            Assert.AreEqual(2, save.LifetimeRunCount);
            Assert.AreEqual(1.04f, save.GetMultiRunScalingFactor(), 0.001f);
            Object.DestroyImmediate(go);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator Map_SetCurrent_GetNext_Transition_NoSceneLoad()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var saveGo = new GameObject("S");
            var save = saveGo.AddComponent<SaveManager>();
            yield return null;
            var a = TestGameBootstrap.CreateTestLocation("loc_a", "loc_b");
            var b = TestGameBootstrap.CreateTestLocation("loc_b");
            var wmGo = new GameObject("WM");
            var wm = wmGo.AddComponent<WorldMapManager>();
            wm.SaveManager = save;
            wm.NarrativeManager = null;
            wm.GameManager = null;
            wm.AllLocations = new List<LocationNode> { a, b };
            wm.RebuildLocationIndex();
            wm.SetCurrentLocation(a);
            Assert.AreEqual(a, wm.CurrentLocation);
            var next = wm.GetNextLocations();
            Assert.IsTrue(next.Exists(l => l.LocationId == "loc_b"));
            wm.TransitionTo(b);
            Assert.AreEqual("loc_b", save.CurrentProgress.CurrentLocationId);

            Object.DestroyImmediate(wmGo);
            Object.DestroyImmediate(saveGo);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator Hazards_SetAndInterval_ExhaustionOnPlayerTurn()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var hzGo = new GameObject("Hz");
            var hz = hzGo.AddComponent<CombatHazardManager>();
            hz.TurnManager = tm;
            yield return null;
            var cfg = ScriptableObject.CreateInstance<PiersHazardConfig>();
            cfg.Type = PiersHazardType.Exhaustion;
            cfg.Strength = 2f;
            cfg.IntervalTurns = 1;
            hz.SetHazards(new List<PiersHazardConfig> { cfg });

            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            p.Stats.Speed = 50f;
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            float st = p.GetCurrentStamina();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();

            Assert.Less(p.GetCurrentStamina(), st);

            Object.DestroyImmediate(hzGo);
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
            Object.DestroyImmediate(cfg);
        }

        [UnityTest]
        public IEnumerator HungerCharacter_Start_AppliesMovesetAndIdentity()
        {
            yield return null;
            var go = new GameObject("Hunger");
            go.AddComponent<BoxCollider>();
            var h = go.AddComponent<HungerCharacter>();
            yield return null;
            Assert.AreEqual("Hunger", h.CharacterName);
            Assert.Greater(h.AvailableActions.Count, 0);

            Object.DestroyImmediate(go);
        }

        [UnityTest]
        public IEnumerator SimpleEnemyAI_ChooseAction_PicksAffordableStrike()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            e.Stats.Speed = 100f;
            p.Stats.Speed = 1f;
            DefaultCombatActions.AddDefaultsTo(e);
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            var aiGo = e.gameObject;
            var ai = aiGo.AddComponent<SimpleEnemyAI>();
            ai.TurnManager = tm;
            ai.DecisionDelay = 0f;
            var mi = typeof(SimpleEnemyAI).GetMethod("ExecuteTurn", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(ai, null);
            Assert.Less(p.GetCurrentHealth(), p.Stats.MaxHealth);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator AdaptiveEnemyAI_ChooseTarget_PrefersLowerHp()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var hi = TestGameBootstrap.CreateTestCharacter("Hi", CharacterClass.Knight, true);
            var lo = TestGameBootstrap.CreateTestCharacter("Lo", CharacterClass.Mage, true);
            hi.Stats.CurrentHealth = 80f;
            lo.Stats.CurrentHealth = 10f;
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Rogue, false);
            tm.InitializeCombat(new List<MedievalCharacter> { hi, lo }, new List<MedievalCharacter> { e });
            var ai = e.gameObject.AddComponent<AdaptiveEnemyAI>();
            ai.TurnManager = tm;
            var mi = typeof(AdaptiveEnemyAI).GetMethod("ChooseTarget", BindingFlags.NonPublic | BindingFlags.Instance);
            var pick = (MedievalCharacter)mi.Invoke(ai, null);
            Assert.AreEqual(lo, pick);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(hi.gameObject);
            Object.DestroyImmediate(lo.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }
    }
}
