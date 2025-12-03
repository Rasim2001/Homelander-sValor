namespace Infastructure.Services.Tutorial
{
    public interface ITutorialState
    {
        void Enter();
        void Exit();
        void Update();
    }
}