using Infastructure.Services.HudFader;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.Pool;
using Loots;
using UI.GameplayUI.HudUI;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerPickUpLoot : MonoBehaviour
    {
        [SerializeField] private ObserverTrigger _observerTrigger;

        private IPersistentProgressService _progressService;
        private IPoolObjects<CoinLoot> _pool;
        private IHudFaderService _hudFaderService;

        [Inject]
        public void Construct(IPersistentProgressService progressService, IPoolObjects<CoinLoot> pool,
            IHudFaderService hudFaderService)
        {
            _hudFaderService = hudFaderService;
            _progressService = progressService;
            _pool = pool;
        }

        private void Start() =>
            _observerTrigger.OnTriggerEnter += Enter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= Enter;

        private void Enter()
        {
            CoinLoot coinLoot = _observerTrigger.CurrentCollider.GetComponent<CoinLoot>();

            _hudFaderService.Show(HudId.Coins);
            _hudFaderService.DoFade(HudId.Coins);

            _progressService.PlayerProgress.CoinData.Collect(1, coinLoot.UniqueId);
            _pool.ReturnObjectToPool(coinLoot);
        }
    }
}