using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PawzyPop.Core
{
    public class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }

        [Header("Board Settings")]
        [SerializeField] private int width = 6;
        [SerializeField] private int height = 6;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.1f;

        [Header("References")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tilesParent;
        [SerializeField] private TileType[] tileTypes;

        public int Width => width;
        public int Height => height;
        public Tile[,] Tiles { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            // 调试信息
            if (tilePrefab == null)
            {
                Debug.LogError("[Board] tilePrefab is NULL! Please configure in Inspector.");
                return;
            }
            if (tilesParent == null)
            {
                Debug.LogError("[Board] tilesParent is NULL! Please configure in Inspector.");
                return;
            }
            if (tileTypes == null || tileTypes.Length == 0)
            {
                Debug.LogError("[Board] tileTypes is empty! Please configure in Inspector.");
                return;
            }
            
            Debug.Log($"[Board] Initializing {width}x{height} board with {tileTypes.Length} tile types");
            
            Tiles = new Tile[width, height];
            GenerateBoard();
            CenterBoard();
            
            Debug.Log("[Board] Board initialized successfully!");
        }

        private void GenerateBoard()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateTile(x, y);
                }
            }

            // 确保初始棋盘没有匹配
            while (MatchFinder.Instance != null && MatchFinder.Instance.FindAllMatches().Count > 0)
            {
                ClearAndRegenerate();
            }
        }

        private void CreateTile(int x, int y)
        {
            Vector3 localPos = GetWorldPosition(x, y);
            GameObject tileObj = Instantiate(tilePrefab, tilesParent);
            tileObj.transform.localPosition = localPos;
            tileObj.name = $"Tile_{x}_{y}";

            Tile tile = tileObj.GetComponent<Tile>();
            TileType randomType = GetRandomTileType();
            tile.Initialize(x, y, randomType);
            Tiles[x, y] = tile;
        }

        private TileType GetRandomTileType()
        {
            if (tileTypes == null || tileTypes.Length == 0)
            {
                Debug.LogError("No tile types configured!");
                return null;
            }
            return tileTypes[Random.Range(0, tileTypes.Length)];
        }

        public TileType GetRandomTileTypeExcluding(TileType exclude)
        {
            if (tileTypes.Length <= 1) return tileTypes[0];
            
            TileType newType;
            do
            {
                newType = tileTypes[Random.Range(0, tileTypes.Length)];
            } while (newType == exclude);
            
            return newType;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            float totalSize = tileSize + tileSpacing;
            return new Vector3(x * totalSize, y * totalSize, 0);
        }

        private void CenterBoard()
        {
            float totalSize = tileSize + tileSpacing;
            float offsetX = (width - 1) * totalSize / 2f;
            float offsetY = (height - 1) * totalSize / 2f;
            
            // 使用 localPosition 确保相对于父对象正确定位
            tilesParent.localPosition = new Vector3(-offsetX, -offsetY, 0);
            
            // 确保 Board 本身在原点
            transform.position = Vector3.zero;
            
            Debug.Log($"[Board] CenterBoard: offsetX={offsetX}, offsetY={offsetY}, " +
                      $"tilesParent.localPosition={tilesParent.localPosition}, " +
                      $"tilesParent.position={tilesParent.position}");
            
            // 确保相机有 CameraScaler 组件并调整大小以适应棋盘
            if (Camera.main != null)
            {
                // 确保相机在正确位置
                Camera.main.transform.position = new Vector3(0, 0, -10);
                
                CameraScaler cameraScaler = Camera.main.GetComponent<CameraScaler>();
                if (cameraScaler == null)
                {
                    cameraScaler = Camera.main.gameObject.AddComponent<CameraScaler>();
                }
                cameraScaler.AdjustToBoard(width, height, tileSize, tileSpacing);
                
                Debug.Log($"[Board] Camera position: {Camera.main.transform.position}, " +
                          $"OrthoSize: {Camera.main.orthographicSize}");
            }
            else
            {
                Debug.LogError("[Board] Camera.main is NULL!");
            }
        }

        private void ClearAndRegenerate()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[x, y].SetType(GetRandomTileType());
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return null;
            return Tiles[x, y];
        }

        public void SwapTiles(Tile tileA, Tile tileB)
        {
            int tempX = tileA.X;
            int tempY = tileA.Y;

            Tiles[tileA.X, tileA.Y] = tileB;
            Tiles[tileB.X, tileB.Y] = tileA;

            tileA.SetPosition(tileB.X, tileB.Y);
            tileB.SetPosition(tempX, tempY);
        }

        public bool AreAdjacent(Tile tileA, Tile tileB)
        {
            int dx = Mathf.Abs(tileA.X - tileB.X);
            int dy = Mathf.Abs(tileA.Y - tileB.Y);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        public IEnumerator RefillBoard()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = Tiles[x, y];
                    if (tile != null && tile.IsEmpty)
                    {
                        tile.SetType(GetRandomTileType());
                        tile.PlaySpawnAnimation();
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }

        public IEnumerator CollapseColumns()
        {
            bool hasMoved = false;

            for (int x = 0; x < width; x++)
            {
                int writeY = 0; // 下一个要写入的位置
                
                for (int readY = 0; readY < height; readY++)
                {
                    Tile tile = Tiles[x, readY];
                    
                    if (tile != null && !tile.IsEmpty)
                    {
                        if (readY != writeY)
                        {
                            // 需要移动这个 tile
                            hasMoved = true;
                            
                            // 交换数组中的引用
                            Tile emptyTile = Tiles[x, writeY];
                            Tiles[x, writeY] = tile;
                            Tiles[x, readY] = emptyTile;
                            
                            // 更新位置信息
                            tile.SetPosition(x, writeY);
                            tile.AnimateToPosition(GetWorldPosition(x, writeY));
                            tile.gameObject.name = $"Tile_{x}_{writeY}";
                            
                            if (emptyTile != null)
                            {
                                emptyTile.SetPosition(x, readY);
                                emptyTile.SetWorldPosition(GetWorldPosition(x, readY));
                                emptyTile.gameObject.name = $"Tile_{x}_{readY}";
                            }
                        }
                        writeY++;
                    }
                }
            }

            if (hasMoved)
            {
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
