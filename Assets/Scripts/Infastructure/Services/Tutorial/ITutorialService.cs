namespace Infastructure.Services.Tutorial
{
    public interface ITutorialService
    {
        void Initialize();
        void ChangeState<TState>() where TState : ITutorialState;
    }
}