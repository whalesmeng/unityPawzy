using UnityEngine;

namespace PawzyPop.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private float swipeThreshold = 0.5f;
        [SerializeField] private LayerMask tileLayer;

        private Camera mainCamera;
        private Tile selectedTile;
        private Vector3 touchStartPos;
        private bool isDragging;

        public System.Action<Tile, Tile> OnSwapRequested;

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
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!GameManager.Instance.CanProcessInput())
                return;

            HandleInput();
        }

        private void HandleInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTouchStart(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                OnTouchMove(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnd();
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchStart(touch.position);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMove(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnd();
                        break;
                }
            }
        }

        private void OnTouchStart(Vector3 screenPosition)
        {
            touchStartPos = screenPosition;
            selectedTile = GetTileAtScreenPosition(screenPosition);
            
            if (selectedTile != null)
            {
                isDragging = true;
                HighlightTile(selectedTile, true);
                Debug.Log($"[Input] Selected tile at ({selectedTile.X}, {selectedTile.Y})");
            }
            else
            {
                Debug.Log("[Input] No tile at click position");
            }
        }

        private void OnTouchMove(Vector3 screenPosition)
        {
            if (selectedTile == null)
                return;

            Vector3 delta = screenPosition - touchStartPos;
            
            if (delta.magnitude >= swipeThreshold * 100) // 转换为屏幕像素
            {
                SwipeDirection direction = GetSwipeDirection(delta);
                Tile targetTile = GetAdjacentTile(selectedTile, direction);

                if (targetTile != null)
                {
                    Debug.Log($"[Input] Swap requested: ({selectedTile.X},{selectedTile.Y}) -> ({targetTile.X},{targetTile.Y})");
                    HighlightTile(selectedTile, false);
                    OnSwapRequested?.Invoke(selectedTile, targetTile);
                    selectedTile = null;
                    isDragging = false;
                }
                else
                {
                    Debug.Log($"[Input] No adjacent tile in direction {direction}");
                }
            }
        }

        private void OnTouchEnd()
        {
            if (selectedTile != null)
            {
                HighlightTile(selectedTile, false);
            }
            selectedTile = null;
            isDragging = false;
        }

        private Tile GetTileAtScreenPosition(Vector3 screenPosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
            worldPos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, tileLayer);
            
            if (hit.collider != null)
            {
                return hit.collider.GetComponent<Tile>();
            }

            return null;
        }

        private SwipeDirection GetSwipeDirection(Vector3 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else
            {
                return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }

        private Tile GetAdjacentTile(Tile tile, SwipeDirection direction)
        {
            int x = tile.X;
            int y = tile.Y;

            switch (direction)
            {
                case SwipeDirection.Left:
                    x--;
                    break;
                case SwipeDirection.Right:
                    x++;
                    break;
                case SwipeDirection.Up:
                    y++;
                    break;
                case SwipeDirection.Down:
                    y--;
                    break;
            }

            return Board.Instance.GetTile(x, y);
        }

        private void HighlightTile(Tile tile, bool highlight)
        {
            if (tile == null) return;
            
            float scale = highlight ? 1.1f : 1f;
            tile.transform.localScale = Vector3.one * scale;
        }

        private enum SwipeDirection
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}
