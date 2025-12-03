using System;

namespace Infastructure.Services.SafeBuildZoneTracker
{
    public interface ISafeBuildZone
    {
        bool IsNight { get; set; }
        bool IsSafeZone(float positionX);
    }
}