using System;
using System.Collections.Generic;
using Infastructure.StaticData.Unit;
using UnityEngine;

namespace Infastructure.Services.UnitTrackingService
{
    public class UnitsTrackerService : IUnitsTrackerService
    {
        private readonly Dictionary<UnitTypeId, int> _unitsCounter = new Dictionary<UnitTypeId, int>
        {
            { UnitTypeId.Builder, 0 },
            { UnitTypeId.Archer, 0 },
            { UnitTypeId.Shielder, 0 },
            { UnitTypeId.Homeless, 0 },
        };

        public Action<UnitTypeId, int> OnUnitCountChanged { get; set; }

        public void AddUnit(UnitTypeId unitTypeId)
        {
            if (!_unitsCounter.ContainsKey(unitTypeId))
                return;

            _unitsCounter[unitTypeId]++;

            OnUnitCountChanged?.Invoke(unitTypeId, _unitsCounter[unitTypeId]);
        }

        public void RemoveUnit(UnitTypeId unitTypeId)
        {
            if (!_unitsCounter.ContainsKey(unitTypeId))
                return;

            _unitsCounter[unitTypeId]--;
            OnUnitCountChanged?.Invoke(unitTypeId, _unitsCounter[unitTypeId]);
        }

        public int GetUnitsCount(UnitTypeId unitTypeId) =>
            _unitsCounter.GetValueOrDefault(unitTypeId, 0);
    }
}