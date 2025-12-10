using System;
using UnityEngine;
using UnityEngine.UI;
using PawzyPop.Data;

namespace PawzyPop.UI
{
    /// <summary>
    /// 游戏卡片组件，用于主菜单显示游戏入口
    /// </summary>
    public class GameCard : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text highScoreText;
        [SerializeField] private Button cardButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject lockOverlay;

        private GameInfo gameInfo;
        private Action<GameInfo> onClickCallback;

        private void Awake()
        {
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(OnCardClicked);
            }
        }

        /// <summary>
        /// 设置卡片数据
        /// </summary>
        public void Setup(GameInfo info, Action<GameInfo> onClick)
        {
            gameInfo = info;
            onClickCallback = onClick;

            if (info == null)
            {
                Debug.LogWarning("[GameCard] Setup: GameInfo 为空");
                return;
            }

            // 设置名称
            if (nameText != null)
            {
                nameText.text = info.gameName;
            }

            // 设置描述
            if (descriptionText != null)
            {
                descriptionText.text = info.description;
            }

            // 设置图标
            if (iconImage != null && info.iconSprite != null)
            {
                iconImage.sprite = info.iconSprite;
            }

            // 设置背景颜色
            if (backgroundImage != null)
            {
                backgroundImage.color = info.backgroundColor;
            }

            // 设置锁定状态
            if (lockOverlay != null)
            {
                lockOverlay.SetActive(!info.isUnlocked);
            }

            // 设置按钮可交互性
            if (cardButton != null)
            {
                cardButton.interactable = info.isUnlocked;
            }

            // 更新高分显示
            UpdateHighScore();

            Debug.Log($"[GameCard] Setup: {info.gameName}");
        }

        /// <summary>
        /// 更新高分显示
        /// </summary>
        public void UpdateHighScore()
        {
            if (highScoreText == null || gameInfo == null) return;

            int highScore = 0;
            if (SaveManager.Instance != null)
            {
                highScore = SaveManager.Instance.GetGameHighScore(gameInfo.gameId);
            }

            if (highScore > 0)
            {
                highScoreText.text = $"最高分: {highScore}";
                highScoreText.gameObject.SetActive(true);
            }
            else
            {
                highScoreText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 卡片点击事件
        /// </summary>
        private void OnCardClicked()
        {
            Debug.Log($"[GameCard] Clicked: {gameInfo?.gameName}");
            onClickCallback?.Invoke(gameInfo);
        }

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        public GameInfo GetGameInfo()
        {
            return gameInfo;
        }

        private void OnDestroy()
        {
            if (cardButton != null)
            {
                cardButton.onClick.RemoveListener(OnCardClicked);
            }
        }
    }
}
