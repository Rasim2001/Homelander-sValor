using System;
using DG.Tweening;
using Player.Orders;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayUI.TowerSelectionUI.Tower
{
    public class OrderSelectionInput : SelectionUIBase
    {
        [SerializeField] private KeyboardUI _keyboardUI;
        [SerializeField] private MoveOrdersUI _moveOrdersUI;
        [SerializeField] private OrderSelectionInfos _orderSelectionInfos;
        [SerializeField] private OrderSelectionUI _mainSelectionUI;
        [SerializeField] private Transform[] _moveableItems;
        [SerializeField] private KeyboardHintsUI _keyboardHintsUI;
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;
        [SerializeField] private OrderMarker _orderMarker;


        private void Start()
        {
            OrderSelectionId selectionId =
                _orderSelectionInfos.Items[_orderSelectionInfos.CorrectIndex].OrderSelectionId;

            _mainSelectionUI.OrderSelectionId = selectionId;

            SelectItem();
        }


        private void Update()
        {
            if (_pauseService.IsPaused)
                return;

            if (orderSelectionUIService.LeftArrowPressed && _moveOrdersUI.CanMoveLeft() &&
                _moveOrdersUI.gameObject.activeInHierarchy)
            {
                _moveOrdersUI.MoveLeft();
                _orderSelectionInfos.CorrectIndex--;

                RegistTower();
                SelectItem();
            }

            else if (orderSelectionUIService.RightArrowPressed && _moveOrdersUI.CanMoveRight() &&
                     _moveOrdersUI.gameObject.activeInHierarchy)
            {
                _moveOrdersUI.MoveRight();
                _orderSelectionInfos.CorrectIndex++;

                RegistTower();
                SelectItem();
            }
        }

        private void LateUpdate()
        {
            if (orderSelectionUIService.LeftArrowPressed && _keyboardUI.isActiveAndEnabled)
                _keyboardUI.ClickOn(0);
            else if (orderSelectionUIService.RightArrowPressed && _keyboardUI.isActiveAndEnabled)
                _keyboardUI.ClickOn(2);
        }

        public void ReInitialize()
        {
            _orderSelectionInfos.CorrectIndex = 0;

            _moveOrdersUI.ReInitialize();

            RegistTower();
            SelectItem();
        }


        private void RegistTower()
        {
            OrderSelectionId selectionId =
                _orderSelectionInfos.Items[_orderSelectionInfos.CorrectIndex].OrderSelectionId;

            _mainSelectionUI.OrderSelectionId = selectionId;

            if (_orderMarker.IsMarkered || _orderMarker.IsStarted)
                return;

            if (selectionId == OrderSelectionId.LevelUp)
                _buildingCoinsUI.Show();
            else
                _buildingCoinsUI.Hide();
        }

        private void SelectItem()
        {
            _keyboardHintsUI.UpdateText(_orderSelectionInfos.CorrectIndex);

            for (int i = 0; i < _moveableItems.Length; i++)
            {
                Transform item = _moveableItems[i];
                Image image = item.GetComponent<Image>();

                item.DOKill();
                image.DOKill();
                item.DOScale(Vector3.one, 0.1f);

                Color newColor = Color.white;
                newColor.a = 170 / 255f;
                image.color = newColor;

                if (i == _orderSelectionInfos.CorrectIndex)
                {
                    item.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.2f);
                    image.DOColor(Color.white, 0.2f);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (Transform item in _moveableItems)
            {
                Image image = item.GetComponent<Image>();

                item.DOKill();
                image.DOKill();
            }
        }
    }
}