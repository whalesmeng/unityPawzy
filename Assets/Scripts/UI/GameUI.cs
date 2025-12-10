using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawzyPop.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI targetText;

        [Header("Panels")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button resumeButton;

        private void Start()
        {
            // 订阅事件
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnScoreChanged += UpdateScore;
                Core.GameManager.Instance.OnMovesChanged += UpdateMoves;
                Core.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }

            // 绑定按钮
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);

            // 隐藏所有面板
            HideAllPanels();
        }

        private void OnDestroy()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnScoreChanged -= UpdateScore;
                Core.GameManager.Instance.OnMovesChanged -= UpdateMoves;
                Core.GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"分数: {score}";
            }
        }

        private void UpdateMoves(int moves)
        {
            if (movesText != null)
            {
                movesText.text = $"步数: {moves}";
            }
        }

        public void SetTarget(int target)
        {
            if (targetText != null)
            {
                targetText.text = $"目标: {target}";
            }
        }

        private void OnGameStateChanged(Core.GameState state)
        {
            HideAllPanels();

            switch (state)
            {
                case Core.GameState.Win:
                    ShowWinPanel();
                    break;
                case Core.GameState.GameOver:
                    ShowLosePanel();
                    break;
            }
        }

        private void HideAllPanels()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void ShowWinPanel()
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }

        private void ShowLosePanel()
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
        }

        private void OnPauseClicked()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        private void OnResumeClicked()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        private void OnRestartClicked()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
}
