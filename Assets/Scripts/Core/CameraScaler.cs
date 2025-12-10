using UnityEngine;

namespace PawzyPop.Core
{
    /// <summary>
    /// 自动调整相机以适应不同屏幕比例，确保游戏内容居中显示
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        [Header("Target Settings")]
        [Tooltip("设计时的目标宽度（世界单位）")]
        [SerializeField] private float targetWidth = 8f;
        
        [Tooltip("设计时的目标高度（世界单位）")]
        [SerializeField] private float targetHeight = 10f;
        
        [Tooltip("额外的边距")]
        [SerializeField] private float padding = 3f;

        private Camera cam;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Start()
        {
            // 延迟一帧执行，确保屏幕尺寸正确
            Invoke(nameof(AdjustCamera), 0.1f);
        }

        private void Update()
        {
            // 检测屏幕尺寸变化
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                AdjustCamera();
            }
        }

        private void AdjustCamera()
        {
            if (cam == null) 
            {
                cam = GetComponent<Camera>();
                if (cam == null) return;
            }

            // 重置相机视口为全屏
            cam.rect = new Rect(0, 0, 1, 1);
            
            // 确保相机位置在原点
            transform.position = new Vector3(0, 0, -10);

            // 计算需要显示的区域（包含边距）
            float requiredWidth = targetWidth + padding * 2;
            float requiredHeight = targetHeight + padding * 2;

            // 获取屏幕宽高比
            float screenAspect = (float)Screen.width / Screen.height;
            float targetAspect = requiredWidth / requiredHeight;

            // 根据宽高比计算正交大小
            float orthoSize;
            if (screenAspect >= targetAspect)
            {
                // 屏幕较宽或相等，以高度为基准
                orthoSize = requiredHeight / 2f;
            }
            else
            {
                // 屏幕较窄（如手机竖屏），以宽度为基准
                // orthographicSize 是半高，所以需要根据宽度反推
                // 可见宽度 = orthographicSize * 2 * screenAspect
                // requiredWidth = orthoSize * 2 * screenAspect
                // orthoSize = requiredWidth / (2 * screenAspect)
                orthoSize = requiredWidth / (2f * screenAspect);
            }
            
            cam.orthographicSize = orthoSize;
            
            Debug.Log($"[CameraScaler] Screen: {Screen.width}x{Screen.height}, " +
                      $"Aspect: {screenAspect:F3}, " +
                      $"TargetAspect: {targetAspect:F3}, " +
                      $"OrthoSize: {orthoSize:F2}, " +
                      $"Required: {requiredWidth}x{requiredHeight}");
        }

        /// <summary>
        /// 根据 Board 的实际大小动态调整相机
        /// </summary>
        public void AdjustToBoard(int boardWidth, int boardHeight, float tileSize, float tileSpacing)
        {
            float totalTileSize = tileSize + tileSpacing;
            targetWidth = boardWidth * totalTileSize;
            targetHeight = boardHeight * totalTileSize;
            
            Debug.Log($"[CameraScaler] AdjustToBoard called: " +
                      $"Board {boardWidth}x{boardHeight}, " +
                      $"TileSize: {tileSize}, Spacing: {tileSpacing}, " +
                      $"Calculated target: {targetWidth}x{targetHeight}");
            
            AdjustCamera();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cam == null) cam = GetComponent<Camera>();
        }
#endif
    }
}
