namespace FogOfWar
{
    public interface IFogOfWarMinimap
    {
        void UpdateFogPosition(float positionX);
        void StartUpdatePosition();
        void UpdateFogPositionAfterDestroyBuild(float positionX);
    }
}