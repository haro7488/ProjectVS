using UnityEngine;

namespace Vs.Player
{
    /// <summary>
    /// 플레이어 이동 및 입력 처리.
    /// WASD/방향키로 8방향 이동 지원.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float _moveSpeed = 5f;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.right;

        /// <summary>
        /// 마지막 이동 방향 (무기 발사 방향 등에 사용).
        /// 이동하지 않을 때도 마지막 방향 유지.
        /// </summary>
        public Vector2 MoveDirection => _lastMoveDirection;

        /// <summary>
        /// 현재 위치.
        /// </summary>
        public Vector2 Position => transform.position;

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
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
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

            _moveInput = new Vector2(horizontal, vertical);

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
            Vector2 targetPosition = _rb.position + _moveInput * (_moveSpeed * Time.fixedDeltaTime);
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
        public void SetMoveInput(Vector2 input)
        {
            _moveInput = input;
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