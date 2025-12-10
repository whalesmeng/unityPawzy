using UnityEngine;

namespace PawzyPop.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Popups")]
        [SerializeField] private WinPopup winPopup;
        [SerializeField] private LosePopup losePopup;
        [SerializeField] private PausePopup pausePopup;

        [Header("HUD")]
        [SerializeField] private GameUI gameUI;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 订阅游戏状态变化
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(Core.GameState state)
        {
            switch (state)
            {
                case Core.GameState.Win:
                    ShowWinPopup();
                    break;
                case Core.GameState.GameOver:
                    ShowLosePopup();
                    break;
            }
        }

        public void ShowWinPopup()
        {
            if (winPopup == null) return;

            int level = 1;
            int score = 0;
            int stars = 0;

            if (Core.LevelLoader.Instance != null)
            {
                level = Core.LevelLoader.Instance.CurrentLevel?.levelId ?? 1;
                stars = Core.LevelLoader.Instance.GetStarRating(Core.GameManager.Instance.Score);
            }

            if (Core.GameManager.Instance != null)
            {
                score = Core.GameManager.Instance.Score;
            }

            winPopup.Setup(level, score, stars);
            winPopup.Show();

            // 播放胜利BGM
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlayWinBGM();
            }
        }

        public void ShowLosePopup()
        {
            if (losePopup == null) return;

            int score = 0;
            int targetScore = 1000;

            if (Core.GameManager.Instance != null)
            {
                score = Core.GameManager.Instance.Score;
            }

            if (Core.LevelLoader.Instance != null && Core.LevelLoader.Instance.CurrentLevel != null)
            {
                targetScore = Core.LevelLoader.Instance.CurrentLevel.targetValue;
            }

            losePopup.Setup(score, targetScore);
            losePopup.Show();
        }

        public void ShowPausePopup()
        {
            if (pausePopup != null)
            {
                pausePopup.Show();
            }
        }

        public void HidePausePopup()
        {
            if (pausePopup != null)
            {
                pausePopup.Hide();
            }
        }

        public void HideAllPopups()
        {
            if (winPopup != null && winPopup.gameObject.activeSelf)
                winPopup.Hide();
            if (losePopup != null && losePopup.gameObject.activeSelf)
                losePopup.Hide();
            if (pausePopup != null && pausePopup.gameObject.activeSelf)
                pausePopup.Hide();
        }
    }
}
