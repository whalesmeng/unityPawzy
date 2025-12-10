using System;
using UnityEngine;
using PawzyPop.Core;
using PawzyPop.Data;

namespace PawzyPop.Games.Gomoku
{
    /// <summary>
    /// 五子棋游戏控制器
    /// </summary>
    public class GomokuGame : GameBase
    {
        public static GomokuGame Instance { get; private set; }

        [Header("Gomoku Settings")]
        [SerializeField] private int boardSize = 15;
        [SerializeField] private int aiDifficulty = 2; // 1-3

        [Header("References")]
        [SerializeField] private GomokuBoard board;
        [SerializeField] private GomokuAI ai;

        /// <summary>
        /// 当前玩家 (1=黑/玩家, 2=白/AI)
        /// </summary>
        public int CurrentPlayer { get; private set; } = 1;

        /// <summary>
        /// 获胜方 (0=未结束, 1=玩家, 2=AI, 3=平局)
        /// </summary>
        public int Winner { get; private set; } = 0;

        /// <summary>
        /// 玩家回合变化事件
        /// </summary>
        public event Action<int> OnPlayerChanged;

        /// <summary>
        /// 游戏胜利事件（参数：获胜方）
        /// </summary>
        public event Action<int> OnGameWin;

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gameId = "gomoku";
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
            Debug.Log($"[GomokuGame] Initialize: boardSize={boardSize}, aiDifficulty={aiDifficulty}");

            currentScore = 0;
            CurrentPlayer = 1; // 玩家先手（黑子）
            Winner = 0;

            // 初始化棋盘
            if (board == null)
            {
                board = FindObjectOfType<GomokuBoard>();
            }
            if (board != null)
            {
                board.Initialize(boardSize);
                board.OnCellClicked += OnCellClicked;
            }
            else
            {
                Debug.LogError("[GomokuGame] GomokuBoard not found!");
            }

            // 初始化 AI
            if (ai == null)
            {
                ai = FindObjectOfType<GomokuAI>();
            }
            if (ai == null)
            {
                // 创建 AI 组件
                ai = gameObject.AddComponent<GomokuAI>();
            }
            ai.Initialize(aiDifficulty);

            SetState(GameState.NotStarted);
            OnPlayerChanged?.Invoke(CurrentPlayer);
        }

        public override void StartGame()
        {
            Debug.Log("[GomokuGame] StartGame");
            SetState(GameState.Playing);
        }

        public override void RestartGame()
        {
            Debug.Log("[GomokuGame] RestartGame");
            Time.timeScale = 1f;

            // 取消订阅旧事件
            if (board != null)
            {
                board.OnCellClicked -= OnCellClicked;
            }

            // 重新初始化
            Initialize();
            StartGame();
        }

        public override void EndGame(GameResult result)
        {
            Debug.Log($"[GomokuGame] EndGame: {result}, Winner: {Winner}");
            base.EndGame(result);
            OnGameWin?.Invoke(Winner);
        }

        protected override void SaveHighScore()
        {
            if (SaveManager.Instance != null)
            {
                // 五子棋记录胜利次数而不是分数
                if (Winner == 1) // 玩家胜利
                {
                    SaveManager.Instance.RecordGameWin(gameId, currentScore);
                }
                else
                {
                    SaveManager.Instance.UpdateGameHighScore(gameId, 0);
                }
            }
        }

        #endregion

        #region Game Logic

        /// <summary>
        /// 棋盘格子点击回调
        /// </summary>
        private void OnCellClicked(int x, int y)
        {
            if (CurrentState != GameState.Playing) return;
            if (CurrentPlayer != 1) return; // 不是玩家回合

            Debug.Log($"[GomokuGame] Player clicked: ({x}, {y})");

            // 尝试落子
            if (board.PlacePiece(x, y, 1))
            {
                currentScore++; // 记录步数
                OnScoreChanged?.Invoke(currentScore);

                // 检查胜负
                if (CheckWin(x, y, 1))
                {
                    Winner = 1;
                    EndGame(GameResult.Win);
                    return;
                }

                // 检查平局
                if (board.IsBoardFull())
                {
                    Winner = 3;
                    EndGame(GameResult.Draw);
                    return;
                }

                // 切换到 AI 回合
                SwitchPlayer();
                StartCoroutine(AITurn());
            }
        }

        /// <summary>
        /// AI 回合
        /// </summary>
        private System.Collections.IEnumerator AITurn()
        {
            Debug.Log("[GomokuGame] AI thinking...");

            // 等待一小段时间，让玩家看到自己的落子
            yield return new WaitForSeconds(0.3f);

            if (CurrentState != GameState.Playing) yield break;

            // AI 计算最佳落子
            Vector2Int aiMove = ai.GetBestMove(board.GetBoardState(), 2);

            if (aiMove.x >= 0 && aiMove.y >= 0)
            {
                Debug.Log($"[GomokuGame] AI move: ({aiMove.x}, {aiMove.y})");

                if (board.PlacePiece(aiMove.x, aiMove.y, 2))
                {
                    // 检查 AI 胜负
                    if (CheckWin(aiMove.x, aiMove.y, 2))
                    {
                        Winner = 2;
                        EndGame(GameResult.Lose);
                        yield break;
                    }

                    // 检查平局
                    if (board.IsBoardFull())
                    {
                        Winner = 3;
                        EndGame(GameResult.Draw);
                        yield break;
                    }

                    // 切换回玩家回合
                    SwitchPlayer();
                }
            }
            else
            {
                Debug.LogError("[GomokuGame] AI failed to find a valid move!");
            }
        }

        /// <summary>
        /// 切换玩家
        /// </summary>
        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
            Debug.Log($"[GomokuGame] Switch to player: {CurrentPlayer}");
            OnPlayerChanged?.Invoke(CurrentPlayer);
        }

        /// <summary>
        /// 检查是否获胜
        /// </summary>
        private bool CheckWin(int x, int y, int player)
        {
            return board.CheckWin(x, y, player);
        }

        #endregion

        #region Scene Navigation

        public void ReturnToMainMenu()
        {
            Debug.Log("[GomokuGame] ReturnToMainMenu");
            Time.timeScale = 1f;

            if (board != null)
            {
                board.OnCellClicked -= OnCellClicked;
            }

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
            if (board != null)
            {
                board.OnCellClicked -= OnCellClicked;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
