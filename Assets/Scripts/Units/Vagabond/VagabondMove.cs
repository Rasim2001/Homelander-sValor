using Infastructure.Services.Flag;
using Infastructure.Services.VagabondCampManagement;
using UnityEngine;
using Zenject;

namespace Units.Vagabond
{
    public class VagabondMove : MonoBehaviour
    {
        [SerializeField] private UnitFlip _unitFlip;

        private float _speed;
        private float _targetFreePositionX;

        private IVagabondCampService _vagabondCampService;

        [Inject]
        public void Construct(IVagabondCampService vagabondCampService) =>
            _vagabondCampService = vagabondCampService;

        public void SetSpeed(float value) =>
            _speed = value;

        public void ChangeTargetPosition()
        {
            bool isRight = transform.position.x > 0;
            float offset = 5;

            Vector3 targetCampPosition =
                _vagabondCampService.GetClosestVagabondCampPosition(isRight, transform.position.x);

            float newTargetPositionX = targetCampPosition.x + Random.Range(-offset, offset);

            _targetFreePositionX = newTargetPositionX;
        }


        public void Move()
        {
            float directionDistanceToMove = _targetFreePositionX - transform.position.x;

            _unitFlip.SetFlip(directionDistanceToMove);

            float moveDistance = Mathf.Sign(directionDistanceToMove) *
                                 Mathf.Min(Mathf.Abs(directionDistanceToMove), _speed * Time.deltaTime);

            transform.Translate(new Vector2(moveDistance, 0));
        }

        public bool IsPathReached() =>
            Mathf.Abs(transform.position.x - _targetFreePositionX) < Mathf.Epsilon;
    }
}