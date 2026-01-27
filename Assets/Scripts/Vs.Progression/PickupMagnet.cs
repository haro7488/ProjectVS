using UnityEngine;
using Vs.Utility;

namespace Vs.Progression
{
    /// <summary>
    /// 플레이어에 부착하여 주변 경험치 젬을 자동 수집.
    /// </summary>
    public class PickupMagnet : MonoBehaviour
    {
        [Header("마그넷 설정")] [SerializeField] private float _magnetRadius = Constants.ExpMagnetRadius;
        [SerializeField] private LayerMask _pickupLayer;

        [Header("Debug")] [SerializeField] private bool _debugMode;

        private readonly Collider[] _overlapResults = new Collider[32];

        private void FixedUpdate()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _magnetRadius,
                _overlapResults,
                _pickupLayer
            );

            for (int i = 0; i < count; i++)
            {
                var col = _overlapResults[i];
                if (col != null && col.TryGetComponent<ExpPickup>(out var pickup))
                {
                    pickup.SetTarget(transform);
                    pickup.StartCollecting();
                }
            }
        }

        /// <summary>
        /// 마그넷 반경 설정.
        /// </summary>
        public void SetRadius(float radius)
        {
            _magnetRadius = radius;
        }

        /// <summary>
        /// 마그넷 반경 증가.
        /// </summary>
        public void AddRadius(float amount)
        {
            _magnetRadius += amount;
        }

        /// <summary>
        /// 현재 마그넷 반경.
        /// </summary>
        public float GetRadius()
        {
            return _magnetRadius;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
            Gizmos.DrawSphere(transform.position, _magnetRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _magnetRadius);
        }
#endif
    }
}