using UnityEngine;

namespace Infastructure.Services.InputPlayerService
{
    public class OrderSelectionUIService : IOrderSelectionUIService
    {
        public bool LeftArrowPressed => Input.GetKeyDown(KeyCode.Q);
        public bool RightArrowPressed => Input.GetKeyDown(KeyCode.E);
    }
}