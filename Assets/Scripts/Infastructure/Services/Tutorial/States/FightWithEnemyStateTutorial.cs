using _Tutorial;
using Enemy;
using Player;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class FightWithEnemyStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly PlayerMove _playerMove;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly GameObject _fightPoint;
        private readonly GameObject _enemyObject;

        private TutorialHints _tutorialHints;

        public FightWithEnemyStateTutorial(
            ITutorialStateMachine tutorialStateMachine,
            PlayerMove playerMove,
            PlayerInputOrders playerInputOrders,
            TutorialArrowHelper tutorialArrowHelper,
            GameObject fightPoint,
            GameObject enemyObject)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _playerMove = playerMove;
            _playerInputOrders = playerInputOrders;
            _tutorialArrowHelper = tutorialArrowHelper;
            _fightPoint = fightPoint;
            _enemyObject = enemyObject;
        }

        public void Enter()
        {
            _tutorialHints = _fightPoint.GetComponent<TutorialHints>();
            _tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(_tutorialHints.transform);
        }

        public void Exit()
        {
            _tutorialHints.Hide();
            _tutorialArrowHelper.SetTarget(null);
        }

        public void Update()
        {
            if (Mathf.Abs(_playerMove.transform.position.x - _fightPoint.transform.position.x) < 1f)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _playerInputOrders.enabled = true;
                    _playerInputOrders.OnReleaseHappendOnTutorial?.Invoke();

                    _enemyObject.GetComponent<EnemyMove>().enabled = true;
                    _enemyObject.GetComponent<EnemyAttack>().enabled = true;

                    _tutorialStateMachine.ChangeState<UnknowState>();
                }
            }
        }
    }
}