#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PawzyPop.Data;

namespace PawzyPop.Editor
{
    /// <summary>
    /// 游戏配置创建工具
    /// </summary>
    public class GameConfigCreator
    {
        [MenuItem("PawzyPop/Create Game Configs")]
        public static void CreateGameConfigs()
        {
            CreateMatch3Config();
            CreateGomokuConfig();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[GameConfigCreator] 游戏配置创建完成");
        }

        [MenuItem("PawzyPop/Create Match3 Config")]
        public static void CreateMatch3Config()
        {
            string path = "Assets/Resources/GameConfigs/Match3Config.asset";
            
            // 检查是否已存在
            var existing = AssetDatabase.LoadAssetAtPath<GameInfo>(path);
            if (existing != null)
            {
                Debug.Log("[GameConfigCreator] Match3Config 已存在");
                return;
            }

            // 确保目录存在
            EnsureDirectoryExists("Assets/Resources/GameConfigs");

            // 创建配置
            GameInfo config = ScriptableObject.CreateInstance<GameInfo>();
            config.gameId = "match3";
            config.gameName = "消消乐";
            config.description = "经典三消游戏，交换相邻方块消除得分";
            config.sceneName = "SampleScene"; // 使用现有场景名
            config.isUnlocked = true;
            config.sortOrder = 1;
            config.backgroundColor = new Color(0.9f, 0.6f, 0.3f); // 橙色

            AssetDatabase.CreateAsset(config, path);
            Debug.Log($"[GameConfigCreator] 创建: {path}");
        }

        [MenuItem("PawzyPop/Create Gomoku Config")]
        public static void CreateGomokuConfig()
        {
            string path = "Assets/Resources/GameConfigs/GomokuConfig.asset";
            
            // 检查是否已存在
            var existing = AssetDatabase.LoadAssetAtPath<GameInfo>(path);
            if (existing != null)
            {
                Debug.Log("[GameConfigCreator] GomokuConfig 已存在");
                return;
            }

            // 确保目录存在
            EnsureDirectoryExists("Assets/Resources/GameConfigs");

            // 创建配置
            GameInfo config = ScriptableObject.CreateInstance<GameInfo>();
            config.gameId = "gomoku";
            config.gameName = "五子棋";
            config.description = "与AI对弈，先连成五子者获胜";
            config.sceneName = "GomokuGame";
            config.isUnlocked = true;
            config.sortOrder = 2;
            config.backgroundColor = new Color(0.4f, 0.7f, 0.9f); // 蓝色

            AssetDatabase.CreateAsset(config, path);
            Debug.Log($"[GameConfigCreator] 创建: {path}");
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string currentPath = folders[0];
                
                for (int i = 1; i < folders.Length; i++)
                {
                    string newPath = currentPath + "/" + folders[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath = newPath;
                }
            }
        }
    }
}
#endif
