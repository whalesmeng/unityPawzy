using UnityEngine;
using System.Collections.Generic;

namespace PawzyPop.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM Clips")]
        [SerializeField] private AudioClip menuBGM;
        [SerializeField] private AudioClip gameBGM;
        [SerializeField] private AudioClip winBGM;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip matchSound;
        [SerializeField] private AudioClip swapSound;
        [SerializeField] private AudioClip invalidSwapSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip starSound;
        [SerializeField] private AudioClip loseSound;

        [Header("Combo Sounds")]
        [SerializeField] private AudioClip[] comboSounds; // 连击音效

        [Header("Settings")]
        [SerializeField] private float bgmVolume = 0.5f;
        [SerializeField] private float sfxVolume = 1f;

        private const string BGM_VOLUME_KEY = "BGMVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string BGM_MUTED_KEY = "BGMMuted";
        private const string SFX_MUTED_KEY = "SFXMuted";

        public bool IsBGMMuted { get; private set; }
        public bool IsSFXMuted { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            ApplySettings();
        }

        #region BGM Control

        public void PlayMenuBGM()
        {
            PlayBGM(menuBGM);
        }

        public void PlayGameBGM()
        {
            PlayBGM(gameBGM);
        }

        public void PlayWinBGM()
        {
            PlayBGM(winBGM);
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || bgmSource == null) return;

            if (bgmSource.clip == clip && bgmSource.isPlaying)
                return;

            bgmSource.clip = clip;
            bgmSource.volume = IsBGMMuted ? 0 : bgmVolume;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }

        public void PauseBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Pause();
            }
        }

        public void ResumeBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.UnPause();
            }
        }

        #endregion

        #region SFX Control

        public void PlayMatch(int comboCount = 0)
        {
            if (comboSounds != null && comboSounds.Length > 0 && comboCount > 0)
            {
                int index = Mathf.Min(comboCount - 1, comboSounds.Length - 1);
                PlaySFX(comboSounds[index]);
            }
            else
            {
                PlaySFX(matchSound);
            }
        }

        public void PlaySwap()
        {
            PlaySFX(swapSound);
        }

        public void PlayInvalidSwap()
        {
            PlaySFX(invalidSwapSound);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickSound);
        }

        public void PlayStar()
        {
            PlaySFX(starSound);
        }

        public void PlayLose()
        {
            PlaySFX(loseSound);
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null || IsSFXMuted) return;

            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
        {
            if (clip == null || IsSFXMuted) return;

            AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
        }

        #endregion

        #region Settings

        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            if (bgmSource != null && !IsBGMMuted)
            {
                bgmSource.volume = bgmVolume;
            }
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
            PlayerPrefs.Save();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
        }

        public void ToggleBGM()
        {
            IsBGMMuted = !IsBGMMuted;
            if (bgmSource != null)
            {
                bgmSource.volume = IsBGMMuted ? 0 : bgmVolume;
            }
            PlayerPrefs.SetInt(BGM_MUTED_KEY, IsBGMMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ToggleSFX()
        {
            IsSFXMuted = !IsSFXMuted;
            PlayerPrefs.SetInt(SFX_MUTED_KEY, IsSFXMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetBGMMuted(bool muted)
        {
            IsBGMMuted = muted;
            if (bgmSource != null)
            {
                bgmSource.volume = IsBGMMuted ? 0 : bgmVolume;
            }
            PlayerPrefs.SetInt(BGM_MUTED_KEY, IsBGMMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetSFXMuted(bool muted)
        {
            IsSFXMuted = muted;
            PlayerPrefs.SetInt(SFX_MUTED_KEY, IsSFXMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public float GetBGMVolume() => bgmVolume;
        public float GetSFXVolume() => sfxVolume;

        private void LoadSettings()
        {
            bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            IsBGMMuted = PlayerPrefs.GetInt(BGM_MUTED_KEY, 0) == 1;
            IsSFXMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, 0) == 1;
        }

        private void ApplySettings()
        {
            if (bgmSource != null)
            {
                bgmSource.volume = IsBGMMuted ? 0 : bgmVolume;
            }
        }

        #endregion
    }
}
