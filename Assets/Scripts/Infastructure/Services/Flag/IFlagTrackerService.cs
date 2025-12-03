using Flag;
using UnityEngine;

namespace Infastructure.Services.Flag
{
    public interface IFlagTrackerService
    {
        void RegisterFlag(Transform flag, float barricadePositionX);
        void DeleteFlag(Transform flag);
        Transform GetLastFlag(bool isRight);
        Transform GetMainFlag();
        float GetClosestFlagPositionX(float positionX);
        int GetAllFlagsCount();
        bool LastFlagIsMainFlag(bool isRight);
        float GetLastBarricadePosition(bool isRight);
    }
}