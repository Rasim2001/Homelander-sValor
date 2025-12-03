namespace Infastructure.Services.Tutorial
{
    public class TutorialCheckerService : ITutorialCheckerService
    {
        public bool TutorialStarted { get; set; }
        
        public bool ReadyToUseAcceleration { get; set; }
    }
}