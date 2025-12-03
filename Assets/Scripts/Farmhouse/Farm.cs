using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Farmhouse
{
    public class Farm : MonoBehaviour
    {
        /*[SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int coinsToSpawn = 5;
        [SerializeField] private float spawnRadius = 1f;

        [SerializeField] private float collectDistance = 0.5f;
        [SerializeField] private float arcHeight = 0.5f;
        [SerializeField] private float waveAmplitude = 0.2f; */

        [Button]
        public void GetFarm()
        {
            PlayerMove playerMove = FindObjectOfType<PlayerMove>();
        }
    }
}