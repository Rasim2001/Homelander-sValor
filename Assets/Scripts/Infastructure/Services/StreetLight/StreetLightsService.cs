using System.Collections.Generic;
using Infastructure.Services.SafeBuildZoneTracker;
using UnityEngine;

namespace Infastructure.Services.StreetLight
{
    public class StreetLightsService : IStreetLightsService
    {
        private readonly List<StreetLightMarker> _lights = new List<StreetLightMarker>();
        private readonly ISafeBuildZone _safeBuildZone;


        public StreetLightsService(ISafeBuildZone safeBuildZone) =>
            _safeBuildZone = safeBuildZone;

        public void RegisterLights(GameObject building)
        {
            foreach (StreetLightMarker lightMarker in building.GetComponentsInChildren<StreetLightMarker>())
            {
                if (!_lights.Contains(lightMarker))
                    _lights.Add(lightMarker);

                lightMarker.gameObject.SetActive(_safeBuildZone.IsNight);
            }
        }

        public void RemoveLight(GameObject building)
        {
        }

        public void ShowStreetLight() =>
            _lights.ForEach(x => x.gameObject.SetActive(true));


        public void HideStreetLight() =>
            _lights.ForEach(x => x.gameObject.SetActive(false));
    }
}