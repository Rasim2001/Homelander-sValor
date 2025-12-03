using System;
using System.Collections;
using DayCycle;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Infastructure;
using Infastructure.Factories.GameFactories;
using Player;
using Player.Orders;
using UI.GameplayUI.CristalUI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CutScenes
{
    public class CristalSignal
    {
        private readonly IGameFactory _gameFactory;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _playerTransform;
        private readonly Transform _spawnPoint;

        private Cristal _cristal;

        private readonly float minRadius = 0.25f;
        private readonly float maxRadius = 3f;
        private readonly float duration = 4;
        private readonly int _loops = 3;
        private readonly int waypointCount = 10;
        private readonly float _heightStep = 0.2f;
        private readonly Vector3 _playerOffset = new Vector3(0, 2, 0);


        public CristalSignal(
            IGameFactory gameFactory,
            ICristalTimeline cristalTimeline,
            ICoroutineRunner coroutineRunner,
            Transform playerTransform,
            Transform spawnPoint)
        {
            _gameFactory = gameFactory;
            _cristalTimeline = cristalTimeline;
            _coroutineRunner = coroutineRunner;
            _playerTransform = playerTransform;
            _spawnPoint = spawnPoint;
        }


        public void PlayCristalAnimation(Action onComplete) =>
            _coroutineRunner.StartCoroutine(StartPlayCristalCoroutine(onComplete));

        private IEnumerator StartPlayCristalCoroutine(Action onComplete)
        {
            SpawnCristal();

            yield return new WaitForSeconds(1);

            DoSpiralAnimation(onComplete);
        }

        private void SpawnCristal()
        {
            DayCycleUpdater dayCycleUpdater = Camera.main.GetComponentInChildren<DayCycleUpdater>();
            GameObject cristalObject = _gameFactory.CreateCristalUI();
            cristalObject.transform.localScale = Vector3.zero;
            cristalObject.transform.position = _spawnPoint.position;

            _cristal = cristalObject.GetComponent<Cristal>();
            Light2D cristalLight = cristalObject.GetComponent<Light2D>();

            PlayerMove playerMove = _playerTransform.GetComponent<PlayerMove>();
            PlayerInputOrders playerInputOrders = _playerTransform.GetComponent<PlayerInputOrders>();

            _cristal.Initialize(playerMove, _cristalTimeline);
            playerInputOrders.InitCristal(_cristal);
            dayCycleUpdater.InitializeCristalLight(cristalLight);
        }

        private void DoSpiralAnimation(Action onComplete)
        {
            Vector3[] waypoints = GenerateWaypoints();

            GameObject newObject = new GameObject();
            newObject.transform.position = _spawnPoint.position;
            newObject.transform
                .DOPath(waypoints, duration, PathType.CatmullRom, PathMode.Ignore, 10, Color.green)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _cristal.CustomUpdate(newObject.transform.position))
                .OnComplete(() =>
                {
                    _cristal.CustomUpdate(newObject.transform.position);
                    onComplete?.Invoke();
                });

            _cristal.transform.DOScale(1, 1).SetEase(Ease.Linear);
        }

        private Vector3[] GenerateWaypoints()
        {
            Vector3[] waypoints = new Vector3[waypointCount];
            waypoints[0] = _spawnPoint.transform.position;
            waypoints[1] = new Vector3(_spawnPoint.transform.position.x, _spawnPoint.transform.position.y + 1);

            for (int i = 2; i < waypointCount; i++)
            {
                float angle = (i * (360f * _loops) / waypointCount) * Mathf.Deg2Rad;
                float radius = minRadius + (i * (maxRadius - minRadius) / (waypointCount - 1));

                float height = -i * _heightStep;

                waypoints[i] =
                    _playerTransform.position +
                    _playerOffset +
                    new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius
                    );
            }

            waypoints[^1] = _playerTransform.position + _playerOffset;
            waypoints[^1] = _playerTransform.position + _playerOffset + new Vector3(0.22f, 0.2f); // for update cristal

            return waypoints;
        }
    }
}