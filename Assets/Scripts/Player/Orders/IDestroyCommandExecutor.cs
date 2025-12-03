using BuildProcessManagement;

namespace Player.Orders
{
    public interface IDestroyCommandExecutor
    {
        void DestroyBuild(BuildInfo buildInfo);
    }
}