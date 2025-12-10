using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawzyPop.Core;
using PawzyPop.Games;

namespace PawzyPop.Games.Gomoku
{
    /// <summary>
    /// 五子棋游戏UI
    /// </summary>
    public class GomokuUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Panels")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject drawPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Win Panel")]
        [SerializeField] private TextMeshProUGUI winTitleText;
        [SerializeField] private TextMeshProUGUI winMessageText;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button resumeButton;

        private void Start()
        {
            // 订阅事件
            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.OnPlayerChanged += UpdateTurnDisplay;
                GomokuGame.Instance.OnStateChanged += OnGameStateChanged;
                GomokuGame.Instance.OnGameWin += OnGameWin;
                
                Debug.Log("[GomokuUI] Subscribed to GomokuGame events");
            }
            else
            {
                Debug.LogWarning("[GomokuUI] GomokuGame.Instance is null");
            }

            // 绑定按钮
            BindButtons();

            // 隐藏所有面板
            HideAllPanels();

            // 初始化显示
            UpdateTurnDisplay(1);
        }

        private void BindButtons()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (backToMenuButton != null)
                backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
        }

        private void OnDestroy()
        {
            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.OnPlayerChanged -= UpdateTurnDisplay;
                GomokuGame.Instance.OnStateChanged -= OnGameStateChanged;
                GomokuGame.Instance.OnGameWin -= OnGameWin;
            }
        }

        /// <summary>
        /// 更新回合显示
        /// </summary>
        private void UpdateTurnDisplay(int player)
        {
            if (turnText != null)
            {
                string playerName = player == 1 ? "你的回合 (黑)" : "AI思考中 (白)";
                turnText.text = playerName;
            }

            if (statusText != null)
            {
                statusText.text = player == 1 ? "请落子" : "请稍候...";
            }
        }

        /// <summary>
        /// 游戏状态变化
        /// </summary>
        private void OnGameStateChanged(GameState state)
        {
            Debug.Log($"[GomokuUI] State changed: {state}");

            if (state == GameState.Paused)
            {
                ShowPausePanel();
            }
            else if (state == GameState.Playing)
            {
                HideAllPanels();
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        private void OnGameWin(int winner)
        {
            Debug.Log($"[GomokuUI] Game ended, winner: {winner}");
            HideAllPanels();

            switch (winner)
            {
                case 1: // 玩家胜利
                    ShowWinPanel("恭喜获胜!", "你击败了AI!");
                    break;
                case 2: // AI 胜利
                    ShowLosePanel();
                    break;
                case 3: // 平局
                    ShowDrawPanel();
                    break;
            }
        }

        private void HideAllPanels()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (drawPanel != null) drawPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void ShowWinPanel(string title, string message)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);

                if (winTitleText != null)
                    winTitleText.text = title;
                if (winMessageText != null)
                    winMessageText.text = message;
            }
        }

        private void ShowLosePanel()
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
            else if (winPanel != null)
            {
                // 复用 winPanel 显示失败信息
                ShowWinPanel("游戏结束", "AI 获胜了，再接再厉!");
            }
        }

        private void ShowDrawPanel()
        {
            if (drawPanel != null)
            {
                drawPanel.SetActive(true);
            }
            else if (winPanel != null)
            {
                ShowWinPanel("平局", "棋逢对手!");
            }
        }

        private void ShowPausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        #region Button Handlers

        private void OnPauseClicked()
        {
            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.PauseGame();
            }
        }

        private void OnResumeClicked()
        {
            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.ResumeGame();
            }
            HideAllPanels();
        }

        private void OnRestartClicked()
        {
            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.RestartGame();
            }
        }

        private void OnBackToMenuClicked()
        {
            Debug.Log("[GomokuUI] Back to menu clicked");

            if (GomokuGame.Instance != null)
            {
                GomokuGame.Instance.ReturnToMainMenu();
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

        #endregion
    }
}
