using System.Linq;
using BuildProcessManagement.Towers;
using Infastructure.Data;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.SaveLoadService;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.ResourceElements
{
    public class ResourceDestruction : BaseDestruction, ISavedProgress
    {
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private float _destructionProgress;

        private IResourceLimiterService _resourceLimiterService;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IResourceLimiterService resourceLimiterService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _resourceLimiterService = resourceLimiterService;
        }

        private void Start()
        {
            ResourceInfo resourceInfo = GetComponent<ResourceInfo>();

            ResourceData resourceData = _staticDataService.ForResource(resourceInfo.ResourceId);
            _destructionProgress = resourceData.DestructionProgress;
        }

        public void UpdateDestructionProgress()
        {
            if (_destruction == null)
                return;

            _destruction.ProgressDestruction += 1f / _destructionProgress;
            _destruction.ShakeBuilding();
            ModifyDestructionBuilding();
        }

        public bool IsDestroyed() =>
            _destruction.ProgressDestruction >= 1;

        private void OnDestroy()
        {
            OrderMarker orderMarker = GetComponent<OrderMarker>();

            if (IsDestroyed())
                _resourceLimiterService.RemoveResource(orderMarker);
        }

        public void LoadProgress(PlayerProgress progress)
        {
            EnvironmentResouceData environmentResouceData =
                progress.WorldData.EnvironmentResoucesData.FirstOrDefault(x => x.UniqueId == _uniqueId.Id);

            if (environmentResouceData == null || environmentResouceData.ProgressDestruction == 0)
                return;

            _destruction.ProgressDestruction = environmentResouceData.ProgressDestruction;
            _destruction.AmountOfDestructionUpdates = environmentResouceData.AmountOfDestructionUpdates;

            ModifyDestructionBuilding();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            EnvironmentResouceData savedData =
                progress.WorldData.EnvironmentResoucesData.FirstOrDefault(x => x.UniqueId == _uniqueId.Id);

            if (_destruction.ProgressDestruction == 0)
                return;

            if (savedData != null)
            {
                savedData.ProgressDestruction = _destruction.ProgressDestruction;
                savedData.AmountOfDestructionUpdates =
                    _destruction.AmountOfDestructionUpdates - 1 < 0
                        ? 0
                        : _destruction.AmountOfDestructionUpdates - 1;
            }
            else
            {
                EnvironmentResouceData newResourceData = new EnvironmentResouceData
                {
                    ProgressDestruction = _destruction.ProgressDestruction,
                    AmountOfDestructionUpdates =
                        _destruction.AmountOfDestructionUpdates - 1 < 0
                            ? 0
                            : _destruction.AmountOfDestructionUpdates - 1,
                    UniqueId = _uniqueId.Id
                };

                progress.WorldData.EnvironmentResoucesData.Add(newResourceData);
            }
        }
    }
}