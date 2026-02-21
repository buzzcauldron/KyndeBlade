using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class AgingPovertyTest
    {
        SaveManager _saveManager;
        AgingManager _agingManager;
        PovertyManager _povertyManager;
        List<MedievalCharacter> _party;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            PlayerPrefs.Save();
            _saveManager = TestGameBootstrap.CreateSaveManager();
            _saveManager.NewGame("malvern");
        }

        [TearDown]
        public void TearDown()
        {
            if (_party != null)
            {
                foreach (var c in _party)
                    if (c != null) Object.DestroyImmediate(c.gameObject);
            }
            if (_agingManager != null) Object.DestroyImmediate(_agingManager.gameObject);
            if (_povertyManager != null) Object.DestroyImmediate(_povertyManager.gameObject);
            if (_saveManager != null) Object.DestroyImmediate(_saveManager.gameObject);
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator AgingManager_AppliesStatModifiers_WhenVisionGreaterThanZero()
        {
            _saveManager.CurrentProgress.VisionIndex = 2;

            _agingManager = TestGameBootstrap.CreateAgingManager(_saveManager);

            _party = new List<MedievalCharacter>
            {
                TestGameBootstrap.CreateTestCharacter("Knight", CharacterClass.Knight, true),
                TestGameBootstrap.CreateTestCharacter("Mage", CharacterClass.Mage, true)
            };
            yield return null; // Let Start() init stats

            float knightSpeedBefore = _party[0].Stats.Speed;
            float knightDefenseBefore = _party[0].Stats.Defense;

            _agingManager.ApplyAgingToParty(_party);

            Assert.Less(_party[0].Stats.Speed, knightSpeedBefore, "Aging should reduce speed");
            Assert.Greater(_party[0].Stats.Defense, knightDefenseBefore, "Aging should increase defense");

            yield return null;
        }

        [UnityTest]
        public IEnumerator AgingManager_GetSpeedMultiplier_ReturnsCorrectValue()
        {
            _agingManager = TestGameBootstrap.CreateAgingManager(_saveManager);

            Assert.AreEqual(1f, _agingManager.GetSpeedMultiplier(), "Vision 0 should have 1x speed");

            _saveManager.CurrentProgress.VisionIndex = 1;
            float mult = _agingManager.GetSpeedMultiplier();
            Assert.Less(mult, 1f, "Vision 1 should reduce speed");

            yield return null;
        }

        [UnityTest]
        public IEnumerator PovertyManager_GetStaminaRegenMultiplier_ReducesWithLevel()
        {
            _povertyManager = TestGameBootstrap.CreatePovertyManager(_saveManager);

            Assert.AreEqual(1f, _povertyManager.GetStaminaRegenMultiplier(), "Level 0 should have 1x");

            _povertyManager.SetPovertyLevel(2);
            float mult = _povertyManager.GetStaminaRegenMultiplier();
            Assert.Less(mult, 1f, "Poverty level 2 should reduce stamina regen");

            yield return null;
        }

        [UnityTest]
        public IEnumerator PovertyManager_GetKyndeGenMultiplier_ReducesWithLevel()
        {
            _povertyManager = TestGameBootstrap.CreatePovertyManager(_saveManager);

            Assert.AreEqual(1f, _povertyManager.GetKyndeGenMultiplier(), "Level 0 should have 1x");

            _povertyManager.SetPovertyLevel(3);
            float mult = _povertyManager.GetKyndeGenMultiplier();
            Assert.Less(mult, 1f, "Poverty level 3 should reduce Kynde generation");

            yield return null;
        }

        [UnityTest]
        public IEnumerator PovertyManager_SetPovertyLevel_ClampsToValidRange()
        {
            _povertyManager = TestGameBootstrap.CreatePovertyManager(_saveManager);

            _povertyManager.SetPovertyLevel(10);
            Assert.LessOrEqual(_povertyManager.CurrentPovertyLevel, 5);

            _povertyManager.SetPovertyLevel(-5);
            Assert.GreaterOrEqual(_povertyManager.CurrentPovertyLevel, 0);

            yield return null;
        }

        [UnityTest]
        public IEnumerator HungerCharacter_HasHungerMoveset()
        {
            var go = new GameObject("Hunger");
            if (go.GetComponent<Collider>() == null)
                go.AddComponent<BoxCollider>();
            var h = go.AddComponent<HungerCharacter>();

            yield return null; // Let Start() run and HungerMoveset.ApplyToCharacter

            Assert.IsNotNull(h.AvailableActions);
            Assert.Greater(h.AvailableActions.Count, 0);
            Assert.IsTrue(h.AvailableActions.Any(a => a != null && a.ActionData.ActionName == "Hunger's Grip"));

            Object.DestroyImmediate(go);
            yield return null;
        }
    }
}
