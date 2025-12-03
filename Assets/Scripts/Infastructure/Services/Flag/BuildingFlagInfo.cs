using System;
using UnityEngine;

namespace Infastructure.Services.Flag
{
    [Serializable]
    public class BuildingFlagInfo
    {
        public Transform FlagTransform;
        public float BarricadePositionX;

        public BuildingFlagInfo(Transform flagTransform, float barricadePositionX)
        {
            FlagTransform = flagTransform;
            BarricadePositionX = barricadePositionX;
        }
    }
}