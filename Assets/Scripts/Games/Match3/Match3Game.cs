using System;
using UnityEngine;
using PawzyPop.Core;
using PawzyPop.Data;

namespace PawzyPop.Games.Match3
{
    /// <summary>
    /// 消消乐游戏控制器，实现 IGame 接口
    /// </summary>
    public class Match3Game : GameBase
    {
        public static Match3Game Instance { get; private set; }

        [Header("Match3 Settings")]
        [SerializeField] private int targetScore = 1000;
        [SerializeField] private int maxMoves = 20;

        [Header("References")]
        [SerializeField] private Board board;

        public int TargetScore => targetScore;
        public int MaxMoves => maxMoves;
        public int MovesLeft { get; private set; }

        /// <summary>
        /// 步数变化事件
        /// </summary>
        public event Action<int> OnMovesChanged;

        /// <summary>
        /// 游戏胜利事件
        /// </summary>
        public event Action OnGameWin;

        /// <summary>
        /// 游戏失败事件
        /// </summary>
        public event Action OnGameLose;

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gameId = "match3";
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
        }

        protected override void Start()
        {
            // 确保 SceneLoader 存在
            EnsureSceneLoader();
            
            base.Start();
        }

        private void EnsureSceneLoader()
        {
            if (SceneLoader.Instance == null)
            {
                GameObject loaderObj = new GameObject("SceneLoader");
                loaderObj.AddComponent<SceneLoader>();
            }
        }

        #region IGame Implementation

        public override void Initialize()
        {
            Debug.Log($"[Match3Game] Initialize: targetScore={targetScore}, maxMoves={maxMoves}");
            
            currentScore = 0;
            MovesLeft = maxMoves;
            
            OnScoreChanged?.Invoke(currentScore);
            OnMovesChanged?.Invoke(MovesLeft);
            
            SetState(GameState.NotStarted);

            // 初始化棋盘
            if (board != null)
            {
                board.InitializeBoard();
            }
            else
            {
                // 尝试查找 Board
                board = FindObjectOfType<Board>();
                if (board != null)
                {
                    board.InitializeBoard();
                }
                else
                {
                    Debug.LogError("[Match3Game] Board not found!");
                }
            }
        }

        public override void StartGame()
        {
            Debug.Log("[Match3Game] StartGame");
            SetState(GameState.Playing);
        }

        public override void RestartGame()
        {
            Debug.Log("[Match3Game] RestartGame");
            Time.timeScale = 1f;
            
            // 重新加载当前场景
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ReloadCurrentScene();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }

        public override void EndGame(GameResult result)
        {
            Debug.Log($"[Match3Game] EndGame: {result}, Score: {currentScore}");
            
            base.EndGame(result);

            if (result == GameResult.Win)
            {
                OnGameWin?.Invoke();
            }
            else if (result == GameResult.Lose)
            {
                OnGameLose?.Invoke();
            }
        }

        protected override void SaveHighScore()
        {
            if (SaveManager.Instance != null)
            {
                bool isNewHighScore = SaveManager.Instance.UpdateGameHighScore(gameId, currentScore);
                if (isNewHighScore)
                {
                    Debug.Log($"[Match3Game] 新高分: {currentScore}");
                }
            }
        }

        #endregion

        #region Game Logic

        /// <summary>
        /// 增加分数（供外部调用）
        /// </summary>
        public new void AddScore(int points)
        {
            if (CurrentState != GameState.Playing) return;
            
            base.AddScore(points);
            
            // 检查是否达到目标分数
            if (currentScore >= targetScore)
            {
                EndGame(GameResult.Win);
            }
        }

        /// <summary>
        /// 使用一步
        /// </summary>
        public void UseMove()
        {
            if (CurrentState != GameState.Playing) return;
            
            MovesLeft--;
            Debug.Log($"[Match3Game] UseMove: {MovesLeft} remaining");
            OnMovesChanged?.Invoke(MovesLeft);

            // 检查是否用完步数
            if (MovesLeft <= 0 && currentScore < targetScore)
            {
                EndGame(GameResult.Lose);
            }
        }

        /// <summary>
        /// 检查是否可以处理输入
        /// </summary>
        public bool CanProcessInput()
        {
            return CurrentState == GameState.Playing;
        }

        /// <summary>
        /// 设置为处理中状态（消除动画播放时）
        /// </summary>
        public void SetProcessing(bool isProcessing)
        {
            // 保持 Playing 状态，但通过其他方式控制输入
            // 这里可以添加一个额外的标志位
        }

        /// <summary>
        /// 加载关卡配置
        /// </summary>
        public void LoadLevel(int levelId, int moves, int target)
        {
            Debug.Log($"[Match3Game] LoadLevel: level={levelId}, moves={moves}, target={target}");
            maxMoves = moves;
            targetScore = target;
            Initialize();
            StartGame();
        }

        #endregion

        #region Scene Navigation

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[Match3Game] ReturnToMainMenu");
            Time.timeScale = 1f;
            
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        #endregion

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
