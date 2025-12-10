using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawzyPop.UI
{
    public class LosePopup : PopupBase
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private Button menuButton;

        [Header("Ad Reward")]
        [SerializeField] private int extraMoves = 5;

        protected override void Awake()
        {
            base.Awake();

            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
            if (watchAdButton != null)
                watchAdButton.onClick.AddListener(OnWatchAdClicked);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
        }

        public void Setup(int score, int targetScore)
        {
            if (titleText != null)
                titleText.text = "挑战失败";

            if (messageText != null)
                messageText.text = $"目标分数: {targetScore}";

            if (scoreText != null)
                scoreText.text = $"当前得分: {score}";
        }

        protected override void OnShowComplete()
        {
            base.OnShowComplete();

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayLose();
            }
        }

        private void OnRetryClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            Time.timeScale = 1f;
            
            if (Core.LevelLoader.Instance != null)
            {
                Core.LevelLoader.Instance.ReloadCurrentLevel();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }

        private void OnWatchAdClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            // TODO: 接入广告SDK
            // 模拟看广告成功
            Debug.Log($"[Ad] 模拟看广告，获得 {extraMoves} 步");
            
            Hide();
            Time.timeScale = 1f;

            // 给予额外步数
            // 这里需要通过事件或直接调用 GameManager
            // GameManager.Instance.AddExtraMoves(extraMoves);
        }

        private void OnMenuClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}
