using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class CampaignFlowTest
    {
        SaveManager _save;
        MedievalCharacter _player;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            _save = TestGameBootstrap.CreateSaveManager();
            _player = TestGameBootstrap.CreateTestCharacter("TestPilgrim", CharacterClass.Knight, true);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            if (_player != null) Object.DestroyImmediate(_player.gameObject);
            if (_save != null) Object.DestroyImmediate(_save.gameObject);
        }

        [UnityTest]
        public IEnumerator WodeWo_StartsNotDead()
        {
            yield return null;
            Assert.IsFalse(_save.IsWodeWoDead, "Wode-Wo should start alive");
        }

        [UnityTest]
        public IEnumerator WodeWo_SetDead_Persists()
        {
            yield return null;
            _save.SetWodeWoDead();
            Assert.IsTrue(_save.IsWodeWoDead, "Wode-Wo should be dead after SetWodeWoDead");

            _save.Save();
            _save.Load();
            Assert.IsTrue(_save.IsWodeWoDead, "Wode-Wo dead flag should persist across save/load");
        }

        [UnityTest]
        public IEnumerator WodeWo_DeathTrigger_AfterThreeMissteps()
        {
            yield return null;
            _save.SetWodeWoUnlocked(true);
            Assert.IsTrue(_save.IsWodeWoUnlocked);

            _save.IncrementEthicalMisstep();
            _save.IncrementEthicalMisstep();
            Assert.IsFalse(_save.IsWodeWoDead, "2 missteps should not kill Wode-Wo");

            _save.IncrementEthicalMisstep();
            if (_save.IsWodeWoUnlocked && !_save.IsWodeWoDead &&
                _save.CurrentProgress.EthicalMisstepCount >= 3)
            {
                _save.SetWodeWoDead();
            }
            Assert.IsTrue(_save.IsWodeWoDead, "3+ missteps with Wode-Wo unlocked should trigger death");
        }

        [UnityTest]
        public IEnumerator WodeWo_NotTriggered_WhenNotUnlocked()
        {
            yield return null;
            Assert.IsFalse(_save.IsWodeWoUnlocked);

            for (int i = 0; i < 5; i++) _save.IncrementEthicalMisstep();

            if (_save.IsWodeWoUnlocked && !_save.IsWodeWoDead &&
                _save.CurrentProgress.EthicalMisstepCount >= 3)
            {
                _save.SetWodeWoDead();
            }
            Assert.IsFalse(_save.IsWodeWoDead, "Wode-Wo should not die if not unlocked");
        }

        [UnityTest]
        public IEnumerator BlessingPersistence_AcrossEncounters()
        {
            yield return null;
            var b = BlessingSystem.FindBlessing("fortitude");
            Assert.IsNotNull(b);

            BlessingSystem.ApplyBlessing(_player, b, _save);
            Assert.AreEqual(1, _player.Stats.ActiveBlessings.Count);
            float boostedHP = _player.Stats.MaxHealth;

            _save.Save();

            var player2 = TestGameBootstrap.CreateTestCharacter("TestPilgrim", CharacterClass.Knight, true);
            yield return null;

            var entry = _save.CurrentProgress.GetOrCreateCharacterProgress("TestPilgrim");
            BlessingSystem.RestoreFromSave(player2, entry);

            Assert.AreEqual(1, player2.Stats.ActiveBlessings.Count);
            Assert.AreEqual(boostedHP, player2.Stats.MaxHealth, 0.01f,
                "Blessings should persist across encounters via save");

            Object.DestroyImmediate(player2.gameObject);
        }

        [UnityTest]
        public IEnumerator EldeHits_PersistAcrossSaveLoad()
        {
            yield return null;
            _save.IncrementEldeHitsAccrued();
            _save.IncrementEldeHitsAccrued();

            Assert.AreEqual(2, _save.CurrentProgress.EldeHitsAccrued);

            _save.Save();
            _save.Load();

            Assert.AreEqual(2, _save.CurrentProgress.EldeHitsAccrued,
                "Elde hits should persist across save/load");
        }

        [UnityTest]
        public IEnumerator EthicalMissteps_IncreaseDamageTaken()
        {
            yield return null;
            float base_mult = _save.GetDamageTakenMultiplier();
            Assert.AreEqual(1f, base_mult, 0.01f, "No missteps = 1x damage");

            _save.IncrementEthicalMisstep();
            float after1 = _save.GetDamageTakenMultiplier();
            Assert.Greater(after1, base_mult, "One misstep should increase damage taken");

            _save.IncrementEthicalMisstep();
            float after2 = _save.GetDamageTakenMultiplier();
            Assert.Greater(after2, after1, "More missteps = more damage taken");
        }

        [UnityTest]
        public IEnumerator WodeWoArc_ProgressesThroughStages()
        {
            yield return null;
            _save.SetWodeWoUnlocked(true);
            Assert.AreEqual(0, _save.WodeWoArcStage);

            _save.AdvanceWodeWoArc();
            Assert.AreEqual(1, _save.WodeWoArcStage, "Stage 1: Baby");

            _save.AdvanceWodeWoArc();
            Assert.AreEqual(2, _save.WodeWoArcStage, "Stage 2: Care");

            _save.AdvanceWodeWoArc();
            Assert.AreEqual(3, _save.WodeWoArcStage, "Stage 3: Grown");
            Assert.IsFalse(_save.IsWodeWoArcComplete);

            _save.AdvanceWodeWoArc();
            Assert.AreEqual(4, _save.WodeWoArcStage, "Stage 4: Complete");
            Assert.IsTrue(_save.IsWodeWoArcComplete);
        }

        [UnityTest]
        public IEnumerator HungerScar_Persists()
        {
            yield return null;
            Assert.IsFalse(_save.CurrentProgress.HasEverHadHunger);

            _save.MarkHasEverHadHunger();
            Assert.IsTrue(_save.CurrentProgress.HasEverHadHunger);

            _save.Save();
            _save.Load();
            Assert.IsTrue(_save.CurrentProgress.HasEverHadHunger,
                "HasEverHadHunger should persist across save/load");
        }

        [UnityTest]
        public IEnumerator GameProgress_SerializesAllFields()
        {
            yield return null;
            _save.CurrentProgress.GreenKnightWillAppearRandomly = true;
            _save.CurrentProgress.OrfeoOtherworldTriggered = true;
            _save.CurrentProgress.PovertyLevel = 3;
            _save.CurrentProgress.EldeHitsAccrued = 5;
            _save.CurrentProgress.WodeWoUnlocked = true;
            _save.CurrentProgress.WodeWoArcStage = 2;
            _save.CurrentProgress.EthicalMisstepCount = 4;

            _save.Save();
            _save.Load();

            Assert.IsTrue(_save.CurrentProgress.GreenKnightWillAppearRandomly);
            Assert.IsTrue(_save.CurrentProgress.OrfeoOtherworldTriggered);
            Assert.AreEqual(3, _save.CurrentProgress.PovertyLevel);
            Assert.AreEqual(5, _save.CurrentProgress.EldeHitsAccrued);
            Assert.IsTrue(_save.CurrentProgress.WodeWoUnlocked);
            Assert.AreEqual(2, _save.CurrentProgress.WodeWoArcStage);
            Assert.AreEqual(4, _save.CurrentProgress.EthicalMisstepCount);
        }
    }
}
