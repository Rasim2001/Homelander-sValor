using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.VagabondCampManagement;
using Units.UnitStates.StateMachineViews;
using Units.UnitStates.VagabondStates;
using UnityEngine;
using Zenject;

namespace Units.Vagabond
{
    public class VagabondCamp : MonoBehaviour
    {
        [SerializeField] private UnitObserverTrigger _observerTrigger;

        [SerializeField] private List<VagabondMarker> _vagabonds = new List<VagabondMarker>();

        [SerializeField] private int _requiredVagabonds = 2;
        [SerializeField] private float _timeSpawn;

        private IGameFactory _gameFactory;

        private SpriteRenderer _spriteRenderer;
        private Coroutine _spawnVagabondCoroutine;
        private IStaticDataService _staticDataService;


        [Inject]
        public void Construct(IGameFactory gameFactory, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _gameFactory = gameFactory;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            VagabondStaticData vagabondStaticData = _staticDataService.VagabondStaticData;

            _requiredVagabonds = vagabondStaticData.RequiredVagabonds;
            _timeSpawn = vagabondStaticData.TimeSpawn;
        }

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;

            StartSpawnVagabondCoroutine();
        }


        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
        }

        public void DestroyCamp()
        {
            StopSpawnVagabondCoroutine();

            foreach (VagabondMarker vagabondMarker in _vagabonds.ToList())
                RemoveVagabond(vagabondMarker);

            Destroy(gameObject);
        }

        private void StartSpawnVagabondCoroutine()
        {
            StopSpawnVagabondCoroutine();

            _spawnVagabondCoroutine = StartCoroutine(StartSpawnVagabond());
        }

        private void StopSpawnVagabondCoroutine()
        {
            if (_spawnVagabondCoroutine != null)
            {
                StopCoroutine(_spawnVagabondCoroutine);
                _spawnVagabondCoroutine = null;
            }
        }

        private IEnumerator StartSpawnVagabond()
        {
            while (!IsFullCamp())
            {
                yield return new WaitForSeconds(_timeSpawn);

                GameObject unit = _gameFactory.CreateUnit(UnitTypeId.Vagabond);
                unit.transform.position = transform.position;

                VagabondStateMachineView vagabondStateMachineView = unit.GetComponent<VagabondStateMachineView>();
                vagabondStateMachineView.ChangeState<WalkVagabondState>();
            }
        }

        private void TriggerEnter()
        {
            List<Collider2D> allHits = _observerTrigger.GetAllHits();

            if (allHits == null || allHits.Count == 0)
                return;

            foreach (Collider2D hit in allHits)
            {
                if (hit.TryGetComponent(out VagabondMarker vagabondMarker))
                {
                    AddVagabond(vagabondMarker);

                    if (IsFullCamp())
                        StopSpawnVagabondCoroutine();
                }
            }
        }

        private void RemoveVagabond(VagabondDeath vagabondDeath)
        {
            vagabondDeath.OnDeathHappened -= RemoveVagabond;

            VagabondMarker vagabondMarker = vagabondDeath.GetComponentInChildren<VagabondMarker>();

            RemoveVagabond(vagabondMarker);

            if (!IsFullCamp())
                StartSpawnVagabondCoroutine();
        }

        private void AddVagabond(VagabondMarker vagabondMarker)
        {
            if (!_vagabonds.Contains(vagabondMarker))
            {
                VagabondDeath vagabondDeath = vagabondMarker.GetComponentInParent<VagabondDeath>();
                vagabondDeath.OnDeathHappened += RemoveVagabond;

                _vagabonds.Add(vagabondMarker);
            }
        }

        private void RemoveVagabond(VagabondMarker vagabondMarker)
        {
            if (_vagabonds.Contains(vagabondMarker))
            {
                VagabondDeath vagabondDeath = vagabondMarker.GetComponentInParent<VagabondDeath>();
                vagabondDeath.OnDeathHappened -= RemoveVagabond;

                _vagabonds.Remove(vagabondMarker);
            }
        }

        private bool IsFullCamp() =>
            _requiredVagabonds <= _vagabonds.Count;
    }
}