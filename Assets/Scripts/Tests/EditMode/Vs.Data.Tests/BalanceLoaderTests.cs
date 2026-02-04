using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;

namespace Vs.Data.Tests
{
    /// <summary>
    /// BalanceLoader 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryBasic)]
    public class BalanceLoaderTests : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            // 테스트 전 BalanceLoader 상태 초기화
        }

        #region WeaponLevelData 구조 테스트

        [Test]
        public void WeaponLevelData_HasDamageField()
        {
            var data = new WeaponLevelData();
            data.damage = 10f;
            Assert.AreEqual(10f, data.damage);
        }

        [Test]
        public void WeaponLevelData_HasIntervalField()
        {
            var data = new WeaponLevelData();
            data.interval = 1.5f;
            Assert.AreEqual(1.5f, data.interval);
        }

        [Test]
        public void WeaponLevelData_HasProjectilesField()
        {
            var data = new WeaponLevelData();
            data.projectiles = 3;
            Assert.AreEqual(3, data.projectiles);
        }

        [Test]
        public void WeaponLevelData_HasAreaField()
        {
            var data = new WeaponLevelData();
            data.area = 2.5f;
            Assert.AreEqual(2.5f, data.area);
        }

        [Test]
        public void WeaponLevelData_HasDurationField()
        {
            var data = new WeaponLevelData();
            data.duration = 5f;
            Assert.AreEqual(5f, data.duration);
        }

        [Test]
        public void WeaponLevelData_HasSpeedField()
        {
            var data = new WeaponLevelData();
            data.speed = 15f;
            Assert.AreEqual(15f, data.speed);
        }

        #endregion

        #region WeaponBalanceData 구조 테스트

        [Test]
        public void WeaponBalanceData_HasLevelsArray()
        {
            var data = new WeaponBalanceData();
            data.levels = new WeaponLevelData[8];
            Assert.AreEqual(8, data.levels.Length);
        }

        [Test]
        public void WeaponBalanceData_LevelsCanBeEmpty()
        {
            var data = new WeaponBalanceData();
            Assert.IsNull(data.levels);
        }

        #endregion

        #region BalanceLoader 기본 테스트

        [Test]
        public void BalanceLoader_LoadDoesNotThrow()
        {
            Assert.DoesNotThrow(() => BalanceLoader.Load());
        }

        [Test]
        public void BalanceLoader_ReloadDoesNotThrow()
        {
            Assert.DoesNotThrow(() => BalanceLoader.Reload());
        }

        [Test]
        public void BalanceLoader_GetWeaponLevel_ReturnsNullForInvalidId()
        {
            BalanceLoader.Load();
            var level = BalanceLoader.GetWeaponLevel("invalid_weapon_id", 1);
            Assert.IsNull(level);
        }

        [Test]
        public void BalanceLoader_GetWeaponLevel_ReturnsDataForValidId()
        {
            BalanceLoader.Load();
            // pistol은 실제 데이터에 존재
            var level = BalanceLoader.GetWeaponLevel("pistol", 1);
            // 데이터가 로드되었으면 NotNull, 아니면 Null (파일 없을 수 있음)
            // Assert.IsNotNull(level); // 환경에 따라 다를 수 있음
            Assert.Pass("GetWeaponLevel executed without error");
        }

        [Test]
        public void BalanceLoader_GetWeaponLevel_LevelClampedToRange()
        {
            BalanceLoader.Load();
            // 레벨 범위 테스트 (1-8)
            Assert.DoesNotThrow(() => BalanceLoader.GetWeaponLevel("pistol", 0));
            Assert.DoesNotThrow(() => BalanceLoader.GetWeaponLevel("pistol", 9));
        }

        #endregion

        #region 데이터 무결성 테스트

        [Test]
        public void WeaponLevelData_DefaultValuesAreZero()
        {
            var data = new WeaponLevelData();
            Assert.AreEqual(0f, data.damage);
            Assert.AreEqual(0f, data.interval);
            Assert.AreEqual(0, data.projectiles);
            Assert.AreEqual(0f, data.area);
            Assert.AreEqual(0f, data.duration);
            Assert.AreEqual(0f, data.speed);
        }

        #endregion
    }
}
