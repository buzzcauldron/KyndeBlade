using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class Phase3CombatActionsTests
    {
        static CombatAction CreateHeal(float amount = 25f)
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Heal,
                ActionName = "Heal",
                Damage = amount,
                StaminaCost = 10f
            };
            return a;
        }

        static CombatAction CreateCounter(float dmg = 12f)
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Counter,
                ActionName = "Counter",
                Damage = dmg,
                StaminaCost = 5f,
                BreakDamage = 2f
            };
            return a;
        }

        static CombatAction CreateRangedStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.RangedStrike,
                ActionName = "Ranged",
                Damage = 14f,
                StaminaCost = 12f
            };
            return a;
        }

        [UnityTest]
        public IEnumerator Rest_RestoresStamina_AndRemovesHungerStack()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("R", CharacterClass.Knight, true);
            c.Stats.CurrentStamina = 10f;
            c.ApplyStatusEffect(StatusEffect.CreateHungerEffect(30f, 1));
            var rest = DefaultCombatActions.CreateRest();
            c.ExecuteCombatAction(rest, null);
            Assert.Greater(c.GetCurrentStamina(), 10f);
            Assert.IsFalse(c.IsHungry());

            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator Heal_TargetRestoresHealth()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("H", CharacterClass.Mage, true);
            c.Stats.CurrentHealth = 20f;
            c.ExecuteCombatAction(CreateHeal(15f), c);
            Assert.AreEqual(35f, c.GetCurrentHealth(), 0.01f);
            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator Strike_GeneratesKynde_AndCostsStamina()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            float kyBefore = p.GetCurrentKynde();
            float stBefore = p.GetCurrentStamina();
            var strike = DefaultCombatActions.CreateStrike();
            p.ExecuteCombatAction(strike, e);
            Assert.Greater(p.GetCurrentKynde(), kyBefore);
            Assert.Less(p.GetCurrentStamina(), stBefore);
            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator RangedStrike_DealsDamageLikeStrike()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Archer, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Knight, false);
            e.Stats.Defense = 0f;
            float hp0 = e.GetCurrentHealth();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();
            p.ExecuteCombatAction(CreateRangedStrike(), e);
            Assert.Less(e.GetCurrentHealth(), hp0);
            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator Counter_MultipliesDamage1_5_WhenBroken()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            e.Stats.Defense = 0f;
            e.BreakCharacter();
            var counter = CreateCounter(10f);
            p.ExecuteCombatAction(counter, e);
            Assert.Less(e.GetCurrentHealth(), e.Stats.MaxHealth);

            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator Action_StaminaGate_BlocksExecution()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("G", CharacterClass.Mage, true);
            c.Stats.CurrentStamina = 0f;
            float hp = c.GetCurrentHealth();
            c.ExecuteCombatAction(DefaultCombatActions.CreateStrike(), c);
            Assert.AreEqual(hp, c.GetCurrentHealth());
            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator Action_KyndeGate_BlocksExecution()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("Gk", CharacterClass.Mage, true);
            c.Stats.CurrentStamina = 200f;
            c.Stats.CurrentKynde = 0f;
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Rest,
                ActionName = "SpendKynde",
                StaminaCost = 0f,
                KyndeCost = 5f
            };
            float st = c.GetCurrentStamina();
            c.ExecuteCombatAction(a, null);
            Assert.AreEqual(st, c.GetCurrentStamina());
            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator DeferredHit_WaitsForAnimationEvent()
        {
            yield return null;
            var go = new GameObject("Anim");
            go.AddComponent<BoxCollider>();
            var vis = new GameObject("v");
            vis.transform.SetParent(go.transform);
            vis.AddComponent<Animator>();
            var med = go.AddComponent<MedievalCharacter>();
            med.CharacterClassType = CharacterClass.Knight;
            med.CharacterName = "Anim";
            DefaultCombatActions.AddDefaultsTo(med);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            e.Stats.Defense = 0f;
            float hpBefore = e.GetCurrentHealth();
            var tm = TestGameBootstrap.CreateTurnManager();
            GameRuntime.TurnManager = tm;
            var strike = DefaultCombatActions.CreateStrike();
            med.ExecuteCombatAction(strike, e);
            Assert.AreEqual(hpBefore, e.GetCurrentHealth(), 0.01f);
            med.OnAnimationHit();
            Assert.Less(e.GetCurrentHealth(), hpBefore);
            GameRuntime.TurnManager = null;
            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(e.gameObject);
        }

        [UnityTest]
        public IEnumerator DefaultCombatActions_AddDefaultsTo_IncludesCoreActions()
        {
            yield return null;
            var go = new GameObject("empty");
            go.AddComponent<BoxCollider>();
            var c = go.AddComponent<MedievalCharacter>();
            DefaultCombatActions.AddDefaultsTo(c);
            Assert.GreaterOrEqual(c.AvailableActions.Count, 4);
            Object.DestroyImmediate(go);
        }

        [UnityTest]
        public IEnumerator CastTime_AndExecutionTime_DelayPhases()
        {
            yield return null;
            var tm = TestGameBootstrap.CreateTurnManager();
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var e = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, false);
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter> { e });
            tm.StartCombat();

            var slow = ScriptableObject.CreateInstance<CombatAction>();
            slow.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "SlowStrike",
                Damage = 5f,
                StaminaCost = 5f,
                CastTime = 0.08f,
                ExecutionTime = 0.08f
            };

            tm.ExecuteAction(slow, e);
            Assert.AreEqual(CombatState.ExecutingAction, tm.State);
            yield return new WaitForSeconds(0.2f);
            Assert.AreNotEqual(CombatState.ExecutingAction, tm.State);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
            Object.DestroyImmediate(e.gameObject);
        }
    }
}
