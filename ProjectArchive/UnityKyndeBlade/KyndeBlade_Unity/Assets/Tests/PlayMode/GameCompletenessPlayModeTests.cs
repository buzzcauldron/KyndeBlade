using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    /// <summary>Smoke tests for input facade, music events, and main scene bootstrap (game-completeness roadmap).</summary>
    public class GameCompletenessPlayModeTests
    {
        [TearDown]
        public void ResetMenuSkipFlag()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = false;
        }

        sealed class ScriptedCombatWindowInput : ICombatWindowInput
        {
            bool _dodge, _parry, _counter;

            public void QueueDodge() => _dodge = true;
            public void QueueParry() => _parry = true;
            public void QueueCounter() => _counter = true;

            public bool WasDodgePressedThisFrame()
            {
                if (!_dodge) return false;
                _dodge = false;
                return true;
            }

            public bool WasParryPressedThisFrame()
            {
                if (!_parry) return false;
                _parry = false;
                return true;
            }

            public bool WasCounterPressedThisFrame()
            {
                if (!_counter) return false;
                _counter = false;
                return true;
            }
        }

        [Order(1)]
        [UnityTest]
        public IEnumerator ParryDodgeInputHandler_Override_TriggersDodgeEvent()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Rogue, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Knight, false);
            p.Stats.Speed = 50f;
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();

            tm.StartRealTimeWindow(2f, e, p, CombatActionType.Escapade);
            p.StartDodge(2f);

            var input = new ScriptedCombatWindowInput();
            input.QueueDodge();

            var go = new GameObject("InputHandler");
            var handler = go.AddComponent<ParryDodgeInputHandler>();
            handler.TurnManager = tm;
            handler.InputSourceOverride = input;

            float remaining = -1f;
            handler.OnDodgePressed += r => remaining = r;
            yield return null;
            Assert.Greater(remaining, 0f);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [Order(2)]
        [UnityTest]
        public IEnumerator MusicManager_PlayTheme_FiresEvent_WhenSuppressed()
        {
            var prev = MusicManager.SuppressAudioOutputForTests;
            MusicManager.SuppressAudioOutputForTests = true;
            var go = new GameObject("Music");
            var mm = go.AddComponent<MusicManager>();
            string last = null;
            mm.OnMusicThemeChangeRequested += id => last = id;
            mm.PlayTheme("orfeo");
            Assert.AreEqual("orfeo", last);
            Assert.AreEqual("orfeo", mm.ActiveThemeId);
            MusicManager.SuppressAudioOutputForTests = prev;
            Object.DestroyImmediate(go);
            yield return null;
        }

        [Order(200)]
        [UnityTest]
        public IEnumerator MainScene_Loads_WithKyndeBladeGameManager()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = true;
            AsyncOperation load = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            Assert.IsNotNull(load);
            yield return load;
            yield return null;
            yield return null;
            var gm = Object.FindFirstObjectByType<KyndeBladeGameManager>();
            Assert.IsNotNull(gm, "Main.unity should contain KyndeBladeGameManager (see Editor Build Settings).");
            var es = Object.FindFirstObjectByType<EventSystem>();
            Assert.IsNotNull(es);
            Assert.IsNotNull(es.GetComponent<InputSystemUIInputModule>(), "EventSystem should use InputSystemUIInputModule (see ARCHITECTURE.md).");
            var gfc = Object.FindFirstObjectByType<GameFlowController>();
            Assert.IsNotNull(gfc);
        }

        [Order(201)]
        [UnityTest]
        public IEnumerator MainScene_SkipMenu_WorldMapGetsCurrentLocation()
        {
            GameFlowController.SkipMainMenuForAutomatedTests = true;
            AsyncOperation load = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            yield return load;
            yield return null;
            yield return null;
            var wm = Object.FindFirstObjectByType<WorldMapManager>();
            Assert.IsNotNull(wm);
            Assert.IsNotNull(wm.CurrentLocation, "WorldMapManager should finish lazy init when main menu is skipped for tests.");
        }
    }
}
