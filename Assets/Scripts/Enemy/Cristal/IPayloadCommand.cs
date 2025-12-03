namespace Enemy.Cristal
{
    public interface IPayloadCommand : ICommand
    {
        void Initialize();
        void Clear();
    }
}