using UnityEngine;

namespace Infastructure.Services.PauseService
{
    public class PauseService : IPauseService
    {
        public bool IsPaused { get; private set; }

        public void TurnOn()
        {
            IsPaused = true;
            Time.timeScale = 0;
        }

        public void TurnOff()
        {
            IsPaused = false;
            Time.timeScale = 1;
        }
    }
}