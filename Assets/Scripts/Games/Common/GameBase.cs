using System;
using UnityEngine;

namespace PawzyPop.Games
{
    /// <summary>
    /// 游戏基类，提供 IGame 接口的默认实现
    /// </summary>
    public abstract class GameBase : MonoBehaviour, IGame
    {
        [Header("Game Info")]
        [SerializeField] protected string gameId;

        protected GameState currentState = GameState.NotStarted;
        protected int currentScore = 0;

        #region IGame Properties

        public string GameId => gameId;

        public GameState CurrentState => currentState;

        public int CurrentScore => currentScore;

        #endregion

        #region Events

        public event Action<GameState> OnStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<GameResult> OnGameEnded;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            Debug.Log($"[{gameId}] GameBase Awake");
        }

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }

        #endregion

        #region IGame Implementation

        public virtual void Initialize()
        {
            Debug.Log($"[{gameId}] Initialize");
            currentScore = 0;
            SetState(GameState.NotStarted);
        }

        public virtual void StartGame()
        {
            Debug.Log($"[{gameId}] StartGame");
            SetState(GameState.Playing);
        }

        public virtual void PauseGame()
        {
            if (currentState != GameState.Playing) return;

            Debug.Log($"[{gameId}] PauseGame");
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public virtual void ResumeGame()
        {
            if (currentState != GameState.Paused) return;

            Debug.Log($"[{gameId}] ResumeGame");
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }

        public virtual void RestartGame()
        {
            Debug.Log($"[{gameId}] RestartGame");
            Time.timeScale = 1f;
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
            Initialize();
            StartGame();
        }

        public virtual void EndGame(GameResult result)
        {
            Debug.Log($"[{gameId}] EndGame: {result}");
            Time.timeScale = 1f;
            SetState(GameState.GameOver);
            OnGameEnded?.Invoke(result);

            // 保存最高分
            SaveHighScore();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 设置游戏状态并触发事件
        /// </summary>
        protected void SetState(GameState newState)
        {
            if (currentState == newState) return;

            Debug.Log($"[{gameId}] State: {currentState} -> {newState}");
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        /// <summary>
        /// 增加分数
        /// </summary>
        protected void AddScore(int points)
        {
            if (points <= 0) return;

            currentScore += points;
            Debug.Log($"[{gameId}] Score: +{points} = {currentScore}");
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// 设置分数
        /// </summary>
        protected void SetScore(int score)
        {
            currentScore = Mathf.Max(0, score);
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// 保存最高分到存档
        /// </summary>
        protected virtual void SaveHighScore()
        {
            // 子类可以重写此方法实现具体的保存逻辑
            Debug.Log($"[{gameId}] SaveHighScore: {currentScore}");
        }

        #endregion
    }
}
