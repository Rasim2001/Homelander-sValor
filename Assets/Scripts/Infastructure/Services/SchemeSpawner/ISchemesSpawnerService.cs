using System;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace Infastructure.Services.SchemeSpawner
{
    public interface ISchemesSpawnerService
    {
        void CreateShemesByMainflag(BonfireLevelData bonfireLevelData, Vector3 position, Action onCompleted);
        void CreateShemesByType(BuildingTypeId buildingTypeId, Vector3 position, Vector2 randomDirection);
    }
}