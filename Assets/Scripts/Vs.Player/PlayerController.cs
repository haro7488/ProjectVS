using UnityEngine;

namespace Vs.Player
{
    /// <summary>
    /// 플레이어 이동 및 입력 처리.
    /// WASD/방향키로 8방향 이동 지원 (XZ 평면).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float _moveSpeed = 5f;

        private Rigidbody _rb;
        private Camera _mainCamera;
        private Vector3 _moveInput;
        private Vector3 _lastMoveDirection = Vector3.forward;

        /// <summary>
        /// 마지막 이동 방향 (무기 발사 방향 등에 사용).
        /// 이동하지 않을 때도 마지막 방향 유지.
        /// </summary>
        public Vector3 MoveDirection => _lastMoveDirection;

        /// <summary>
        /// 현재 위치.
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// 현재 이동 중인지 여부.
        /// </summary>
        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        /// <summary>
        /// 현재 이동 속도.
        /// </summary>
        public float MoveSpeed => _moveSpeed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void HandleInput()
        {
            // Legacy Input System (WASD + 방향키)
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // 카메라 기준 방향 계산 (XZ 평면)
            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;

            // Y축 제거 후 정규화 (XZ 평면에 투영)
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 카메라 기준으로 이동 방향 계산
            _moveInput = cameraForward * vertical + cameraRight * horizontal;

            // 대각선 이동 시 정규화
            if (_moveInput.sqrMagnitude > 1f)
            {
                _moveInput.Normalize();
            }

            // 이동 방향 저장 (이동 중일 때만 업데이트)
            if (IsMoving)
            {
                _lastMoveDirection = _moveInput.normalized;
            }
        }

        private void ApplyMovement()
        {
            Vector3 targetPosition = _rb.position + _moveInput * (_moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(targetPosition);
        }

        /// <summary>
        /// 이동 속도 설정 (PlayerStats에서 호출).
        /// </summary>
        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = Mathf.Max(0f, speed);
        }

        /// <summary>
        /// 외부에서 이동 입력 주입 (AI 또는 자동 플레이용).
        /// </summary>
        public void SetMoveInput(Vector3 input)
        {
            _moveInput = new Vector3(input.x, 0f, input.z);
            if (_moveInput.sqrMagnitude > 1f)
            {
                _moveInput.Normalize();
            }

            if (_moveInput.sqrMagnitude > 0.01f)
            {
                _lastMoveDirection = _moveInput.normalized;
            }
        }
    }
}