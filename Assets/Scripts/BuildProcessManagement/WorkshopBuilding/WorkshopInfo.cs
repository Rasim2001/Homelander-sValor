using System.Collections.Generic;
using Infastructure.StaticData.Unit;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class WorkshopInfo : MonoBehaviour
    {
        public List<Transform> TargetsPoint;

        public WorkshopItemId WorkshopItemId;
        public UnitTypeId VendorTypeId;
        public float Cooldown;
    }
}