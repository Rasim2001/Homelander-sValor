using System;
using System.Collections.Generic;
using BuildProcessManagement;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.StaticData.Unit;
using Player.Orders;
using Units;
using UnityEngine;

namespace Infastructure.Services.BuildingRegistry
{
    public class BuildingRegistryService : IBuildingRegistryService
    {
        private readonly List<BuildInfo> _allBuildInfos = new List<BuildInfo>();

        private readonly IGameFactory _gameFactory;
        private readonly IHomelessOrdersService _homelessOrdersService;

        public event Action OnBuildAddHappened;

        public BuildingRegistryService(IGameFactory gameFactory, IHomelessOrdersService homelessOrdersService)
        {
            _gameFactory = gameFactory;
            _homelessOrdersService = homelessOrdersService;
        }

        public void AddBuild(BuildInfo buildInfo)
        {
            if (!_allBuildInfos.Contains(buildInfo))
            {
                if (buildInfo.TryGetComponent(out IHomelessOrder order))
                {
                    UniqueId uniqueId = buildInfo.GetComponent<UniqueId>();
                    OrderMarker orderMarker = buildInfo.GetComponent<OrderMarker>();

                    _homelessOrdersService.AddOrder(order, orderMarker, buildInfo.transform.position.x, uniqueId.Id);
                    _gameFactory.CreateUnit(UnitTypeId.Homeless);
                }

                _allBuildInfos.Add(buildInfo);

                OnBuildAddHappened?.Invoke();
            }
        }

        public void RemoveBuild(BuildInfo buildInfo)
        {
            if (_allBuildInfos.Contains(buildInfo))
            {
                if (buildInfo.TryGetComponent(out IHomelessOrder previousOrder))
                    _homelessOrdersService.RemoveOrder(previousOrder);

                _allBuildInfos.Remove(buildInfo);
            }
        }

        public List<BuildInfo> GetAllBuildInfos() =>
            _allBuildInfos;
    }
}