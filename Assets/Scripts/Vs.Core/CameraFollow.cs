using UnityEngine;

namespace Vs.Core
{
    /// <summary>
    /// 쿼터뷰 카메라가 타겟(플레이어)을 부드럽게 따라가는 컴포넌트.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")] [SerializeField] private Transform _target;

        [Header("Settings")] [SerializeField] private float _smoothSpeed = 5f;

        [Header("Isometric View")] [SerializeField]
        private Vector3 _offset = new Vector3(10f, 15f, -10f);

        [SerializeField] private Vector3 _rotation = new Vector3(45f, -45f, 0f);

        [Header("Bounds (Optional)")] [SerializeField]
        private bool _useBounds;

        [SerializeField] private Vector2 _minBounds = new Vector2(-50f, -50f); // XZ 평면
        [SerializeField] private Vector2 _maxBounds = new Vector2(50f, 50f); // XZ 평면

        private void Awake()
        {
            // 카메라 회전 설정
            transform.rotation = Quaternion.Euler(_rotation);
        }

        private void Start()
        {
            // 타겟이 없으면 Player 태그로 자동 검색
            if (_target == null)
            {
                var player = GameObject.FindGameObjectWithTag(Vs.Utility.Constants.TagPlayer);
                if (player != null)
                {
                    _target = player.transform;
                    SnapToTarget();
                }
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;

            // 경계 제한 적용 (XZ 평면)
            if (_useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minBounds.x + _offset.x, _maxBounds.x + _offset.x);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, _minBounds.y + _offset.z, _maxBounds.y + _offset.z);
            }

            // 부드러운 이동
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                _smoothSpeed * Time.deltaTime
            );

            transform.position = smoothedPosition;
        }

        /// <summary>
        /// 타겟 설정 (런타임에서 플레이어 참조)
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// 즉시 타겟 위치로 이동 (씬 시작 시)
        /// </summary>
        public void SnapToTarget()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;
            transform.rotation = Quaternion.Euler(_rotation);
        }

        /// <summary>
        /// 경계 설정 (XZ 평면)
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            _minBounds = min;
            _maxBounds = max;
            _useBounds = true;
        }

        /// <summary>
        /// 카메라 오프셋 설정
        /// </summary>
        public void SetOffset(Vector3 offset)
        {
            _offset = offset;
        }

        /// <summary>
        /// 카메라 회전 설정
        /// </summary>
        public void SetRotation(Vector3 rotation)
        {
            _rotation = rotation;
            transform.rotation = Quaternion.Euler(_rotation);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_useBounds) return;

            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3(
                (_minBounds.x + _maxBounds.x) / 2f,
                0f,
                (_minBounds.y + _maxBounds.y) / 2f
            );
            Vector3 size = new Vector3(
                _maxBounds.x - _minBounds.x,
                0.1f,
                _maxBounds.y - _minBounds.y
            );
            Gizmos.DrawWireCube(center, size);
        }

        private void OnValidate()
        {
            // 에디터에서 회전값 변경 시 미리보기
            if (!Application.isPlaying)
            {
                transform.rotation = Quaternion.Euler(_rotation);
            }
        }
#endif
    }
}