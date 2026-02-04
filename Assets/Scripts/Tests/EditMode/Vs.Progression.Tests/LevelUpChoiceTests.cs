using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;
using static Vs.Progression.LevelUpChoice;

namespace Vs.Progression.Tests
{
    /// <summary>
    /// LevelUpChoice 단위 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryProgression)]
    public class LevelUpChoiceTests : TestBase
    {
        #region 생성자 테스트

        [Test]
        public void Constructor_SetsTypeCorrectly()
        {
            var choice = new LevelUpChoice(
                ChoiceType.NewWeapon,
                null,
                1,
                "테스트",
                "설명",
                null
            );

            Assert.AreEqual(ChoiceType.NewWeapon, choice.Type);
        }

        [Test]
        public void Constructor_SetsNewLevelCorrectly()
        {
            var choice = new LevelUpChoice(
                ChoiceType.WeaponUpgrade,
                null,
                5,
                "테스트",
                "설명",
                null
            );

            Assert.AreEqual(5, choice.NewLevel);
        }

        [Test]
        public void Constructor_SetsDisplayNameCorrectly()
        {
            var choice = new LevelUpChoice(
                ChoiceType.NewWeapon,
                null,
                1,
                "테스트 무기",
                "설명",
                null
            );

            Assert.AreEqual("테스트 무기", choice.DisplayName);
        }

        [Test]
        public void Constructor_SetsDescriptionCorrectly()
        {
            var choice = new LevelUpChoice(
                ChoiceType.NewWeapon,
                null,
                1,
                "테스트",
                "테스트 설명입니다",
                null
            );

            Assert.AreEqual("테스트 설명입니다", choice.Description);
        }

        [Test]
        public void Constructor_SourceDataIsOptional()
        {
            var choice = new LevelUpChoice(
                ChoiceType.NewWeapon,
                null,
                1,
                "테스트",
                "설명",
                null
            );

            Assert.IsNull(choice.SourceData);
        }

        #endregion

        #region IsNewItem 테스트

        [Test]
        public void IsNewItem_TrueForNewWeapon()
        {
            var choice = new LevelUpChoice(ChoiceType.NewWeapon, null, 1, "", "", null);
            Assert.IsTrue(choice.IsNewItem);
        }

        [Test]
        public void IsNewItem_TrueForNewPassive()
        {
            var choice = new LevelUpChoice(ChoiceType.NewPassive, null, 1, "", "", null);
            Assert.IsTrue(choice.IsNewItem);
        }

        [Test]
        public void IsNewItem_FalseForWeaponUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsNewItem);
        }

        [Test]
        public void IsNewItem_FalseForPassiveUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.PassiveUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsNewItem);
        }

        [Test]
        public void IsNewItem_FalseForWeaponEvolution()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponEvolution, null, 1, "", "", null);
            Assert.IsFalse(choice.IsNewItem);
        }

        #endregion

        #region IsUpgrade 테스트

        [Test]
        public void IsUpgrade_TrueForWeaponUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponUpgrade, null, 2, "", "", null);
            Assert.IsTrue(choice.IsUpgrade);
        }

        [Test]
        public void IsUpgrade_TrueForPassiveUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.PassiveUpgrade, null, 2, "", "", null);
            Assert.IsTrue(choice.IsUpgrade);
        }

        [Test]
        public void IsUpgrade_FalseForNewWeapon()
        {
            var choice = new LevelUpChoice(ChoiceType.NewWeapon, null, 1, "", "", null);
            Assert.IsFalse(choice.IsUpgrade);
        }

        [Test]
        public void IsUpgrade_FalseForNewPassive()
        {
            var choice = new LevelUpChoice(ChoiceType.NewPassive, null, 1, "", "", null);
            Assert.IsFalse(choice.IsUpgrade);
        }

        [Test]
        public void IsUpgrade_FalseForWeaponEvolution()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponEvolution, null, 1, "", "", null);
            Assert.IsFalse(choice.IsUpgrade);
        }

        #endregion

        #region IsWeapon 테스트

        [Test]
        public void IsWeapon_TrueForNewWeapon()
        {
            var choice = new LevelUpChoice(ChoiceType.NewWeapon, null, 1, "", "", null);
            Assert.IsTrue(choice.IsWeapon);
        }

        [Test]
        public void IsWeapon_TrueForWeaponUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponUpgrade, null, 2, "", "", null);
            Assert.IsTrue(choice.IsWeapon);
        }

        [Test]
        public void IsWeapon_FalseForNewPassive()
        {
            var choice = new LevelUpChoice(ChoiceType.NewPassive, null, 1, "", "", null);
            Assert.IsFalse(choice.IsWeapon);
        }

        [Test]
        public void IsWeapon_FalseForPassiveUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.PassiveUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsWeapon);
        }

        #endregion

        #region IsPassive 테스트

        [Test]
        public void IsPassive_TrueForNewPassive()
        {
            var choice = new LevelUpChoice(ChoiceType.NewPassive, null, 1, "", "", null);
            Assert.IsTrue(choice.IsPassive);
        }

        [Test]
        public void IsPassive_TrueForPassiveUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.PassiveUpgrade, null, 2, "", "", null);
            Assert.IsTrue(choice.IsPassive);
        }

        [Test]
        public void IsPassive_FalseForNewWeapon()
        {
            var choice = new LevelUpChoice(ChoiceType.NewWeapon, null, 1, "", "", null);
            Assert.IsFalse(choice.IsPassive);
        }

        [Test]
        public void IsPassive_FalseForWeaponUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsPassive);
        }

        #endregion

        #region IsEvolution 테스트

        [Test]
        public void IsEvolution_TrueForWeaponEvolution()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponEvolution, null, 1, "", "", null);
            Assert.IsTrue(choice.IsEvolution);
        }

        [Test]
        public void IsEvolution_FalseForNewWeapon()
        {
            var choice = new LevelUpChoice(ChoiceType.NewWeapon, null, 1, "", "", null);
            Assert.IsFalse(choice.IsEvolution);
        }

        [Test]
        public void IsEvolution_FalseForWeaponUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.WeaponUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsEvolution);
        }

        [Test]
        public void IsEvolution_FalseForNewPassive()
        {
            var choice = new LevelUpChoice(ChoiceType.NewPassive, null, 1, "", "", null);
            Assert.IsFalse(choice.IsEvolution);
        }

        [Test]
        public void IsEvolution_FalseForPassiveUpgrade()
        {
            var choice = new LevelUpChoice(ChoiceType.PassiveUpgrade, null, 2, "", "", null);
            Assert.IsFalse(choice.IsEvolution);
        }

        #endregion

        #region ChoiceType enum 테스트

        [Test]
        public void ChoiceType_HasFiveValues()
        {
            var values = System.Enum.GetValues(typeof(ChoiceType));
            Assert.AreEqual(5, values.Length);
        }

        [Test]
        public void ChoiceType_ContainsNewWeapon()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ChoiceType), ChoiceType.NewWeapon));
        }

        [Test]
        public void ChoiceType_ContainsWeaponUpgrade()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ChoiceType), ChoiceType.WeaponUpgrade));
        }

        [Test]
        public void ChoiceType_ContainsNewPassive()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ChoiceType), ChoiceType.NewPassive));
        }

        [Test]
        public void ChoiceType_ContainsPassiveUpgrade()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ChoiceType), ChoiceType.PassiveUpgrade));
        }

        [Test]
        public void ChoiceType_ContainsWeaponEvolution()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ChoiceType), ChoiceType.WeaponEvolution));
        }

        #endregion
    }
}
