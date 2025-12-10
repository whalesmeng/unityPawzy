using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PawzyPop.UI
{
    public class MenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private string gameSceneName = "Game";

        private void Start()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked - not implemented yet");
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
