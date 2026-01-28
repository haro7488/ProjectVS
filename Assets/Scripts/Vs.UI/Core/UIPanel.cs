using UnityEngine;

namespace Vs.UI
{
    /// <summary>
    /// UI 패널 베이스 클래스.
    /// 모든 UI 패널은 이 클래스를 상속받아 일관된 Show/Hide 동작 제공.
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content;

        public bool IsVisible { get; private set; }

        protected GameObject Content => _content != null ? _content : gameObject;

        protected virtual void Awake()
        {
            if (_content == null)
            {
                _content = gameObject;
            }
        }

        public virtual void Show()
        {
            if (IsVisible) return;

            IsVisible = true;
            Content.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            if (!IsVisible) return;

            IsVisible = false;
            Content.SetActive(false);
            OnHide();
        }

        public void Toggle()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// 패널이 표시될 때 호출. 오버라이드하여 초기화 로직 추가.
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// 패널이 숨겨질 때 호출. 오버라이드하여 정리 로직 추가.
        /// </summary>
        protected virtual void OnHide() { }
    }
}
