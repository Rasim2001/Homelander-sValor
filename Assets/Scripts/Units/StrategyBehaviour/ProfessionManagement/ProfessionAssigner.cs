using System;
using BuildProcessManagement.WorkshopBuilding;
using DG.Tweening;
using Units.UnitStates;
using Units.UnitStatusManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.StrategyBehaviour.ProfessionManagement
{
    public class ProfessionAssigner : IProfessionAssigner
    {
        private readonly IWorkshopService _workshopService;
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly Animator _animator;

        private WorkshopItemId _workshopItemId;
        //private UnitsRecruiterService _unitsRecruiterService;
        private Workshop _workshop;


        public ProfessionAssigner(
            IWorkshopService workshopService,
            Transform unitTransform,
            UnitFlip unitFlip,
            Animator animator)
        {
            _workshopService = workshopService;
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _animator = animator;
        }


        public void DoAction(
            Workshop workshop,
            WorkshopItemId workshopItemId,
            float positionX,
            float speed)
        {
            _workshop = workshop;
            _workshop.ReduceIndex();

            _workshopItemId = workshopItemId;
            //_unitsRecruiterService = unitsRecruiterService;

            float distance = Mathf.Abs(positionX - _unitTransform.position.x);

            SetCorrectFlip(positionX);
            SetMove(speed, positionX, distance, GetProfession);
        }

        private void SetCorrectFlip(float targetPositionX)
        {
            bool flipValie = targetPositionX - _unitTransform.position.x < 0;
            _unitFlip.SetFlip(flipValie);
        }

        public void StopAction()
        {
        }

        private void SetMove(float speed, float targetPositionX, float distance,
            Action onCompleted)
        {
            _animator.Play(UnitStatesPath.RunHash, 0, 0);

            _unitTransform.DOMoveX(targetPositionX, distance / speed).SetEase(Ease.Linear)
                .OnComplete(onCompleted.Invoke);
        }

        private void GetProfession()
        {
            _workshop.ReduceItemsAmount();

            GameObject unitObject = _workshopService.SpawnUnitWithProfession(_workshopItemId);
            unitObject.transform.position = _unitTransform.position;
            UnitStatus unitStatus = unitObject.GetComponent<UnitStatus>();

            //_unitsRecruiterService.AddUnitToList(unitStatus);
            //_unitsRecruiterService.BindUnitToPlayer(unitStatus);

            if (_unitTransform != null)
                Object.Destroy(_unitTransform.gameObject);
        }
    }
}