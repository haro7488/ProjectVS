using UnityEngine;

namespace Vs.Debug
{
    /// <summary>
    /// 디버그 탭 베이스 클래스.
    /// 각 기능별 탭은 이 클래스를 상속.
    /// </summary>
    public abstract class DebugTabBase : MonoBehaviour
    {
        [Header("Tab Info")]
        [SerializeField] protected string _tabName = "Tab";
        [SerializeField] protected Sprite _tabIcon;

        [Header("Content")]
        [SerializeField] protected Transform _contentRoot;
        [SerializeField] protected GameObject _buttonPrefab;

        public string TabName => _tabName;
        public Sprite TabIcon => _tabIcon;

        /// <summary>
        /// 탭이 활성화될 때 호출.
        /// </summary>
        public virtual void OnTabActivated()
        {
            gameObject.SetActive(true);
            RefreshContent();
        }

        /// <summary>
        /// 탭이 비활성화될 때 호출.
        /// </summary>
        public virtual void OnTabDeactivated()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 탭 컨텐츠를 새로고침.
        /// 탭 활성화 시 자동 호출.
        /// </summary>
        public abstract void RefreshContent();

        /// <summary>
        /// 컨텐츠 영역 클리어.
        /// </summary>
        protected void ClearContent()
        {
            if (_contentRoot == null) return;

            for (int i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(_contentRoot.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 액션 버튼 생성 헬퍼.
        /// </summary>
        protected GameObject CreateButton(string label, System.Action onClick)
        {
            if (_buttonPrefab == null || _contentRoot == null)
            {
                UnityEngine.Debug.LogWarning($"[{_tabName}] Button prefab or content root not set");
                return null;
            }

            var buttonGO = Instantiate(_buttonPrefab, _contentRoot);
            var button = buttonGO.GetComponent<UnityEngine.UI.Button>();
            var text = buttonGO.GetComponentInChildren<TMPro.TMP_Text>();

            if (text != null)
            {
                text.text = label;
            }

            if (button != null && onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            return buttonGO;
        }
    }
}
