using System;
using System.Collections.Generic;
using BuildProcessManagement;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AssetProvider;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Windows;
using Player.Orders;
using UI.GameplayUI.Card;
using UI.Windows;

namespace Infastructure.Services.Cards
{
    public class CardSpawnService : ICardSpawnService
    {
        private readonly IGameUIFactory _gameUIFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IAssetProviderService _assetProviderService;


        public CardSpawnService(
            IGameUIFactory gameUIFactory,
            IStaticDataService staticDataService,
            IAssetProviderService assetProviderService)
        {
            _gameUIFactory = gameUIFactory;
            _staticDataService = staticDataService;
            _assetProviderService = assetProviderService;
        }

        public void ShowCardsWindow(OrderMarker orderMarker, Action onCardSelected = null)
        {
            List<CardItemUI> cards = new List<CardItemUI>();

            CardWindow cardWindow = _gameUIFactory.CreateCardsWindow(WindowId.CardsWindow).GetComponent<CardWindow>();
            CardContentMarker cardContentMarker = cardWindow.GetComponentInChildren<CardContentMarker>();

            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            BuildingUpgradeData buildingUpgradeData =
                _staticDataService.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId,
                    buildInfo.CardKey);

            foreach (CardData cardData in buildingUpgradeData.Cards)
            {
                CardItemUI cardItemUI = _assetProviderService
                    .Instantiate(cardData.CardPrefabUI, cardContentMarker.transform)
                    .GetComponent<CardItemUI>();

                cardItemUI.CardId = cardData.CardId;
                cards.Add(cardItemUI);
            }

            cardWindow.RegisterCards(cards, orderMarker, onCardSelected);
        }
    }
}