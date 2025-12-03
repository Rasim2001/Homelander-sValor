using BuildProcessManagement.WorkshopBuilding;
using UnityEngine;

namespace Infastructure.StaticData.Workshop
{
    [CreateAssetMenu(fileName = "WorkshopData", menuName = "StaticData/Workshop")]
    public class WorkshopStaticData : ScriptableObject
    {
        public WorkshopItemId WorkshopItemId;
        public int CoinsValue;
    }
}