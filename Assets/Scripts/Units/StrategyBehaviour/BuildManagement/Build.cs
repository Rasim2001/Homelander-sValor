using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement;
using DG.Tweening;
using Enviroment;
using Flag;
using FogOfWar;
using Infastructure;
using Infastructure.Data;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.Fence;
using Infastructure.Services.MarkerSignCoordinator;
using Infastructure.Services.PlayerProgressService;
using Infastructure.StaticData.Building;
using MinimapCore;
using Player.Orders;
using Tooltip.World;
using UI.GameplayUI;
using Units.Animators;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Units.StrategyBehaviour.BuildManagement
{
    public class Build : IBuild
    {
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStatus _unitStatus;
        private readonly BuilderActionHandler _builderActionHandler;
        private readonly BuilderAnimator _unitAnimator;
        private readonly UnitStateMachineView _unitStateMachineView;

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IMarkerSignCoordinatorService _markerSignService;
        private readonly IPersistentProgressService _progressService;
        private readonly IFogOfWarMinimap _fogOfWarMinimap;
        private readonly IBuildingRegistryService _buildingRegistryService;
        private readonly IFenceService _fenceService;
        private readonly List<OrderMarker> _ordersList = new();


        private OrderMarker _orderMarker;
        private BuildInfo _buildInfo;
        private Coroutine _buildCoroutine;
        private Coroutine _finishBuildCoroutine;
        private BuildingProgress _buildingProgress;
        private Tween _moveTween;
        private Action<OrderMarker> _onOrderCompleted;
        private Action _onContinueOrderHappened;

        private int _indexForDelete = -1;


        public Build(
            Transform unitTransform,
            UnitFlip unitFlip,
            UnitStatus unitStatus,
            BuilderActionHandler builderActionHandler,
            BuilderAnimator unitAnimator,
            UnitStateMachineView unitStateMachineView,
            ICoroutineRunner coroutineRunner,
            IMarkerSignCoordinatorService markerSignService,
            IPersistentProgressService progressService,
            IFogOfWarMinimap fogOfWarMinimap,
            IBuildingRegistryService buildingRegistryService,
            IFenceService fenceService)
        {
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStatus = unitStatus;
            _builderActionHandler = builderActionHandler;
            _unitAnimator = unitAnimator;
            _unitStateMachineView = unitStateMachineView;
            _coroutineRunner = coroutineRunner;
            _markerSignService = markerSignService;
            _progressService = progressService;
            _fogOfWarMinimap = fogOfWarMinimap;
            _buildingRegistryService = buildingRegistryService;
            _fenceService = fenceService;
        }

        public void StopAction()
        {
            if (_buildCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_buildCoroutine);
                _buildCoroutine = null;
            }

            _moveTween?.Kill();

            if (_orderMarker != null && _orderMarker.OrderID != OrderID.Heal)
            {
                if (!_orderMarker.IsStarted)
                    _markerSignService.RemoveMarker(_orderMarker);

                SpeachBubleOrderUpdater speachBubleUpdater =
                    _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
                speachBubleUpdater.DisableSpeachBuble();

                ReleaseUnit();

                if (_buildInfo.CurrentWoodsCount == 0)
                {
                    ShowNewBuild();
                    _onOrderCompleted.Invoke(_orderMarker);
                }

                _orderMarker = null;
            }

            _unitAnimator.SetWorkingStateAnimation(false);
            _unitAnimator.SetRunAnimation(false);
        }

        public void DoAction(
            float speed,
            int freePlaceIndex,
            OrderMarker orderMarker,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened)
        {
            InitAndGetStatusBuild(orderMarker, freePlaceIndex);

            _onOrderCompleted = onOrderCompleted;
            _onContinueOrderHappened = onContinueOrderHappened;

            float targetPositionX = _orderMarker.Places[freePlaceIndex].ChopPlace.position.x;
            float distance = Mathf.Abs(targetPositionX - _unitTransform.position.x);

            SetCorrectFlip(targetPositionX);
            SetMove(speed, targetPositionX, distance);
        }


        private void SetMove(float speed, float targetPositionX, float distance)
        {
            _unitAnimator.SetRunAnimation(true);

            _moveTween = _unitTransform.DOMoveX(targetPositionX, distance / speed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _unitAnimator.SetRunAnimation(false);
                    _buildCoroutine = _coroutineRunner.StartCoroutine(StartBuildCoroutine());
                });
        }


        private void InitAndGetStatusBuild(OrderMarker orderMarker, int freePlaceIndex)
        {
            IlluminateObject illuminateObject = orderMarker.GetComponent<IlluminateObject>();
            illuminateObject.Release();

            if (orderMarker.TryGetComponent(out HintsDisplayBase hintsDisplay))
                hintsDisplay.HideHints();

            UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

            _buildInfo = orderMarker.GetComponent<BuildInfo>();
            _buildingProgress = _buildInfo.GetComponent<BuildingProgress>();
            _builderActionHandler.OnWorkHappened += _buildingProgress.BuildWoods;

            _orderMarker = orderMarker;
            _orderMarker.Places[freePlaceIndex].IsBusy = true;

            if (!_ordersList.Contains(_orderMarker))
                _ordersList.Add(_orderMarker);

            Minimap minimap = _orderMarker.GetComponentInChildren<Minimap>();
            minimap.ShowUpgradeProcess();

            TooltipWorld tooltipWorld = _orderMarker.GetComponentInChildren<TooltipWorld>();
            if (tooltipWorld != null)
            {
                BoxCollider2D boxCollider2D = tooltipWorld.GetComponent<BoxCollider2D>();
                boxCollider2D.enabled = false;
            }

            _unitStatus.FreePlaceIndex = freePlaceIndex;
            _unitStatus.IsWorked = true;
            _unitStatus.OrderUniqueId = uniqueId.Id;
        }

        private void SetCorrectFlip(float targetPositionX)
        {
            bool flipValie = targetPositionX - _unitTransform.position.x < 0;
            _unitFlip.SetFlip(flipValie);
        }

        private IEnumerator StartBuildCoroutine()
        {
            SpeachBubleOrderUpdater speachBubleUpdater =
                _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
            speachBubleUpdater.DisableSpeachBuble();

            _unitAnimator.SetWorkingStateAnimation(true);
            _unitFlip.SetFlip(_orderMarker.Places[_unitStatus.FreePlaceIndex].Flip);

            while (_buildInfo.CurrentWoodsCount > 0)
                yield return null;

            ShowNewBuild();

            float timeDelay = Random.Range(0.25f, 0.5f);
            yield return new WaitForSeconds(timeDelay);

            _unitStateMachineView.ChangeState<WalkState>();

            ReleaseUnit();

            _onOrderCompleted.Invoke(_orderMarker);
            _onContinueOrderHappened?.Invoke();
        }

        private void ShowNewBuild()
        {
            if (!_orderMarker.IsStarted)
                return;

            _orderMarker.IsStarted = false;
            _fogOfWarMinimap.UpdateFogPosition(_orderMarker.transform.position.x);

            ShowBarricadeFlag();
            ShowUpgradedBuild();
            ClearOldBuildingProgress();

            _indexForDelete = _ordersList.IndexOf(_orderMarker);
            if (_indexForDelete == -1)
                return;

            _buildingProgress.ShakeAllWoods(5,
                () => _finishBuildCoroutine = _coroutineRunner.StartCoroutine(FinishBuildAnimationCoroutine()));
        }

        private void ShowBarricadeFlag()
        {
            FlagActivator currentFlag = _orderMarker.GetComponentInChildren<FlagActivator>();
            FlagActivator nextFlag = _buildInfo.NextBuild.GetComponentInChildren<FlagActivator>();

            if (currentFlag == null && nextFlag == null)
                return;

            if (currentFlag == nextFlag)
                nextFlag.SpawnFlag();
            else if (currentFlag.HasFlag())
                nextFlag.Initialize(currentFlag.GetFlag());

            _fenceService.BuildFence((int)_orderMarker.transform.position.x);
        }

        private void ClearOldBuildingProgress()
        {
            UniqueId uniqueId = _buildInfo.GetComponent<UniqueId>();

            BuildingProgressData savedData =
                _progressService.PlayerProgress.WorldData.BuildingProgressData.FirstOrDefault(x =>
                    x.UniqueId == uniqueId.Id);

            _progressService.PlayerProgress.WorldData.BuildingProgressData.Remove(savedData);
        }

        private void ShowUpgradedBuild()
        {
            if (_buildInfo.NextBuild != null)
            {
                DecorOnBuild decorOnBuild = _buildInfo.GetComponent<DecorOnBuild>();
                decorOnBuild?.Hide();

                _buildInfo.NextBuild.SetActive(true);
            }
        }

        private IEnumerator FinishBuildAnimationCoroutine()
        {
            OrderMarker currentOrderMarker = _ordersList[_indexForDelete];
            _ordersList.RemoveAt(_indexForDelete);

            if (currentOrderMarker != null)
                EnableFirstBuild(currentOrderMarker);

            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();

            float centerOfWoodsX = buildInfo.GetComponentInChildren<WoodBuild>().transform.localPosition.x;
            float centerOfScaffoldsX = buildInfo.GetComponentInChildren<ScaffoldsBuild>().transform.localPosition.x;

            List<GameObject> woodsList = buildInfo.WoodsList.ToList();
            List<GameObject> scaffoldsList = buildInfo.ScaffoldsList.ToList();

            yield return AnimateBuildObjects(woodsList, centerOfWoodsX);
            yield return AnimateBuildObjects(scaffoldsList, centerOfScaffoldsX);

            if (currentOrderMarker != null)
            {
                RegisterNewBuild(currentOrderMarker);
                ClearCurrentOrder(currentOrderMarker);
            }
        }

        private void RegisterNewBuild(OrderMarker currentOrderMarker)
        {
            currentOrderMarker.GetComponent<BoxCollider2D>().enabled = false;

            Minimap minimap = currentOrderMarker.GetComponentInChildren<Minimap>();
            minimap.HideUpgradedProcess();

            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();

            BuildInfo nextBuildInfo = buildInfo.NextBuild.GetComponent<BuildInfo>();
            _buildingRegistryService.AddBuild(nextBuildInfo);
        }

        private void EnableFirstBuild(OrderMarker currentOrderMarker)
        {
            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();

            if (currentOrderMarker.gameObject == buildInfo.NextBuild.gameObject)
            {
                Minimap minimap = currentOrderMarker.GetComponentInChildren<Minimap>();
                minimap.Show();

                DecorOnBuild decorOnBuild = currentOrderMarker.GetComponent<DecorOnBuild>();
                decorOnBuild?.Show();
            }
        }


        private IEnumerator AnimateBuildObjects(IEnumerable<GameObject> objects, float centerX)
        {
            foreach (GameObject obj in objects)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    AddRandomMovement(rb, centerX);
            }

            yield return new WaitForSeconds(0.75f);
        }

        private void ClearCurrentOrder(OrderMarker currentOrderMarker)
        {
            currentOrderMarker.GetComponent<BoxCollider2D>().enabled = true;

            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();
            BuildingProgress buildingProgress = buildInfo.GetComponent<BuildingProgress>();

            if (currentOrderMarker.gameObject != buildInfo.NextBuild.gameObject)
            {
                _buildingRegistryService.RemoveBuild(buildInfo);
                Object.Destroy(currentOrderMarker.gameObject);
            }
            else
                DeleteMarkingBuild(buildInfo, buildingProgress);
        }

        private void DeleteMarkingBuild(BuildInfo buildInfo, BuildingProgress buildingProgress)
        {
            List<GameObject> currentWoodsList = new List<GameObject>(buildInfo.WoodsList);
            List<GameObject> currentScaffoldsList = new List<GameObject>(buildInfo.ScaffoldsList);

            foreach (GameObject woods in currentWoodsList)
                Object.Destroy(woods.gameObject); //TODO:

            foreach (GameObject scaffold in currentScaffoldsList)
                Object.Destroy(scaffold.gameObject);

            buildInfo.WoodsList.Clear();
            buildInfo.ScaffoldsList.Clear();
            buildingProgress.Clear();

            buildInfo.CurrentWoodsCount = 0;
        }

        private void AddRandomMovement(Rigidbody2D woodRb, float centerOfBuildX)
        {
            float deltaX = woodRb.transform.localPosition.x - centerOfBuildX;
            int randomHeight = Random.Range(3, 7);
            int randomX = Random.Range(-2, 2);

            woodRb.gravityScale = 1.5f;
            woodRb.AddRelativeForce(new Vector2(deltaX + randomX, randomHeight), ForceMode2D.Impulse);
        }


        private void ReleaseUnit()
        {
            _orderMarker.Places[_unitStatus.FreePlaceIndex].IsBusy = false;
            _unitStatus.IsWorked = false;
            _unitStatus.OrderUniqueId = string.Empty;

            _unitAnimator.SetWorkingStateAnimation(false);

            _builderActionHandler.OnWorkHappened -= _buildingProgress.BuildWoods;
        }
    }
}