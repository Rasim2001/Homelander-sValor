using Infastructure.Services.BuildModeServices;
using Infastructure.Services.Tooltip;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tooltip;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace Tooltip.World
{
    public class TooltipTriggerWorld : MonoBehaviour
    {
        [SerializeField] private TooltipPositionId _tooltipPositionId;
        [SerializeField] private FlagTooltipId FlagTooltipId;

        public bool IsTriggeredWithPlayer;

        private IStaticDataService _staticDataService;
        private ITooltipWorldService _tooltipWorldService;
        private IBuildingModifyService _buildingModifyService;
        private IBuildingModeService _buildingModeService;

        private OrderMarker _orderMarker;

        private Material _unlitMaterial;
        private Material _litMaterial;

        private SpriteRenderer _iconSpriteRenderer;
        private string _tooltipText;

        private bool _isMouseOver;

        [Inject]
        public void Construct(
            IStaticDataService staticDataService,
            ITooltipWorldService tooltipWorldService,
            IBuildingModeService buildingModeService,
            IBuildingModifyService buildingModifyService)
        {
            _buildingModifyService = buildingModifyService;
            _buildingModeService = buildingModeService;
            _staticDataService = staticDataService;
            _tooltipWorldService = tooltipWorldService;
        }


        private void Awake()
        {
            _iconSpriteRenderer = GetComponent<SpriteRenderer>();
            _orderMarker = GetComponentInParent<OrderMarker>();

            _unlitMaterial = new Material(_staticDataService.GetTooltipUnlitMaterial());
            _litMaterial = new Material(_iconSpriteRenderer.material);

            _tooltipText = _staticDataService.ForTooltip(FlagTooltipId);
        }

        private void Start()
        {
            _buildingModifyService.OnActiveChanged += BuildModifyChanged;
            _buildingModeService.OnBuildingStateChanged += BuildStateChanged;
        }

        private void OnDestroy()
        {
            _buildingModifyService.OnActiveChanged -= BuildModifyChanged;
            _buildingModeService.OnBuildingStateChanged -= BuildStateChanged;
        }

        public void MouseEnterCustom()
        {
            if (IsBuildingUIActive() && !_isMouseOver || _orderMarker.IsStarted)
                return;

            _isMouseOver = true;

            ShowIlluminate();
            _tooltipWorldService.Show(_tooltipText, transform.position, _tooltipPositionId, _iconSpriteRenderer.size);
        }

        public void MouseExitCustom()
        {
            _isMouseOver = false;

            HideIlluminate();

            _tooltipWorldService.Hide();
        }

        public void ShowIlluminate()
        {
            _iconSpriteRenderer.color = Color.yellow;
            _iconSpriteRenderer.material = _unlitMaterial;
        }

        public void HideIlluminate()
        {
            if (IsTriggeredWithPlayer)
                return;

            _iconSpriteRenderer.color = Color.white;
            _iconSpriteRenderer.material = _litMaterial;
        }

        private bool IsBuildingUIActive() =>
            _buildingModeService.IsBuildingState || _buildingModifyService.IsActive;

        private void BuildStateChanged()
        {
            if (_buildingModeService.IsBuildingState && _isMouseOver)
                MouseExitCustom();
        }

        private void BuildModifyChanged()
        {
            if (_buildingModifyService.IsActive && _isMouseOver)
                MouseExitCustom();
        }
    }
}