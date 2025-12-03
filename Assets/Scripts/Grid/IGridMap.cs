using Infastructure.StaticData.Building;
using Infastructure.StaticData.RecourceElements;
using UnityEngine;

namespace Grid
{
    public interface IGridMap
    {
        void RegisterBuild(BuildingUpgradeData buildingUpgradeData, BuildingStaticData buildingStaticData);
        bool AreCellsFree(int currentPosition);
        void OccupyCells(int currentPosition);
        BlockDirectionEnum GetBlockDirection(int currentPosition);
        void OccupyCells(int currentPosition, BuildingUpgradeData flagBuildingUpgradeData);
        void Initialize();

        void ClearCells(int currentPosition, BuildingStaticData buildingStaticData,
            BuildingUpgradeData buildingUpgradeData);

        void OccupyCells(int currentPosition, ResourceData resourceData);
        void ClearCells(int currentPosition, ResourceData resourceData);
    }
}