using BuildProcessManagement.WorkshopBuilding.Product;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.Pool;
using Infastructure.StaticData.DefaultMaterial;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.UnitStatusManagement;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IPoolObjects<HammerObject> _hammersPool;
        private readonly IPoolObjects<ArrowObject> _arrowsPool;
        private readonly IPoolObjects<ShieldObject> _shieldPool;

        private readonly IGameFactory _gameFactory;
        private readonly IFutureOrdersService _futureOrdersService;
        private readonly IStaticDataService _staticDataService;
        private GameObject _vendorPrefab;
        private Transform _vendorPoint;


        public WorkshopService(
            IPoolObjects<HammerObject> hammersPool,
            IPoolObjects<ArrowObject> arrowsPool,
            IPoolObjects<ShieldObject> shieldPool,
            IGameFactory gameFactory,
            IFutureOrdersService futureOrdersService,
            IStaticDataService staticDataService)
        {
            _hammersPool = hammersPool;
            _arrowsPool = arrowsPool;
            _shieldPool = shieldPool;
            _gameFactory = gameFactory;
            _futureOrdersService = futureOrdersService;
            _staticDataService = staticDataService;
        }

        public IWorkshopItemCreator Initialize(WorkshopItemId workshopItemId)
        {
            DefaultMaterialStaticData defaultMaterialStaticData = _staticDataService.DefaultMaterialStaticData;

            switch (workshopItemId)
            {
                case WorkshopItemId.Hammer:
                    return new HammerItemCreator(_hammersPool, defaultMaterialStaticData);
                case WorkshopItemId.Arrow:
                    return new ArrowItemCreator(_arrowsPool, defaultMaterialStaticData);
                case WorkshopItemId.Shield:
                    return new ShieldItemCreator(_shieldPool, defaultMaterialStaticData);
            }

            return null;
        }

        public GameObject CreateVendor(Vector3 position, UnitTypeId unitTypeId)
        {
            GameObject vendorObject = _gameFactory.CreateUnit(unitTypeId);
            vendorObject.transform.position = position;

            return vendorObject;
        }


        public GameObject SpawnUnitWithProfession(WorkshopItemId workshopItemId)
        {
            switch (workshopItemId)
            {
                case WorkshopItemId.Hammer:
                    return CreateAndRegisterBuilder();
                case WorkshopItemId.Arrow:
                    return _gameFactory.CreateUnit(UnitTypeId.Archer);
                case WorkshopItemId.Shield:
                    return _gameFactory.CreateUnit(UnitTypeId.Shielder);
                case WorkshopItemId.Reset:
                    return _gameFactory.CreateUnit(UnitTypeId.Homeless);
            }

            return null;
        }

        private GameObject CreateAndRegisterBuilder()
        {
            GameObject gameObject = _gameFactory.CreateUnit(UnitTypeId.Builder);
            _futureOrdersService.AddBuilder(gameObject.GetComponent<UnitStatus>());
            return gameObject;
        }
    }
}