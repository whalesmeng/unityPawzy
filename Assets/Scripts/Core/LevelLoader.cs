using UnityEngine;
using PawzyPop.Data;

namespace PawzyPop.Core
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance { get; private set; }

        [SerializeField] private int currentLevelId = 1;
        
        public LevelData CurrentLevel { get; private set; }

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
            LoadLevel(currentLevelId);
        }

        public void LoadLevel(int levelId)
        {
            currentLevelId = levelId;
            string path = $"Levels/level_{levelId}";
            TextAsset jsonFile = Resources.Load<TextAsset>(path);

            if (jsonFile != null)
            {
                CurrentLevel = JsonUtility.FromJson<LevelData>(jsonFile.text);
                ApplyLevelSettings();
                Debug.Log($"Loaded level {levelId}: {CurrentLevel.moves} moves, target {CurrentLevel.targetValue}");
            }
            else
            {
                Debug.LogWarning($"Level {levelId} not found, using defaults");
                CurrentLevel = CreateDefaultLevel(levelId);
                ApplyLevelSettings();
            }
        }

        private void ApplyLevelSettings()
        {
            if (GameManager.Instance != null && CurrentLevel != null)
            {
                GameManager.Instance.LoadLevel(
                    CurrentLevel.levelId,
                    CurrentLevel.moves,
                    CurrentLevel.targetValue
                );
            }
        }

        private LevelData CreateDefaultLevel(int levelId)
        {
            return new LevelData
            {
                levelId = levelId,
                boardSize = new int[] { 6, 6 },
                moves = 20,
                targetType = "score",
                targetValue = 1000,
                elements = new string[] { "shiba", "corgi", "golden", "husky" },
                starThresholds = new int[] { 1000, 1500, 2000 }
            };
        }

        public void LoadNextLevel()
        {
            LoadLevel(currentLevelId + 1);
        }

        public void ReloadCurrentLevel()
        {
            LoadLevel(currentLevelId);
        }

        public int GetStarRating(int score)
        {
            if (CurrentLevel == null || CurrentLevel.starThresholds == null)
                return 0;

            int stars = 0;
            foreach (int threshold in CurrentLevel.starThresholds)
            {
                if (score >= threshold)
                    stars++;
            }
            return stars;
        }
    }
}
