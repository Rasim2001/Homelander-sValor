using Infastructure.Services.PlayerProgressService;
using Player;
using Units;
using UnityEngine;
using Zenject;

namespace Enemy.Camp
{
    public class EnemyCampDestruction : MonoBehaviour
    {
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private Health _health;

        private IPersistentProgressService _progressService;

        [Inject]
        public void Consturct(IPersistentProgressService progressService) =>
            _progressService = progressService;

        private void Start() =>
            _health.OnDeathHappened += Destruct;

        private void OnDestroy() =>
            _health.OnDeathHappened -= Destruct;

        private void Destruct()
        {
            Destroy(gameObject);
            _progressService.PlayerProgress.KillData.ClearedEnemyCamps.Add(_uniqueId.Id);
        }
    }
}