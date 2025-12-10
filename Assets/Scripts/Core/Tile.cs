using UnityEngine;

namespace PawzyPop.Core
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public TileType Type { get; private set; }
        public bool IsEmpty { get; private set; }
        public bool IsMoving { get; private set; }

        private Vector3 targetPosition;
        private float moveSpeed = 10f;

        public void Initialize(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            SetType(type);
            IsEmpty = type == null;
        }

        public void SetType(TileType type)
        {
            Type = type;
            
            // 确保获取 SpriteRenderer
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (type != null && spriteRenderer != null)
            {
                // 只有当 TileType 有自定义 sprite 时才替换
                if (type.sprite != null)
                {
                    spriteRenderer.sprite = type.sprite;
                }
                // 保留 Prefab 的默认 sprite，只改变颜色
                spriteRenderer.color = type.color;
                spriteRenderer.enabled = true;
                IsEmpty = false;
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
                IsEmpty = true;
            }
            else
            {
                Debug.LogError("[Tile] SpriteRenderer is NULL!");
            }
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
            gameObject.name = $"Tile_{x}_{y}";
        }

        public void SetEmpty(bool empty)
        {
            IsEmpty = empty;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !empty;
            }
        }

        public void AnimateToPosition(Vector3 localPos)
        {
            targetPosition = localPos;
            IsMoving = true;
        }

        public void SetWorldPosition(Vector3 localPos)
        {
            transform.localPosition = localPos;
            targetPosition = localPos;
            IsMoving = false;
        }

        private void Update()
        {
            if (IsMoving)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
                
                if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
                {
                    transform.localPosition = targetPosition;
                    IsMoving = false;
                }
            }
        }

        public void PlayMatchAnimation()
        {
            // 简单的缩放动画
            StartCoroutine(ScaleAnimation());
        }

        public void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            StartCoroutine(SpawnScaleAnimation());
        }

        private System.Collections.IEnumerator ScaleAnimation()
        {
            float duration = 0.15f;
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
                transform.localScale = originalScale * scale;
                yield return null;
            }

            transform.localScale = Vector3.zero;
        }

        private System.Collections.IEnumerator SpawnScaleAnimation()
        {
            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = Mathf.Lerp(0f, 1f, t);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        public bool IsSameType(Tile other)
        {
            if (other == null || IsEmpty || other.IsEmpty)
                return false;
            return Type == other.Type;
        }
    }
}
