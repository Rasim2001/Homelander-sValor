using Infastructure.Services.CameraFocus;
using UnityEngine;
using Zenject;

namespace HealthSystem
{
    public class BonfireHealth : MonoBehaviour, IHealth
    {
        [field: SerializeField] public int CurrentHP { get; set; }
        public int MaxHp { get; set; }
        public bool IsDeath { get; set; }

        private ICameraFocusService _cameraFocusService;

        [Inject]
        public void Construct(ICameraFocusService cameraFocusService) =>
            _cameraFocusService = cameraFocusService;

        public void Initialize(int hp)
        {
            CurrentHP = hp;
            MaxHp = hp;
        }

        public void TakeDamage(int damage)
        {
            if (IsDeath)
                return;

            CurrentHP -= damage;

            if (CurrentHP <= 0)
            {
                IsDeath = true;

                _cameraFocusService.ShowMainFlagDestruction();
            }
        }

        public void Reset() =>
            CurrentHP = MaxHp;
    }
}