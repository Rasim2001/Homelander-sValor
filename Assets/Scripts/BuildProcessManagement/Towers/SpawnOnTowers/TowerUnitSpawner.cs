using System.Collections.Generic;
using BuildProcessManagement.Towers.UnitSpawnStrategies;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.UnitSpawnStrategy;
using Infastructure.StaticData.Unit;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers.SpawnOnTowers
{
    public class TowerUnitSpawner : MonoBehaviour, IHomelessOrder
    {
        [SerializeField] private UnitTypeId _unitType;
        [SerializeField] private List<TowerUnitSpawnInfo> _locations;

        private readonly List<GameObject> _units = new List<GameObject>();

        private IGameFactory _gameFactory;
        private IUnitSpawnStrategy _unitSpawnStrategy;
        private IUnitSpawnStrategyService _unitSpawnStrategyService;

        private int _index;


        [Inject]
        public void Construct(IGameFactory gameFactory, IUnitSpawnStrategyService unitSpawnStrategyService)
        {
            _gameFactory = gameFactory;
            _unitSpawnStrategyService = unitSpawnStrategyService;
        }

        private void Awake() =>
            _unitSpawnStrategy = _unitSpawnStrategyService.GetStrategy(_unitType);

        private void Start()
        {
            if (transform.position.x > 0)
                _locations.Reverse();
        }

        public bool HasAvailableSlot() => _locations.Count > _index;

        public int NumberOfOrders() => _locations.Count - _index;

        public int GetAmountOfUnits() => _units.Count;

        public void SpawnUnit()
        {
            if (_index >= _locations.Count)
                return;

            if (_locations.Count == 1)
                _locations[_index].DirectionX = transform.position.x > 0 ? 1 : -1; //TODO:

            GameObject unit = _unitSpawnStrategy.SpawnUnit(_gameFactory, _locations[_index], transform);

            _index++;
            _units.Add(unit);
        }


        public GameObject SpawnHomeless()
        {
            if (_units.Count == 0)
                return null;

            int indexForDelete = _units.Count - 1;
            GameObject unitForDestroy = _units[indexForDelete];

            _units.RemoveAt(indexForDelete);
            _index--;

            Destroy(unitForDestroy);

            GameObject homeless = _gameFactory.CreateUnit(UnitTypeId.Homeless, transform.position.x);

            return homeless;
        }
    }
}