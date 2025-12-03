using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Data;
using Infastructure.Services.Forest;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.ResourceLimiter
{
    public class ResourceLimiterService : IResourceLimiterService
    {
        private readonly List<OrderMarker> _leftSideResources = new List<OrderMarker>();
        private readonly List<OrderMarker> _rightSideResources = new List<OrderMarker>();

        private readonly IForestService _forestService;

        public Action<bool, Transform> OnResourceChanged { get; set; }

        public ResourceLimiterService(IForestService forestService) =>
            _forestService = forestService;

        public void CleanUp()
        {
            _rightSideResources.Clear();
            _leftSideResources.Clear();
        }

        public void AddResource(OrderMarker orderMarker)
        {
            bool isRight = orderMarker.transform.position.x > 0;

            if (isRight)
                _rightSideResources.AddAndSort(orderMarker);
            else
                _leftSideResources.AddAndSort(orderMarker);

            OnResourceChanged?.Invoke(isRight,
                isRight
                    ? _rightSideResources.FirstOrDefault()?.transform
                    : _leftSideResources.FirstOrDefault()?.transform);
        }

        public void RemoveResource(OrderMarker orderMarker)
        {
            if (orderMarker == null)
                throw new ArgumentNullException(nameof(orderMarker));

            bool isRight = orderMarker.transform.position.x > 0;

            List<OrderMarker> targetList = isRight ? _rightSideResources : _leftSideResources;
            int indexResource = targetList.IndexOf(orderMarker);
            int nextIndex = 1;

            if (indexResource == 0 && targetList.Count > 1)
            {
                if (targetList[nextIndex] != null)
                {
                    _forestService.DestroyForest(orderMarker.transform.position.x,
                        targetList[nextIndex].transform.position.x);
                }
            }


            if (isRight && _rightSideResources.Contains(orderMarker))
                _rightSideResources.Remove(orderMarker);
            else if (_leftSideResources.Contains(orderMarker))
                _leftSideResources.Remove(orderMarker);


            OnResourceChanged?.Invoke(isRight,
                isRight
                    ? _rightSideResources.FirstOrDefault()?.transform
                    : _leftSideResources.FirstOrDefault()?.transform);
        }

        public bool IsActive(OrderMarker orderMarker)
        {
            if (orderMarker == null)
                return false;

            List<OrderMarker> targetList =
                orderMarker.transform.position.x > 0 ? _rightSideResources : _leftSideResources;

            if (targetList.Count <= 3)
                return true;

            int index = targetList.IndexOf(orderMarker);
            return index >= 0 && index < 3;
        }
    }
}