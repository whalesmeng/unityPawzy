using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;

namespace PawzyPop.Editor
{
    public class GameSetupWindow : EditorWindow
    {
        [MenuItem("PawzyPop/1. Complete Setup (One Click)", priority = 0)]
        public static void CompleteSetup()
        {
            Debug.Log("=== PawzyPop Complete Setup Started ===");
            
            // Step 1: 创建 TileTypes
            TileTypeCreator.CreateDefaultTileTypes();
            
            // Step 2: 创建 Tile Prefab
            CreateTilePrefab();
            
            // Step 3: 设置场景
            SetupGameScene();
            
            // Step 4: 自动配置 Board 组件
            ConfigureBoardComponent();
            
            // Step 5: 配置 InputManager
            ConfigureInputManager();
            
            Debug.Log("=== PawzyPop Complete Setup Finished! ===");
            Debug.Log("点击 Play 按钮即可开始游戏！");
        }

        [MenuItem("PawzyPop/2. Setup Game Scene", priority = 1)]
        public static void SetupGameScene()
        {
            // 创建主相机
            GameObject cameraObj = GameObject.Find("Main Camera");
            if (cameraObj == null)
            {
                cameraObj = new GameObject("Main Camera");
                cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
            }
            
            Camera cam = cameraObj.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.2f, 0.6f, 0.8f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, 0, -10);
            cameraObj.tag = "MainCamera";

            // 创建 GameManager
            CreateManagerObject("GameManager", typeof(Core.GameManager));
            
            // 创建 Board
            GameObject boardObj = CreateManagerObject("Board", typeof(Core.Board));
            
            // 创建 Tiles 父对象
            Transform existingTiles = boardObj.transform.Find("Tiles");
            if (existingTiles == null)
            {
                GameObject tilesParent = new GameObject("Tiles");
                tilesParent.transform.SetParent(boardObj.transform);
            }

            // 创建 MatchFinder
            CreateManagerObject("MatchFinder", typeof(Core.MatchFinder));

            // 创建 MatchProcessor
            CreateManagerObject("MatchProcessor", typeof(Core.MatchProcessor));

            // 创建 InputManager
            CreateManagerObject("InputManager", typeof(Core.InputManager));
            
            // 创建 LevelLoader
            CreateManagerObject("LevelLoader", typeof(Core.LevelLoader));

            Debug.Log("[Setup] Game scene setup complete!");
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        
        [MenuItem("PawzyPop/4. Configure Board Component", priority = 3)]
        public static void ConfigureBoardComponent()
        {
            // 找到 Board 对象
            GameObject boardObj = GameObject.Find("Board");
            if (boardObj == null)
            {
                Debug.LogError("[Config] Board object not found! Run 'Setup Game Scene' first.");
                return;
            }
            
            Core.Board board = boardObj.GetComponent<Core.Board>();
            if (board == null)
            {
                Debug.LogError("[Config] Board component not found!");
                return;
            }
            
            // 使用 SerializedObject 来设置私有字段
            SerializedObject serializedBoard = new SerializedObject(board);
            
            // 设置 Tile Prefab
            SerializedProperty tilePrefabProp = serializedBoard.FindProperty("tilePrefab");
            GameObject tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tile.prefab");
            if (tilePrefab != null)
            {
                tilePrefabProp.objectReferenceValue = tilePrefab;
                Debug.Log("[Config] Tile prefab assigned.");
            }
            else
            {
                Debug.LogWarning("[Config] Tile prefab not found! Run 'Create Tile Prefab' first.");
            }
            
            // 设置 Tiles Parent
            SerializedProperty tilesParentProp = serializedBoard.FindProperty("tilesParent");
            Transform tilesParent = boardObj.transform.Find("Tiles");
            if (tilesParent != null)
            {
                tilesParentProp.objectReferenceValue = tilesParent;
                Debug.Log("[Config] Tiles parent assigned.");
            }
            
            // 设置 TileTypes
            SerializedProperty tileTypesProp = serializedBoard.FindProperty("tileTypes");
            string[] tileTypeNames = { "Shiba", "Corgi", "Golden", "Husky", "Teddy", "Samoyed" };
            
            tileTypesProp.arraySize = tileTypeNames.Length;
            int loadedCount = 0;
            
            for (int i = 0; i < tileTypeNames.Length; i++)
            {
                string path = $"Assets/Resources/TileTypes/{tileTypeNames[i]}.asset";
                Core.TileType tileType = AssetDatabase.LoadAssetAtPath<Core.TileType>(path);
                
                if (tileType != null)
                {
                    tileTypesProp.GetArrayElementAtIndex(i).objectReferenceValue = tileType;
                    loadedCount++;
                }
            }
            
            if (loadedCount > 0)
            {
                Debug.Log($"[Config] {loadedCount} TileTypes assigned.");
            }
            else
            {
                Debug.LogWarning("[Config] No TileTypes found! Run 'Create Default Tile Types' first.");
            }
            
            serializedBoard.ApplyModifiedProperties();
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Config] Board component configured successfully!");
        }
        
