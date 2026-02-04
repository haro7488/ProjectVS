using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Combat;
using Vs.Data;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    /// <summary>
    /// WeaponController 통합 테스트.
    /// 무기 슬롯 관리 및 무기 추가/제거 동작 검증.
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryIntegration)]
    public class WeaponControllerTests : PlayModeTestBase
    {
        private WeaponController _controller;
        private Transform _owner;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            // Owner 생성
            var ownerGo = CreateEmptyGameObject("Player");
            _owner = ownerGo.transform;

            // WeaponController 생성
            var controllerGo = CreateEmptyGameObject("WeaponController");
            controllerGo.transform.SetParent(_owner);
            _controller = controllerGo.AddComponent<WeaponController>();
            _controller.Initialize(_owner);
        }

        #region 초기 상태 테스트

        [UnityTest]
        public IEnumerator InitialState_WeaponCountIsZero()
        {
            yield return null;
            Assert.AreEqual(0, _controller.WeaponCount);
        }

        [UnityTest]
        public IEnumerator InitialState_HasEmptySlot()
        {
            yield return null;
            Assert.IsTrue(_controller.HasEmptySlot);
        }

        [UnityTest]
        public IEnumerator InitialState_WeaponsListIsEmpty()
        {
            yield return null;
            Assert.IsEmpty(_controller.Weapons);
        }

        #endregion

        #region AddWeapon 테스트 (null 입력)

        [UnityTest]
        public IEnumerator AddWeapon_NullData_ReturnsFalse()
        {
            yield return null;

            bool result = _controller.AddWeapon(null);

            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator AddWeapon_NullData_DoesNotIncreaseCount()
        {
            yield return null;

            _controller.AddWeapon(null);

            Assert.AreEqual(0, _controller.WeaponCount);
        }

        #endregion

        #region HasWeapon 테스트

        [UnityTest]
        public IEnumerator HasWeapon_NonExistent_ReturnsFalse()
        {
            yield return null;

            bool result = _controller.HasWeapon("non_existent_weapon");

            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator GetWeapon_NonExistent_ReturnsNull()
        {
            yield return null;

            var weapon = _controller.GetWeapon("non_existent_weapon");

            Assert.IsNull(weapon);
        }

        #endregion

        #region LevelUpWeapon 테스트 (존재하지 않는 무기)

        [UnityTest]
        public IEnumerator LevelUpWeapon_NonExistent_ReturnsFalse()
        {
            var data = MockFactory.CreateWeaponData(id: "test_weapon");
            CreatedScriptableObjects.Add(data);

            yield return null;

            bool result = _controller.LevelUpWeapon(data);

            Assert.IsFalse(result);
        }

        #endregion

        #region RemoveWeapon 테스트

        [UnityTest]
        public IEnumerator RemoveWeapon_NonExistent_ReturnsFalse()
        {
            yield return null;

            bool result = _controller.RemoveWeapon("non_existent_weapon");

            Assert.IsFalse(result);
        }

        #endregion

        #region ClearAllWeapons 테스트

        [UnityTest]
        public IEnumerator ClearAllWeapons_WhenEmpty_DoesNotThrow()
        {
            yield return null;

            Assert.DoesNotThrow(() => _controller.ClearAllWeapons());
            Assert.AreEqual(0, _controller.WeaponCount);
        }

        #endregion

        #region ReplaceWeapon 테스트 (null 입력)

        [UnityTest]
        public IEnumerator ReplaceWeapon_NullOldData_ReturnsFalse()
        {
            var newData = MockFactory.CreateWeaponData(id: "new_weapon");
            CreatedScriptableObjects.Add(newData);

            yield return null;

            bool result = _controller.ReplaceWeapon(null, newData);

            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator ReplaceWeapon_NullNewData_ReturnsFalse()
        {
            var oldData = MockFactory.CreateWeaponData(id: "old_weapon");
            CreatedScriptableObjects.Add(oldData);

            yield return null;

            bool result = _controller.ReplaceWeapon(oldData, null);

            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator ReplaceWeapon_NonExistentOldWeapon_ReturnsFalse()
        {
            var oldData = MockFactory.CreateWeaponData(id: "old_weapon");
            var newData = MockFactory.CreateWeaponData(id: "new_weapon");
            CreatedScriptableObjects.Add(oldData);
            CreatedScriptableObjects.Add(newData);

            yield return null;

            bool result = _controller.ReplaceWeapon(oldData, newData);

            Assert.IsFalse(result);
        }

        #endregion

        #region SetStartingWeapon 테스트

        [UnityTest]
        public IEnumerator SetStartingWeapon_NullData_ClearsWeapons()
        {
            yield return null;

            _controller.SetStartingWeapon(null);

            Assert.AreEqual(0, _controller.WeaponCount);
        }

        #endregion

        #region 슬롯 제한 테스트

        [UnityTest]
        public IEnumerator MaxWeaponSlots_IsSix()
        {
            yield return null;
            Assert.AreEqual(TestConstants.MaxWeaponSlots, 6);
        }

        #endregion
    }
}
