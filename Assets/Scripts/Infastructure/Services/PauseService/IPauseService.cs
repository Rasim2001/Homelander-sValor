namespace Infastructure.Services.PauseService
{
    public interface IPauseService
    {
        bool IsPaused { get; }
        void TurnOn();
        void TurnOff();
    }
}