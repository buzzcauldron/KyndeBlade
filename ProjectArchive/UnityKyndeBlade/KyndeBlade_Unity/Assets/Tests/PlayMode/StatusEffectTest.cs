using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Tests
{
    public class StatusEffectTest
    {
        MedievalCharacter _target;

        [SetUp]
        public void SetUp()
        {
            _target = TestGameBootstrap.CreateTestCharacter("TestTarget", CharacterClass.Knight, false);
        }

        [TearDown]
        public void TearDown()
        {
            if (_target != null) Object.DestroyImmediate(_target.gameObject);
        }

        [UnityTest]
        public IEnumerator FrostEffect_SlowsSpeed()
        {
            yield return null;
            float baseSp = _target.Stats.Speed;
            var frost = StatusEffect.CreateFrostEffect(3f);
            _target.ApplyStatusEffect(frost);

            Assert.IsTrue(_target.HasStatusEffect(StatusEffectType.Frost));
            Assert.Less(_target.Stats.Speed, baseSp, "Frost should reduce speed");
        }

        [UnityTest]
        public IEnumerator BurningEffect_DealsDamageOnTick()
        {
            yield return null;
            float hpBefore = _target.GetCurrentHealth();
            var burn = StatusEffect.CreateBurningEffect(3f, 5f);
            _target.ApplyStatusEffect(burn);

            burn.TickEffect(_target);
            Assert.Less(_target.GetCurrentHealth(), hpBefore, "Burning should deal tick damage");
        }

        [UnityTest]
        public IEnumerator StunEffect_SetsSpeedToZero()
        {
            yield return null;
            var stun = StatusEffect.CreateStunEffect(1f);
            _target.ApplyStatusEffect(stun);

            Assert.IsTrue(_target.HasStatusEffect(StatusEffectType.Stun));
            Assert.AreEqual(0f, _target.Stats.Speed, "Stun should set speed to 0");
        }

        [UnityTest]
        public IEnumerator WeakEffect_ReducesAttack()
        {
            yield return null;
            float atkBefore = _target.Stats.AttackPower;
            var weak = StatusEffect.CreateWeakEffect(2f);
            _target.ApplyStatusEffect(weak);

            Assert.Less(_target.Stats.AttackPower, atkBefore, "Weak should reduce attack power");
        }

        [UnityTest]
        public IEnumerator VulnerableEffect_ReducesDefense()
        {
            yield return null;
            float defBefore = _target.Stats.Defense;
            var vuln = StatusEffect.CreateVulnerableEffect(2f);
            _target.ApplyStatusEffect(vuln);

            Assert.Less(_target.Stats.Defense, defBefore, "Vulnerable should reduce defense");
        }

        [UnityTest]
        public IEnumerator BlessedEffect_BoostsAllStats()
        {
            yield return null;
            float atkBefore = _target.Stats.AttackPower;
            float defBefore = _target.Stats.Defense;
            float spdBefore = _target.Stats.Speed;

            var blessed = StatusEffect.CreateBlessedEffect(3f);
            _target.ApplyStatusEffect(blessed);

            Assert.Greater(_target.Stats.AttackPower, atkBefore, "Blessed should increase attack");
            Assert.Greater(_target.Stats.Defense, defBefore, "Blessed should increase defense");
            Assert.Greater(_target.Stats.Speed, spdBefore, "Blessed should increase speed");
        }

        [UnityTest]
        public IEnumerator KyndeBoostEffect_IncreasesKyndeGen()
        {
            yield return null;
            var boost = StatusEffect.CreateKyndeBoostEffect(2f, 2f);

            Assert.AreEqual(StatusEffectType.KyndeBoost, boost.Data.EffectType);
            Assert.AreEqual(2f, boost.Data.KyndeGenerationModifier);
        }

        [UnityTest]
        public IEnumerator SlowEffect_ReducesSpeed()
        {
            yield return null;
            float spdBefore = _target.Stats.Speed;
            var slow = StatusEffect.CreateSlowEffect(2f, 0.5f);
            _target.ApplyStatusEffect(slow);

            Assert.Less(_target.Stats.Speed, spdBefore, "Slow should reduce speed");
        }

        [UnityTest]
        public IEnumerator PoisonEffect_DealsDamageAndReducesStaminaRegen()
        {
            yield return null;
            var poison = StatusEffect.CreatePoisonEffect(3f, 2);
            Assert.AreEqual(StatusEffectType.Poison, poison.Data.EffectType);
            Assert.Greater(poison.Data.DamagePerSecond, 0f, "Poison should have DoT");
            Assert.Less(poison.Data.StaminaRegenModifier, 1f, "Poison should reduce stamina regen");
        }

        [UnityTest]
        public IEnumerator TickEffect_DecrementsRemainingTime()
        {
            yield return null;
            var burn = StatusEffect.CreateBurningEffect(3f, 1f);
            Assert.AreEqual(3f, burn.Data.RemainingTime);

            burn.TickEffect(_target);
            Assert.AreEqual(2f, burn.Data.RemainingTime, 0.01f);

            burn.TickEffect(_target);
            Assert.AreEqual(1f, burn.Data.RemainingTime, 0.01f);

            burn.TickEffect(_target);
            Assert.AreEqual(0f, burn.Data.RemainingTime, 0.01f);
            Assert.IsTrue(burn.IsExpired(), "Effect should expire after duration depleted");
        }

        [UnityTest]
        public IEnumerator AgeEffect_NeverExpires()
        {
            yield return null;
            var age = StatusEffect.CreateAgeEffect(2);
            Assert.IsNotNull(age);
            Assert.AreEqual(0f, age.Data.Duration, "Age has no duration (permanent)");
            Assert.IsFalse(age.IsExpired(), "Permanent effects should never expire");
        }

        [UnityTest]
        public IEnumerator HungerEffect_ReducesMultipleStats()
        {
            yield return null;
            var hunger = StatusEffect.CreateHungerEffect(0f, 3);
            Assert.Less(hunger.Data.AttackPowerModifier, 1f);
            Assert.Less(hunger.Data.DefenseModifier, 1f);
            Assert.Less(hunger.Data.SpeedModifier, 1f);
            Assert.Greater(hunger.Data.DamagePerSecond, 0f, "Hunger should deal DoT");
        }

        [UnityTest]
        public IEnumerator Pipeline_FlammeAppliesBurning()
        {
            yield return null;

            int burnCount = 0;
            for (int i = 0; i < 100; i++)
            {
                var test = TestGameBootstrap.CreateTestCharacter($"PipeTest{i}", CharacterClass.Knight, false);
                yield return null;
                StatusEffectPipeline.TryApplyElement(KyndeElementType.Flamme, test);
                if (test.HasStatusEffect(StatusEffectType.Burning)) burnCount++;
                Object.DestroyImmediate(test.gameObject);
            }

            Assert.Greater(burnCount, 0, "Flamme should apply Burning at least once in 100 tries (~30% chance)");
            Assert.Less(burnCount, 100, "Flamme should not always apply Burning");
        }

        [UnityTest]
        public IEnumerator Pipeline_NoneElement_NoEffect()
        {
            yield return null;
            StatusEffectPipeline.TryApplyElement(KyndeElementType.None, _target);
            Assert.AreEqual(0, _target.ActiveStatusEffects.Count, "None element should apply no effects");
        }
    }
}
