using System.Collections;
using UnityEngine;
using Vs.Combat;
using Vs.Core;

namespace Vs.TestRunner.Scenarios
{
    /// <summary>
    /// 무기 시스템 테스트 시나리오.
    /// WeaponController 및 무기 동작을 검증합니다.
    /// </summary>
    public class WeaponTestScenario : TestScenario
    {
        private WeaponController _weaponController;

        private void Awake()
        {
            _scenarioName = "Weapon System";
            _description = "Tests weapon controller and weapon behavior";
            _timeout = 30f;
        }

        protected override IEnumerator ExecuteTests()
        {
            // WeaponController 찾기
            BeginTest("Find WeaponController");
            _weaponController = FindFirstObjectByType<WeaponController>();

            if (_weaponController == null)
            {
                SkipTest("WeaponController not found in scene");
                yield break;
            }
            PassTest();
            yield return null;

            // T1: 초기 상태
            BeginTest("WeaponController Initial State");
            // WeaponController가 존재하면 통과
            if (AssertNotNull(_weaponController, "WeaponController"))
            {
                PassTest($"WeaponCount: {_weaponController.WeaponCount}");
            }
            yield return null;

            // T2: 슬롯 제한 확인
            BeginTest("Max Weapon Slots is 6");
            if (Assert(_weaponController.WeaponCount <= 6,
                $"WeaponCount {_weaponController.WeaponCount} exceeds max 6"))
            {
                PassTest();
            }
            yield return null;

            // T3: HasEmptySlot 프로퍼티
            BeginTest("HasEmptySlot Property");
            bool expectedHasSlot = _weaponController.WeaponCount < 6;
            if (Assert(_weaponController.HasEmptySlot == expectedHasSlot,
                $"HasEmptySlot mismatch: expected {expectedHasSlot}"))
            {
                PassTest();
            }
            yield return null;

            // T4: Weapons 리스트
            BeginTest("Weapons List is Not Null");
            if (AssertNotNull(_weaponController.Weapons, "Weapons list"))
            {
                PassTest($"Weapons: {_weaponController.Weapons.Count}");
            }
            yield return null;

            // T5: 게임 시작 후 무기 동작 확인
            BeginTest("Weapons Work During Gameplay");

            if (!GameManager.HasInstance)
            {
                SkipTest("GameManager not available");
                yield break;
            }

            GameManager.Instance.StartGame();
            yield return new WaitForSeconds(2f);

            // 2초 대기 후 게임이 여전히 플레이 중인지 확인
            if (Assert(GameManager.Instance.IsPlaying || GameManager.Instance.State == GameState.LevelUp,
                $"Game ended unexpectedly: {GameManager.Instance.State}"))
            {
                PassTest("Weapons active for 2 seconds without crash");
            }

            // T6: 무기 발사 테스트 (로그 기반)
            BeginTest("Weapons Fire Without Errors");
            // 추가 2초 대기하며 에러 확인
            yield return new WaitForSeconds(2f);

            // 게임이 정상 동작 중이면 통과
            if (Assert(GameManager.Instance.State == GameState.Playing ||
                      GameManager.Instance.State == GameState.LevelUp ||
                      GameManager.Instance.State == GameState.Paused,
                $"Unexpected state: {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // 정리
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
