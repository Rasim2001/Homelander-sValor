using Infastructure.Services.ECSInput;
using Infastructure.Services.PauseService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows.Mainflag
{
    public class TaskBookWindow : MonoBehaviour, IEcsWatcherWindow
    {
        [SerializeField] private TaskItem _taskItemPrefab;
        [SerializeField] private AwardItem _awardItemPrefab;
        [SerializeField] private Image _mainFlagImage;

        [SerializeField] private TextMeshProUGUI _mainFlagLevelText;

        [SerializeField] private Transform _tasksContainer;
        [SerializeField] private Transform _awardsContainer;

        private IEcsWatchersService _ecsWatchersService;
        private IPauseService _pauseService;

        [Inject]
        public void Construct(IEcsWatchersService ecsWatchersService, IPauseService pauseService)
        {
            _pauseService = pauseService;
            _ecsWatchersService = ecsWatchersService;
        }

        private void Start() =>
            _pauseService.TurnOn();

        public void CreateTask(Sprite iconSprite, int completedLocalTasks, int allLocalTasks)
        {
            TaskItem taskItem = Instantiate(_taskItemPrefab, _tasksContainer);
            taskItem.SetProgressFill((float)completedLocalTasks / allLocalTasks);
            taskItem.SetAmountText(allLocalTasks);
            taskItem.SetIcon(iconSprite);
        }

        public void CreateAward(Sprite iconSprite, int amount)
        {
            AwardItem awardItem = Instantiate(_awardItemPrefab, _awardsContainer);
            awardItem.SetIcon(iconSprite);
            awardItem.SetAmount(amount);
        }

        public void SetMainFlagLevel(int level) =>
            _mainFlagLevelText.text = level.ToString();

        public void SetMainFlagIcon(Sprite icon) =>
            _mainFlagImage.sprite = icon;

        public void EcsCancel()
        {
            _pauseService.TurnOff();

            Destroy(gameObject);
        }

        private void OnDestroy() =>
            _ecsWatchersService.Release(this);
    }
}