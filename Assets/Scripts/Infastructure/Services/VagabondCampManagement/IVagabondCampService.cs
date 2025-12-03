using Units.Vagabond;
using UnityEngine;

namespace Infastructure.Services.VagabondCampManagement
{
    public interface IVagabondCampService
    {
        void AddCamp(VagabondCamp vagabondCamp);
        Vector3 GetClosestVagabondCampPosition(bool isRight, float positionX);
        VagabondCamp GetClosestVagabondCamp(bool isRight, float positionX);
        void RemoveCamp(VagabondCamp vagabondCamp);
        void CleanUp();
    }
}