using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Data;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.SaveLoadService;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Units;
using UnityEngine;
using Zenject;
using BuildingData = Infastructure.Data.BuildingData;

namespace BuildProcessManagement
{
    public class BuildInfo : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private UniqueId _uniqueId;

        public int CurrentWoodsCount { get; set; }
        public GameObject NextBuild { get; set; }
        public BuildingLevelId NextBuildingLevelId { get; set; }
        public BuildingTypeId BuildingTypeId { get; set; }
        public BuildingLevelId CurrentLevelId { get; set; }
        public CardId CardKey { get; set; }
        public CardId PreviousCardId { get; set; }

        public VisualBuilding VisualBuilding { get; set; }

        private IProgressWatchersService _progressWatchersService;
        public List<GameObject> WoodsList = new List<GameObject>();
        public List<GameObject> ScaffoldsList = new List<GameObject>();


        [Inject]
        public void Construct(IProgressWatchersService progressWatchersService) =>
            _progressWatchersService = progressWatchersService;

        private void Awake() =>
            VisualBuilding = GetComponentInChildren<VisualBuilding>();

        public void OnDestroy() =>
            _progressWatchersService.Release(this);

        public void LoadProgress(PlayerProgress progress)
        {
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if (gameObject == null || !gameObject.activeInHierarchy)
                return;

            BuildingData savedData =
                progress.WorldData.BuildingData.FirstOrDefault(x => x.UniqueId == _uniqueId.Id);

            if (savedData != null)
            {
                savedData.BuildingTypeId = BuildingTypeId;
                savedData.CurrentBuildingLevelId = CurrentLevelId;
            }
            else
            {
                BuildingData newBuildData =
                    new BuildingData(_uniqueId.Id, BuildingTypeId, CurrentLevelId);

                progress.WorldData.BuildingData.Add(newBuildData);
            }
        }


        public void SortWood() =>
            WoodsList = WoodsList.OrderByDescending(obj => obj.transform.position.y).ToList();

        public void SortScaffold() =>
            ScaffoldsList = ScaffoldsList.OrderBy(obj => obj.transform.position.x).ToList();
    }
}