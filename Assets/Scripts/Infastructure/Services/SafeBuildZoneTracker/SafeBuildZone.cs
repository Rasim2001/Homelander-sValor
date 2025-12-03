using Infastructure.Services.Flag;

namespace Infastructure.Services.SafeBuildZoneTracker
{
    public class SafeBuildZone : ISafeBuildZone
    {
        private readonly IFlagTrackerService _flagTrackerService;

        public bool IsNight { get; set; }

        public SafeBuildZone(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;

        public bool IsSafeZone(float positionX)
        {
            float leftFlag = _flagTrackerService.GetLastFlag(false).position.x;
            float rightFlag = _flagTrackerService.GetLastFlag(true).position.x;

            return positionX > leftFlag && positionX < rightFlag;
        }
    }
}