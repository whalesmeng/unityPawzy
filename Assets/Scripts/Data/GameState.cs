namespace PawzyPop.Games
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// 游戏中
        /// </summary>
        Playing = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused = 2,

        /// <summary>
        /// 游戏结束
        /// </summary>
        GameOver = 3
    }
}
