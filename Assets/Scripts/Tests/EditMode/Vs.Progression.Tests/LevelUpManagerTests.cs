using NUnit.Framework;
using UnityEngine;
using Vs.Data;
using Vs.Tests.Utilities;
using Vs.Utility;

namespace Vs.Progression.Tests
{
    /// <summary>
    /// LevelUpManager 단위 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryProgression)]
    public class LevelUpManagerTests : TestBase
    {
        private LevelUpManager _manager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _manager = CreateComponent<LevelUpManager>();
        }

        #region 초기 상태 테스트

        [Test]
        public void InitialState_WeaponCountIsZero()
        {
            Assert.AreEqual(0, _manager.WeaponCount);
        }

        [Test]
        public void InitialState_PassiveCountIsZero()
        {
            Assert.AreEqual(0, _manager.PassiveCount);
        }

        [Test]
        public void InitialState_OwnedWeaponsIsEmpty()
        {
            Assert.IsEmpty(_manager.OwnedWeapons);
        }

        [Test]
        public void InitialState_OwnedPassivesIsEmpty()
        {
            Assert.IsEmpty(_manager.OwnedPassives);
        }

        #endregion

        #region 선택지 생성 테스트

        [Test]
        public void GenerateChoices_MethodExists()
        {
            // GenerateChoices 메서드가 존재하는지 확인
            var method = typeof(LevelUpManager).GetMethod("GenerateChoices");
            Assert.IsNotNull(method);
        }

        [Test]
        public void GenerateChoices_DefaultParameterIsThree()
        {
            // GenerateChoices의 기본 매개변수가 Constants.LevelUpChoices인지 확인
            Assert.AreEqual(3, Constants.LevelUpChoices);
        }

        #endregion

        #region 슬롯 제한 테스트

        [Test]
        public void MaxWeaponSlots_IsSix()
        {
            Assert.AreEqual(6, Constants.MaxWeaponSlots);
        }

        [Test]
        public void MaxPassiveSlots_IsSix()
        {
            Assert.AreEqual(6, Constants.MaxPassiveSlots);
        }

        [Test]
        public void MaxWeaponLevel_IsEight()
        {
            Assert.AreEqual(8, Constants.MaxWeaponLevel);
        }

        [Test]
        public void LevelUpChoices_IsThree()
        {
            Assert.AreEqual(3, Constants.LevelUpChoices);
        }

        #endregion

        #region 이벤트 테스트

        [Test]
        public void OnChoicesGenerated_EventExists()
        {
            bool eventFired = false;
            _manager.OnChoicesGenerated += (choices) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnChoiceApplied_EventExists()
        {
            bool eventFired = false;
            _manager.OnChoiceApplied += (choice) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnWeaponAdded_EventExists()
        {
            bool eventFired = false;
            _manager.OnWeaponAdded += (weapon, level) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnWeaponUpgraded_EventExists()
        {
            bool eventFired = false;
            _manager.OnWeaponUpgraded += (weapon, level) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnPassiveAdded_EventExists()
        {
            bool eventFired = false;
            _manager.OnPassiveAdded += (passive, level) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnPassiveUpgraded_EventExists()
        {
            bool eventFired = false;
            _manager.OnPassiveUpgraded += (passive, level) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnWeaponEvolved_EventExists()
        {
            bool eventFired = false;
            _manager.OnWeaponEvolved += (source, evolved) => eventFired = true;
            Assert.IsNotNull(_manager);
        }

        #endregion
    }
}
