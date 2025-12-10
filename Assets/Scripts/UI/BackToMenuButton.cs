using UnityEngine;
using UnityEngine.UI;
using PawzyPop.Core;

namespace PawzyPop.UI
{
    /// <summary>
    /// 返回主菜单按钮组件
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class BackToMenuButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            Debug.Log("[BackToMenuButton] 返回主菜单");
            
            // 恢复时间缩放
            Time.timeScale = 1f;

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnBackClicked);
            }
        }
    }
}
