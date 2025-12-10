using UnityEngine;

namespace PawzyPop.Core
{
    [CreateAssetMenu(fileName = "NewTileType", menuName = "PawzyPop/Tile Type")]
    public class TileType : ScriptableObject
    {
        public string typeName;
        public Sprite sprite;
        public Color color = Color.white;
        public int scoreValue = 10;

        [Header("Audio")]
        public AudioClip matchSound;
    }
}
