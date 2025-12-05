using UnityEngine;

namespace UI.GameplayUI
{
    public class FlagHintsDisplay : HintsDisplayBase
    {
        [SerializeField] private GameObject _target;

        public override void ShowHints()
        {
            if (!TutorialProgressService.TutorialStarted && !buildingModeService.IsBuildingState)
                Show(true);
        }

        protected override void Show(bool value) =>
            _target.SetActive(value);
    }
}