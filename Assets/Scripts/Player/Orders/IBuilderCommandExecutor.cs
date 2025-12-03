using System;
using UI.GameplayUI.SpeachBubleUI;

namespace Player.Orders
{
    public interface IBuilderCommandExecutor
    {
        void Initialize(SpeachBuble speachBuble, SelectUnitArrow selectUnitArrow);
        void StartBuild(OrderMarker orderMarker);
        void StartBuildAfterBuildingMode(OrderMarker orderMarker);
        Action OnBuildHappened { get; set; }
        void StartHarvest(OrderMarker orderMarker);
    }
}