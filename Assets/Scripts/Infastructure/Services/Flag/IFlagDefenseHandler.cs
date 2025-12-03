using Flag;
using UnityEngine;

namespace Infastructure.Services.Flag
{
    public interface IFlagDefenseHandler
    {
        void PrepareToDefense(Transform unit);
        void StartRetreat(FlagSlotInfo slotInfo);
        void RelocateAfterFight(Transform unit);
    }
}