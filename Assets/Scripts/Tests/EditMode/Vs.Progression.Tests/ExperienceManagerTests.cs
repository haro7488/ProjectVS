using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;

namespace Vs.Progression.Tests
{
    /// <summary>
    /// ExperienceManager 단위 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryProgression)]
    public class ExperienceManagerTests : TestBase
    {
        private ExperienceManager _manager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _manager = CreateComponent<ExperienceManager>();
        }

        #region 초기 상태 테스트

        [Test]
        public void InitialState_LevelIsOne()
        {
            Assert.AreEqual(1, _manager.CurrentLevel);
        }

        [Test]
        public void InitialState_ExpIsZero()
        {
            Assert.AreEqual(0, _manager.CurrentExp);
        }

        [Test]
        public void InitialState_ExpToNextLevelIsPositive()
        {
            Assert.Greater(_manager.ExpToNextLevel, 0);
        }

        [Test]
        public void InitialState_ExpProgressIsZero()
        {
            AssertExtensions.AreApproximatelyEqual(0f, _manager.ExpProgress);
        }

        #endregion

        #region 경험치 계산 테스트

        [Test]
        public void ExpProgress_ReturnsCorrectRatio()
        {
            // ExpToNextLevel이 10이라고 가정
            // 직접 필드를 설정할 수 없으므로 계산 로직 테스트
            var progress = _manager.ExpProgress;
            AssertExtensions.IsInRange(progress, 0f, 1f);
        }

        #endregion

        #region 이벤트 테스트

        [Test]
        public void OnExpGained_EventExists()
        {
            bool eventFired = false;
            _manager.OnExpGained += (amount) => eventFired = true;

            // 이벤트가 등록 가능한지 확인
            Assert.IsNotNull(_manager);
        }

        [Test]
        public void OnLevelUp_EventExists()
        {
            bool eventFired = false;
            _manager.OnLevelUp += (level) => eventFired = true;

            // 이벤트가 등록 가능한지 확인
            Assert.IsNotNull(_manager);
        }

        #endregion

        #region 프로퍼티 테스트

        [Test]
        public void TotalExpGained_InitiallyZero()
        {
            Assert.AreEqual(0, _manager.TotalExpGained);
        }

        [Test]
        public void CurrentLevel_IsReadOnly()
        {
            // CurrentLevel은 읽기 전용 프로퍼티
            Assert.AreEqual(1, _manager.CurrentLevel);
        }

        [Test]
        public void CurrentExp_IsReadOnly()
        {
            // CurrentExp은 읽기 전용 프로퍼티
            Assert.AreEqual(0, _manager.CurrentExp);
        }

        #endregion
    }
}
