using System;
using Player.Orders;

namespace Infastructure.Services.Cards
{
    public interface ICardSpawnService
    {
        void ShowCardsWindow(OrderMarker orderMarker, Action onCardSelected = null);
    }
}