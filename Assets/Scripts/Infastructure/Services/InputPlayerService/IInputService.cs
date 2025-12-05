namespace Infastructure.Services.InputPlayerService
{
    public interface IInputService
    {
        float AxisX { get; }
        bool MoveKeysPressed { get; }
        bool AccelerationPressed { get; }
        bool CallUnitsPressed { get; }
        bool ReleaseUnitsPressed { get; }
        bool ExecuteOrderPressedDown { get; }
        bool SelectUnitPressed { get; }
        bool MoveKeysButtonUp { get; }
        bool AccelerationButtonUp { get; }
        bool CastSkillPressed { get; }
        bool MoveKeysButtonDown { get; }
        bool ExecuteOrderButtonUp { get; }
        bool ExecuteOrderPressed { get; }
        bool AccelerationPressedDown { get; }
        bool TabPressed { get; }
        bool MouseClicked { get; }
        bool ECSPressed { get; }
        bool TaskBookPressed { get; }
        bool ShootPressed { get; }
        bool ShootPressedUp { get; }
        bool ShootPressedDown { get; }
        bool EnterPressed { get; }
    }
}