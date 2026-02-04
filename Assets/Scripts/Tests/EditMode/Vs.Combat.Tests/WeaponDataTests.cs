using NUnit.Framework;
using UnityEngine;
using Vs.Data;
using Vs.Tests.Utilities;

namespace Vs.Combat.Tests
{
    /// <summary>
    /// WeaponData ScriptableObject 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryWeapon)]
    public class WeaponDataTests : TestBase
    {
        #region WeaponType enum 테스트

        [Test]
        public void WeaponType_HasFiveValues()
        {
            var values = System.Enum.GetValues(typeof(WeaponType));
            Assert.AreEqual(5, values.Length);
        }

        [Test]
        public void WeaponType_ContainsProjectile()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponType), WeaponType.Projectile));
        }

        [Test]
        public void WeaponType_ContainsMelee()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponType), WeaponType.Melee));
        }

        [Test]
        public void WeaponType_ContainsArea()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponType), WeaponType.Area));
        }

        [Test]
        public void WeaponType_ContainsOrbital()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponType), WeaponType.Orbital));
        }

        [Test]
        public void WeaponType_ContainsMagic()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(WeaponType), WeaponType.Magic));
        }

        #endregion

        #region MockFactory WeaponData 테스트

        [Test]
        public void MockFactory_CreateWeaponData_ReturnsNonNull()
        {
            var data = MockFactory.CreateWeaponData();
            Assert.IsNotNull(data);

            // 정리
            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsIdCorrectly()
        {
            var data = MockFactory.CreateWeaponData(id: "my_weapon");
            Assert.AreEqual("my_weapon", data.Id);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsDisplayNameCorrectly()
        {
            var data = MockFactory.CreateWeaponData(displayName: "마이 웨폰");
            Assert.AreEqual("마이 웨폰", data.DisplayName);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsTypeCorrectly()
        {
            var data = MockFactory.CreateWeaponData(type: WeaponType.Melee);
            Assert.AreEqual(WeaponType.Melee, data.Type);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsBaseDamageCorrectly()
        {
            var data = MockFactory.CreateWeaponData(baseDamage: 25f);
            Assert.AreEqual(25f, data.BaseDamage);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsBaseIntervalCorrectly()
        {
            var data = MockFactory.CreateWeaponData(baseInterval: 0.5f);
            Assert.AreEqual(0.5f, data.BaseInterval);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void MockFactory_CreateWeaponData_SetsBaseProjectileCountCorrectly()
        {
            var data = MockFactory.CreateWeaponData(baseProjectileCount: 3);
            Assert.AreEqual(3, data.BaseProjectileCount);

            Object.DestroyImmediate(data);
        }

        #endregion

        #region WeaponData 프로퍼티 테스트

        [Test]
        public void WeaponData_CanEvolve_FalseWhenNoEvolutionTarget()
        {
            var data = MockFactory.CreateWeaponData();
            Assert.IsFalse(data.CanEvolve);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void WeaponData_IsEvolved_DefaultFalse()
        {
            var data = MockFactory.CreateWeaponData();
            Assert.IsFalse(data.IsEvolved);

            Object.DestroyImmediate(data);
        }

        #endregion
    }
}
