using System;
using Grid;
using Infastructure.StaticData.Building;
using UI.GameplayUI;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;

namespace Infastructure.Services.BuildModeServices
{
    public interface IBuildingModeService
    {
        void Initialize(
            MoveBuildingUI moveBuildingUI,
            BuildHintsUI buildHints,
            SpriteRenderer ghostSpriteRender,
            BuildingCoinsUI buildingCoinsUI, 
            SpeachBuble speachBuble);

        bool CanOccupyCells(int currentPosition);
        void RegistBuild();
        void MoveGhost(Vector3 position);
        void PaintGhost(int currentPosition);
        void StartBuildingState();
        void StopBuildingState();
        bool IsBuildingState { get; }
        void SubscribeUpdates();
        void Cleanup();
        event Action OnBuildingStateChanged;
    }
}