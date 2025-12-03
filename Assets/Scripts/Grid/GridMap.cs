using Infastructure.StaticData.Building;
using Infastructure.StaticData.RecourceElements;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridMap : IGridMap
    {
        private Vector2IntMinMax _sizeMinMax;
        private BuildingUpgradeData _buildingUpgradeData;
        private BuildingStaticData _buildingStaticData;

        private bool[] _grid;

        public void Initialize()
        {
            _sizeMinMax = new Vector2IntMinMax(-200, 200);
            _grid = new bool[_sizeMinMax.MaxX - _sizeMinMax.MinX];
        }

        public void RegisterBuild(BuildingUpgradeData buildingUpgradeData, BuildingStaticData buildingStaticData)
        {
            _buildingUpgradeData = buildingUpgradeData;
            _buildingStaticData = buildingStaticData;
        }


        public bool AreCellsFree(int currentPosition)
        {
            if (_buildingUpgradeData == null)
                return false;

            int startX = _buildingStaticData.BuildingTypeId == BuildingTypeId.Baricade
                ? CalculateGridBarricade(currentPosition, out int endX)
                : CalculateGridBuild(currentPosition, out endX);

            if (startX < _sizeMinMax.MinX || endX > _sizeMinMax.MaxX)
                return false;

            for (int i = startX; i <= endX; i++)
                if (_grid[i - _sizeMinMax.MinX])
                    return false;

            return true;
        }

        public void OccupyCells(int currentPosition)
        {
            if (_buildingUpgradeData == null)
                return;

            int startX = _buildingStaticData.BuildingTypeId == BuildingTypeId.Baricade
                ? CalculateGridBarricade(currentPosition, out int endX)
                : CalculateGridBuild(currentPosition, out endX);

            for (int i = startX; i <= endX; i++)
                _grid[i - _sizeMinMax.MinX] = true;
        }

        private int CalculateGridBuild(int currentPosition, out int endX)
        {
            int startX = currentPosition - _buildingUpgradeData.GridSizeX / 2;
            endX = currentPosition + _buildingUpgradeData.GridSizeX / 2;

            return startX;
        }

        private int CalculateGridBarricade(int currentPosition, out int endX)
        {
            int startX;
            if (currentPosition > 0)
            {
                startX = currentPosition - _buildingUpgradeData.GridSizeX / 2;
                endX = currentPosition;
            }
            else
            {
                startX = currentPosition;
                endX = currentPosition + _buildingUpgradeData.GridSizeX / 2;
            }

            return startX;
        }

        private int CalculateGridBuildForDelete(BuildingUpgradeData buildingUpgradeData, int currentPosition,
            out int endX)
        {
            int startX = currentPosition - buildingUpgradeData.GridSizeX / 2;
            endX = currentPosition + buildingUpgradeData.GridSizeX / 2;

            return startX;
        }

        private int CalculateGridBarricadeForDelete(BuildingUpgradeData buildingUpgradeData, int currentPosition,
            out int endX)
        {
            int startX;
            if (currentPosition > 0)
            {
                startX = currentPosition - buildingUpgradeData.GridSizeX / 2;
                endX = currentPosition;
            }
            else
            {
                startX = currentPosition;
                endX = currentPosition + buildingUpgradeData.GridSizeX / 2;
            }

            return startX;
        }


        public BlockDirectionEnum GetBlockDirection(int currentPosition)
        {
            if (_buildingUpgradeData == null)
                return BlockDirectionEnum.None;

            int startX = _buildingStaticData.BuildingTypeId == BuildingTypeId.Baricade
                ? CalculateGridBarricade(currentPosition, out int endX)
                : CalculateGridBuild(currentPosition, out endX);

            bool isBlockedLeft = false;
            bool isBlockedRight = false;

            if (_grid[startX - _sizeMinMax.MinX])
                isBlockedLeft = true;

            if (_grid[endX - _sizeMinMax.MinX])
                isBlockedRight = true;

            if (isBlockedRight && isBlockedLeft)
                return BlockDirectionEnum.AllDirection;

            if (isBlockedLeft)
                return BlockDirectionEnum.Left;

            if (isBlockedRight)
                return BlockDirectionEnum.Right;

            return BlockDirectionEnum.None;
        }

        public void ClearCells(int currentPosition, BuildingStaticData buildingStaticData,
            BuildingUpgradeData buildingUpgradeData)
        {
            int startX = buildingStaticData.BuildingTypeId == BuildingTypeId.Baricade
                ? CalculateGridBarricadeForDelete(buildingUpgradeData, currentPosition, out int endX)
                : CalculateGridBuildForDelete(buildingUpgradeData, currentPosition, out endX);

            for (int i = startX; i <= endX; i++)
                _grid[i - _sizeMinMax.MinX] = false;
        }

        public void ClearCells(int currentPosition, ResourceData resourceData)
        {
            int startX = currentPosition - resourceData.GridSizeX / 2;
            int endX = currentPosition + resourceData.GridSizeX / 2;

            for (int i = startX; i <= endX; i++)
                _grid[i - _sizeMinMax.MinX] = false;
        }

        public void OccupyCells(int currentPosition, BuildingUpgradeData flagBuildingUpgradeData)
        {
            int startX = currentPosition - flagBuildingUpgradeData.GridSizeX / 2;
            int endX = currentPosition + flagBuildingUpgradeData.GridSizeX / 2;

            for (int i = startX; i <= endX; i++)
                _grid[i - _sizeMinMax.MinX] = true;
        }

        public void OccupyCells(int currentPosition, ResourceData resourceData)
        {
            int startX = currentPosition - resourceData.GridSizeX / 2;
            int endX = currentPosition + resourceData.GridSizeX / 2;

            for (int i = startX; i <= endX; i++)
                _grid[i - _sizeMinMax.MinX] = true;
        }
    }
}