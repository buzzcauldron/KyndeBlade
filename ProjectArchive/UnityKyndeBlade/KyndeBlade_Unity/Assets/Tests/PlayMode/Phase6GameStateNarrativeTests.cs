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
    public class Phase6GameStateNarrativeTests
    {
        [UnityTest]
        public IEnumerator GameStateManager_CalculateVictoryXP_Formula()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var e1 = TestGameBootstrap.CreateTestCharacter("E1", CharacterClass.Mage, false);
            var e2 = TestGameBootstrap.CreateTestCharacter("E2", CharacterClass.Mage, false);
            e1.Stats.CurrentHealth = 0f;
            e2.Stats.CurrentHealth = 0f;
            tm.InitializeCombat(new List<MedievalCharacter>(), new List<MedievalCharacter> { e1, e2 });

            var gsmGo = new GameObject("GSM");
            var gsm = gsmGo.AddComponent<GameStateManager>();
            gsm.TurnManager = tm;
            gsm.BonusXPForVictory = 100;
            gsm.BaseXPPerEnemy = 50;
            yield return null;

            var mi = typeof(GameStateManager).GetMethod("CalculateVictoryXP",
                BindingFlags.NonPublic | BindingFlags.Instance);
            int xp = (int)mi.Invoke(gsm, null);
            Assert.AreEqual(200, xp);

            Object.DestroyImmediate(gsmGo);
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(e1.gameObject);
            Object.DestroyImmediate(e2.gameObject);
        }

        [UnityTest]
        public IEnumerator BossDialogueManager_TriggerEntry_FiresLine()
        {
            yield return null;
            var dmGo = new GameObject("BDM");
            var dm = dmGo.AddComponent<BossDialogueManager>();
            var bossGo = TestGameBootstrap.CreateTestCharacter("Boss", CharacterClass.Knight, false);
            var script = ScriptableObject.CreateInstance<BossDialogueScript>();
            script.EntryLine = "Test entry line.";
            string got = null;
            dm.OnBossLineRequested += (line, _) => got = line;
            dm.SetCurrentBoss(bossGo, script);
            dm.TriggerEntry();
            Assert.AreEqual("Test entry line.", got);

            Object.DestroyImmediate(dmGo);
            Object.DestroyImmediate(bossGo.gameObject);
            Object.DestroyImmediate(script);
        }

        [UnityTest]
        public IEnumerator NarrativeManager_ShowStoryBeat_StartsWithoutDialogueSystem()
        {
            yield return null;
            var nmGo = new GameObject("NM");
            var nm = nmGo.AddComponent<NarrativeManager>();
            nm.DialogueSystem = null;
            var beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.Text = "Narrative test";
            StoryBeat started = null;
            nm.OnStoryBeatStarted += b => started = b;
            nm.ShowStoryBeat(beat);
            Assert.AreEqual(beat, started);

            Object.DestroyImmediate(nmGo);
            Object.DestroyImmediate(beat);
        }

        [UnityTest]
        public IEnumerator SecretCodeListener_SetsWodeWoUnlocked()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var saveGo = new GameObject("S");
            var save = saveGo.AddComponent<SaveManager>();
            yield return null;
            var scGo = new GameObject("SC");
            var sc = scGo.AddComponent<SecretCodeListener>();
            sc.SimulatePasscodeEnteredForTests();
            Assert.IsTrue(save.IsWodeWoUnlocked);
            Object.DestroyImmediate(scGo);
            Object.DestroyImmediate(saveGo);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator AgingManager_FieldOfGrace_DeathOfOldAgeRequested()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var saveGo = new GameObject("S");
            var save = saveGo.AddComponent<SaveManager>();
            yield return null;
            save.CurrentProgress.TotalPlayTimeSeconds = 40000f;

            var loc = TestGameBootstrap.CreateTestLocation("field_of_grace");
            var wmGo = new GameObject("WM");
            var wm = wmGo.AddComponent<WorldMapManager>();
            wm.SaveManager = save;
            wm.AllLocations = new List<LocationNode> { loc };
            wm.RebuildLocationIndex();
            wm.SetCurrentLocation(loc);

            var amGo = new GameObject("AM");
            var am = amGo.AddComponent<AgingManager>();
            am.SaveManager = save;
            am.WorldMapManager = wm;
            am.AgeTierDeathThreshold = 5;
            am.FieldOfGraceTimeMultiplier = 1f;

            bool fired = false;
            am.OnDeathOfOldAgeRequested += () => fired = true;
            yield return null;
            yield return null;
            Assert.IsTrue(fired);

            Object.DestroyImmediate(amGo);
            Object.DestroyImmediate(wmGo);
            Object.DestroyImmediate(saveGo);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator Integration_QuickCombat_PlayerWins()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            p.Stats.Speed = 50f;
            e.Stats.MaxHealth = 5f;
            e.Stats.CurrentHealth = 5f;
            e.Stats.Defense = 0f;
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            tm.ExecuteAction(DefaultCombatActions.CreateStrike(), e);
            yield return new WaitForSeconds(0.15f);
            Assert.IsFalse(e.IsAlive());
            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator EldeHit_IncrementsSaveAndAppliesAge()
        {
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var saveGo = new GameObject("S");
            var save = saveGo.AddComponent<SaveManager>();
            var amGo = new GameObject("AM");
            var am = amGo.AddComponent<AgingManager>();
            am.SaveManager = save;
            yield return null;

            var victim = TestGameBootstrap.CreateTestCharacter("V", CharacterClass.Knight, true);
            var eldeGo = new GameObject("Elde");
            eldeGo.AddComponent<BoxCollider>();
            var elde = eldeGo.AddComponent<EldeCharacter>();
            yield return null;

            victim.Stats.Defense = 0f;
            float hpBefore = victim.GetCurrentHealth();
            victim.ApplyCustomDamage(1f, elde);
            Assert.Less(victim.GetCurrentHealth(), hpBefore);
            Assert.Greater(save.CurrentProgress.EldeHitsAccrued, 0);
            Assert.IsTrue(victim.HasStatusEffect(StatusEffectType.Age));

            Object.DestroyImmediate(eldeGo);
            Object.DestroyImmediate(victim.gameObject);
            Object.DestroyImmediate(amGo);
            Object.DestroyImmediate(saveGo);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }
    }
}
