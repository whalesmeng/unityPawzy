using UnityEngine;
using System.Collections.Generic;

namespace PawzyPop.Core
{
    public class MatchFinder : MonoBehaviour
    {
        public static MatchFinder Instance { get; private set; }

        private Board board;

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
        }

        private void EnsureBoardReference()
        {
            if (board == null)
            {
                board = Board.Instance;
            }
        }

        public List<Tile> FindAllMatches()
        {
            EnsureBoardReference();
            
            if (board == null)
            {
                Debug.LogWarning("[MatchFinder] Board is null, returning empty matches");
                return new List<Tile>();
            }
            
            HashSet<Tile> matchedTiles = new HashSet<Tile>();

            // 检查水平匹配
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width - 2; x++)
                {
                    List<Tile> horizontalMatch = GetHorizontalMatch(x, y);
                    if (horizontalMatch.Count >= 3)
                    {
                        foreach (var tile in horizontalMatch)
                        {
                            matchedTiles.Add(tile);
                        }
                    }
                }
            }

            // 检查垂直匹配
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height - 2; y++)
                {
                    List<Tile> verticalMatch = GetVerticalMatch(x, y);
                    if (verticalMatch.Count >= 3)
                    {
                        foreach (var tile in verticalMatch)
                        {
                            matchedTiles.Add(tile);
                        }
                    }
                }
            }

            return new List<Tile>(matchedTiles);
        }

        private List<Tile> GetHorizontalMatch(int startX, int y)
        {
            List<Tile> match = new List<Tile>();
            Tile startTile = board.GetTile(startX, y);

            if (startTile == null || startTile.IsEmpty)
                return match;

            match.Add(startTile);

            for (int x = startX + 1; x < board.Width; x++)
            {
                Tile tile = board.GetTile(x, y);
                if (tile != null && tile.IsSameType(startTile))
                {
                    match.Add(tile);
                }
                else
                {
                    break;
                }
            }

            return match;
        }

        private List<Tile> GetVerticalMatch(int x, int startY)
        {
            List<Tile> match = new List<Tile>();
            Tile startTile = board.GetTile(x, startY);

            if (startTile == null || startTile.IsEmpty)
                return match;

            match.Add(startTile);

            for (int y = startY + 1; y < board.Height; y++)
            {
                Tile tile = board.GetTile(x, y);
                if (tile != null && tile.IsSameType(startTile))
                {
                    match.Add(tile);
                }
                else
                {
                    break;
                }
            }

            return match;
        }

        public List<Tile> FindMatchesAt(int x, int y)
        {
            EnsureBoardReference();
            
            if (board == null)
                return new List<Tile>();
                
            HashSet<Tile> matchedTiles = new HashSet<Tile>();
            Tile centerTile = board.GetTile(x, y);

            if (centerTile == null || centerTile.IsEmpty)
                return new List<Tile>();

            // 水平检查
            List<Tile> horizontalMatch = new List<Tile> { centerTile };
            
            // 向左检查
            for (int i = x - 1; i >= 0; i--)
            {
                Tile tile = board.GetTile(i, y);
                if (tile != null && tile.IsSameType(centerTile))
                    horizontalMatch.Add(tile);
                else
                    break;
            }
            
            // 向右检查
            for (int i = x + 1; i < board.Width; i++)
            {
                Tile tile = board.GetTile(i, y);
                if (tile != null && tile.IsSameType(centerTile))
                    horizontalMatch.Add(tile);
                else
                    break;
            }

            if (horizontalMatch.Count >= 3)
            {
                foreach (var tile in horizontalMatch)
                    matchedTiles.Add(tile);
            }

            // 垂直检查
            List<Tile> verticalMatch = new List<Tile> { centerTile };
            
            // 向下检查
            for (int i = y - 1; i >= 0; i--)
            {
                Tile tile = board.GetTile(x, i);
                if (tile != null && tile.IsSameType(centerTile))
                    verticalMatch.Add(tile);
                else
                    break;
            }
            
            // 向上检查
            for (int i = y + 1; i < board.Height; i++)
            {
                Tile tile = board.GetTile(x, i);
                if (tile != null && tile.IsSameType(centerTile))
                    verticalMatch.Add(tile);
                else
                    break;
            }

            if (verticalMatch.Count >= 3)
            {
                foreach (var tile in verticalMatch)
                    matchedTiles.Add(tile);
            }

            return new List<Tile>(matchedTiles);
        }

        public bool HasPossibleMoves()
        {
            EnsureBoardReference();
            
            if (board == null)
                return false;
                
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    // 检查向右交换
                    if (x < board.Width - 1)
                    {
                        if (WouldCreateMatch(x, y, x + 1, y))
                            return true;
                    }
                    // 检查向上交换
                    if (y < board.Height - 1)
                    {
                        if (WouldCreateMatch(x, y, x, y + 1))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool WouldCreateMatch(int x1, int y1, int x2, int y2)
        {
            Tile tile1 = board.GetTile(x1, y1);
            Tile tile2 = board.GetTile(x2, y2);

            if (tile1 == null || tile2 == null || tile1.IsEmpty || tile2.IsEmpty)
                return false;

            // 临时交换
            board.SwapTiles(tile1, tile2);

            // 检查是否有匹配
            bool hasMatch = FindMatchesAt(x1, y1).Count >= 3 || FindMatchesAt(x2, y2).Count >= 3;

            // 换回来
            board.SwapTiles(tile1, tile2);

            return hasMatch;
        }
    }
}
