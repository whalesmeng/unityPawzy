using System;

namespace PawzyPop.Games
{
    /// <summary>
    /// 游戏通用接口，所有小游戏必须实现此接口
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// 游戏唯一标识
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        GameState CurrentState { get; }

        /// <summary>
        /// 当前分数
        /// </summary>
        int CurrentScore { get; }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        void Initialize();

        /// <summary>
        /// 开始游戏
        /// </summary>
        void StartGame();

        /// <summary>
        /// 暂停游戏
        /// </summary>
        void PauseGame();

        /// <summary>
        /// 恢复游戏
        /// </summary>
        void ResumeGame();

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        void RestartGame();

        /// <summary>
        /// 结束游戏
        /// </summary>
        /// <param name="result">游戏结果</param>
        void EndGame(GameResult result);

        /// <summary>
        /// 游戏状态变化事件
        /// </summary>
        event Action<GameState> OnStateChanged;

        /// <summary>
        /// 分数变化事件
        /// </summary>
        event Action<int> OnScoreChanged;

        /// <summary>
        /// 游戏结束事件
        /// </summary>
        event Action<GameResult> OnGameEnded;
    }
}
