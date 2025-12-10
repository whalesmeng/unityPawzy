using UnityEngine;
using System;

namespace PawzyPop.Data
{
    [Serializable]
    public class LevelData
    {
        public int levelId;
        public int[] boardSize = new int[] { 6, 6 };
        public int moves;
        public string targetType;
        public int targetValue;
        public string[] elements;
        public int[] starThresholds;
    }

    [Serializable]
    public class LevelDataList
    {
        public LevelData[] levels;
    }

    [CreateAssetMenu(fileName = "LevelConfig", menuName = "PawzyPop/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        public int levelId;
        public int width = 6;
        public int height = 6;
        public int moves = 20;
        public int targetScore = 1000;
        public int[] starThresholds = new int[] { 1000, 1500, 2000 };
    }
}
