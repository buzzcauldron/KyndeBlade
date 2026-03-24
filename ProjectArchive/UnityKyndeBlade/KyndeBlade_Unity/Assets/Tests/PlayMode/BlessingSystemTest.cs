using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class BlessingSystemTest
    {
        MedievalCharacter _player;
        SaveManager _save;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            _save = TestGameBootstrap.CreateSaveManager();
            _player = TestGameBootstrap.CreateTestCharacter("TestKnight", CharacterClass.Knight, true);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            if (_player != null) Object.DestroyImmediate(_player.gameObject);
            if (_save != null) Object.DestroyImmediate(_save.gameObject);
        }

        [UnityTest]
        public IEnumerator GenerateChoices_ReturnsUpToThree()
        {
            yield return null;
            var choices = BlessingSystem.GenerateChoices(0f, _save);
            Assert.IsNotNull(choices);
            Assert.LessOrEqual(choices.Count, 3);
            Assert.Greater(choices.Count, 0, "Should generate at least one choice");
        }

        [UnityTest]
        public IEnumerator GenerateChoices_NoDuplicates()
        {
            yield return null;
            var choices = BlessingSystem.GenerateChoices(15f, _save);
            var ids = new HashSet<string>();
            foreach (var c in choices)
            {
                Assert.IsFalse(ids.Contains(c.BlessingId), $"Duplicate blessing: {c.BlessingId}");
                ids.Add(c.BlessingId);
            }
        }

        [UnityTest]
        public IEnumerator ApplyBlessing_ModifiesStats()
        {
            yield return null;
            var b = BlessingSystem.FindBlessing("fortitude");
            Assert.IsNotNull(b, "Fortitude blessing should exist in pool");

            float hpBefore = _player.Stats.MaxHealth;
            BlessingSystem.ApplyBlessing(_player, b, _save);

            Assert.Greater(_player.Stats.MaxHealth, hpBefore, "Fortitude should increase MaxHealth");
            Assert.AreEqual(1, _player.Stats.ActiveBlessings.Count);
            Assert.AreEqual("fortitude", _player.Stats.ActiveBlessings[0].BlessingId);
        }

        [UnityTest]
        public IEnumerator ApplyBlessing_PersistsToSave()
        {
            yield return null;
            var b = BlessingSystem.FindBlessing("charity");
            Assert.IsNotNull(b);

            BlessingSystem.ApplyBlessing(_player, b, _save);

            Assert.IsNotNull(_save.CurrentProgress);
            var entry = _save.CurrentProgress.GetOrCreateCharacterProgress("TestKnight");
            Assert.IsNotNull(entry.Blessings);
            Assert.AreEqual(1, entry.Blessings.Count);
            Assert.AreEqual("charity", entry.Blessings[0].BlessingId);
        }

        [UnityTest]
        public IEnumerator RestoreFromSave_AppliesSavedBlessings()
        {
            yield return null;
            var b = BlessingSystem.FindBlessing("fortitude");
            Assert.IsNotNull(b);
            BlessingSystem.ApplyBlessing(_player, b, _save);
            float hpAfterApply = _player.Stats.MaxHealth;

            var player2 = TestGameBootstrap.CreateTestCharacter("TestKnight", CharacterClass.Knight, true);
            yield return null;
            float hpBase = player2.Stats.MaxHealth;
            var entry = _save.CurrentProgress.GetOrCreateCharacterProgress("TestKnight");
            BlessingSystem.RestoreFromSave(player2, entry);

            Assert.AreEqual(hpAfterApply, player2.Stats.MaxHealth, 0.01f,
                "Restored character should have same HP boost");
            Assert.AreEqual(1, player2.Stats.ActiveBlessings.Count);

            Object.DestroyImmediate(player2.gameObject);
        }

        [UnityTest]
        public IEnumerator GetModifiers_ReturnsCorrectValues()
        {
            yield return null;
            var patience = BlessingSystem.FindBlessing("patience");
            Assert.IsNotNull(patience);

            BlessingSystem.ApplyBlessing(_player, patience, _save);
            var mods = BlessingSystem.GetModifiers(_player);

            Assert.AreEqual(1.2f, mods.TimingWindowMultiplier, 0.01f,
                "Patience should set timing window multiplier to 1.2");
        }

        [UnityTest]
        public IEnumerator GetModifiers_HealPerTurn_FromCharity()
        {
            yield return null;
            var charity = BlessingSystem.FindBlessing("charity");
            Assert.IsNotNull(charity);

            BlessingSystem.ApplyBlessing(_player, charity, _save);
            var mods = BlessingSystem.GetModifiers(_player);

            Assert.AreEqual(3f, mods.HealPerTurn, 0.01f,
                "Charity should provide 3 HP per turn");
        }

        [UnityTest]
        public IEnumerator LocationGated_NotOfferedWithoutVisit()
        {
            yield return null;
            var pool = BlessingPool.CreateDefaultBlessings();
            var pilgrim = pool.Find(b => b.Category == BlessingCategory.Pilgrimage &&
                                         !string.IsNullOrEmpty(b.RequiredLocationVisited));
            Assert.IsNotNull(pilgrim, "Pool should contain pilgrimage blessings");

            bool foundInChoices = false;
            for (int i = 0; i < 50; i++)
            {
                var choices = BlessingSystem.GenerateChoices(100f, _save);
                if (choices.Exists(c => c.BlessingId == pilgrim.BlessingId))
                {
                    foundInChoices = true;
                    break;
                }
            }
            Assert.IsFalse(foundInChoices,
                $"Pilgrimage blessing '{pilgrim.BlessingId}' should not appear without visiting '{pilgrim.RequiredLocationVisited}'");
        }

        [UnityTest]
        public IEnumerator FirstHitImmunity_SetByHolyChurchShield()
        {
            yield return null;
            var shield = BlessingSystem.FindBlessing("holy_church_shield");
            Assert.IsNotNull(shield);

            BlessingSystem.ApplyBlessing(_player, shield, _save);
            var mods = BlessingSystem.GetModifiers(_player);

            Assert.IsTrue(mods.FirstHitImmunity, "Holy Church Shield should grant first-hit immunity");
        }

        [UnityTest]
        public IEnumerator SinTemptation_HasDrawback()
        {
            yield return null;
            var wrathEdge = BlessingSystem.FindBlessing("wraths_edge");
            Assert.IsNotNull(wrathEdge);

            float defBefore = _player.Stats.Defense;
            BlessingSystem.ApplyBlessing(_player, wrathEdge, _save);

            Assert.Less(_player.Stats.Defense, defBefore,
                "Wrath's Edge should reduce defense as drawback");
            var mods = BlessingSystem.GetModifiers(_player);
            Assert.Greater(mods.DamageMultiplier, 1f,
                "Wrath's Edge should boost damage multiplier");
        }
    }
}
