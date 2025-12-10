using UnityEngine;

namespace PawzyPop.Data
{
    /// <summary>
    /// 游戏信息配置，用于主菜单显示和游戏加载
    /// </summary>
    [CreateAssetMenu(fileName = "GameInfo", menuName = "PawzyPop/Game Info")]
    public class GameInfo : ScriptableObject
    {
        [Header("基本信息")]
        [Tooltip("游戏唯一标识")]
        public string gameId;

        [Tooltip("游戏显示名称")]
        public string gameName;

        [TextArea(2, 4)]
        [Tooltip("游戏简介")]
        public string description;

        [Header("显示")]
        [Tooltip("游戏图标")]
        public Sprite iconSprite;

        [Tooltip("游戏背景颜色")]
        public Color backgroundColor = Color.white;

        [Header("场景")]
        [Tooltip("游戏场景名称")]
        public string sceneName;

        [Header("状态")]
        [Tooltip("是否已解锁")]
        public bool isUnlocked = true;

        [Tooltip("排序顺序（越小越靠前）")]
        public int sortOrder = 0;

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(gameId) &&
                   !string.IsNullOrEmpty(gameName) &&
                   !string.IsNullOrEmpty(sceneName);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogWarning($"[GameInfo] {name}: gameId 不能为空");
            }
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"[GameInfo] {name}: sceneName 不能为空");
            }
        }
    }
}
