using System;
using DG.Tweening;
using Infastructure.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class YesNoMiniWindow : MonoBehaviour
    {
        [SerializeField] private GameObject _bg;
        [SerializeField] private Transform _container;

        [SerializeField] private Button _yes;
        [SerializeField] private Button _no;


        private bool _isRestarted;
        private IStateMachine _stateMachine;

        [Inject]
        public void Construct(IStateMachine stateMachine) =>
            _stateMachine = stateMachine;


        private void Awake()
        {
            _yes.onClick.AddListener(DoAction);
            _no.onClick.AddListener(Deinitialize);
        }


        private void OnDestroy()
        {
            _yes.onClick.RemoveListener(DoAction);
            _no.onClick.RemoveListener(Deinitialize);
        }

        public void SetRestart(bool value) =>
            _isRestarted = value;

        public void Initialize()
        {
            _bg.SetActive(true);
            _container.DOScale(Vector3.one, 0.25f).SetUpdate(true);
        }


        private void Deinitialize()
        {
            _bg.SetActive(false);
            _container.DOScale(Vector3.zero, 0.25f).SetUpdate(true);
        }

        private void DoAction()
        {
            if (_isRestarted)
                _stateMachine.Enter<LoadLevelState>();
            else
                _stateMachine.Enter<LoadProgressState>();
        }
    }
}