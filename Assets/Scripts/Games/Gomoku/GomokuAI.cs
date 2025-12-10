using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawzyPop.Games.Gomoku
{
    /// <summary>
    /// 五子棋 AI，使用 Minimax + Alpha-Beta 剪枝算法
    /// </summary>
    public class GomokuAI : MonoBehaviour
    {
        private int difficulty = 2; // 1=简单, 2=中等, 3=困难
        private int searchDepth = 2;
        private int boardSize = 15;

        // 评分权重
        private const int FIVE = 100000;      // 五连
        private const int LIVE_FOUR = 10000;  // 活四
        private const int RUSH_FOUR = 1000;   // 冲四
        private const int LIVE_THREE = 1000;  // 活三
        private const int SLEEP_THREE = 100;  // 眠三
        private const int LIVE_TWO = 100;     // 活二
        private const int SLEEP_TWO = 10;     // 眠二

        /// <summary>
        /// 初始化 AI
        /// </summary>
        public void Initialize(int difficultyLevel)
        {
            difficulty = Mathf.Clamp(difficultyLevel, 1, 3);
            
            // 根据难度设置搜索深度
            switch (difficulty)
            {
                case 1:
                    searchDepth = 1;
                    break;
                case 2:
                    searchDepth = 2;
                    break;
                case 3:
                    searchDepth = 3;
                    break;
            }

            Debug.Log($"[GomokuAI] Initialized with difficulty {difficulty}, depth {searchDepth}");
        }

        /// <summary>
        /// 获取最佳落子位置
        /// </summary>
        public Vector2Int GetBestMove(int[,] board, int aiPlayer)
        {
            boardSize = board.GetLength(0);

            // 如果是第一步，下在中心
            if (IsFirstMove(board))
            {
                return new Vector2Int(boardSize / 2, boardSize / 2);
            }

            // 获取候选位置（只考虑已有棋子周围的位置）
            List<Vector2Int> candidates = GetCandidateMoves(board);

            if (candidates.Count == 0)
            {
                return new Vector2Int(-1, -1);
            }

            // 简单难度：随机选择一个不错的位置
            if (difficulty == 1)
            {
                return GetSimpleMove(board, candidates, aiPlayer);
            }

            // 中等/困难难度：使用 Minimax
            Vector2Int bestMove = candidates[0];
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (var move in candidates)
            {
                board[move.x, move.y] = aiPlayer;
                int score = Minimax(board, searchDepth - 1, alpha, beta, false, aiPlayer);
                board[move.x, move.y] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                alpha = Mathf.Max(alpha, score);
            }

            Debug.Log($"[GomokuAI] Best move: ({bestMove.x}, {bestMove.y}), score: {bestScore}");
            return bestMove;
        }

        /// <summary>
        /// 简单难度的落子逻辑
        /// </summary>
        private Vector2Int GetSimpleMove(int[,] board, List<Vector2Int> candidates, int aiPlayer)
        {
            int humanPlayer = aiPlayer == 1 ? 2 : 1;

            // 检查是否能赢
            foreach (var move in candidates)
            {
                board[move.x, move.y] = aiPlayer;
                if (CheckWin(board, move.x, move.y, aiPlayer))
                {
                    board[move.x, move.y] = 0;
                    return move;
                }
                board[move.x, move.y] = 0;
            }

            // 检查是否需要防守
            foreach (var move in candidates)
            {
                board[move.x, move.y] = humanPlayer;
                if (CheckWin(board, move.x, move.y, humanPlayer))
                {
                    board[move.x, move.y] = 0;
                    return move;
                }
                board[move.x, move.y] = 0;
            }

            // 随机选择
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        /// <summary>
        /// Minimax 算法 + Alpha-Beta 剪枝
        /// </summary>
        private int Minimax(int[,] board, int depth, int alpha, int beta, bool isMaximizing, int aiPlayer)
        {
            int humanPlayer = aiPlayer == 1 ? 2 : 1;

            // 终止条件
            if (depth == 0)
            {
                return EvaluateBoard(board, aiPlayer);
            }

            List<Vector2Int> candidates = GetCandidateMoves(board);
            if (candidates.Count == 0)
            {
                return EvaluateBoard(board, aiPlayer);
            }

            if (isMaximizing)
            {
                int maxScore = int.MinValue;
                foreach (var move in candidates)
                {
                    board[move.x, move.y] = aiPlayer;

                    // 检查是否获胜
                    if (CheckWin(board, move.x, move.y, aiPlayer))
                    {
                        board[move.x, move.y] = 0;
                        return FIVE * 10;
                    }

                    int score = Minimax(board, depth - 1, alpha, beta, false, aiPlayer);
                    board[move.x, move.y] = 0;

                    maxScore = Mathf.Max(maxScore, score);
                    alpha = Mathf.Max(alpha, score);

                    if (beta <= alpha)
                        break; // Beta 剪枝
                }
                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;
                foreach (var move in candidates)
                {
                    board[move.x, move.y] = humanPlayer;

                    // 检查对手是否获胜
                    if (CheckWin(board, move.x, move.y, humanPlayer))
                    {
                        board[move.x, move.y] = 0;
                        return -FIVE * 10;
                    }

                    int score = Minimax(board, depth - 1, alpha, beta, true, aiPlayer);
                    board[move.x, move.y] = 0;

                    minScore = Mathf.Min(minScore, score);
                    beta = Mathf.Min(beta, score);

                    if (beta <= alpha)
                        break; // Alpha 剪枝
                }
                return minScore;
            }
        }

        /// <summary>
        /// 评估整个棋盘
        /// </summary>
        private int EvaluateBoard(int[,] board, int aiPlayer)
        {
            int humanPlayer = aiPlayer == 1 ? 2 : 1;
            int aiScore = 0;
            int humanScore = 0;

            // 评估所有位置
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (board[x, y] == aiPlayer)
                    {
                        aiScore += EvaluatePosition(board, x, y, aiPlayer);
                    }
                    else if (board[x, y] == humanPlayer)
                    {
                        humanScore += EvaluatePosition(board, x, y, humanPlayer);
                    }
                }
            }

            return aiScore - humanScore;
        }

        /// <summary>
        /// 评估单个位置的价值
        /// </summary>
        private int EvaluatePosition(int[,] board, int x, int y, int player)
        {
            int score = 0;

            // 四个方向
            int[][] directions = new int[][]
            {
                new int[] { 1, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 1 },
                new int[] { 1, -1 }
            };

            foreach (var dir in directions)
            {
                int count = 1;
                int blocked = 0;

                // 正方向
                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;

                    if (nx < 0 || nx >= boardSize || ny < 0 || ny >= boardSize)
                    {
                        blocked++;
                        break;
                    }

                    if (board[nx, ny] == player)
                        count++;
                    else if (board[nx, ny] != 0)
                    {
                        blocked++;
                        break;
                    }
                    else
                        break;
                }

                // 反方向
                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;

                    if (nx < 0 || nx >= boardSize || ny < 0 || ny >= boardSize)
                    {
                        blocked++;
                        break;
                    }

                    if (board[nx, ny] == player)
                        count++;
                    else if (board[nx, ny] != 0)
                    {
                        blocked++;
                        break;
                    }
                    else
                        break;
                }

                // 根据连子数和封堵情况评分
                score += GetPatternScore(count, blocked);
            }

            return score;
        }

        /// <summary>
        /// 根据棋型获取分数
        /// </summary>
        private int GetPatternScore(int count, int blocked)
        {
            if (blocked == 2) return 0; // 两端都被封

            switch (count)
            {
                case 5:
                    return FIVE;
                case 4:
                    return blocked == 0 ? LIVE_FOUR : RUSH_FOUR;
                case 3:
                    return blocked == 0 ? LIVE_THREE : SLEEP_THREE;
                case 2:
                    return blocked == 0 ? LIVE_TWO : SLEEP_TWO;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取候选落子位置
        /// </summary>
        private List<Vector2Int> GetCandidateMoves(int[,] board)
        {
            HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();
            int range = 2; // 搜索已有棋子周围 2 格范围

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (board[x, y] != 0)
                    {
                        // 添加周围的空位
                        for (int dx = -range; dx <= range; dx++)
                        {
                            for (int dy = -range; dy <= range; dy++)
                            {
                                int nx = x + dx;
                                int ny = y + dy;

                                if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize && board[nx, ny] == 0)
                                {
                                    candidates.Add(new Vector2Int(nx, ny));
                                }
                            }
                        }
                    }
                }
            }

            // 按照位置评分排序，优先考虑更有价值的位置
            List<Vector2Int> sortedCandidates = new List<Vector2Int>(candidates);
            
            // 限制候选数量以提高性能
            if (sortedCandidates.Count > 20)
            {
                sortedCandidates.Sort((a, b) => {
                    int scoreA = EvaluateMoveQuick(board, a.x, a.y);
                    int scoreB = EvaluateMoveQuick(board, b.x, b.y);
                    return scoreB.CompareTo(scoreA);
                });
                sortedCandidates = sortedCandidates.GetRange(0, 20);
            }

            return sortedCandidates;
        }

        /// <summary>
        /// 快速评估落子价值（用于排序）
        /// </summary>
        private int EvaluateMoveQuick(int[,] board, int x, int y)
        {
            int score = 0;

            // 检查周围棋子数量
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize && board[nx, ny] != 0)
                    {
                        score += 10;
                    }
                }
            }

            // 中心位置加分
            int center = boardSize / 2;
            int distToCenter = Mathf.Abs(x - center) + Mathf.Abs(y - center);
            score += (boardSize - distToCenter);

            return score;
        }

        /// <summary>
        /// 检查是否是第一步
        /// </summary>
        private bool IsFirstMove(int[,] board)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (board[x, y] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查是否获胜
        /// </summary>
        private bool CheckWin(int[,] board, int x, int y, int player)
        {
            int[][] directions = new int[][]
            {
                new int[] { 1, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 1 },
                new int[] { 1, -1 }
            };

            foreach (var dir in directions)
            {
                int count = 1;

                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize && board[nx, ny] == player)
                        count++;
                    else
                        break;
                }

                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;
                    if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize && board[nx, ny] == player)
                        count++;
                    else
                        break;
                }

                if (count >= 5)
                    return true;
            }

            return false;
        }
    }
}
