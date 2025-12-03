using BuildProcessManagement.WorkshopBuilding;
using Player;
using UnityEngine;

namespace Units.StrategyBehaviour.ProfessionManagement
{
    public interface IProfessionAssigner
    {
        void StopAction();

        void DoAction(Workshop workshop,
            WorkshopItemId workshopItemId,
            float positionX,
            float speed);
    }
}