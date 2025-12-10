using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PawzyPop.Core
{
    public class MatchProcessor : MonoBehaviour
    {
        public static MatchProcessor Instance { get; private set; }

        [SerializeField] private float swapDuration = 0.2f;
        [SerializeField] private float matchDelay = 0.1f;

        private Board board;
        private MatchFinder matchFinder;
        private bool isProcessing;

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
            board = Board.Instance;
            matchFinder = MatchFinder.Instance;

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnSwapRequested += HandleSwapRequest;
            }
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnSwapRequested -= HandleSwapRequest;
            }
        }

        private void HandleSwapRequest(Tile tileA, Tile tileB)
        {
            Debug.Log($"[MatchProcessor] HandleSwapRequest called: ({tileA.X},{tileA.Y}) <-> ({tileB.X},{tileB.Y})");
            
            if (isProcessing)
            {
                Debug.Log("[MatchProcessor] Already processing, ignoring");
                return;
            }

            if (!board.AreAdjacent(tileA, tileB))
            {
                Debug.Log("[MatchProcessor] Tiles not adjacent, ignoring");
                return;
            }

            StartCoroutine(ProcessSwap(tileA, tileB));
        }

        private IEnumerator ProcessSwap(Tile tileA, Tile tileB)
        {
            isProcessing = true;
            GameManager.Instance.SetState(GameState.Processing);

            // 执行交换动画
            yield return StartCoroutine(AnimateSwap(tileA, tileB));

            // 交换数据
            board.SwapTiles(tileA, tileB);

            // 检查匹配
            List<Tile> matchesA = matchFinder.FindMatchesAt(tileA.X, tileA.Y);
            List<Tile> matchesB = matchFinder.FindMatchesAt(tileB.X, tileB.Y);

            Debug.Log($"[Match] Checking matches at ({tileA.X},{tileA.Y}): found {matchesA.Count}, at ({tileB.X},{tileB.Y}): found {matchesB.Count}");

            HashSet<Tile> allMatches = new HashSet<Tile>(matchesA);
            foreach (var tile in matchesB)
            {
                allMatches.Add(tile);
            }

            if (allMatches.Count >= 3)
            {
                Debug.Log($"[Match] Total matches: {allMatches.Count}, processing...");
                
                // 有匹配，消耗步数
                GameManager.Instance.UseMove();

                // 处理消除和连锁
                yield return StartCoroutine(ProcessMatches(new List<Tile>(allMatches)));
            }
            else
            {
                Debug.Log("[Match] No match, swapping back");
                // 无匹配，换回去
                yield return StartCoroutine(AnimateSwap(tileA, tileB));
                board.SwapTiles(tileA, tileB);
            }

            // 检查是否还有可能的移动
            if (!matchFinder.HasPossibleMoves())
            {
                yield return StartCoroutine(ShuffleBoard());
            }

            isProcessing = false;
            
            if (GameManager.Instance.CurrentState == GameState.Processing)
            {
                GameManager.Instance.SetState(GameState.WaitingInput);
            }
        }

        private IEnumerator AnimateSwap(Tile tileA, Tile tileB)
        {
            Vector3 posA = tileA.transform.localPosition;
            Vector3 posB = tileB.transform.localPosition;

            float elapsed = 0f;
            while (elapsed < swapDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / swapDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                tileA.transform.localPosition = Vector3.Lerp(posA, posB, t);
                tileB.transform.localPosition = Vector3.Lerp(posB, posA, t);

                yield return null;
            }

            tileA.transform.localPosition = posB;
            tileB.transform.localPosition = posA;
        }

        private IEnumerator ProcessMatches(List<Tile> matches)
        {
            // 播放消除动画
            foreach (var tile in matches)
            {
                tile.PlayMatchAnimation();
            }

            yield return new WaitForSeconds(0.2f);

            // 计算分数
            int score = 0;
            foreach (var tile in matches)
            {
                if (tile.Type != null)
                {
                    score += tile.Type.scoreValue;
                }
                tile.SetEmpty(true);
            }
            GameManager.Instance.AddScore(score);

            yield return new WaitForSeconds(matchDelay);

            // 下落填充
            yield return StartCoroutine(board.CollapseColumns());
            yield return StartCoroutine(board.RefillBoard());

            // 检查连锁
            List<Tile> newMatches = matchFinder.FindAllMatches();
            if (newMatches.Count >= 3)
            {
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(ProcessMatches(newMatches));
            }
        }

        private IEnumerator ShuffleBoard()
        {
            Debug.Log("No possible moves, shuffling board...");
            
            // 简单的重新生成
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Tile tile = board.GetTile(x, y);
                    if (tile != null && !tile.IsEmpty)
                    {
                        tile.PlayMatchAnimation();
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);

            board.InitializeBoard();

            yield return new WaitForSeconds(0.3f);
        }
    }
}
