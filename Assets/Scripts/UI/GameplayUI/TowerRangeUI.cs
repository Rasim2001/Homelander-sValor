using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace UI.GameplayUI
{
    public class TowerRangeUI : MonoBehaviour
    {
        [SerializeField] private List<Transform> _rangeIcons;
        [SerializeField] private BuildInfo _buildInfo;

        private readonly int _rangeX = 6;

        private void Start()
        {
            int rangeIconIndex = _buildInfo.transform.position.x > 0 ? 1 : -1;

            int slotIndex = _buildInfo.transform.position.x > 0 ? 1 : 0;


            if (_buildInfo.CurrentLevelId == BuildingLevelId.Level1)
            {
                _rangeIcons[slotIndex].localPosition = new Vector3(_rangeX * rangeIconIndex, 0);
                _rangeIcons[slotIndex].gameObject.SetActive(true);
            }

            else
            {
                _rangeIcons[0].localPosition = new Vector3(_rangeX * -1, 0);
                _rangeIcons[1].localPosition = new Vector3(_rangeX, 0);

                _rangeIcons[0].gameObject.SetActive(true);
                _rangeIcons[1].gameObject.SetActive(true);
            }
        }
    }
}