using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Infastructure.Services.Trees
{
    public class SortingLayerTrees : ISortingLayerTrees
    {
        private readonly List<int> _sortingLayersId = Enumerable.Range(0, 101).ToList();

        public int GenerateSortingLayerId()
        {
            if (_sortingLayersId.Count == 0)
                return -1;

            int randomIndex = Random.Range(0, _sortingLayersId.Count);
            int randomLayerId = _sortingLayersId[randomIndex];

            _sortingLayersId.RemoveAt(randomIndex);

            return randomLayerId;
        }
    }
}