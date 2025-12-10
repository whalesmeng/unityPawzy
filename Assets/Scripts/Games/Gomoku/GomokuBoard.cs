using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawzyPop.Games.Gomoku
{
    /// <summary>
    /// 五子棋棋盘
    /// </summary>
    public class GomokuBoard : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int size = 15;
        [SerializeField] private float cellSize = 0.5f;
        [SerializeField] private float lineWidth = 0.02f;

        [Header("Prefabs")]
        [SerializeField] private GameObject blackPiecePrefab;
        [SerializeField] private GameObject whitePiecePrefab;
        [SerializeField] private GameObject cellHighlightPrefab;

        [Header("Colors")]
        [SerializeField] private Color boardColor = new Color(0.87f, 0.72f, 0.53f);
        [SerializeField] private Color lineColor = Color.black;

        // 棋盘状态: 0=空, 1=黑, 2=白
        private int[,] boardState;
        private GameObject[,] pieces;
        private List<Vector2Int> moveHistory = new List<Vector2Int>();

        // 最后落子位置高亮
        private GameObject lastMoveHighlight;

        /// <summary>
        /// 格子点击事件
        /// </summary>
        public event Action<int, int> OnCellClicked;

        public int Size => size;

        private void Awake()
        {
            // 如果没有设置预制体，创建简单的棋子
            if (blackPiecePrefab == null)
            {
                blackPiecePrefab = CreatePiecePrefab(Color.black, "BlackPiece");
            }
            if (whitePiecePrefab == null)
            {
                whitePiecePrefab = CreatePiecePrefab(Color.white, "WhitePiece");
            }
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        public void Initialize(int boardSize)
        {
            size = boardSize;
            boardState = new int[size, size];
            pieces = new GameObject[size, size];
            moveHistory.Clear();

            // 清除旧棋子
            ClearPieces();

            // 绘制棋盘
            DrawBoard();

            Debug.Log($"[GomokuBoard] Initialized {size}x{size} board");
        }

        /// <summary>
        /// 清除所有棋子
        /// </summary>
        private void ClearPieces()
        {
            if (pieces != null)
            {
                for (int x = 0; x < pieces.GetLength(0); x++)
                {
                    for (int y = 0; y < pieces.GetLength(1); y++)
                    {
                        if (pieces[x, y] != null)
                        {
                            Destroy(pieces[x, y]);
                        }
                    }
                }
            }

            // 清除高亮
            if (lastMoveHighlight != null)
            {
                Destroy(lastMoveHighlight);
            }
        }

        /// <summary>
        /// 绘制棋盘背景和网格线
        /// </summary>
        private void DrawBoard()
        {
            // 清除旧的绘制
            foreach (Transform child in transform)
            {
                if (child.name.StartsWith("BoardLine") || child.name == "BoardBackground")
                {
                    Destroy(child.gameObject);
                }
            }

            float boardWidth = (size - 1) * cellSize;

            // 创建背景
            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
            background.name = "BoardBackground";
            background.transform.SetParent(transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = new Vector3(boardWidth + cellSize, boardWidth + cellSize, 1);
            background.GetComponent<Renderer>().material.color = boardColor;
            Destroy(background.GetComponent<Collider>());

            // 绘制网格线
            for (int i = 0; i < size; i++)
            {
                // 横线
                CreateLine($"BoardLine_H_{i}",
                    new Vector3(-boardWidth / 2, (i - (size - 1) / 2f) * cellSize, -0.01f),
                    new Vector3(boardWidth / 2, (i - (size - 1) / 2f) * cellSize, -0.01f));

                // 竖线
                CreateLine($"BoardLine_V_{i}",
                    new Vector3((i - (size - 1) / 2f) * cellSize, -boardWidth / 2, -0.01f),
                    new Vector3((i - (size - 1) / 2f) * cellSize, boardWidth / 2, -0.01f));
            }

            // 绘制星位点（天元和四个角星）
            if (size == 15)
            {
                DrawStarPoint(7, 7);   // 天元
                DrawStarPoint(3, 3);
                DrawStarPoint(3, 11);
                DrawStarPoint(11, 3);
                DrawStarPoint(11, 11);
            }

            // 调整相机
            AdjustCamera();
        }

        /// <summary>
        /// 创建线条
        /// </summary>
        private void CreateLine(string name, Vector3 start, Vector3 end)
        {
            GameObject lineObj = new GameObject(name);
            lineObj.transform.SetParent(transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, transform.TransformPoint(start));
            lr.SetPosition(1, transform.TransformPoint(end));
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.useWorldSpace = true;
        }

        /// <summary>
        /// 绘制星位点
        /// </summary>
        private void DrawStarPoint(int x, int y)
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = $"StarPoint_{x}_{y}";
            star.transform.SetParent(transform);
            star.transform.localPosition = GetLocalPosition(x, y) + new Vector3(0, 0, -0.02f);
            star.transform.localScale = Vector3.one * cellSize * 0.15f;
            star.GetComponent<Renderer>().material.color = lineColor;
            Destroy(star.GetComponent<Collider>());
        }

        /// <summary>
        /// 调整相机以适应棋盘
        /// </summary>
        private void AdjustCamera()
        {
            if (Camera.main != null)
            {
                float boardWidth = size * cellSize;
                Camera.main.orthographicSize = boardWidth * 0.6f;
                Camera.main.transform.position = new Vector3(0, 0, -10);
            }
        }

        /// <summary>
        /// 获取格子的本地坐标
        /// </summary>
        private Vector3 GetLocalPosition(int x, int y)
        {
            float offset = (size - 1) / 2f;
            return new Vector3((x - offset) * cellSize, (y - offset) * cellSize, 0);
        }

        /// <summary>
        /// 落子
        /// </summary>
        public bool PlacePiece(int x, int y, int player)
        {
            if (x < 0 || x >= size || y < 0 || y >= size)
            {
                Debug.LogWarning($"[GomokuBoard] Invalid position: ({x}, {y})");
                return false;
            }

            if (boardState[x, y] != 0)
            {
                Debug.LogWarning($"[GomokuBoard] Position occupied: ({x}, {y})");
                return false;
            }

            // 更新状态
            boardState[x, y] = player;
            moveHistory.Add(new Vector2Int(x, y));

            // 创建棋子
            GameObject prefab = player == 1 ? blackPiecePrefab : whitePiecePrefab;
            GameObject piece = Instantiate(prefab, transform);
            piece.transform.localPosition = GetLocalPosition(x, y) + new Vector3(0, 0, -0.05f);
            piece.name = $"Piece_{x}_{y}_{player}";
            pieces[x, y] = piece;

            // 更新高亮
            UpdateLastMoveHighlight(x, y);

            Debug.Log($"[GomokuBoard] Placed piece at ({x}, {y}) for player {player}");
            return true;
        }

        /// <summary>
        /// 更新最后落子高亮
        /// </summary>
        private void UpdateLastMoveHighlight(int x, int y)
        {
            if (lastMoveHighlight != null)
            {
                Destroy(lastMoveHighlight);
            }

            lastMoveHighlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
            lastMoveHighlight.name = "LastMoveHighlight";
            lastMoveHighlight.transform.SetParent(transform);
            lastMoveHighlight.transform.localPosition = GetLocalPosition(x, y) + new Vector3(0, 0, -0.03f);
            lastMoveHighlight.transform.localScale = Vector3.one * cellSize * 0.3f;
            lastMoveHighlight.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
            Destroy(lastMoveHighlight.GetComponent<Collider>());
        }

        /// <summary>
        /// 检查是否获胜
        /// </summary>
        public bool CheckWin(int x, int y, int player)
        {
            // 四个方向：横、竖、左斜、右斜
            int[][] directions = new int[][]
            {
                new int[] { 1, 0 },  // 横
                new int[] { 0, 1 },  // 竖
                new int[] { 1, 1 },  // 右斜
                new int[] { 1, -1 }  // 左斜
            };

            foreach (var dir in directions)
            {
                int count = 1;

                // 正方向
                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx >= 0 && nx < size && ny >= 0 && ny < size && boardState[nx, ny] == player)
                    {
                        count++;
                    }
                    else break;
                }

                // 反方向
                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;
                    if (nx >= 0 && nx < size && ny >= 0 && ny < size && boardState[nx, ny] == player)
                    {
                        count++;
                    }
                    else break;
                }

                if (count >= 5)
                {
                    Debug.Log($"[GomokuBoard] Player {player} wins! Direction: ({dir[0]}, {dir[1]})");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查棋盘是否已满
        /// </summary>
        public bool IsBoardFull()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (boardState[x, y] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取棋盘状态副本
        /// </summary>
        public int[,] GetBoardState()
        {
            int[,] copy = new int[size, size];
            Array.Copy(boardState, copy, boardState.Length);
            return copy;
        }

        /// <summary>
        /// 创建简单的棋子预制体
        /// </summary>
        private GameObject CreatePiecePrefab(Color color, string name)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            piece.name = name;
            piece.transform.localScale = Vector3.one * cellSize * 0.8f;
            piece.GetComponent<Renderer>().material.color = color;
            piece.SetActive(false);
            DontDestroyOnLoad(piece);
            return piece;
        }

        #region Input Handling

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }

            // 触摸输入
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleClick(Input.GetTouch(0).position);
            }
        }

        private void HandleClick(Vector2 screenPosition)
        {
            if (Camera.main == null) return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (gridPos.x >= 0 && gridPos.x < size && gridPos.y >= 0 && gridPos.y < size)
            {
                Debug.Log($"[GomokuBoard] Click detected at grid: ({gridPos.x}, {gridPos.y})");
                OnCellClicked?.Invoke(gridPos.x, gridPos.y);
            }
        }

        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            float offset = (size - 1) / 2f;

            int x = Mathf.RoundToInt(localPos.x / cellSize + offset);
            int y = Mathf.RoundToInt(localPos.y / cellSize + offset);

            return new Vector2Int(x, y);
        }

        #endregion
    }
}
