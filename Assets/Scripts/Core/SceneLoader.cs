using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PawzyPop.Core
{
    /// <summary>
    /// 场景加载管理器，提供场景切换功能
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float minLoadTime = 0.5f;

        private bool isLoading = false;

        /// <summary>
        /// 场景加载开始事件
        /// </summary>
        public event Action<string> OnLoadStarted;

        /// <summary>
        /// 场景加载进度事件 (0-1)
        /// </summary>
        public event Action<float> OnLoadProgress;

        /// <summary>
        /// 场景加载完成事件
        /// </summary>
        public event Action<string> OnLoadCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[SceneLoader] Initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 加载场景（同步）
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning($"[SceneLoader] 正在加载中，忽略请求: {sceneName}");
                return;
            }

            Debug.Log($"[SceneLoader] LoadScene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        public void LoadSceneAsync(string sceneName, Action onComplete = null)
        {
            if (isLoading)
            {
                Debug.LogWarning($"[SceneLoader] 正在加载中，忽略请求: {sceneName}");
                return;
            }

            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
        }

        /// <summary>
        /// 加载主菜单场景
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene("MainMenu");
        }

        /// <summary>
        /// 异步加载主菜单场景
        /// </summary>
        public void LoadMainMenuAsync(Action onComplete = null)
        {
            LoadSceneAsync("MainMenu", onComplete);
        }

        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        public void ReloadCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }

        /// <summary>
        /// 获取当前场景名称
        /// </summary>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action onComplete)
        {
            isLoading = true;
            OnLoadStarted?.Invoke(sceneName);
            Debug.Log($"[SceneLoader] 开始异步加载: {sceneName}");

            float startTime = Time.realtimeSinceStartup;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                // 进度 0-0.9 表示加载中，0.9 表示加载完成等待激活
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                OnLoadProgress?.Invoke(progress);

                // 加载完成且满足最小加载时间
                if (asyncLoad.progress >= 0.9f)
                {
                    float elapsed = Time.realtimeSinceStartup - startTime;
                    if (elapsed >= minLoadTime)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            isLoading = false;
            OnLoadCompleted?.Invoke(sceneName);
            Debug.Log($"[SceneLoader] 加载完成: {sceneName}");

            onComplete?.Invoke();
        }

        /// <summary>
        /// 检查场景是否存在于 Build Settings 中
        /// </summary>
        public bool IsSceneInBuildSettings(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
