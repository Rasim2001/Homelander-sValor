using DG.Tweening;
using Flag;
using HealthSystem;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Cards;
using Infastructure.StaticData.Building;
using Player.Orders;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement
{
    public class MarkingBuild : MonoBehaviour
    {
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private BuildInfo _buildInfo;

        private IGameFactory _gameFactory;
        private ICardTrackerService _cardTrackerService;

        [Inject]
        public void Construct(IGameFactory gameFactory, ICardTrackerService cardTrackerService)
        {
            _gameFactory = gameFactory;
            _cardTrackerService = cardTrackerService;
        }

        public void InitializeNextBuild()
        {
            GetNextBuildSpriteRender();

            SpriteRenderer spriteRenderer =
                _buildInfo.NextBuild.GetComponent<BuildInfo>().VisualBuilding.BuildSpriteRender;

            ScaffoldsBuild scaffoldsBuild = _buildInfo.GetComponentInChildren<ScaffoldsBuild>();
            scaffoldsBuild.Initialize(spriteRenderer);

            WoodBuild woodBuild = _buildInfo.GetComponentInChildren<WoodBuild>();
            woodBuild.Initialize(spriteRenderer);

            _buildInfo.CurrentWoodsCount = _buildInfo.WoodsList.Count;

            BuildingProgress buildingProgress = _buildInfo.GetComponent<BuildingProgress>();
            BuildingProgress nextBuildingProgress = _buildInfo.NextBuild.GetComponent<BuildingProgress>();

            buildingProgress.ProgressValue = nextBuildingProgress.ProgressValue;
        }

        public void PrepareBuild()
        {
            SpriteRenderer spriteRenderer = _buildInfo.VisualBuilding.BuildSpriteRender;

            ScaffoldsBuild scaffoldsBuild = _buildInfo.GetComponentInChildren<ScaffoldsBuild>();
            scaffoldsBuild.Initialize(spriteRenderer);

            WoodBuild woodBuild = _buildInfo.GetComponentInChildren<WoodBuild>();
            woodBuild.Initialize(spriteRenderer);

            _buildInfo.CurrentWoodsCount = _buildInfo.WoodsList.Count;

            BuildScaffolds();
        }

        public void StartBuild()
        {
            if (IsNewProgressBuild())
                _orderMarker.IsStarted = true;

            RegisterBuildingBarricade();
            BuildScaffolds();
        }


        private void RegisterBuildingBarricade()
        {
            BuildingHealth buildingHealth = _orderMarker.GetComponentInChildren<BuildingHealth>();
            if (buildingHealth != null)
                Destroy(buildingHealth.gameObject);

            FlagActivator flagActivator = _orderMarker.GetComponentInChildren<FlagActivator>();
            if (flagActivator != null)
                flagActivator.ReleaseUnitFromBuildingFlag();
        }

        private bool IsNewProgressBuild() =>
            !_orderMarker.IsStarted;


        private void GetNextBuildSpriteRender()
        {
            _buildInfo.NextBuild =
                _gameFactory.CreateBuilding(_buildInfo.BuildingTypeId, _buildInfo.transform.position, _uniqueId.Id,
                    _buildInfo.NextBuildingLevelId, _cardTrackerService.CardId, _buildInfo.CardKey);

            _buildInfo.NextBuild.SetActive(false);
        }

        private void BuildScaffolds()
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _buildInfo.ScaffoldsList.Count; i++)
            {
                GameObject scaffold = _buildInfo.ScaffoldsList[i];

                if (scaffold.activeInHierarchy)
                    return;

                scaffold.SetActive(true);
                SpriteRenderer spriteRenderer = scaffold.GetComponent<SpriteRenderer>();

                Tween colorTween = spriteRenderer.DOColor(Color.white, 0.1f);
                sequence.Append(colorTween);
            }

            sequence.Play();
        }
    }
}