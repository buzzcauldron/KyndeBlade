using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    /// <summary>Phase 1: pure logic / EditMode — CombatCalculator, timing, status factories, class stats, GameProgress JSON.</summary>
    public class Phase1LogicTests
    {
        static void InvokeInitializeStatsByClass(MedievalCharacter c)
        {
            var mi = typeof(MedievalCharacter).GetMethod("InitializeStatsByClass",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(mi);
            mi.Invoke(c, null);
        }

        [Test]
        public void CombatCalculator_BaseDamage_UsesDefenseMitigationAndMinimum()
        {
            var atkGo = new GameObject("a");
            atkGo.AddComponent<BoxCollider>();
            var attacker = atkGo.AddComponent<MedievalCharacter>();
            attacker.Stats.AttackPower = 20f;
            attacker.Stats.Defense = 0f;

            var defGo = new GameObject("d");
            defGo.AddComponent<BoxCollider>();
            var target = defGo.AddComponent<MedievalCharacter>();
            target.Stats.Defense = 10f;

            Random.InitState(42);
            var action = DefaultCombatActions.CreateStrike();
            action.ActionData.Damage = 20f;
            float dmg = CombatCalculator.CalculateDamage(attacker, target, action);
            float raw = 20f - 10f * GameWorldConstants.DefenseMitigationFactor;
            float expectedMin = Mathf.Max(GameWorldConstants.MinimumDamage, raw);
            float v = GameWorldConstants.DamageVarianceHalfRange;
            Assert.Greater(dmg, 0f);
            Assert.GreaterOrEqual(dmg, expectedMin * (1f - v) - 0.001f);
            Assert.LessOrEqual(dmg, expectedMin * (1f + v) + 0.001f);

            Object.DestroyImmediate(atkGo);
            Object.DestroyImmediate(defGo);
        }

        [Test]
        public void CombatCalculator_Elemental_None_IsOne()
        {
            var go = new GameObject("t");
            go.AddComponent<BoxCollider>();
            var target = go.AddComponent<MedievalCharacter>();
            Assert.AreEqual(1f, CombatCalculator.GetElementalMultiplier(target, KyndeElementType.None));
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CombatCalculator_IsPerfectTiming_LastFifteenPercent()
        {
            Assert.IsFalse(CombatCalculator.IsPerfectTiming(0.5f, 2f));
            Assert.IsTrue(CombatCalculator.IsPerfectTiming(0.2f, 2f));
            Assert.IsFalse(CombatCalculator.IsPerfectTiming(0.2f, 0f));
        }

        [Test]
        public void GameSettings_GetAdjustedWindow_ScalesByDifficulty()
        {
            var gs = ScriptableObject.CreateInstance<GameSettings>();
            gs.TimingWindowMultiplier = 1f;
            gs.Difficulty = DifficultyMode.Normal;
            Assert.AreEqual(2f, gs.GetAdjustedWindow(2f));
            gs.Difficulty = DifficultyMode.Easy;
            Assert.AreEqual(3f, gs.GetAdjustedWindow(2f));
            Object.DestroyImmediate(gs);
        }

        [Test]
        public void ClassStats_Knight_Mage_Archer_Rogue_Dreamer_MatchDesign()
        {
            void AssertClass(CharacterClass cls, float hp, float stam, float atk, float def, float spd)
            {
                var go = new GameObject(cls.ToString());
                go.AddComponent<BoxCollider>();
                var c = go.AddComponent<MedievalCharacter>();
                c.CharacterClassType = cls;
                InvokeInitializeStatsByClass(c);
                Assert.AreEqual(hp, c.Stats.MaxHealth, 0.01f, cls.ToString());
                Assert.AreEqual(hp, c.Stats.CurrentHealth, 0.01f);
                Assert.AreEqual(stam, c.Stats.MaxStamina, 0.01f);
                Assert.AreEqual(atk, c.Stats.AttackPower, 0.01f);
                Assert.AreEqual(def, c.Stats.Defense, 0.01f);
                Assert.AreEqual(spd, c.Stats.Speed, 0.01f);
                Object.DestroyImmediate(go);
            }

            AssertClass(CharacterClass.Knight, 150f, 120f, 15f, 10f, 8f);
            AssertClass(CharacterClass.Mage, 80f, 150f, 20f, 3f, 10f);
            AssertClass(CharacterClass.Archer, 100f, 130f, 12f, 5f, 12f);
            AssertClass(CharacterClass.Rogue, 90f, 140f, 14f, 4f, 15f);
            AssertClass(CharacterClass.Dreamer, 120f, 130f, 14f, 6f, 9f);
        }

        [Test]
        public void StatusEffect_Hunger_Age_Scar_Frost_Burning_Factories()
        {
            var h = StatusEffect.CreateHungerEffect(8f, 2);
            Assert.AreEqual(StatusEffectType.Hunger, h.Data.EffectType);
            Assert.AreEqual(2, h.Data.StackCount);
            Assert.Less(h.Data.KyndeGenerationModifier, 1f);

            var age = StatusEffect.CreateAgeEffect(3);
            Assert.AreEqual(StatusEffectType.Age, age.Data.EffectType);
            Assert.AreEqual(3, age.Data.StackCount);
            Assert.AreEqual(0f, age.Data.Duration);

            var scar = StatusEffect.CreateHungerScarEffect();
            Assert.AreEqual(StatusEffectType.HungerScar, scar.Data.EffectType);
            Assert.AreEqual(0.95f, scar.Data.AttackPowerModifier, 0.001f);

            var frost = StatusEffect.CreateFrostEffect(5f, 0.2f);
            Assert.AreEqual(StatusEffectType.Frost, frost.Data.EffectType);
            Assert.AreEqual(0.8f, frost.Data.SpeedModifier, 0.001f);

            var burn = StatusEffect.CreateBurningEffect(4f, 3f);
            Assert.AreEqual(StatusEffectType.Burning, burn.Data.EffectType);
            Assert.AreEqual(3f, burn.Data.DamagePerSecond);
        }

        [Test]
        public void StatusEffect_StatModifiers_ApplyAndRemoveAreReversible()
        {
            var go = new GameObject("ch");
            go.AddComponent<BoxCollider>();
            var c = go.AddComponent<MedievalCharacter>();
            c.Stats.AttackPower = 100f;
            c.Stats.Defense = 100f;
            c.Stats.Speed = 100f;

            var e = StatusEffect.CreateFrostEffect(2f, 0.1f);
            e.ApplyEffect(c);
            Assert.AreEqual(90f, c.Stats.Speed, 0.01f);
            e.RemoveEffect(c);
            Assert.AreEqual(100f, c.Stats.Speed, 0.01f);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void StatusEffect_TimedExpiry_AndDoT()
        {
            var go = new GameObject("ch");
            go.AddComponent<BoxCollider>();
            var c = go.AddComponent<MedievalCharacter>();
            c.Stats.MaxHealth = 100f;
            c.Stats.CurrentHealth = 100f;

            var burn = StatusEffect.CreateBurningEffect(1f, 10f);
            c.ActiveStatusEffects.Add(burn);
            burn.ApplyEffect(c);
            burn.ApplyDamageOverTime(c, 0.5f);
            Assert.Less(c.GetCurrentHealth(), 100f);

            burn.Data.RemainingTime = 0.01f;
            burn.UpdateEffect(1f);
            Assert.IsTrue(burn.IsExpired());

            Object.DestroyImmediate(go);
        }

        [Test]
        public void GameProgress_ToJson_FromJson_Roundtrip()
        {
            var p = GameProgress.CreateNew("test_loc");
            p.VisitedLocationIds.Add("b");
            p.EthicalMisstepCount = 2;
            p.HasEverHadHunger = true;
            string json = p.ToJson();
            var back = GameProgress.FromJson(json);
            Assert.AreEqual("test_loc", back.CurrentLocationId);
            Assert.IsTrue(back.VisitedLocationIds.Contains("b"));
            Assert.AreEqual(2, back.EthicalMisstepCount);
            Assert.IsTrue(back.HasEverHadHunger);
        }
    }
}
