using System;
using Units;
using UnityEngine;

namespace BuildProcessManagement.Towers.SpawnOnTowers
{
    [Serializable]
    public class TowerUnitSpawnInfo
    {
        public UnitObserverTrigger UnitObserverTrigger;
        public Transform Point;

        [Range(-1, 1)] public int DirectionX;
    }
}