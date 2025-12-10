using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace PawzyPop.UI
{
    public class WinPopup : PopupBase
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI levelText;

        [Header("Stars")]
        [SerializeField] private Image[] starImages;
        [SerializeField] private Sprite starFilledSprite;
        [SerializeField] private Sprite starEmptySprite;

        [Header("Buttons")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;

        [Header("Animation")]
        [SerializeField] private float starAnimationDelay = 0.3f;

        private int currentStars;

        protected override void Awake()
        {
            base.Awake();

            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
        }

        public void Setup(int level, int score, int stars)
        {
            currentStars = stars;

            if (levelText != null)
                levelText.text = $"第 {level} 关";

            if (scoreText != null)
                scoreText.text = $"得分: {score}";

            // 初始化星星为空
            if (starImages != null)
            {
                foreach (var star in starImages)
                {
                    if (star != null && starEmptySprite != null)
                    {
                        star.sprite = starEmptySprite;
                        star.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        protected override void OnShowComplete()
        {
            base.OnShowComplete();
            StartCoroutine(AnimateStars());
        }

        private IEnumerator AnimateStars()
        {
            if (starImages == null) yield break;

            for (int i = 0; i < currentStars && i < starImages.Length; i++)
            {
                yield return new WaitForSecondsRealtime(starAnimationDelay);

                if (starImages[i] != null)
                {
                    // 播放星星动画
                    StartCoroutine(AnimateSingleStar(starImages[i]));

                    // 播放音效
                    if (Audio.AudioManager.Instance != null)
                    {
                        Audio.AudioManager.Instance.PlayStar();
                    }
                }
            }
        }

        private IEnumerator AnimateSingleStar(Image star)
        {
            if (starFilledSprite != null)
            {
                star.sprite = starFilledSprite;
            }

            // 缩放动画
            float duration = 0.2f;
            float elapsed = 0f;

            star.transform.localScale = Vector3.zero;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
                star.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            star.transform.localScale = Vector3.one;
        }

        private void OnNextLevelClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            
            if (Core.LevelLoader.Instance != null)
            {
                Core.LevelLoader.Instance.LoadNextLevel();
            }
        }

        private void OnRetryClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            
            if (Core.LevelLoader.Instance != null)
            {
                Core.LevelLoader.Instance.ReloadCurrentLevel();
            }
        }

        private void OnMenuClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}
