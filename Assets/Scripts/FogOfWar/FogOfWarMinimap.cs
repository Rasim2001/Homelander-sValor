using System;
using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement;
using DG.Tweening;
using Infastructure.Services.BuildingRegistry;
using UnityEngine;
using Zenject;

namespace FogOfWar
{
    public class FogOfWarMinimap : MonoBehaviour, IFogOfWarMinimap
    {
        [SerializeField] private Transform _leftFog;
        [SerializeField] private Transform _rightFog;

        private IBuildingRegistryService _buildingRegistryService;
        private bool _canUpdatePosition;

        private Tween _leftTween;
        private Tween _rightTween;

        [Inject]
        public void Construct(IBuildingRegistryService buildingRegistryService) =>
            _buildingRegistryService = buildingRegistryService;


        public void StartUpdatePosition()
        {
            _canUpdatePosition = true;

            foreach (BuildInfo buildInfo in _buildingRegistryService.GetAllBuildInfos())
                UpdateFogPosition(buildInfo.transform.position.x);
        }

        public void UpdateFogPosition(float positionX)
        {
            if (!_canUpdatePosition)
                return;

            if (positionX > 0 && positionX < _rightFog.position.x ||
                positionX < 0 && positionX > _leftFog.position.x)
                return;

            if (positionX > 0)
                MoveFor(positionX, ref _rightTween, _rightFog);
            else
                MoveFor(positionX, ref _leftTween, _leftFog);
        }

        public void UpdateFogPositionAfterDestroyBuild(float positionX)
        {
            List<BuildInfo> buildInfos = positionX > 0
                ? _buildingRegistryService.GetAllBuildInfos().Where(x => x.transform.position.x > 0).ToList()
                : _buildingRegistryService.GetAllBuildInfos().Where(x => x.transform.position.x < 0).ToList();

            if (buildInfos.Count == 0)
                MoveToMainflag(positionX);
            else
                UpdatePositionSide(buildInfos);
        }

        private void UpdatePositionSide(List<BuildInfo> buildInfos)
        {
            foreach (BuildInfo buildInfo in buildInfos)
                UpdateFogPosition(buildInfo.transform.position.x);
        }

        private void MoveToMainflag(float positionX)
        {
            if (positionX > 0)
                MoveFor(0, ref _rightTween, _rightFog);
            else
                MoveFor(0, ref _leftTween, _leftFog);
        }

        private void MoveFor(float positionX, ref Tween tween, Transform fogOfWar)
        {
            int offsetDelta = positionX > 0 ? 2 : -2;

            if (tween != null && tween.IsActive())
                tween.Kill();

            tween = fogOfWar.DOMoveX(positionX + offsetDelta, 5);
        }
    }
}