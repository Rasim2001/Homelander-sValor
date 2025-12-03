using System.Linq;
using Infastructure.Data;
using Infastructure.Services.SaveLoadService;
using Units;
using UnityEngine;

namespace Player.Orders
{
    public class MarkerSign : MonoBehaviour, ISavedProgress
    {
        public int IndexMarkerSign { get; set; }

        public void LoadProgress(PlayerProgress progress)
        {
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            UniqueId uniqueId = gameObject.GetComponentInParent<UniqueId>();

            if (uniqueId == null)
                return;

            OrderData savedData =
                progress.FutureOrdersData.OrderDatas.FirstOrDefault(x => x.UniqueId.Contains(uniqueId.Id));
            if (savedData == null)
                progress.FutureOrdersData.OrderDatas.Add(new OrderData(uniqueId.Id, IndexMarkerSign));
            else
                savedData.IndexOrder = IndexMarkerSign;
        }
    }
}