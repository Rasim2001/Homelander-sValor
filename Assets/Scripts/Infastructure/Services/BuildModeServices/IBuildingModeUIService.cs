using UI.GameplayUI.BuildingCoinsUIManagement;
using UnityEngine.UI;

namespace Infastructure.Services.BuildModeServices
{
    public interface IBuildingModeUIService
    {
        void Initialize(Image fillImag);

        void NavigateSelection(int direction);
        void SelectBuildItem();
        void UpdateProgressUI(float progress);
        void ResetProgress();
    }
}