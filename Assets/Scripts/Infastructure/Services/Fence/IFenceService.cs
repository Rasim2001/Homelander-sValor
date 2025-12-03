namespace Infastructure.Services.Fence
{
    public interface IFenceService
    {
        void BuildFence(int positionX);
        void DestroyFence(int positionX);
    }
}