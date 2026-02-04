namespace Vs.Tests.Utilities
{
    /// <summary>
    /// 테스트에서 사용하는 상수 정의
    /// </summary>
    public static class TestConstants
    {
        #region Timeouts

        /// <summary>
        /// 기본 테스트 타임아웃 (밀리초)
        /// </summary>
        public const int DefaultTimeoutMs = 5000;

        /// <summary>
        /// 긴 테스트 타임아웃 (밀리초)
        /// </summary>
        public const int LongTimeoutMs = 30000;

        /// <summary>
        /// 짧은 테스트 타임아웃 (밀리초)
        /// </summary>
        public const int ShortTimeoutMs = 1000;

        #endregion

        #region Tolerances

        /// <summary>
        /// float 비교 기본 허용 오차
        /// </summary>
        public const float DefaultFloatTolerance = 0.0001f;

        /// <summary>
        /// 위치 비교 기본 허용 오차
        /// </summary>
        public const float DefaultPositionTolerance = 0.01f;

        /// <summary>
        /// 시간 비교 기본 허용 오차
        /// </summary>
        public const float DefaultTimeTolerance = 0.1f;

        #endregion

        #region Test Data IDs

        /// <summary>
        /// 테스트용 무기 ID
        /// </summary>
        public const string TestWeaponId = "test_weapon";

        /// <summary>
        /// 테스트용 적 ID
        /// </summary>
        public const string TestEnemyId = "test_enemy";

        /// <summary>
        /// 테스트용 패시브 ID
        /// </summary>
        public const string TestPassiveId = "test_passive";

        /// <summary>
        /// 테스트용 캐릭터 ID
        /// </summary>
        public const string TestCharacterId = "test_character";

        /// <summary>
        /// 테스트용 스테이지 ID
        /// </summary>
        public const string TestStageId = "test_stage";

        #endregion

        #region Default Values

        /// <summary>
        /// 기본 플레이어 체력
        /// </summary>
        public const float DefaultPlayerHealth = 100f;

        /// <summary>
        /// 기본 플레이어 이동 속도
        /// </summary>
        public const float DefaultPlayerMoveSpeed = 5f;

        /// <summary>
        /// 기본 무기 대미지
        /// </summary>
        public const float DefaultWeaponDamage = 10f;

        /// <summary>
        /// 기본 무기 간격
        /// </summary>
        public const float DefaultWeaponInterval = 1f;

        /// <summary>
        /// 기본 적 체력
        /// </summary>
        public const float DefaultEnemyHealth = 100f;

        /// <summary>
        /// 기본 적 이동 속도
        /// </summary>
        public const float DefaultEnemyMoveSpeed = 2f;

        /// <summary>
        /// 기본 경험치
        /// </summary>
        public const int DefaultExpValue = 1;

        /// <summary>
        /// 기본 레벨업 경험치
        /// </summary>
        public const int DefaultLevelUpExp = 10;

        #endregion

        #region Game Constants (Mirror of Vs.Utility.Constants)

        /// <summary>
        /// 최대 무기 슬롯 수
        /// </summary>
        public const int MaxWeaponSlots = 6;

        /// <summary>
        /// 최대 패시브 슬롯 수
        /// </summary>
        public const int MaxPassiveSlots = 6;

        /// <summary>
        /// 최대 무기 레벨
        /// </summary>
        public const int MaxWeaponLevel = 8;

        /// <summary>
        /// 레벨업 선택지 수
        /// </summary>
        public const int LevelUpChoiceCount = 3;

        /// <summary>
        /// 무적 시간 (초)
        /// </summary>
        public const float InvincibilityDuration = 0.5f;

        #endregion

        #region Tags

        /// <summary>
        /// 플레이어 태그
        /// </summary>
        public const string TagPlayer = "Player";

        /// <summary>
        /// 적 태그
        /// </summary>
        public const string TagEnemy = "Enemy";

        /// <summary>
        /// 픽업 태그
        /// </summary>
        public const string TagPickup = "Pickup";

        #endregion

        #region Layers

        /// <summary>
        /// 플레이어 레이어
        /// </summary>
        public const int LayerPlayer = 8;

        /// <summary>
        /// 적 레이어
        /// </summary>
        public const int LayerEnemy = 9;

        /// <summary>
        /// 투사체 레이어
        /// </summary>
        public const int LayerProjectile = 10;

        /// <summary>
        /// 픽업 레이어
        /// </summary>
        public const int LayerPickup = 11;

        #endregion

        #region Test Categories

        /// <summary>
        /// 기본 기능 테스트 카테고리
        /// </summary>
        public const string CategoryBasic = "Basic";

        /// <summary>
        /// 무기 테스트 카테고리
        /// </summary>
        public const string CategoryWeapon = "Weapon";

        /// <summary>
        /// 진행 테스트 카테고리
        /// </summary>
        public const string CategoryProgression = "Progression";

        /// <summary>
        /// 전투 테스트 카테고리
        /// </summary>
        public const string CategoryCombat = "Combat";

        /// <summary>
        /// 난이도 테스트 카테고리
        /// </summary>
        public const string CategoryDifficulty = "Difficulty";

        /// <summary>
        /// 엣지 케이스 테스트 카테고리
        /// </summary>
        public const string CategoryEdgeCase = "EdgeCase";

        /// <summary>
        /// 통합 테스트 카테고리
        /// </summary>
        public const string CategoryIntegration = "Integration";

        #endregion
    }
}
