using Infastructure.Services.PlayerProgressService;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsCountText;

        private IPersistentProgressService _progressService;

        [Inject]
        public void Construct(IPersistentProgressService progressService) =>
            _progressService = progressService;


        private void Start()
        {
            UpdateCoins();

            _progressService.PlayerProgress.CoinData.Changed += UpdateCoins;
        }


        private void OnDestroy() =>
            _progressService.PlayerProgress.CoinData.Changed -= UpdateCoins;

        private void UpdateCoins() =>
            _coinsCountText.text = _progressService.PlayerProgress.CoinData.NumberOfCoins.ToString();
    }
}