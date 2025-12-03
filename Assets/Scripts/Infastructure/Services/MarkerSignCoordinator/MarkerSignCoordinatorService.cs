using System.Collections.Generic;
using System.Linq;
using Infastructure.Data;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.Pool;
using Infastructure.Services.ProgressWatchers;
using Player.Orders;
using Units;
using UnityEngine;

namespace Infastructure.Services.MarkerSignCoordinator
{
    public class MarkerSignCoordinatorService : IMarkerSignCoordinatorService
    {
        private readonly List<MarkerSignsInfo> _markerSignsInfos = new List<MarkerSignsInfo>();
        private readonly IPoolObjects<MarkerSign> _pool;
        private readonly IProgressWatchersService _progressWatchersService;
        private readonly IPersistentProgressService _progressService;

        public MarkerSignCoordinatorService(IPoolObjects<MarkerSign> pool,
            IProgressWatchersService progressWatchersService, IPersistentProgressService progressService)
        {
            _pool = pool;
            _progressWatchersService = progressWatchersService;
            _progressService = progressService;
        }

        public void AddMarker(OrderMarker orderMarker)
        {
            MarkerSign markerSign = _pool.GetObjectFromPool();
            markerSign.transform.SetParent(orderMarker.transform);
            markerSign.transform.localPosition = Vector3.zero;

            _markerSignsInfos.Add(new MarkerSignsInfo(orderMarker, markerSign));
            _progressWatchersService.RegisterWatchers(markerSign.gameObject);

            UpdateMarkerSings();
        }

        public void RemoveMarker(OrderMarker orderMarker)
        {
            MarkerSignsInfo markerSignInfo = FindMarkerSignInfo(orderMarker);

            if (markerSignInfo == null)
                return;

            _pool.ReturnObjectToPool(markerSignInfo.Value);
            _markerSignsInfos.Remove(markerSignInfo);
            _progressWatchersService.Release(markerSignInfo.Value);

            UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

            OrderData savedData =
                _progressService.PlayerProgress.FutureOrdersData.OrderDatas.FirstOrDefault(x =>
                    x.UniqueId == uniqueId.Id);

            if (savedData != null)
                _progressService.PlayerProgress.FutureOrdersData.OrderDatas.Remove(savedData);

            UpdateMarkerSings();
        }

        private MarkerSignsInfo FindMarkerSignInfo(OrderMarker orderMarker)
        {
            foreach (MarkerSignsInfo markerSignInfo in _markerSignsInfos)
            {
                if (markerSignInfo.Key == orderMarker)
                    return markerSignInfo;
            }

            return null;
        }

        private void UpdateMarkerSings()
        {
            for (int i = 0; i < _markerSignsInfos.Count; i++)
                _markerSignsInfos[i].Value.IndexMarkerSign = i;
        }
    }

    public class MarkerSignsInfo
    {
        public readonly OrderMarker Key;
        public readonly MarkerSign Value;

        public MarkerSignsInfo(OrderMarker key, MarkerSign value)
        {
            Key = key;
            Value = value;
        }
    }
}