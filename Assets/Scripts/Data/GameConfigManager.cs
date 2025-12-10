using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PawzyPop.Data
{
    /// <summary>
    /// 游戏配置管理器，负责加载和管理所有游戏配置
    /// </summary>
    public class GameConfigManager : MonoBehaviour
    {
        public static GameConfigManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private string configPath = "GameConfigs";

        private List<GameInfo> gameInfoList = new List<GameInfo>();
        private Dictionary<string, GameInfo> gameInfoDict = new Dictionary<string, GameInfo>();

        /// <summary>
        /// 获取所有游戏信息（已排序）
        /// </summary>
        public IReadOnlyList<GameInfo> AllGames => gameInfoList;

        /// <summary>
        /// 获取已解锁的游戏
        /// </summary>
        public IEnumerable<GameInfo> UnlockedGames => gameInfoList.Where(g => g.isUnlocked);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllConfigs();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 加载所有游戏配置
        /// </summary>
        private void LoadAllConfigs()
        {
            gameInfoList.Clear();
            gameInfoDict.Clear();

            // 从 Resources 加载所有 GameInfo
            GameInfo[] configs = Resources.LoadAll<GameInfo>(configPath);

            if (configs == null || configs.Length == 0)
            {
                Debug.LogWarning($"[GameConfigManager] 未找到游戏配置: Resources/{configPath}");
                return;
            }

            foreach (var config in configs)
            {
                if (config.IsValid())
                {
                    gameInfoList.Add(config);
                    gameInfoDict[config.gameId] = config;
                    Debug.Log($"[GameConfigManager] 加载配置: {config.gameName} ({config.gameId})");
                }
                else
                {
                    Debug.LogWarning($"[GameConfigManager] 无效配置: {config.name}");
                }
            }

            // 按 sortOrder 排序
            gameInfoList = gameInfoList.OrderBy(g => g.sortOrder).ToList();

            Debug.Log($"[GameConfigManager] 共加载 {gameInfoList.Count} 个游戏配置");
        }

        /// <summary>
        /// 根据 GameId 获取游戏信息
        /// </summary>
        public GameInfo GetGameInfo(string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogWarning("[GameConfigManager] GetGameInfo: gameId 为空");
                return null;
            }

            if (gameInfoDict.TryGetValue(gameId, out GameInfo info))
            {
                return info;
            }

            Debug.LogWarning($"[GameConfigManager] 未找到游戏: {gameId}");
            return null;
        }

        /// <summary>
        /// 检查游戏是否存在
        /// </summary>
        public bool HasGame(string gameId)
        {
            return gameInfoDict.ContainsKey(gameId);
        }

        /// <summary>
        /// 获取游戏数量
        /// </summary>
        public int GetGameCount()
        {
            return gameInfoList.Count;
        }

        /// <summary>
        /// 重新加载配置（用于热更新）
        /// </summary>
        public void ReloadConfigs()
        {
            Debug.Log("[GameConfigManager] 重新加载配置");
            LoadAllConfigs();
        }
    }
}
