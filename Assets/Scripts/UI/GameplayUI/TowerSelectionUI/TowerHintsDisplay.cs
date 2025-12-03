using UI.GameplayUI.TowerSelectionUI.Tower;
using UnityEngine;

namespace UI.GameplayUI.TowerSelectionUI
{
    public class TowerHintsDisplay : HintsDisplayBase
    {
        [SerializeField] private GameObject[] _targets;

        private OrderSelectionInput _orderSelectionInput;

        private void Start() =>
            _orderSelectionInput = GetComponentInChildren<OrderSelectionInput>();

        public override void ShowHints()
        {
            _orderSelectionInput.ReInitialize();

            base.ShowHints();
        }

        protected override void Show(bool value)
        {
            foreach (GameObject target in _targets)
                target.SetActive(value);
        }
    }
}