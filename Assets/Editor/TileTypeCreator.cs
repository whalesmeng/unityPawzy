using UnityEngine;
using UnityEditor;
using PawzyPop.Core;

namespace PawzyPop.Editor
{
    public class TileTypeCreator : EditorWindow
    {
        [MenuItem("PawzyPop/Create Default Tile Types", priority = 10)]
        public static void CreateDefaultTileTypes()
        {
            string path = "Assets/Resources/TileTypes";
            
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "TileTypes");
            }

            CreateTileType("Shiba", new Color(0.85f, 0.55f, 0.45f), path);   // 柴犬 - 莫兰迪红/珊瑚
            CreateTileType("Corgi", new Color(0.55f, 0.70f, 0.55f), path);   // 柯基 - 莫兰迪绿/灰豆绿
            CreateTileType("Golden", new Color(0.90f, 0.80f, 0.55f), path);  // 金毛 - 莫兰迪黄/奶油杏
            CreateTileType("Husky", new Color(0.50f, 0.60f, 0.75f), path);   // 哈士奇 - 莫兰迪蓝/雾霾蓝
            CreateTileType("Teddy", new Color(0.75f, 0.50f, 0.55f), path);   // 泰迪 - 莫兰迪紫/豆沙粉
            CreateTileType("Samoyed", new Color(0.45f, 0.55f, 0.60f), path); // 萨摩 - 莫兰迪灰/青灰

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[TileType] Created 6 default tile types in " + path);
        }

        private static void CreateTileType(string typeName, Color color, string path)
        {
            string assetPath = $"{path}/{typeName}.asset";
            
            // 检查是否已存在
            TileType existing = AssetDatabase.LoadAssetAtPath<TileType>(assetPath);
            if (existing != null)
            {
                Debug.Log($"[TileType] {typeName} already exists, skipping.");
                return;
            }
            
            TileType tileType = ScriptableObject.CreateInstance<TileType>();
            tileType.typeName = typeName;
            tileType.color = color;
            tileType.scoreValue = 10;

            AssetDatabase.CreateAsset(tileType, assetPath);
        }
    }
}
