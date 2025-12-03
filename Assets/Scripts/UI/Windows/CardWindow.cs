using System;
using System.Collections.Generic;
using Infastructure.Services.Cards;
using Infastructure.Services.ECSInput;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using Player.Orders;
using UI.GameplayUI.Card;
using Zenject;

namespace UI.Windows
{
    public class CardWindow : WindowBase, IEcsWatcherWindow
    {
        private IPauseService _pauseService;
        private ICardTrackerService _trackerService;
        private IOrderSelectionUIService _orderSelectionUIService;
        private IBuilderCommandExecutor _builderCommandExecutor;
        private IInputService _inputService;
        private IEcsWatchersService _ecsWatchersService;

        private List<CardItemUI> _cards;
        private OrderMarker _orderMarker;
        private Action _onCardSelected;

        private int _index = 0;

        [Inject]
        public void Construct(
            IPauseService pauseService,
            ICardTrackerService trackerService,
            IOrderSelectionUIService orderSelectionUIService,
            IBuilderCommandExecutor builderCommandExecutor,
            IInputService inputService,
            IEcsWatchersService ecsWatchersService)
        {
            _pauseService = pauseService;
            _trackerService = trackerService;
            _orderSelectionUIService = orderSelectionUIService;
            _builderCommandExecutor = builderCommandExecutor;
            _inputService = inputService;
            _ecsWatchersService = ecsWatchersService;
        }

        public void RegisterCards(List<CardItemUI> cards, OrderMarker orderMarker, Action onCardSelected)
        {
            _cards = cards;
            _orderMarker = orderMarker;
            _onCardSelected = onCardSelected;

            SelectCardItem();
        }


        private void Update()
        {
            if (_cards == null)
                return;

            if (_orderSelectionUIService.LeftArrowPressed && _index > 0)
            {
                _index--;

                SelectCardItem();
            }
            else if (_orderSelectionUIService.RightArrowPressed && _index < _cards.Count - 1)
            {
                _index++;

                SelectCardItem();
            }
            else if (_inputService.ExecuteOrderPressedDown)
            {
                _builderCommandExecutor.StartBuild(_orderMarker);
                _onCardSelected?.Invoke();

                Destroy(gameObject);
            }
        }

        protected override void Initialize()
        {
            _pauseService.TurnOn();
        }

        protected override void Cleanup()
        {
            _pauseService.TurnOff();

            _ecsWatchersService.Release(this);
        }

        private void SelectCardItem()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_index == i)
                {
                    _cards[i].Select();
                    _trackerService.CardId = _cards[i].CardId;
                }
                else
                    _cards[i].Deselect();
            }
        }

        public void EcsCancel() =>
            Destroy(gameObject);
    }
}