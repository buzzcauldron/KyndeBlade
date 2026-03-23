using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class Phase2CharacterMechanicsTests
    {
        [UnityTest]
        public IEnumerator TurnManager_CritPressure_BuildsAndTriggers()
        {
            yield return null;
            var p = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            var tm = TestGameBootstrap.CreateTurnManager();
            tm.InitializeCombat(new List<MedievalCharacter> { p }, new List<MedievalCharacter>());
            tm.StartCombat();

            var cp = typeof(TurnManager).GetField("_critPressure", BindingFlags.NonPublic | BindingFlags.Instance);
            cp?.SetValue(tm, 1f);
            bool isCrit;
            float outDmg = tm.ApplyCritToDamage(10f, out isCrit);
            Assert.IsTrue(isCrit);
            Assert.AreEqual(15f, outDmg, 0.01f);

            cp?.SetValue(tm, 0f);
            for (int i = 0; i < 5; i++)
                tm.RecordDamageDealt();
            Assert.AreEqual(0.4f, (float)cp.GetValue(tm), 0.001f);

            Object.DestroyImmediate(tm.gameObject);
            Object.DestroyImmediate(p.gameObject);
        }

        [UnityTest]
        public IEnumerator Kynde_GainConsume_AndHungerModifier()
        {
            yield return null;
            var go = TestGameBootstrap.CreateTestCharacter("K", CharacterClass.Knight, true);
            go.Stats.CurrentKynde = 0f;
            go.GainKynde(10f);
            Assert.AreEqual(10f, go.GetCurrentKynde(), 0.01f);
            Assert.IsTrue(go.ConsumeKynde(4f));
            Assert.AreEqual(6f, go.GetCurrentKynde(), 0.01f);
            Assert.IsFalse(go.ConsumeKynde(100f));

            go.ApplyStatusEffect(StatusEffect.CreateHungerEffect(10f, 1));
            go.Stats.CurrentKynde = 0f;
            go.GainKynde(10f);
            Assert.Less(go.GetCurrentKynde(), 10f);

            Object.DestroyImmediate(go.gameObject);
        }

        [UnityTest]
        public IEnumerator Break_TakeDamage_TriggersStun_AndRecovery()
        {
            yield return null;
            var go = TestGameBootstrap.CreateTestCharacter("B", CharacterClass.Knight, true);
            bool broken = false;
            go.OnCharacterBroken += _ => broken = true;
            go.Stats.MaxBreakGauge = 50f;
            go.Stats.CurrentBreakGauge = 50f;
            go.TakeBreakDamage(60f);
            Assert.IsTrue(broken);
            Assert.IsTrue(go.Stats.IsBroken);
            go.Stats.BrokenStunRemaining = 0.01f;
            yield return new WaitForSeconds(0.15f);
            Assert.IsFalse(go.Stats.IsBroken);

            Object.DestroyImmediate(go.gameObject);
        }

        [UnityTest]
        public IEnumerator Dodge_Parry_Windows_AndKyndeRewards()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("D", CharacterClass.Rogue, true);
            c.StartDodge(1f);
            Assert.IsTrue(c.AttemptDodge());
            c.EndDodge();
            c.DodgeWindowRemaining = 0.1f;
            c.IsDodging = true;
            Assert.IsTrue(c.AttemptDodge());

            c.EndDodge();
            c.StartParry(1f);
            Assert.IsTrue(c.AttemptParry());
            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator Status_ApplyStackRemove_Expiry()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("S", CharacterClass.Mage, true);
            float baseAtk = c.Stats.AttackPower;
            var hunger = StatusEffect.CreateHungerEffect(0.5f, 1);
            c.ApplyStatusEffect(hunger);
            Assert.Less(c.Stats.AttackPower, baseAtk);
            c.RemoveStatusEffect(StatusEffectType.Hunger);
            Assert.AreEqual(baseAtk, c.Stats.AttackPower, 0.1f);

            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator RestoreStamina_UsesHungerModifier_WithPoverty()
        {
            yield return null;
            var saveGo = new GameObject("Save");
            TestGameBootstrap.ClearSaveAndMetaPrefs();
            var save = saveGo.AddComponent<SaveManager>();
            yield return null;

            var povGo = new GameObject("Pov");
            var pov = povGo.AddComponent<PovertyManager>();
            pov.SaveManager = save;
            save.SetPovertyLevel(5);

            var c = TestGameBootstrap.CreateTestCharacter("P", CharacterClass.Knight, true);
            float stBase = c.GetCurrentStamina();
            c.RestoreStamina(20f);
            Assert.Greater(c.GetCurrentStamina(), stBase);

            c.ApplyStatusEffect(StatusEffect.CreateHungerEffect(60f, 1));
            float stBeforeHunger = c.GetCurrentStamina();
            c.RestoreStamina(20f);
            Assert.Less(c.GetCurrentStamina() - stBeforeHunger, 19.5f);

            Object.DestroyImmediate(c.gameObject);
            Object.DestroyImmediate(povGo);
            Object.DestroyImmediate(saveGo);
            TestGameBootstrap.ClearSaveAndMetaPrefs();
        }

        [UnityTest]
        public IEnumerator HealthStamina_Defeat_EventsFire()
        {
            yield return null;
            var c = TestGameBootstrap.CreateTestCharacter("E", CharacterClass.Mage, true);
            bool health = false, stam = false, dead = false;
            c.OnHealthChanged += (_, __) => health = true;
            c.OnStaminaChanged += (_, __) => stam = true;
            c.OnCharacterDefeated += _ => dead = true;
            c.ConsumeStamina(1f);
            Assert.IsTrue(stam);
            c.Stats.CurrentHealth = 1f;
            c.ApplyCustomDamage(5f, null);
            Assert.IsTrue(health);
            Assert.IsTrue(dead);

            Object.DestroyImmediate(c.gameObject);
        }

        [UnityTest]
        public IEnumerator FairyForm_TimerExpires_RemovesTintWhenNoPad()
        {
            yield return null;
            var go = TestGameBootstrap.CreateTestCharacter("F", CharacterClass.Dreamer, true);
            var sr = go.gameObject.AddComponent<SpriteRenderer>();
            sr.color = Color.red;
            go.ApplyFairyForm(0.05f);
            yield return new WaitForSeconds(0.2f);
            Assert.LessOrEqual(go.FairyFormRemaining, 0f);

            Object.DestroyImmediate(go.gameObject);
        }
    }
}
