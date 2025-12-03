using UnityEngine;

namespace Infastructure.Services.InputPlayerService
{
    public class InputService : IInputService
    {
        private const string Horizontal = "Horizontal";
        public float AxisX => Input.GetAxis(Horizontal);
        public bool MoveKeysPressed => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A);
        public bool MoveKeysButtonUp => Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A);
        public bool MoveKeysButtonDown => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A);

        public bool AccelerationPressed => Input.GetKey(KeyCode.LeftShift);
        public bool AccelerationPressedDown => Input.GetKeyDown(KeyCode.LeftShift);
        public bool AccelerationButtonUp => Input.GetKeyUp(KeyCode.LeftShift);

        public bool CallUnitsPressed => Input.GetKeyDown(KeyCode.S);
        public bool ReleaseUnitsPressed => Input.GetKeyDown(KeyCode.R);
        public bool ExecuteOrderPressedDown => Input.GetKeyDown(KeyCode.W);
        public bool ExecuteOrderPressed => Input.GetKey(KeyCode.W);
        public bool ExecuteOrderButtonUp => Input.GetKeyUp(KeyCode.W);
        public bool SelectUnitPressed => Input.GetKeyDown(KeyCode.F);
        public bool CastSkillPressed => Input.GetKeyDown(KeyCode.Space);
        public bool ShootPressedDown => Input.GetMouseButtonDown(0);
        public bool ShootPressed => Input.GetMouseButton(0);
        public bool ShootPressedUp => Input.GetMouseButtonUp(0);

        public bool TabPressed => Input.GetKeyDown(KeyCode.Tab);

        public bool MouseClicked => Input.GetMouseButtonDown(0);
        public bool ECSPressed => Input.GetKeyDown(KeyCode.Escape);

        public bool TaskBookPressed => Input.GetKeyDown(KeyCode.J);
    }
}