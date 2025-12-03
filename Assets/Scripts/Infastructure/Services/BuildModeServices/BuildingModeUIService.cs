using DG.Tweening;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.TowerSelectionUI;
using UnityEngine;
using UnityEngine.UI;

namespace Infastructure.Services.BuildModeServices
{
    public class BuildingModeUIService : IBuildingModeUIService
    {
        private readonly IBuildingModeConfigurationService _buildingModeConfigurationService;

        private Image _fillImage;

        public BuildingModeUIService(IBuildingModeConfigurationService buildingModeConfigurationService) =>
            _buildingModeConfigurationService = buildingModeConfigurationService;

        public void Initialize(Image fillImag) =>
            _fillImage = fillImag;

        public void NavigateSelection(int direction)
        {
            _buildingModeConfigurationService.CorrectIndex += direction;

            SelectBuildItem();
        }


        public void SelectBuildItem()
        {
            for (int i = 0; i < _buildingModeConfigurationService.BuildingTypeInfos.Count; i++)
            {
                Transform item = _buildingModeConfigurationService.BuildingTypeInfos[i].MoveTransform;
                Image image = item.GetComponent<Image>();

                item.DOKill();
                image.DOKill();
                item.DOScale(Vector3.one, 0.1f);

                Color newColor = Color.white;
                newColor.a = 170 / 255f;
                image.color = newColor;

                if (i == _buildingModeConfigurationService.CorrectIndex)
                {
                    item.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.2f);
                    image.DOColor(Color.white, 0.2f);
                }
            }
        }

        public void UpdateProgressUI(float progress) =>
            _fillImage.fillAmount = progress;

        public void ResetProgress()
        {
            _fillImage.fillAmount = 0;

            foreach (BuildingTypeInfo item in _buildingModeConfigurationService.BuildingTypeInfos)
            {
                Image image = item.MoveTransform.GetComponent<Image>();

                item.MoveTransform.DOKill();
                image.DOKill();
            }
        }
    }
}