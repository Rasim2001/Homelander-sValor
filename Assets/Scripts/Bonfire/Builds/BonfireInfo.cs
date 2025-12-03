using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bonfire.Builds
{
    public class BonfireInfo : MonoBehaviour
    {
        public int CurrentWoodsCount { get; set; }
        public GameObject NextBuild { get; set; }

        public List<GameObject> WoodsList = new List<GameObject>();
        public List<GameObject> ScaffoldsList = new List<GameObject>();

        public void SortWood() =>
            WoodsList = WoodsList.OrderByDescending(obj => obj.transform.position.y).ToList();

        public void SortScaffold() =>
            ScaffoldsList = ScaffoldsList.OrderBy(obj => obj.transform.position.x).ToList();
    }
}