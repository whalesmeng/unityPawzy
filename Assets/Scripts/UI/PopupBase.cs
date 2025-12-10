using UnityEngine;
using System.Collections;

namespace PawzyPop.UI
{
    public abstract class PopupBase : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] protected float animationDuration = 0.3f;
        [SerializeField] protected AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] protected AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("References")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected RectTransform contentPanel;

        protected bool isAnimating;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            // 初始隐藏
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            if (isAnimating) return;

            gameObject.SetActive(true);
            StartCoroutine(ShowAnimation());
        }

        public virtual void Hide()
        {
            if (isAnimating) return;

            StartCoroutine(HideAnimation());
        }

        protected virtual IEnumerator ShowAnimation()
        {
            isAnimating = true;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
            }

            if (contentPanel != null)
            {
                contentPanel.localScale = Vector3.zero;
            }

            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / animationDuration;
                float curveValue = showCurve.Evaluate(t);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = curveValue;
                }

                if (contentPanel != null)
                {
                    contentPanel.localScale = Vector3.one * curveValue;
                }

                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
            }

            if (contentPanel != null)
            {
                contentPanel.localScale = Vector3.one;
            }

            isAnimating = false;
            OnShowComplete();
        }

        protected virtual IEnumerator HideAnimation()
        {
            isAnimating = true;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
            }

            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / animationDuration;
                float curveValue = hideCurve.Evaluate(t);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = curveValue;
                }

                if (contentPanel != null)
                {
                    contentPanel.localScale = Vector3.one * curveValue;
                }

                yield return null;
            }

            isAnimating = false;
            gameObject.SetActive(false);
            OnHideComplete();
        }

        protected virtual void OnShowComplete() { }
        protected virtual void OnHideComplete() { }

        // 点击背景关闭（可选）
        public virtual void OnBackgroundClicked()
        {
            Hide();
        }
    }
}
