using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PawzyPop.Core;
using PawzyPop.Data;

namespace PawzyPop.UI
{
    /// <summary>
    /// 主菜单UI控制器
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform gameListContainer;
        [SerializeField] private GameObject gameCardPrefab;
        [SerializeField] private Text titleText;

        [Header("Settings")]
        [SerializeField] private string titleString = "小游戏集合";

        private List<GameCard> gameCards = new List<GameCard>();

        private void Start()
        {
            Debug.Log("[MainMenuUI] Start");
            InitializeUI();
            LoadGameList();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            if (titleText != null)
            {
                titleText.text = titleString;
            }

            // 确保 SceneLoader 存在
            if (SceneLoader.Instance == null)
            {
                GameObject loaderObj = new GameObject("SceneLoader");
                loaderObj.AddComponent<SceneLoader>();
            }

            // 确保 GameConfigManager 存在
            if (GameConfigManager.Instance == null)
            {
                GameObject configObj = new GameObject("GameConfigManager");
                configObj.AddComponent<GameConfigManager>();
            }
        }

        /// <summary>
        /// 加载游戏列表
        /// </summary>
        private void LoadGameList()
        {
            if (gameListContainer == null)
            {
                Debug.LogError("[MainMenuUI] gameListContainer 未设置");
                return;
            }

            if (gameCardPrefab == null)
            {
                Debug.LogError("[MainMenuUI] gameCardPrefab 未设置");
                return;
            }

            // 清空现有卡片
            ClearGameCards();

            // 获取所有游戏配置
            var games = GameConfigManager.Instance?.AllGames;
            if (games == null || games.Count == 0)
            {
                Debug.LogWarning("[MainMenuUI] 没有找到游戏配置");
                return;
            }

            Debug.Log($"[MainMenuUI] 加载 {games.Count} 个游戏");

            // 创建游戏卡片
            foreach (var gameInfo in games)
            {
                CreateGameCard(gameInfo);
            }
        }

        /// <summary>
        /// 创建游戏卡片
        /// </summary>
        private void CreateGameCard(GameInfo gameInfo)
        {
            if (gameInfo == null) return;

            GameObject cardObj = Instantiate(gameCardPrefab, gameListContainer);
            GameCard card = cardObj.GetComponent<GameCard>();

            if (card != null)
            {
                card.Setup(gameInfo, OnGameCardClicked);
                gameCards.Add(card);
                Debug.Log($"[MainMenuUI] 创建卡片: {gameInfo.gameName}");
            }
            else
            {
                Debug.LogError("[MainMenuUI] gameCardPrefab 缺少 GameCard 组件");
                Destroy(cardObj);
            }
        }

        /// <summary>
        /// 清空游戏卡片
        /// </summary>
        private void ClearGameCards()
        {
            foreach (var card in gameCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            gameCards.Clear();
        }

        /// <summary>
        /// 游戏卡片点击回调
        /// </summary>
        private void OnGameCardClicked(GameInfo gameInfo)
        {
            if (gameInfo == null) return;

            Debug.Log($"[MainMenuUI] 选择游戏: {gameInfo.gameName}");

            if (!gameInfo.isUnlocked)
            {
                Debug.Log($"[MainMenuUI] 游戏未解锁: {gameInfo.gameName}");
                // TODO: 显示解锁提示
                return;
            }

            // 检查场景是否存在
            if (SceneLoader.Instance != null && !SceneLoader.Instance.IsSceneInBuildSettings(gameInfo.sceneName))
            {
                Debug.LogError($"[MainMenuUI] 场景不存在: {gameInfo.sceneName}");
                return;
            }

            // 加载游戏场景
            LoadGameScene(gameInfo.sceneName);
        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        private void LoadGameScene(string sceneName)
        {
            Debug.Log($"[MainMenuUI] 加载场景: {sceneName}");

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(sceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        /// <summary>
        /// 刷新游戏列表
        /// </summary>
        public void RefreshGameList()
        {
            LoadGameList();
        }
    }
}
