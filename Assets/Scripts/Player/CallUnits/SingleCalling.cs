using Infastructure.Factories.GameFactories;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Units;
using Units.UnitStatusManagement;
using Units.Vagabond;
using UnityEngine;

namespace Player.CallUnits
{
    public class SingleCalling : ICallingUnits
    {
        private const string UnitLayer = "Unit";

        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly IGameFactory _gameFactory;
        private readonly PlayerFlip _playerFlip;
        private readonly float _maxDistance = 3;

        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[10];
        private int _unitLayerMask;

        public SingleCalling(IUnitsRecruiterService unitsRecruiterService, IGameFactory gameFactory,
            PlayerFlip playerFlip)
        {
            _unitsRecruiterService = unitsRecruiterService;
            _gameFactory = gameFactory;
            _playerFlip = playerFlip;
        }

        public void Initialize() =>
            _unitLayerMask = 1 << LayerMask.NameToLayer(UnitLayer);

        public void DetectUnits()
        {
            Vector2 rayDirection = new Vector2(-_playerFlip.FlipValue(), 0);
            Vector2 rayOrigin = _playerFlip.transform.position;

            int hitCount =
                Physics2D.RaycastNonAlloc(rayOrigin, rayDirection, _raycastHits, _maxDistance, _unitLayerMask);

            if (hitCount == 0)
                return;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _raycastHits[i];
                if (hit.collider == null)
                    return;

                UnitStatus unitStatus = hit.collider.GetComponentInParent<UnitStatus>();

                if (unitStatus == null)
                    return;

                if (unitStatus.UnitTypeId == UnitTypeId.Vagabond)
                {
                    GameObject homelessUnit = SpawnHomelessUnit(unitStatus);
                    _unitsRecruiterService.AddUnitToList(homelessUnit.GetComponent<UnitStatus>());

                    return;
                }

                if (!unitStatus.IsBindedToPlayer())
                {
                    _unitsRecruiterService.AddUnitToList(unitStatus);

                    return;
                }
            }
        }

        private GameObject SpawnHomelessUnit(UnitStatus unitStatus)
        {
            SpriteRenderer vagabondSpriteRenderer = unitStatus.GetComponent<SpriteRenderer>();

            GameObject homelessUnit = _gameFactory.CreateUnit(UnitTypeId.Homeless);
            homelessUnit.transform.position = unitStatus.transform.position;

            UnitFlip homelessUnitFlip = homelessUnit.GetComponent<UnitFlip>();
            homelessUnitFlip.SetFlip(vagabondSpriteRenderer.flipX);

            VagabondDeath vagabondDeath = unitStatus.GetComponent<VagabondDeath>();
            vagabondDeath.Die();

            return homelessUnit;
        }
    }
}