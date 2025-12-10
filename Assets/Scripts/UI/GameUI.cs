using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawzyPop.Core;
using PawzyPop.Games;
using PawzyPop.Games.Match3;

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
        [SerializeField] private Button backToMenuButton;

        // 使用新的 Match3Game 还是旧的 GameManager
        private bool useMatch3Game = false;

        private void Start()
        {
            // 优先使用新的 Match3Game
            if (Match3Game.Instance != null)
            {
                useMatch3Game = true;
                SubscribeToMatch3Game();
                Debug.Log("[GameUI] Using Match3Game");
            }
            // 回退到旧的 GameManager
            else if (GameManager.Instance != null)
            {
                SubscribeToGameManager();
                Debug.Log("[GameUI] Using GameManager (legacy)");
            }

            // 绑定按钮
            BindButtons();

            // 隐藏所有面板
            HideAllPanels();
        }

        private void SubscribeToMatch3Game()
        {
            Match3Game.Instance.OnScoreChanged += UpdateScore;
            Match3Game.Instance.OnMovesChanged += UpdateMoves;
            Match3Game.Instance.OnStateChanged += OnMatch3StateChanged;
            
            // 设置目标分数
            SetTarget(Match3Game.Instance.TargetScore);
        }

        private void SubscribeToGameManager()
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnMovesChanged += UpdateMoves;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        private void BindButtons()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (backToMenuButton != null)
                backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        }

        private void OnDestroy()
        {
            if (useMatch3Game && Match3Game.Instance != null)
            {
                Match3Game.Instance.OnScoreChanged -= UpdateScore;
                Match3Game.Instance.OnMovesChanged -= UpdateMoves;
                Match3Game.Instance.OnStateChanged -= OnMatch3StateChanged;
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= UpdateScore;
                GameManager.Instance.OnMovesChanged -= UpdateMoves;
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
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

        // 新的 Match3Game 状态变化处理
        private void OnMatch3StateChanged(Games.GameState state)
        {
            HideAllPanels();

            switch (state)
            {
                case Games.GameState.GameOver:
                    // 需要根据游戏结果判断显示哪个面板
                    // 这里先检查分数是否达标
                    if (Match3Game.Instance != null && 
                        Match3Game.Instance.CurrentScore >= Match3Game.Instance.TargetScore)
                    {
                        ShowWinPanel();
                    }
                    else
                    {
                        ShowLosePanel();
                    }
                    break;
                case Games.GameState.Paused:
                    ShowPausePanel();
                    break;
            }
        }

        // 旧的 GameManager 状态变化处理
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

        private void ShowPausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        private void OnPauseClicked()
        {
            if (useMatch3Game && Match3Game.Instance != null)
            {
                Match3Game.Instance.PauseGame();
            }
            else
            {
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                    Time.timeScale = 0f;
                }
            }
        }

        private void OnResumeClicked()
        {
            if (useMatch3Game && Match3Game.Instance != null)
            {
                Match3Game.Instance.ResumeGame();
                HideAllPanels();
            }
            else
            {
                if (pausePanel != null)
                {
                    pausePanel.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }

        private void OnRestartClicked()
        {
            if (useMatch3Game && Match3Game.Instance != null)
            {
                Match3Game.Instance.RestartGame();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }

        private void OnBackToMenuClicked()
        {
            Debug.Log("[GameUI] OnBackToMenuClicked");
            
            if (useMatch3Game && Match3Game.Instance != null)
            {
                Match3Game.Instance.ReturnToMainMenu();
            }
            else if (SceneLoader.Instance != null)
            {
                Time.timeScale = 1f;
                SceneLoader.Instance.LoadMainMenu();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
