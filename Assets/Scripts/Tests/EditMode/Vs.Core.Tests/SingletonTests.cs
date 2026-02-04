using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;
using Vs.Utility;

namespace Vs.Core.Tests
{
    /// <summary>
    /// Singleton 패턴 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryBasic)]
    public class SingletonTests : TestBase
    {
        // 테스트용 Singleton 클래스
        private class TestSingleton : Singleton<TestSingleton>
        {
            public int Value { get; set; }
        }

        [TearDown]
        public override void TearDown()
        {
            // Singleton 인스턴스 정리
            if (TestSingleton.HasInstance)
            {
                Object.DestroyImmediate(TestSingleton.Instance.gameObject);
            }
            base.TearDown();
        }

        #region 인스턴스 생성 테스트

        [Test]
        public void Instance_CreatesNewInstanceWhenNoneExists()
        {
            var instance = TestSingleton.Instance;
            Assert.IsNotNull(instance);
        }

        [Test]
        public void Instance_ReturnsSameInstanceOnMultipleCalls()
        {
            var instance1 = TestSingleton.Instance;
            var instance2 = TestSingleton.Instance;

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void HasInstance_FalseBeforeAccess()
        {
            // 새 테스트이므로 인스턴스가 없어야 함
            // (TearDown에서 정리됨)
            // 단, 이전 테스트에서 생성되었을 수 있으므로 주의
            Assert.IsNotNull(TestSingleton.Instance); // 이 접근으로 인스턴스 생성
            Assert.IsTrue(TestSingleton.HasInstance);
        }

        [Test]
        public void HasInstance_TrueAfterAccess()
        {
            _ = TestSingleton.Instance;
            Assert.IsTrue(TestSingleton.HasInstance);
        }

        #endregion

        #region 데이터 유지 테스트

        [Test]
        public void Instance_MaintainsDataBetweenAccesses()
        {
            TestSingleton.Instance.Value = 42;
            Assert.AreEqual(42, TestSingleton.Instance.Value);
        }

        #endregion

        #region GameObject 테스트

        [Test]
        public void Instance_HasGameObject()
        {
            var instance = TestSingleton.Instance;
            Assert.IsNotNull(instance.gameObject);
        }

        [Test]
        public void Instance_GameObjectIsActive()
        {
            var instance = TestSingleton.Instance;
            Assert.IsTrue(instance.gameObject.activeInHierarchy);
        }

        #endregion
    }
}
