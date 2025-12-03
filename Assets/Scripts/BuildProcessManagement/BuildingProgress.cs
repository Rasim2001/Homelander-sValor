using System;
using System.Linq;
using DG.Tweening;
using Infastructure.Data;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.SaveLoadService;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement
{
    [RequireComponent(typeof(UniqueId))]
    public class BuildingProgress : MonoBehaviour, ISavedProgress
    {
        private const float ANIMATION_SPEED_BUILD = 1.05f;

        [SerializeField] private BuildInfo _buildInfo;
        [SerializeField] private float _progress;
        [SerializeField] private int _amountOfUpdates;

        public int ProgressValue;

        private int _buildingTime;

        private SpriteRenderer _spriteRenderer;
        private MarkingBuild _markingBuild;
        private IProgressWatchersService _progressWatchersService;
        private IStaticDataService _staticDataService;


        [Inject]
        public void Construct(IProgressWatchersService progressWatchersService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _progressWatchersService = progressWatchersService;
        }

        private void Awake()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _markingBuild = gameObject.GetComponent<MarkingBuild>();
        }

        private void Start()
        {
            BuildingUpgradeData buildingUpgradeData = _staticDataService.ForBuilding(_buildInfo.BuildingTypeId,
                _buildInfo.CurrentLevelId,
                _buildInfo.CardKey);

            if (buildingUpgradeData != null)
                _buildingTime = buildingUpgradeData.BuildingTime;
        }

        public void LoadProgress(PlayerProgress progress)
        {
            UniqueId uniqueId = gameObject.GetComponent<UniqueId>();

            BuildingProgressData savedData =
                progress.WorldData.BuildingProgressData.FirstOrDefault(x => x.UniqueId == uniqueId.Id);

            if (savedData == null)
                return;

            _markingBuild.InitializeNextBuild();
            _markingBuild.StartBuild();

            float savedProgress = savedData.Progress;
            int savedAmountOfUpdates = savedData.AmountOfUpdates;

            _progress = savedProgress;
            _amountOfUpdates = savedAmountOfUpdates;

            Debug.Log($"updates : {savedProgress} and savedAmountOfUpdates : {savedAmountOfUpdates}");

            for (int i = 0; i < savedAmountOfUpdates; i++)
            {
                CreateWood();
                _buildInfo.CurrentWoodsCount--;
            }
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if (gameObject == null || !gameObject.activeInHierarchy ||
                _spriteRenderer.enabled == false)
                return;

            OrderMarker orderMarker = gameObject.GetComponent<OrderMarker>();
            if (orderMarker.IsStarted == false)
                return;

            UniqueId uniqueId = gameObject.GetComponent<UniqueId>();

            BuildingProgressData savedData =
                progress.WorldData.BuildingProgressData.FirstOrDefault(x => x.UniqueId == uniqueId.Id);

            if (savedData != null)
            {
                savedData.Progress = _progress;
                savedData.AmountOfUpdates = _amountOfUpdates;
            }
            else
            {
                Debug.Log(
                    $"_progress : {_progress} and _amountOfUpdates : {_amountOfUpdates} and {gameObject.name} and {transform.position.x}");

                BuildingProgressData newBuildingData =
                    new BuildingProgressData(uniqueId.Id, _progress, _amountOfUpdates);

                progress.WorldData.BuildingProgressData.Add(newBuildingData);
            }
        }

        public void Clear()
        {
            _progress = 0;
            _amountOfUpdates = 0;

            BuildingUpgradeData buildingUpgradeData = _staticDataService.ForBuilding(_buildInfo.BuildingTypeId,
                _buildInfo.NextBuildingLevelId,
                _buildInfo.CardKey);

            if (buildingUpgradeData != null)
                _buildingTime = buildingUpgradeData.BuildingTime;
        }

        public void BuildWoods()
        {
            if (_progress == 0)
                ProgressValue = EstimateProgressValue(_buildingTime, _buildInfo.WoodsList.Count, ANIMATION_SPEED_BUILD);

            _progress += (float)_buildInfo.WoodsList.Count / ProgressValue;

            ShakeAllWoods(1);

            if (HasUpdate())
            {
                CreateWood();

                _amountOfUpdates++;
                _buildInfo.CurrentWoodsCount--;
            }
        }

        public void ShakeAllWoods(int loop, Action onComplete = null)
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < _buildInfo.WoodsList.Count; i++)
            {
                GameObject wood = _buildInfo.WoodsList[i];
                Vector3 defaultWoodPosition = wood.transform.position;

                Tweener shakeTween = wood.transform.DOShakePosition(
                        0.1f,
                        new Vector3(0.1f, 0.05f),
                        100,
                        90,
                        false,
                        true,
                        ShakeRandomnessMode.Harmonic)
                    .OnComplete(() => wood.transform.position = defaultWoodPosition);

                sequence.Join(shakeTween);
            }

            sequence.Play()
                .SetLoops(loop)
                .OnComplete(() => onComplete?.Invoke());
        }

        private int EstimateProgressValue(float targetTime, int woodsCount, float animationSpeed)
        {
            int maxIterations = 10000;

            for (int progressValue = 1; progressValue < maxIterations; progressValue++)
            {
                float simulatedTime = EstimateBuildTime(woodsCount, progressValue, animationSpeed);

                if (simulatedTime >= targetTime)
                    return progressValue;
            }

            Debug.LogWarning("Couldn't find suitable ProgressValue");
            return -1;
        }

        private float EstimateBuildTime(int woodsCount, int progressValue, float animationSpeed)
        {
            if (progressValue < woodsCount)
                return -1;

            float progress = 0;
            int _amountOfUpdates = 0;
            int woodRemaining = woodsCount;

            int countOfProgress = 0;

            while (woodRemaining > 0)
            {
                countOfProgress++;
                progress += (float)woodsCount / progressValue;

                if (progress >= _amountOfUpdates)
                {
                    _amountOfUpdates++;
                    woodRemaining--;
                }
            }

            return countOfProgress * animationSpeed;
        }


        private void CreateWood()
        {
            int index = _buildInfo.CurrentWoodsCount - 1;
            if (index >= 0)
            {
                GameObject wood = _buildInfo.WoodsList[index];
                wood.SetActive(true);
            }
        }

        private bool HasUpdate() =>
            _progress >= _amountOfUpdates;

        private void OnDestroy() =>
            _progressWatchersService.Release(this);
    }
}