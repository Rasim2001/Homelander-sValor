using System;
using DG.Tweening;
using Infastructure.Services.UnitRecruiter;
using UnityEngine;
using Zenject;

namespace Player
{
    public class SelectUnitArrow : MonoBehaviour
    {
        [SerializeField] private Transform _arrowTransform;

        private IUnitsRecruiterService _unitsRecruiterService;

        private Sequence _sequence;
        private Transform _unitTransform;

        public int SelectableUnitIndex;

        [Inject]
        public void Construct(IUnitsRecruiterService unitsRecruiterService) =>
            _unitsRecruiterService = unitsRecruiterService;

        public bool IsActive() =>
            _unitTransform;

        public void SelectUnit()
        {
            if (_unitsRecruiterService.AllUnits.Count == 0)
                return;

            _arrowTransform.localScale = Vector3.zero;
            _sequence?.Kill();

            _sequence = DOTween.Sequence();
            _sequence.Append(_arrowTransform.DOScale(Vector3.one, 0.5f));
            _sequence.Append(_arrowTransform.DOScale(Vector3.zero, 0.5f));
            _sequence.SetLoops(3).OnComplete(() =>
            {
                SelectableUnitIndex = 0;
                _unitTransform = null;
            });

            if (SelectableUnitIndex == _unitsRecruiterService.AllUnits.Count)
                SelectableUnitIndex = 0;

            ShowSelectableUnit(_unitsRecruiterService.AllUnits[SelectableUnitIndex].transform);
        }

        public void UnSelectUnit()
        {
            SelectableUnitIndex = 0;

            _sequence.Kill();
            _arrowTransform.localScale = Vector3.zero;
            _unitTransform = null;
        }

        private void ShowSelectableUnit(Transform unitTransform)
        {
            _unitTransform = unitTransform;
            SelectableUnitIndex++;
        }


        private void LateUpdate()
        {
            if (_unitTransform != null)
                _arrowTransform.transform.position = new Vector3(_unitTransform.transform.position.x, 1);
        }
    }
}