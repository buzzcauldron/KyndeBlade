using System.Collections;
using KyndeBlade;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace KyndeBlade.Tests
{
    /// <summary>
    /// PlayMode coverage for TDAD workflow <c>demo-vertical-slice</c> (Tier A ship path).
    /// Pairs with <see cref="PlayableSliceEditModeTests"/> and <see cref="MapSaveTest"/>.
    /// </summary>
    public class DemoVerticalSlicePlayModeTests
    {
        [TearDown]
        public void ResetMenuSkipFlag()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = false;
        }

        [UnityTest]
        public IEnumerator MainScene_SkipMenu_CurrentLocationId_IsTour()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = true;
            AsyncOperation load = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            Assert.IsNotNull(load);
            yield return load;
            yield return null;
            yield return null;

            var wm = Object.FindFirstObjectByType<WorldMapManager>();
            Assert.IsNotNull(wm);
            Assert.IsNotNull(wm.CurrentLocation, "WorldMapManager should finish lazy init when main menu is skipped for tests.");
            Assert.AreEqual(
                GameWorldConstants.DefaultStartLocationId,
                wm.CurrentLocation.LocationId,
                "Playable slice expects cold start at Tower on the Toft (tour) per PLAYABLE_SLICE.md.");
        }

        [UnityTest]
        public IEnumerator MainScene_SkipMenu_PauseRoot_ExistsAndStartsHidden()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = true;
            AsyncOperation load = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            yield return load;
            yield return null;
            yield return null;

            var gfc = Object.FindFirstObjectByType<GameFlowController>();
            Assert.IsNotNull(gfc);
            var pauseTx = gfc.transform.Find("PauseRoot");
            Assert.IsNotNull(pauseTx, "GameFlowController should create PauseRoot under MenuCanvas.");
            Assert.IsFalse(pauseTx.gameObject.activeSelf, "Pause overlay should start hidden until Escape.");
        }
    }
}
