using UnityEngine;

namespace Vs.Core
{
    /// <summary>
    /// 카메라가 타겟(플레이어)을 부드럽게 따라가는 컴포넌트.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")] [SerializeField] private Transform _target;

        [Header("Settings")] [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        [Header("Bounds (Optional)")] [SerializeField]
        private bool _useBounds;

        [SerializeField] private Vector2 _minBounds = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 _maxBounds = new Vector2(50f, 50f);

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;

            // 경계 제한 적용
            if (_useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minBounds.x, _maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, _minBounds.y, _maxBounds.y);
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
        }

        /// <summary>
        /// 경계 설정
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            _minBounds = min;
            _maxBounds = max;
            _useBounds = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_useBounds) return;

            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3(
                (_minBounds.x + _maxBounds.x) / 2f,
                (_minBounds.y + _maxBounds.y) / 2f,
                0f
            );
            Vector3 size = new Vector3(
                _maxBounds.x - _minBounds.x,
                _maxBounds.y - _minBounds.y,
                0f
            );
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}