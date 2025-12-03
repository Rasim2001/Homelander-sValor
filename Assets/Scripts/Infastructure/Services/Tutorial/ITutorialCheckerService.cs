namespace Infastructure.Services.Tutorial
{
    public interface ITutorialCheckerService
    {
        bool TutorialStarted { get; set; }
        bool ReadyToUseAcceleration { get; set; }
    }
}