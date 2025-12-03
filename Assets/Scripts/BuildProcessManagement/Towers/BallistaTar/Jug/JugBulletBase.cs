using Infastructure.Services.Effects;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class JugBulletBase : MonoBehaviour
    {
        protected static readonly int Break = Animator.StringToHash("Break");

        [SerializeField] private ObserverTrigger _observerTrigger;

        public float Speed = 3;

        protected bool TargetIsActive;
        protected ISlowEffectService SlowEffectService;

        private bool _isTriggered;


        [Inject]
        public void Construct(ISlowEffectService slowEffectService) =>
            SlowEffectService = slowEffectService;


        private void Start() =>
            _observerTrigger.OnTriggerEnter += TriggerEnter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        public void Shoot() =>
            TargetIsActive = true;

        protected virtual void Update()
        {
            if (TargetIsActive)
                transform.localPosition += new Vector3(0, -Speed * Time.deltaTime);
        }

        protected virtual void TriggerEnter()
        {
            if (_isTriggered)
                return;

            _isTriggered = true;
        }
    }
}