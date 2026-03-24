using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class Phase4TurnSystemTests
    {
        static CombatAction CreateCounterForPlayer(MedievalCharacter p)
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Counter,
                ActionName = "Counter",
                Damage = 20f,
                StaminaCost = 5f
            };
            p.AvailableActions.Add(a);
            return a;
        }

        [UnityTest]
        public IEnumerator TurnOrder_SortsBySpeedDescending()
        {
            yield return null;
            var a = TestGameBootstrap.CreateTestCharacter("Slow", CharacterClass.Knight, true);
            var b = TestGameBootstrap.CreateTestCharacter("Fast", CharacterClass.Rogue, true);
            a.Stats.Speed = 5f;
            b.Stats.Speed = 20f;
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { a, b }, new List<MedievalCharacter>());
            Assert.AreEqual(b, tm.TurnOrder[0]);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(a.gameObject);
            Object.DestroyImmediate(b.gameObject);
        }

        [UnityTest]
        public IEnumerator IsPlayerTurn_ReflectsCurrentCharacter()
        {
            yield return null;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            p.Stats.Speed = 100f;
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            Assert.IsTrue(tm.IsPlayerTurn());

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator NextTurn_SkipsDeadCharacters()
        {
            yield return null;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var p2 = TestGameBootstrap.CreateTestCharacter("P2", CharacterClass.Mage, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Archer, false);
            p.Stats.Speed = 30f;
            p2.Stats.Speed = 25f;
            e.Stats.Speed = 10f;
            p2.Stats.CurrentHealth = 0f;
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { p, p2 }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            Assert.AreEqual(p, tm.CurrentCharacter);
            tm.EndTurn();
            tm.NextTurn();
            Assert.AreEqual(e, tm.CurrentCharacter);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(p2.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator EndTurn_InvokesCharacterTurnEnd()
        {
            yield return null;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            bool ended = false;
            p.OnTurnEnd += () => ended = true;
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter>());
            tm.StartCombat();
            tm.EndTurn();
            Assert.IsTrue(ended);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
        }

        [UnityTest]
        public IEnumerator OnActionExecuted_FiresFromStrike()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            MedievalCharacter exec = null, tgt = null;
            CombatAction act = null;
            tm.OnActionExecuted += (a, b, c) => { exec = a; tgt = b; act = c; };
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            p.Stats.Speed = 50f;
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            tm.ExecuteAction(DefaultCombatActions.CreateStrike(), e);
            yield return new WaitForSeconds(0.05f);
            Assert.AreEqual(p, exec);
            Assert.AreEqual(e, tgt);
            Assert.IsNotNull(act);
            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator AddCharacterMidCombat_ReSortsOrder()
        {
            yield return null;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            p.Stats.Speed = 10f;
            e.Stats.Speed = 5f;
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            var late = TestGameBootstrap.CreateTestCharacter("Late", CharacterClass.Rogue, true);
            late.Stats.Speed = 100f;
            tm.AddCharacterToCombat(late, true);
            Assert.AreEqual(late, tm.TurnOrder[0]);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
            Object.DestroyImmediate(late.gameObject);
        }

        [UnityTest]
        public IEnumerator ParryResolve_StartsCounterWindow_AndExecuteCounterHits()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            e.Stats.Speed = 1f;
            p.Stats.Speed = 50f;
            DefaultCombatActions.AddDefaultsTo(e);
            CreateCounterForPlayer(p);
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            var ward = DefaultCombatActions.CreateWard();
            tm.ExecuteAction(ward, e);
            yield return new WaitForSeconds(0.02f);
            Assert.AreEqual(CombatState.RealTimeWindow, tm.State);
            p.StartParry(tm.RealTimeWindowRemaining);
            tm.SetRealTimeWindowRemainingForTests(0f);
            tm.ResolveDefenseWindow();
            Assert.IsTrue(tm.IsCounterWindowActive);
            float hpBefore = e.GetCurrentHealth();
            tm.ExecuteCounter();
            Assert.Less(e.GetCurrentHealth(), hpBefore);

            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator DefenseWindow_DodgeAvoidsDamage()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Rogue, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Knight, false);
            p.Stats.Speed = 50f;
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            var esc = DefaultCombatActions.CreateEscapade();
            tm.ExecuteAction(esc, e);
            yield return new WaitForSeconds(0.02f);
            float hp = p.GetCurrentHealth();
            p.StartDodge(tm.RealTimeWindowRemaining);
            tm.SetRealTimeWindowRemainingForTests(0f);
            tm.ResolveDefenseWindow();
            Assert.AreEqual(hp, p.GetCurrentHealth(), 0.01f);

            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator CounterWindow_Countdown_CallsNextTurn()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            p.Stats.Speed = 50f;
            GameRuntime.TurnManager = tm;
            DefaultCombatActions.AddDefaultsTo(e);
            CreateCounterForPlayer(p);
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            tm.ExecuteAction(DefaultCombatActions.CreateWard(), e);
            yield return new WaitForSeconds(0.02f);
            p.StartParry(tm.RealTimeWindowRemaining);
            tm.SetRealTimeWindowRemainingForTests(0f);
            tm.ResolveDefenseWindow();
            int turn = tm.TurnNumber;
            yield return new WaitForSeconds(0.6f);
            Assert.Greater(tm.TurnNumber, turn);

            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }
    }
}
