using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;

namespace Vs.Combat.Tests
{
    /// <summary>
    /// DamageInfo 및 대미지 계산 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryCombat)]
    public class DamageCalculationTests : TestBase
    {
        #region DamageInfo 생성 테스트

        [Test]
        public void DamageInfo_Constructor_SetsAmountCorrectly()
        {
            var damage = new DamageInfo(amount: 50f);
            Assert.AreEqual(50f, damage.Amount);
        }

        [Test]
        public void DamageInfo_Constructor_DefaultTypeIsPhysical()
        {
            var damage = new DamageInfo(amount: 10f);
            Assert.AreEqual(DamageType.Physical, damage.Type);
        }

        [Test]
        public void DamageInfo_Constructor_SetsTypeCorrectly()
        {
            var damage = new DamageInfo(amount: 10f, type: DamageType.Fire);
            Assert.AreEqual(DamageType.Fire, damage.Type);
        }

        [Test]
        public void DamageInfo_Constructor_DefaultIsCriticalIsFalse()
        {
            var damage = new DamageInfo(amount: 10f);
            Assert.IsFalse(damage.IsCritical);
        }

        [Test]
        public void DamageInfo_Constructor_DefaultKnockbackIsZero()
        {
            var damage = new DamageInfo(amount: 10f);
            Assert.AreEqual(0f, damage.Knockback);
        }

        [Test]
        public void DamageInfo_Constructor_SetsKnockbackCorrectly()
        {
            var damage = new DamageInfo(amount: 10f, knockback: 5f);
            Assert.AreEqual(5f, damage.Knockback);
        }

        [Test]
        public void DamageInfo_Constructor_SetsPositionCorrectly()
        {
            var pos = new Vector3(1, 2, 3);
            var damage = new DamageInfo(amount: 10f, position: pos);
            Assert.AreEqual(pos, damage.Position);
        }

        [Test]
        public void DamageInfo_Constructor_SetsDirectionCorrectly()
        {
            var dir = new Vector3(0, 0, 1);
            var damage = new DamageInfo(amount: 10f, direction: dir);
            Assert.AreEqual(dir, damage.Direction);
        }

        #endregion

        #region WithAmount 테스트

        [Test]
        public void WithAmount_ReturnsNewInstanceWithNewAmount()
        {
            var original = new DamageInfo(amount: 10f, type: DamageType.Fire);
            var modified = original.WithAmount(20f);

            Assert.AreEqual(20f, modified.Amount);
        }

        [Test]
        public void WithAmount_PreservesOtherProperties()
        {
            var original = new DamageInfo(amount: 10f, type: DamageType.Fire, knockback: 3f);
            var modified = original.WithAmount(20f);

            Assert.AreEqual(DamageType.Fire, modified.Type);
            Assert.AreEqual(3f, modified.Knockback);
        }

        [Test]
        public void WithAmount_DoesNotModifyOriginal()
        {
            var original = new DamageInfo(amount: 10f);
            _ = original.WithAmount(20f);

            Assert.AreEqual(10f, original.Amount);
        }

        #endregion

        #region AsCritical 테스트

        [Test]
        public void AsCritical_SetsIsCriticalToTrue()
        {
            var original = new DamageInfo(amount: 10f);
            var critical = original.AsCritical();

            Assert.IsTrue(critical.IsCritical);
        }

        [Test]
        public void AsCritical_DefaultMultiplierIsTwo()
        {
            var original = new DamageInfo(amount: 10f);
            var critical = original.AsCritical();

            Assert.AreEqual(20f, critical.Amount);
        }

        [Test]
        public void AsCritical_CustomMultiplier()
        {
            var original = new DamageInfo(amount: 10f);
            var critical = original.AsCritical(multiplier: 3f);

            Assert.AreEqual(30f, critical.Amount);
        }

        [Test]
        public void AsCritical_PreservesOtherProperties()
        {
            var original = new DamageInfo(amount: 10f, type: DamageType.Electric, knockback: 2f);
            var critical = original.AsCritical();

            Assert.AreEqual(DamageType.Electric, critical.Type);
            Assert.AreEqual(2f, critical.Knockback);
        }

        [Test]
        public void AsCritical_DoesNotModifyOriginal()
        {
            var original = new DamageInfo(amount: 10f);
            _ = original.AsCritical();

            Assert.AreEqual(10f, original.Amount);
            Assert.IsFalse(original.IsCritical);
        }

        #endregion

        #region DamageType enum 테스트

        [Test]
        public void DamageType_HasFourValues()
        {
            var values = System.Enum.GetValues(typeof(DamageType));
            Assert.AreEqual(4, values.Length);
        }

        [Test]
        public void DamageType_ContainsPhysical()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(DamageType), DamageType.Physical));
        }

        [Test]
        public void DamageType_ContainsFire()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(DamageType), DamageType.Fire));
        }

        [Test]
        public void DamageType_ContainsElectric()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(DamageType), DamageType.Electric));
        }

        [Test]
        public void DamageType_ContainsPoison()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(DamageType), DamageType.Poison));
        }

        #endregion

        #region 대미지 계산 시나리오 테스트

        [Test]
        public void Damage_ZeroAmount_IsValid()
        {
            var damage = new DamageInfo(amount: 0f);
            Assert.AreEqual(0f, damage.Amount);
        }

        [Test]
        public void Damage_NegativeAmount_IsValid()
        {
            // 음수 대미지도 허용 (힐링 등에 사용 가능)
            var damage = new DamageInfo(amount: -10f);
            Assert.AreEqual(-10f, damage.Amount);
        }

        [Test]
        public void Damage_LargeAmount_IsValid()
        {
            var damage = new DamageInfo(amount: 999999f);
            Assert.AreEqual(999999f, damage.Amount);
        }

        #endregion
    }
}
