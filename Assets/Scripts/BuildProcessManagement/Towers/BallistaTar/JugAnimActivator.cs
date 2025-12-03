using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class JugAnimActivator : MonoBehaviour
    {
        [SerializeField] private Jug _jug;

        public void ActivateAnimation() =>
            _jug.PlayAnimation();

        public void DisableJugAnimation() =>
            _jug.ExitAnimation();
    }
}