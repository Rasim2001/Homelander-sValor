namespace Infastructure.Services.ECSInput
{
    public interface IEcsWatcher : IEcsWatcherWindow
    {
        bool CanUseEcsMenu();
    }


    public interface IEcsWatcherWindow
    {
        void EcsCancel();
    }
}