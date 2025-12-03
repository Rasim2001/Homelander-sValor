using UnityEngine;
using UnityEngine.UI;

namespace Infastructure.StaticData.Coins
{
    [CreateAssetMenu(fileName = "CoinsStaticData", menuName = "StaticData/CoinsData")]
    public class CoinsStaticData : ScriptableObject
    {
        [Header("Prefab")]
        public Image CoingImagePrefab;
        public GameObject CoinPrefab;

        [Header("Sprites")]
        public Sprite CoinsSprite;
        public Sprite ActiveCoinsSprite;

        [Header("Settings")]
        public float SpendCoinsAnimationDuration;
    }
}