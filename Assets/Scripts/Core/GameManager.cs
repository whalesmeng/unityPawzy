using UnityEngine;
using System;

namespace PawzyPop.Core
{
    public enum GameState
    {
        WaitingInput,
        Processing,
        GameOver,
        Win
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int targetScore = 1000;
        [SerializeField] private int maxMoves = 20;

        public GameState CurrentState { get; private set; } = GameState.WaitingInput;
        public int Score { get; private set; }
        public int MovesLeft { get; private set; }

        public event Action<int> OnScoreChanged;
        public event Action<int> OnMovesChanged;
        public event Action<GameState> OnGameStateChanged;

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
            InitializeGame();
        }

        public void InitializeGame()
        {
            Score = 0;
            MovesLeft = maxMoves;
            CurrentState = GameState.WaitingInput;
            
            OnScoreChanged?.Invoke(Score);
            OnMovesChanged?.Invoke(MovesLeft);
            OnGameStateChanged?.Invoke(CurrentState);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(CurrentState);
        }

        public void AddScore(int points)
        {
            Score += points;
            OnScoreChanged?.Invoke(Score);

            if (Score >= targetScore && CurrentState != GameState.Win)
            {
                SetState(GameState.Win);
            }
        }

        public void UseMove()
        {
            MovesLeft--;
            OnMovesChanged?.Invoke(MovesLeft);

            if (MovesLeft <= 0 && Score < targetScore)
            {
                SetState(GameState.GameOver);
            }
        }

        public bool CanProcessInput()
        {
            return CurrentState == GameState.WaitingInput;
        }

        public void LoadLevel(int levelId, int moves, int target)
        {
            maxMoves = moves;
            targetScore = target;
            InitializeGame();
        }
    }
}
