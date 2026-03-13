using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;

namespace KyndeBlade.Tests
{
    public class CombatSnapshotSaveLoadTest
    {
        SaveManager _save;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            _save = TestGameBootstrap.CreateSaveManager();
        }

        [TearDown]
        public void TearDown()
        {
            if (_save != null) Object.DestroyImmediate(_save.gameObject);
        }

        [UnityTest]
        public IEnumerator CombatSnapshot_PersistsAcrossSaveLoad()
        {
            yield return null;

            _save.SaveCombatSnapshot("sandbox_wave_2", 1, 3, 0.15f);
            _save.Save();
            _save.Load();

            Assert.IsTrue(_save.CurrentProgress.CombatSnapshotActive);
            Assert.AreEqual("sandbox_wave_2", _save.CurrentProgress.CombatEncounterId);
            Assert.AreEqual(1, _save.CurrentProgress.CombatWaveIndex);
            Assert.AreEqual(3, _save.CurrentProgress.CombatAliveEnemies);
            Assert.AreEqual(0.15f, _save.CurrentProgress.CombatRecoveryRemaining, 0.001f);
        }

        [UnityTest]
        public IEnumerator CombatSnapshot_Clear_ResetsFields()
        {
            yield return null;

            _save.SaveCombatSnapshot("sandbox_wave_1", 0, 2, 0.05f);
            _save.ClearCombatSnapshot();
            _save.Save();
            _save.Load();

            Assert.IsFalse(_save.CurrentProgress.CombatSnapshotActive);
            Assert.AreEqual(string.Empty, _save.CurrentProgress.CombatEncounterId);
            Assert.AreEqual(0, _save.CurrentProgress.CombatWaveIndex);
            Assert.AreEqual(0, _save.CurrentProgress.CombatAliveEnemies);
            Assert.AreEqual(0f, _save.CurrentProgress.CombatRecoveryRemaining, 0.001f);
        }
    }
}
