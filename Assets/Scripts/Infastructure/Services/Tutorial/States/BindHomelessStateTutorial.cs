using System.Collections.Generic;
using _Tutorial;
using Player;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class BindHomelessStateTutorial : ITutorialState
    {
        private const string UnitLayer = "Unit";

        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly PlayerFlip _playerFlip;
        private readonly Transform _unitsRecruiterTransform;
        private readonly float _maxDistance = 3;
        private int _unitLayerMask;

        private readonly PlayerInputOrders _playerInputOrders;
        private readonly List<GameObject> _homelessList;

        private int _currentHomeless;

        public BindHomelessStateTutorial(ITutorialStateMachine tutorialStateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            PlayerFlip playerFlip,
            Transform unitsRecruiterTransform,
            PlayerInputOrders playerInputOrders,
            List<GameObject> homelessList)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _playerFlip = playerFlip;
            _unitsRecruiterTransform = unitsRecruiterTransform;
            _playerInputOrders = playerInputOrders;
            _homelessList = homelessList;

            Initialize();
        }


        public void Enter()
        {
            _homelessList.Sort((homeless1, homeless2) =>
            {
                float distanceToPlayerX1 = Mathf.Abs(homeless1.transform.position.x - _playerFlip.transform.position.x);
                float distanceToPlayerX2 = Mathf.Abs(homeless2.transform.position.x - _playerFlip.transform.position.x);

                return distanceToPlayerX1.CompareTo(distanceToPlayerX2);
            });

            TutorialHints tutorialHints = _homelessList[_currentHomeless].GetComponent<TutorialHints>();
            tutorialHints.Show();

            _playerInputOrders.enabled = false;

            _tutorialArrowHelper.SetTarget(tutorialHints.transform);
        }

        public void Exit() =>
            _tutorialArrowHelper.SetTarget(null);

        public void Update()
        {
            Vector2 rayDirection = new Vector2(-_playerFlip.FlipValue(), 0);
            Vector2 rayOrigin = _unitsRecruiterTransform.position;

            if (Input.GetKeyDown(KeyCode.S))
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, _maxDistance, _unitLayerMask);

                if (hit.collider == null)
                    return;

                TutorialHints hint = hit.collider.GetComponentInParent<TutorialHints>();

                if (hint != null)
                {
                    if (hint.HintIsActive())
                    {
                        _playerInputOrders.enabled = true;
                        _playerInputOrders.OnCallUnitsHappenedOnTutorial?.Invoke();

                        hint.Hide();

                        _currentHomeless++;

                        if (_currentHomeless == _homelessList.Count)
                            _tutorialStateMachine.ChangeState<BonfireStateTutorial>();
                        else
                        {
                            TutorialHints currentHint = _homelessList[_currentHomeless].GetComponent<TutorialHints>();
                            currentHint.Show();
                            _tutorialArrowHelper.SetTarget(currentHint.transform);
                        }
                    }

                    _playerInputOrders.enabled = false;
                }
            }
        }

        private void Initialize() =>
            _unitLayerMask = 1 << LayerMask.NameToLayer(UnitLayer);
    }
}