using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KyndeBlade.Tests
{
    public class MapSaveTest
    {
        SaveManager _saveManager;
        WorldMapManager _worldMapManager;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            PlayerPrefs.Save();
            _saveManager = TestGameBootstrap.CreateSaveManager();
        }

        [TearDown]
        public void TearDown()
        {
            if (_worldMapManager != null)
                Object.DestroyImmediate(_worldMapManager.gameObject);
            if (_saveManager != null)
                Object.DestroyImmediate(_saveManager.gameObject);
            PlayerPrefs.DeleteKey("KyndeBlade_Save");
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator SaveManager_NewGame_CreatesProgressWithStartLocation()
        {
            _saveManager.NewGame("malvern");

            Assert.IsNotNull(_saveManager.CurrentProgress);
            Assert.AreEqual("malvern", _saveManager.CurrentProgress.CurrentLocationId);
            Assert.IsTrue(_saveManager.CurrentProgress.VisitedLocationIds.Contains("malvern"));
            Assert.IsTrue(_saveManager.CurrentProgress.UnlockedLocationIds.Contains("malvern"));

            yield return null;
        }

        [UnityTest]
        public IEnumerator SaveCheckpoint_UpdatesLocationAndUnlocks()
        {
            _saveManager.NewGame("malvern");
            _saveManager.SaveCheckpoint("fayre_felde");

            Assert.AreEqual("fayre_felde", _saveManager.CurrentProgress.CurrentLocationId);
            Assert.IsTrue(_saveManager.HasVisited("fayre_felde"));
            Assert.IsTrue(_saveManager.IsUnlocked("fayre_felde"));

            yield return null;
        }

        [UnityTest]
        public IEnumerator UnlockLocation_AddsToUnlockedList()
        {
            _saveManager.NewGame("malvern");
            _saveManager.UnlockLocation("dongeoun");

            Assert.IsTrue(_saveManager.IsUnlocked("dongeoun"));

            yield return null;
        }

        [UnityTest]
        public IEnumerator GameProgress_ToJsonFromJson_Roundtrips()
        {
            var original = GameProgress.CreateNew("malvern");
            original.VisitedLocationIds.Add("fayre_felde");
            original.UnlockedLocationIds.Add("dongeoun");
            original.VisionIndex = 1;

            var json = original.ToJson();
            var restored = GameProgress.FromJson(json);

            Assert.AreEqual(original.CurrentLocationId, restored.CurrentLocationId);
            Assert.AreEqual(original.VisionIndex, restored.VisionIndex);
            Assert.AreEqual(original.VisitedLocationIds.Count, restored.VisitedLocationIds.Count);
            Assert.AreEqual(original.UnlockedLocationIds.Count, restored.UnlockedLocationIds.Count);

            yield return null;
        }

        [UnityTest]
        public IEnumerator WorldMapManager_SetCurrentLocation_UpdatesCurrentAndFiresEvent()
        {
            var loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "test_loc";
            loc.DisplayName = "Test Location";
            loc.NextLocationIds = new List<string>();

            var wmGo = new GameObject("WorldMapManager");
            _worldMapManager = wmGo.AddComponent<WorldMapManager>();
            _worldMapManager.AllLocations.Add(loc);
            _worldMapManager.RebuildLocationIndex();

            LocationNode received = null;
            _worldMapManager.OnLocationChanged += l => received = l;

            _worldMapManager.SetCurrentLocation(loc);

            Assert.AreEqual(loc, _worldMapManager.CurrentLocation);
            Assert.AreEqual(loc, received);

            Object.DestroyImmediate(loc);
            yield return null;
        }

        [UnityTest]
        public IEnumerator WorldMapManager_GetNextLocations_ReturnsReachableLocations()
        {
            var loc1 = ScriptableObject.CreateInstance<LocationNode>();
            loc1.LocationId = "a";
            loc1.NextLocationIds = new List<string> { "b", "c" };

            var loc2 = ScriptableObject.CreateInstance<LocationNode>();
            loc2.LocationId = "b";

            var loc3 = ScriptableObject.CreateInstance<LocationNode>();
            loc3.LocationId = "c";

            var wmGo = new GameObject("WorldMapManager");
            _worldMapManager = wmGo.AddComponent<WorldMapManager>();
            _worldMapManager.AllLocations.Add(loc1);
            _worldMapManager.AllLocations.Add(loc2);
            _worldMapManager.AllLocations.Add(loc3);
            _worldMapManager.RebuildLocationIndex();

            _worldMapManager.SetCurrentLocation(loc1);
            var next = _worldMapManager.GetNextLocations();

            Assert.AreEqual(2, next.Count);

            Object.DestroyImmediate(loc1);
            Object.DestroyImmediate(loc2);
            Object.DestroyImmediate(loc3);
            yield return null;
        }
    }
}
