using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Services.UnitTrackingService;
using Infastructure.StaticData.Unit;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI.UnitCounter
{
    public class UnitCounterUI : MonoBehaviour
    {
        [SerializeField] private UnitUI[] _units;
        
        private Dictionary<UnitTypeId, TextMeshProUGUI> _unitsDictionary =>
            _units.ToDictionary(x => x.UnitTypeId, x => x.AmountText);

        private IUnitsTrackerService _unitsTrackerService;

        [Inject]
        public void Construct(IUnitsTrackerService unitsTrackerService) =>
            _unitsTrackerService = unitsTrackerService;

        private void Awake() =>
            InitializeUnitsCount();

        private void Start() =>
            _unitsTrackerService.OnUnitCountChanged += UpdateCounter;

        private void OnDestroy() =>
            _unitsTrackerService.OnUnitCountChanged -= UpdateCounter;

        private void InitializeUnitsCount()
        {
            foreach (UnitTypeId unitTypeId in _unitsDictionary.Keys)
            {
                int unitsCount = _unitsTrackerService.GetUnitsCount(unitTypeId);

                UpdateCounter(unitTypeId, unitsCount);
            }
        }

        private void UpdateCounter(UnitTypeId type, int count) =>
            _unitsDictionary[type].text = count.ToString();
    }
}