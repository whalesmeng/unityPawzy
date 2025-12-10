using UnityEngine;
using UnityEngine.UI;

namespace PawzyPop.UI
{
    public class PausePopup : PopupBase
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        [Header("Sound Toggles")]
        [SerializeField] private Toggle bgmToggle;
        [SerializeField] private Toggle sfxToggle;

        [Header("Volume Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        protected override void Awake()
        {
            base.Awake();

            // 按钮绑定
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);

            // 开关绑定
            if (bgmToggle != null)
                bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
            if (sfxToggle != null)
                sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);

            // 滑块绑定
            if (bgmSlider != null)
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            if (sfxSlider != null)
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        public override void Show()
        {
            base.Show();
            Time.timeScale = 0f;
            LoadSettings();
        }

        public override void Hide()
        {
            base.Hide();
            Time.timeScale = 1f;
        }

        private void LoadSettings()
        {
            if (Audio.AudioManager.Instance == null) return;

            if (bgmToggle != null)
                bgmToggle.isOn = !Audio.AudioManager.Instance.IsBGMMuted;
            if (sfxToggle != null)
                sfxToggle.isOn = !Audio.AudioManager.Instance.IsSFXMuted;
            if (bgmSlider != null)
                bgmSlider.value = Audio.AudioManager.Instance.GetBGMVolume();
            if (sfxSlider != null)
                sfxSlider.value = Audio.AudioManager.Instance.GetSFXVolume();
        }

        private void OnResumeClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
        }

        private void OnRestartClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            
            if (Core.LevelLoader.Instance != null)
            {
                Core.LevelLoader.Instance.ReloadCurrentLevel();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }

        private void OnMenuClicked()
        {
            if (Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlayButtonClick();

            Hide();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        private void OnBGMToggleChanged(bool isOn)
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetBGMMuted(!isOn);
            }
        }

        private void OnSFXToggleChanged(bool isOn)
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetSFXMuted(!isOn);
                if (isOn)
                {
                    Audio.AudioManager.Instance.PlayButtonClick();
                }
            }
        }

        private void OnBGMVolumeChanged(float value)
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetBGMVolume(value);
            }
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetSFXVolume(value);
            }
        }

        public override void OnBackgroundClicked()
        {
            // 暂停界面点击背景不关闭
        }
    }
}
