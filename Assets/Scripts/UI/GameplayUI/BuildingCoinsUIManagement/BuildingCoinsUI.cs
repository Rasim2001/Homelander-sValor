using System;
using DG.Tweening;
using Infastructure.Services.HudFader;
using Infastructure.Services.PlayerProgressService;
using Infastructure.StaticData.Coins;
using Infastructure.StaticData.StaticDataService;
using TMPro;
using UI.GameplayUI.HudUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace UI.GameplayUI.BuildingCoinsUIManagement
{
    public class BuildingCoinsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private GameObject _showableObject;
        [SerializeField] private Image _iconImage;

        private IStaticDataService _staticData;
        private IHudFaderService _hudFaderService;
        private IPersistentProgressService _persistentProgressService;
        private int _coinsCount;


        [Inject]
        public void Construct(
            IStaticDataService staticDataService,
            IHudFaderService hudFaderService,
            IPersistentProgressService persistentProgressService)
        {
            _persistentProgressService = persistentProgressService;
            _staticData = staticDataService;
            _hudFaderService = hudFaderService;
        }


        public void Show()
        {
            _hudFaderService.Show(HudId.Coins);

            UpdateCoinSprite();
            _showableObject.SetActive(true);
        }

        public void Hide()
        {
            _hudFaderService.DoFade(HudId.Coins);
            _showableObject.SetActive(false);
        }

        public void Show(bool value)
        {
            if (value)
                _hudFaderService.Show(HudId.Coins);
            else
                _hudFaderService.DoFade(HudId.Coins);

            UpdateCoinSprite();

            _showableObject.SetActive(value);
        }

        public void UpdateCoinsUI(int amount)
        {
            _coinsCount = amount;
            UpdateCoinSprite();

            _coinsText.text = $"X{amount}";
        }

        public void UpdateCoinSprite()
        {
            CoinsStaticData coinData = _staticData.CoinsStaticData;

            _iconImage.sprite = _persistentProgressService.PlayerProgress.CoinData.IsEnoughCoins(_coinsCount)
                ? coinData.ActiveCoinsSprite
                : coinData.CoinsSprite;
        }

        public void PlaySpendAnimation(int amount, Transform moveTarget)
        {
            GameObject prefab = _staticData.CoinsStaticData.CoinPrefab;

            for (int i = amount; i > 0; i--)
            {
                GameObject coin = Instantiate(prefab, transform.position, Quaternion.identity); // TODO:

                Vector3 randomOffset1 =
                    transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 2f), 0f);
                Vector3 randomOffset2 =
                    moveTarget.position + new Vector3(0, Random.Range(-1f, 1f), 0f);

                coin.transform.DOPath(
                        new[] { randomOffset1, randomOffset2, moveTarget.position },
                        1,
                        PathType.CatmullRom
                    )
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => { Destroy(coin.gameObject); }); //TODO:
            }
        }
    }
}