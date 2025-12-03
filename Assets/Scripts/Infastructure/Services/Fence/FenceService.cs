using System.Collections.Generic;
using System.Linq;
using Infastructure.Services.Flag;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.Fence
{
    public class FenceService : IInitializable, IFenceService
    {
        private readonly Dictionary<int, IEnumerable<FenceMarker>> _fenceMarkers =
            new Dictionary<int, IEnumerable<FenceMarker>>();

        private readonly IFlagTrackerService _flagTrackerService;
        private readonly IPoolObjects<LeftFenceMarker> _leftPool;
        private readonly IPoolObjects<RightFenceMarker> _rightPool;

        private float _defaultWidth;

        public FenceService(
            IFlagTrackerService flagTrackerService,
            IPoolObjects<LeftFenceMarker> leftPool,
            IPoolObjects<RightFenceMarker> rightPool)
        {
            _flagTrackerService = flagTrackerService;
            _leftPool = leftPool;
            _rightPool = rightPool;
        }

        public void Initialize()
        {
            InitDefaultWidth();

            _fenceMarkers.Add(0, new List<FenceMarker>());
        }

        public void BuildFence(int positionX)
        {
            if (Mathf.Approximately(positionX, 0))
                return;

            bool isRight = positionX > 0;
            float lastBarricadePosition = _flagTrackerService.GetLastBarricadePosition(isRight);

            if (!IsLastBarricadePosition(positionX, lastBarricadePosition))
            {
                IEnumerable<FenceMarker> defaultFence = new List<FenceMarker>();
                _fenceMarkers.TryAdd(positionX, defaultFence);

                int? nextPositionKey = GetNextFencePosition(positionX);
                if (!nextPositionKey.HasValue)
                    return;

                IEnumerable<FenceMarker> nextFenceMarkers = _fenceMarkers[nextPositionKey.Value];

                List<FenceMarker> leftSide = new List<FenceMarker>();
                List<FenceMarker> rightSide = new List<FenceMarker>();

                foreach (FenceMarker fenceMarker in nextFenceMarkers)
                {
                    if (fenceMarker.transform.position.x > positionX)
                        rightSide.Add(fenceMarker);
                    else
                        leftSide.Add(fenceMarker);
                }

                if (isRight)
                {
                    _fenceMarkers[nextPositionKey.Value] = rightSide;
                    _fenceMarkers[positionX] = leftSide;
                }
                else
                {
                    _fenceMarkers[nextPositionKey.Value] = leftSide;
                    _fenceMarkers[positionX] = rightSide;
                }
            }
            else

                CreateNewFence(positionX, isRight, lastBarricadePosition);
        }


        public void DestroyFence(int positionX)
        {
            int? nextPositionKey = GetNextFencePosition(positionX);

            if (IsLastFencePositionKey(nextPositionKey))
            {
                if (_fenceMarkers.TryGetValue(positionX, out IEnumerable<FenceMarker> fenceMarkers))
                {
                    foreach (FenceMarker fenceMarker in fenceMarkers)
                    {
                        switch (fenceMarker)
                        {
                            case LeftFenceMarker left:
                                _leftPool.ReturnObjectToPool(left);
                                break;
                            case RightFenceMarker right:
                                _rightPool.ReturnObjectToPool(right);
                                break;
                        }
                    }

                    int? previousFencePosition = GetPreviousFencePosition(positionX);
                    if (previousFencePosition.HasValue)
                    {
                        IEnumerable<FenceMarker> previousFenceMarker = _fenceMarkers[previousFencePosition.Value];
                        FenceMarker fenceMarker = previousFenceMarker.Last();

                        float fencePosition = Mathf.Abs(fenceMarker.transform.position.x - previousFencePosition.Value);
                        fenceMarker.SetWidth(fencePosition);
                    }
                }
            }
            else if (nextPositionKey.HasValue)
            {
                int nextKey = nextPositionKey.Value;

                if (_fenceMarkers.TryGetValue(nextKey, out var fenceMarkersNext) &&
                    _fenceMarkers.TryGetValue(positionX, out var fenceMarkersCurrent))
                {
                    _fenceMarkers[nextKey] = fenceMarkersNext.Concat(fenceMarkersCurrent).ToList();
                }
            }

            _fenceMarkers.Remove(positionX);
        }

        private void CreateNewFence(int positionX, bool isRight, float lastBarricadePosition)
        {
            List<FenceMarker> fenceMarkers = new List<FenceMarker>();

            float lastFencePosition = GetLastFencePosition(isRight);
            float width = Mathf.Abs(lastBarricadePosition - lastFencePosition);
            int count = (int)(width / _defaultWidth);
            float currentPosition = lastFencePosition;

            for (int i = 0; i < count; i++)
            {
                FenceMarker fenceMarker = PlaceFenceMarker(isRight, currentPosition, _defaultWidth);
                fenceMarkers.Add(fenceMarker);
                currentPosition += isRight ? _defaultWidth : -_defaultWidth;
            }

            float remainingWidth = Mathf.Abs(lastBarricadePosition - currentPosition);
            FenceMarker remainingFenceMarker = PlaceFenceMarker(isRight, currentPosition, remainingWidth);
            fenceMarkers.Add(remainingFenceMarker);

            _fenceMarkers.TryAdd(positionX, fenceMarkers);
        }

        private bool IsLastBarricadePosition(int positionX, float lastBarricadePosition) =>
            Mathf.Approximately(lastBarricadePosition, positionX);


        private bool IsLastFencePositionKey(int? nextPositionKey) =>
            nextPositionKey == null;

        private FenceMarker PlaceFenceMarker(bool isRight, float positionX, float width)
        {
            if (isRight)
            {
                RightFenceMarker marker = _rightPool.GetObjectFromPool();
                marker.SetDefaultSettings();
                marker.SetPosition(positionX);
                marker.SetWidth(width);

                return marker;
            }
            else
            {
                LeftFenceMarker marker = _leftPool.GetObjectFromPool();
                marker.SetDefaultSettings();
                marker.SetPosition(positionX);
                marker.SetWidth(width);

                return marker;
            }
        }

        private void InitDefaultWidth()
        {
            LeftFenceMarker sampleMarker = _leftPool.GetObjectFromPool();
            _defaultWidth = sampleMarker.GetComponent<SpriteRenderer>().size.x;
            _leftPool.ReturnObjectToPool(sampleMarker);
        }


        private float GetLastFencePosition(bool isRight) =>
            isRight ? _fenceMarkers.Keys.Max() : _fenceMarkers.Keys.Min();


        private int? GetNextFencePosition(int currentPositionKey)
        {
            bool isRight = currentPositionKey > 0;

            List<int> sortedKeys = _fenceMarkers.Keys
                .Where(k => (isRight && k > 0) || (!isRight && k <= 0))
                .OrderBy(k => isRight ? k : -k)
                .ToList();

            int index = sortedKeys.IndexOf(currentPositionKey);
            if (index >= 0 && index + 1 < sortedKeys.Count)
                return sortedKeys[index + 1];

            return null;
        }

        private int? GetPreviousFencePosition(int currentPositionKey)
        {
            bool isRight = currentPositionKey > 0;

            List<int> sortedKeys = _fenceMarkers.Keys
                .Where(k => (isRight && k > 0) || (!isRight && k < 0))
                .OrderBy(k => isRight ? k : -k)
                .ToList();

            int index = sortedKeys.IndexOf(currentPositionKey);
            if (index > 0)
                return sortedKeys[index - 1];

            return null;
        }
    }
}