using System;
using DG.Tweening;
using UnityEngine;

namespace UI.GameplayUI.TowerSelectionUI.MoveItems
{
    public class MoveUIItemsBase : MonoBehaviour
    {
        [SerializeField] private int _stepMove;
        [SerializeField] private RectTransform _itemContainer;

        protected int NumberOfItems;

        private int _allStepMovements;
        private Tween _moveTween;
        private float _remainingMoveOffsetX;

        private void Awake() =>
            NumberOfItems = _itemContainer.childCount;


        public virtual void ReInitialize()
        {
            _allStepMovements = 0;
            _itemContainer.localPosition = new Vector3(-48, _itemContainer.localPosition.y);
        }

        public bool CanMoveLeft() =>
            _allStepMovements != 0;

        public bool CanMoveRight() =>
            _allStepMovements / _stepMove != 1 - NumberOfItems;

        public void MoveLeft(Action onCompleted = null)
        {
            _allStepMovements += _stepMove;
            Move(onCompleted);
        }

        public void MoveRight(Action onCompleted = null)
        {
            _allStepMovements -= _stepMove;
            Move(onCompleted);
        }


        private void Move(Action onCompleted = null)
        {
            if (_moveTween.IsActive())
                _moveTween.Kill();

            _moveTween = _itemContainer
                .DOLocalMoveX(_allStepMovements - 48, 0.1f) //50 pivot offset
                .OnComplete(() => onCompleted?.Invoke());
        }

        private void OnDestroy() =>
            _moveTween?.Kill();
    }
}