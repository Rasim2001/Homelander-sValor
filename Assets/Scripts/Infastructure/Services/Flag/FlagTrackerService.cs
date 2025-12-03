using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.Flag
{
    public class FlagTrackerService : IFlagTrackerService
    {
        private Transform _mainFlag;

        private readonly List<BuildingFlagInfo> _leftSideFlags = new List<BuildingFlagInfo>();
        private readonly List<BuildingFlagInfo> _rightSideFlags = new List<BuildingFlagInfo>();

        public void RegisterFlag(Transform flag, float barricadePositionX)
        {
            BuildingFlagInfo flagInfo = new BuildingFlagInfo(flag, barricadePositionX);

            if (_leftSideFlags.Exists(x => x.FlagTransform == flag) ||
                _rightSideFlags.Exists(x => x.FlagTransform == flag))
                return;

            if (flag.position.x > 0)
                _rightSideFlags.Add(flagInfo);
            else if (flag.position.x < 0)
                _leftSideFlags.Add(flagInfo);
            else
                _mainFlag = flag;
        }

        public int GetAllFlagsCount() =>
            _leftSideFlags.Count + _rightSideFlags.Count + 1;

        public void DeleteFlag(Transform flag)
        {
            BuildingFlagInfo flagInfo = _leftSideFlags.Find(x => x.FlagTransform == flag);
            
            if (flagInfo != null)
            {
                _leftSideFlags.Remove(flagInfo);
                return;
            }

            flagInfo = _rightSideFlags.Find(f => f.FlagTransform == flag);
            if (flagInfo != null)
                _rightSideFlags.Remove(flagInfo);
        }

        public Transform GetLastFlag(bool isRight)
        {
            if (isRight)
                return FindEndSideFlag(_rightSideFlags);

            return FindEndSideFlag(_leftSideFlags);
        }

        public float GetLastBarricadePosition(bool isRight)
        {
            Transform lastFlag = GetLastFlag(isRight);

            BuildingFlagInfo flagInfo = isRight
                ? _rightSideFlags.Find(f => f.FlagTransform == lastFlag)
                : _leftSideFlags.Find(f => f.FlagTransform == lastFlag);

            return flagInfo?.BarricadePositionX ?? 0;
        }

        public bool LastFlagIsMainFlag(bool isRight) =>
            GetLastFlag(isRight) == GetMainFlag();

        public float GetClosestFlagPositionX(float positionX) =>
            FindClosestFlag(positionX);

        public Transform GetMainFlag() =>
            _mainFlag;

        private Transform FindEndSideFlag(List<BuildingFlagInfo> sideFlags)
        {
            Transform desiredFlag = _mainFlag;
            float lastestFlag = 0;

            foreach (BuildingFlagInfo flagInfo in sideFlags)
            {
                if (Mathf.Abs(flagInfo.FlagTransform.position.x) > lastestFlag)
                {
                    lastestFlag = Mathf.Abs(flagInfo.FlagTransform.position.x);
                    desiredFlag = flagInfo.FlagTransform;
                }
            }

            return desiredFlag;
        }


        private float FindClosestFlag(float positionX)
        {
            if (_mainFlag == null)
                return 0;

            float closestFlagPositionX = 0;
            float closestDistance = Mathf.Infinity;

            foreach (BuildingFlagInfo flagInfo in _rightSideFlags)
            {
                if (Mathf.Abs(flagInfo.FlagTransform.position.x - positionX) < closestDistance)
                {
                    closestDistance = Mathf.Abs(flagInfo.FlagTransform.position.x - positionX);
                    closestFlagPositionX = flagInfo.FlagTransform.position.x;
                }
            }

            foreach (BuildingFlagInfo flagInfo in _leftSideFlags)
            {
                if (Mathf.Abs(flagInfo.FlagTransform.position.x - positionX) < closestDistance)
                {
                    closestDistance = Mathf.Abs(flagInfo.FlagTransform.position.x - positionX);
                    closestFlagPositionX = flagInfo.FlagTransform.position.x;
                }
            }

            return Mathf.Abs(_mainFlag.position.x - positionX) < closestDistance
                ? _mainFlag.position.x
                : closestFlagPositionX;
        }
    }
}