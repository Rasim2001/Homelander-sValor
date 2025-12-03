using System.Linq;
using Infastructure.Data;
using Infastructure.Services.SaveLoadService;
using UnityEngine;

namespace Loots
{
    public class CoinLoot : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private Animator _animator;
        [field: SerializeField] public string UniqueId { get; set; }

        public void LoadProgress(PlayerProgress progress)
        {
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if (string.IsNullOrEmpty(UniqueId) || !gameObject.activeInHierarchy)
                return;

            LootData savedData = progress.CoinData.LootDatas.FirstOrDefault(x => x.UniqueId.Contains(UniqueId));
            if (savedData == null)
                progress.CoinData.LootDatas.Add(new LootData(transform.position.AsVectorData(), UniqueId));
        }

        private void OnDisable()
        {
            UniqueId = string.Empty;
            _animator.speed = 1;
        }
    }
}