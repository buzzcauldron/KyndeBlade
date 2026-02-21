using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class CombatMechanicsTest
    {
        TurnManager _turnManager;
        List<MedievalCharacter> _players;
        List<MedievalCharacter> _enemies;

        [SetUp]
        public void SetUp()
        {
            _turnManager = TestGameBootstrap.CreateTurnManager();
            TestGameBootstrap.CreateCombatSetup(out _players, out _enemies);
        }

        [TearDown]
        public void TearDown()
        {
            TestGameBootstrap.Cleanup(_turnManager);
            foreach (var c in _players ?? new List<MedievalCharacter>())
                if (c != null) Object.DestroyImmediate(c.gameObject);
            foreach (var c in _enemies ?? new List<MedievalCharacter>())
                if (c != null) Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator TurnManager_InitializesCombat_WithCorrectTurnOrder()
        {
            _turnManager.InitializeCombat(_players, _enemies);
            _turnManager.StartCombat();

            Assert.AreEqual(CombatState.WaitingForInput, _turnManager.State);
            Assert.IsNotNull(_turnManager.CurrentCharacter);
            Assert.Greater(_turnManager.TurnOrder.Count, 0);
            Assert.AreEqual(1, _turnManager.TurnNumber);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Strike_DealsDamage_ToTarget()
        {
            yield return null; // Let character Start() run
            _players[0].Stats.Speed = 20f;
            _players[1].Stats.Speed = 19f;
            _turnManager.InitializeCombat(_players, _enemies);
            _turnManager.StartCombat();

            var target = _turnManager.EnemyCharacters[0];
            float healthBefore = target.GetCurrentHealth();

            var strike = DefaultCombatActions.CreateStrike();
            _turnManager.ExecuteAction(strike, target);

            yield return new WaitForSeconds(0.1f);

            float healthAfter = target.GetCurrentHealth();
            Assert.Less(healthAfter, healthBefore, "Strike should reduce target health");
        }

        [UnityTest]
        public IEnumerator DefeatAllEnemies_EndsCombat_WithVictory()
        {
            // Single weak enemy, multiple strong players
            var weakEnemy = TestGameBootstrap.CreateTestCharacter("WeakEnemy", CharacterClass.Mage, false);
            var strongPlayer = TestGameBootstrap.CreateTestCharacter("StrongPlayer", CharacterClass.Knight, true);
            yield return null; // Let Start() run and init stats
            weakEnemy.Stats.MaxHealth = 10f;
            weakEnemy.Stats.CurrentHealth = 10f;
            weakEnemy.Stats.Defense = 0f;
            strongPlayer.Stats.MaxHealth = 200f;
            strongPlayer.Stats.CurrentHealth = 200f;
            strongPlayer.Stats.AttackPower = 50f;
            strongPlayer.Stats.Speed = 20f; // Ensure player goes first

            var players = new List<MedievalCharacter> { strongPlayer };
            var enemies = new List<MedievalCharacter> { weakEnemy };

            _turnManager.InitializeCombat(players, enemies);
            _turnManager.StartCombat();

            bool combatEnded = false;
            _turnManager.OnCombatEnded += () => combatEnded = true;

            var strike = DefaultCombatActions.CreateStrike();
            _turnManager.ExecuteAction(strike, weakEnemy);

            yield return new WaitForSeconds(0.5f);

            // May need multiple frames for action + NextTurn + CheckCombatEnd
            for (int i = 0; i < 30 && !combatEnded; i++)
                yield return null;

            Assert.IsTrue(combatEnded, "Combat should end when all enemies defeated");
            Assert.AreEqual(CombatState.CombatEnded, _turnManager.State);
            Assert.IsTrue(AreAllDefeated(_turnManager.EnemyCharacters), "All enemies should be defeated");

            Object.DestroyImmediate(weakEnemy.gameObject);
            Object.DestroyImmediate(strongPlayer.gameObject);
        }

        [UnityTest]
        public IEnumerator DefeatAllPlayers_EndsCombat_WithDefeat()
        {
            var weakPlayer = TestGameBootstrap.CreateTestCharacter("WeakPlayer", CharacterClass.Mage, true);
            var strongEnemy = TestGameBootstrap.CreateTestCharacter("StrongEnemy", CharacterClass.Knight, false);
            yield return null; // Let Start() run
            weakPlayer.Stats.MaxHealth = 5f;
            weakPlayer.Stats.CurrentHealth = 5f;
            weakPlayer.Stats.Defense = 0f;
            strongEnemy.Stats.MaxHealth = 200f;
            strongEnemy.Stats.CurrentHealth = 200f;
            strongEnemy.Stats.AttackPower = 50f;
            strongEnemy.Stats.Speed = 20f; // Ensure enemy goes first

            var players = new List<MedievalCharacter> { weakPlayer };
            var enemies = new List<MedievalCharacter> { strongEnemy };

            _turnManager.InitializeCombat(players, enemies);
            _turnManager.StartCombat();

            bool combatEnded = false;
            _turnManager.OnCombatEnded += () => combatEnded = true;

            var strike = DefaultCombatActions.CreateStrike();
            _turnManager.ExecuteAction(strike, weakPlayer);

            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 30 && !combatEnded; i++)
                yield return null;

            Assert.IsTrue(combatEnded);
            Assert.AreEqual(CombatState.CombatEnded, _turnManager.State);
            Assert.IsTrue(AreAllDefeated(_turnManager.PlayerCharacters));

            Object.DestroyImmediate(weakPlayer.gameObject);
            Object.DestroyImmediate(strongEnemy.gameObject);
        }

        [UnityTest]
        public IEnumerator NextTurn_AdvancesToNextAliveCharacter()
        {
            _turnManager.InitializeCombat(_players, _enemies);
            _turnManager.StartCombat();

            var first = _turnManager.CurrentCharacter;
            _turnManager.NextTurn();
            yield return null;

            var second = _turnManager.CurrentCharacter;
            Assert.AreNotEqual(first, second, "NextTurn should advance to different character");
        }

        static bool AreAllDefeated(List<MedievalCharacter> list)
        {
            foreach (var c in list)
                if (c != null && c.IsAlive()) return false;
            return true;
        }
    }
}