        [MenuItem("PawzyPop/5. Configure InputManager", priority = 4)]
        public static void ConfigureInputManager()
        {
            GameObject inputObj = GameObject.Find("InputManager");
            if (inputObj == null)
            {
                Debug.LogError("[Config] InputManager not found!");
                return;
            }
            
            Core.InputManager inputManager = inputObj.GetComponent<Core.InputManager>();
            if (inputManager == null) return;
            
            SerializedObject serializedInput = new SerializedObject(inputManager);
            
            // 设置 tileLayer - 使用 Default layer (0) 或创建 Tile layer
            SerializedProperty tileLayerProp = serializedInput.FindProperty("tileLayer");
            // 设置为 Everything 或 Default，确保能检测到碰撞
            tileLayerProp.intValue = -1; // Everything
            
            serializedInput.ApplyModifiedProperties();
            Debug.Log("[Config] InputManager configured.");
        }

        private static GameObject CreateManagerObject(string name, System.Type componentType)
        {
            GameObject obj = GameObject.Find(name);
            if (obj == null)
            {
                obj = new GameObject(name);
            }

            if (obj.GetComponent(componentType) == null)
            {
                obj.AddComponent(componentType);
            }

            return obj;
        }

        [MenuItem("PawzyPop/3. Create Tile Prefab", priority = 2)]
        public static void CreateTilePrefab()
        {
            string prefabPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // 先创建 Sprite 资源
            string spritePath = CreateDefaultSpriteAsset();
            AssetDatabase.Refresh();
            
            // 加载创建好的 Sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            // 创建 Tile GameObject
            GameObject tileObj = new GameObject("Tile");
            
            // 添加 SpriteRenderer 并设置 Sprite
            SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            sr.sprite = sprite;
            sr.color = Color.white;

            // 添加 BoxCollider2D
            BoxCollider2D collider = tileObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.9f, 0.9f);

            // 添加 Tile 脚本
            tileObj.AddComponent<Core.Tile>();

            // 保存为 Prefab
            string path = $"{prefabPath}/Tile.prefab";
            PrefabUtility.SaveAsPrefabAsset(tileObj, path);
            DestroyImmediate(tileObj);

            Debug.Log("[Prefab] Created Tile prefab at " + path);
        }

        private static string CreateDefaultSpriteAsset()
        {
            string spritePath = "Assets/Resources/Sprites";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(spritePath))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Sprites");
            }
            
            string texturePath = $"{spritePath}/DefaultTile.png";
            
            // 如果已存在，直接返回
            if (System.IO.File.Exists(texturePath))
            {
                return texturePath;
            }

            // 创建一个简单的白色圆形纹理
            Texture2D texture = new Texture2D(128, 128);
            Color[] colors = new Color[128 * 128];
            
            Vector2 center = new Vector2(64, 64);
            float radius = 60;

            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    if (dist <= radius)
                    {
                        colors[y * 128 + x] = Color.white;
                    }
                    else
                    {
                        colors[y * 128 + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(colors);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(texturePath, bytes);
            AssetDatabase.Refresh();

            // 设置纹理导入设置
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 128;
                importer.SaveAndReimport();
            }
            
            return texturePath;
        }
    }
}
