using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PawzyPop.Data
{
    [Serializable]
    public class PlayerSaveData
    {
        public int currentLevel = 1;
        public int[] levelStars;        // 每关获得的星数
        public int[] levelHighScores;   // 每关最高分
        public int coins = 0;
        public int diamonds = 0;
        public int hammerCount = 0;
        public int refreshCount = 0;
        public int extraMovesCount = 0;
        public string lastPlayDate;
        public int consecutiveLoginDays = 0;
        public int totalMatchCount = 0;
        public int totalLevelCompleted = 0;

        public PlayerSaveData()
        {
            levelStars = new int[100];
            levelHighScores = new int[100];
            lastPlayDate = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_FILE_NAME = "pawzy_save.dat";
        private const string ENCRYPTION_KEY = "PawzyPop2025Key!"; // 16字符

        public PlayerSaveData Data { get; private set; }

        public event Action OnDataLoaded;
        public event Action OnDataSaved;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region Save/Load

        public void SaveData()
        {
            try
            {
                string json = JsonUtility.ToJson(Data, true);
                string encrypted = Encrypt(json);
                string path = GetSavePath();
                
                File.WriteAllText(path, encrypted);
                Debug.Log($"[Save] 数据已保存: {path}");
                
                OnDataSaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] 保存失败: {e.Message}");
            }
        }

        public void LoadData()
        {
            string path = GetSavePath();

            if (File.Exists(path))
            {
                try
                {
                    string encrypted = File.ReadAllText(path);
                    string json = Decrypt(encrypted);
                    Data = JsonUtility.FromJson<PlayerSaveData>(json);
                    Debug.Log($"[Save] 数据已加载: 关卡{Data.currentLevel}, 金币{Data.coins}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Save] 加载失败，创建新存档: {e.Message}");
                    Data = new PlayerSaveData();
                }
            }
            else
            {
                Debug.Log("[Save] 未找到存档，创建新存档");
                Data = new PlayerSaveData();
            }

            CheckDailyLogin();
            OnDataLoaded?.Invoke();
        }

        public void DeleteSaveData()
        {
            string path = GetSavePath();
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("[Save] 存档已删除");
            }
            Data = new PlayerSaveData();
            OnDataLoaded?.Invoke();
        }

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }

        #endregion

        #region Game Progress

        public void CompleteLevel(int levelId, int score, int stars)
        {
            if (levelId < 1 || levelId > Data.levelStars.Length) return;

            int index = levelId - 1;

            // 更新最高分
            if (score > Data.levelHighScores[index])
            {
                Data.levelHighScores[index] = score;
            }

            // 更新星数（只保留最高）
            if (stars > Data.levelStars[index])
            {
                Data.levelStars[index] = stars;
            }

            // 解锁下一关
            if (levelId >= Data.currentLevel)
            {
                Data.currentLevel = levelId + 1;
            }

            Data.totalLevelCompleted++;
            SaveData();
        }

        public int GetLevelStars(int levelId)
        {
            if (levelId < 1 || levelId > Data.levelStars.Length) return 0;
            return Data.levelStars[levelId - 1];
        }

        public int GetLevelHighScore(int levelId)
        {
            if (levelId < 1 || levelId > Data.levelHighScores.Length) return 0;
            return Data.levelHighScores[levelId - 1];
        }

        public bool IsLevelUnlocked(int levelId)
        {
            return levelId <= Data.currentLevel;
        }

        public int GetTotalStars()
        {
            int total = 0;
            foreach (int stars in Data.levelStars)
            {
                total += stars;
            }
            return total;
        }

        #endregion

        #region Currency

        public void AddCoins(int amount)
        {
            Data.coins += amount;
            SaveData();
        }

        public bool SpendCoins(int amount)
        {
            if (Data.coins >= amount)
            {
                Data.coins -= amount;
                SaveData();
                return true;
            }
            return false;
        }

        public void AddDiamonds(int amount)
        {
            Data.diamonds += amount;
            SaveData();
        }

        public bool SpendDiamonds(int amount)
        {
            if (Data.diamonds >= amount)
            {
                Data.diamonds -= amount;
                SaveData();
                return true;
            }
            return false;
        }

        #endregion

        #region Items

        public void AddItem(string itemType, int count = 1)
        {
            switch (itemType.ToLower())
            {
                case "hammer":
                    Data.hammerCount += count;
                    break;
                case "refresh":
                    Data.refreshCount += count;
                    break;
                case "extramoves":
                    Data.extraMovesCount += count;
                    break;
            }
            SaveData();
        }

        public bool UseItem(string itemType)
        {
            switch (itemType.ToLower())
            {
                case "hammer":
                    if (Data.hammerCount > 0)
                    {
                        Data.hammerCount--;
                        SaveData();
                        return true;
                    }
                    break;
                case "refresh":
                    if (Data.refreshCount > 0)
                    {
                        Data.refreshCount--;
                        SaveData();
                        return true;
                    }
                    break;
                case "extramoves":
                    if (Data.extraMovesCount > 0)
                    {
                        Data.extraMovesCount--;
                        SaveData();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public int GetItemCount(string itemType)
        {
            switch (itemType.ToLower())
            {
                case "hammer": return Data.hammerCount;
                case "refresh": return Data.refreshCount;
                case "extramoves": return Data.extraMovesCount;
                default: return 0;
            }
        }

        #endregion

        #region Daily Login

        private void CheckDailyLogin()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            
            if (Data.lastPlayDate != today)
            {
                DateTime lastDate;
                if (DateTime.TryParse(Data.lastPlayDate, out lastDate))
                {
                    TimeSpan diff = DateTime.Now - lastDate;
                    
                    if (diff.Days == 1)
                    {
                        // 连续登录
                        Data.consecutiveLoginDays++;
                    }
                    else if (diff.Days > 1)
                    {
                        // 中断了
                        Data.consecutiveLoginDays = 1;
                    }
                }
                else
                {
                    Data.consecutiveLoginDays = 1;
                }

                Data.lastPlayDate = today;
                SaveData();

                Debug.Log($"[Save] 连续登录: {Data.consecutiveLoginDays} 天");
            }
        }

        public int GetConsecutiveLoginDays()
        {
            return Data.consecutiveLoginDays;
        }

        #endregion

        #region Encryption

        private string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string Decrypt(string cipherText)
        {
            byte[] key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
