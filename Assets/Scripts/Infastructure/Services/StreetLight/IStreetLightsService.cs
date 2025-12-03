using UnityEngine;

namespace Infastructure.Services.StreetLight
{
    public interface IStreetLightsService
    {
        void RegisterLights(GameObject building);
        void RemoveLight(GameObject building);
        void ShowStreetLight();
        void HideStreetLight();
    }
}