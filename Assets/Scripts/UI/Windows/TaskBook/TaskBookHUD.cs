using Bonfire;
using DG.Tweening;
using Infastructure.Services.BuildingRegistry;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Windows.Mainflag
{
    public class TaskBookHUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _marker;
        [SerializeField] private Transform _target;

        private readonly float _swayAngle = 10f;
        private readonly float _swayDuration = 2f;
        private readonly Vector3 _swayAxis = new Vector3(0, 0, 1);

        private IUpgradeMainFlag _upgradeMainFlag;
        private IBuildingRegistryService _buildingRegistryService;

        private Tween _rotateTween;
        private Tween _scaleTween;
        private Sequence _sequence;


        [Inject]
        public void Construct(IUpgradeMainFlag upgradeMainFlag, IBuildingRegistryService buildingRegistryService)
        {
            _buildingRegistryService = buildingRegistryService;
            _upgradeMainFlag = upgradeMainFlag;
        }

        private void Awake()
        {
            _upgradeMainFlag.OnUpgradeFailed += UpgradeMainFlagFailed;
            _upgradeMainFlag.OnUpgradeHappened += UpgradeMainHappened;

            _buildingRegistryService.OnBuildAddHappened += BuildAdd;
        }

        private void OnDestroy()
        {
            _upgradeMainFlag.OnUpgradeFailed -= UpgradeMainFlagFailed;
            _upgradeMainFlag.OnUpgradeHappened -= UpgradeMainHappened;

            _buildingRegistryService.OnBuildAddHappened -= BuildAdd;

            _rotateTween?.Kill();
        }


        private void Start()
        {
            _rotateTween = _target.DORotate(
                    _swayAxis * _swayAngle,
                    _swayDuration / 2
                )
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerEnter(PointerEventData eventData) =>
            _scaleTween = _target.DOScale(1.5f, 0.5f).SetEase(Ease.InOutSine);


        public void OnPointerExit(PointerEventData eventData)
        {
            _scaleTween.Kill();
            _scaleTween = _target.DOScale(1, 0.25f).SetEase(Ease.InOutSine);
        }

        private void UpgradeMainHappened() =>
            _marker.SetActive(false);

        private void BuildAdd()
        {
            if (_upgradeMainFlag.HasUpgradeRightNow())
                _marker.SetActive(true);
        }

        private void UpgradeMainFlagFailed()
        {
            if (_sequence.IsActive())
                return;

            _sequence = DOTween.Sequence()
                .Append(transform.DOScale(2, 0.2f).SetEase(Ease.InOutSine))
                .Append(transform.DOScale(1, 0.2f).SetEase(Ease.InOutSine))
                .SetLoops(3, LoopType.Yoyo);
        }
    }
}