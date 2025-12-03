using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows.Mainflag
{
    public class TaskBookButton : MonoBehaviour
    {
        private IGameUIFactory _gameUIFactory;

        private Button _button;

        [Inject]
        public void Construct(IGameUIFactory gameUIFactory) =>
            _gameUIFactory = gameUIFactory;

        private void Awake() =>
            _button = GetComponent<Button>();

        private void Start() =>
            _button.onClick.AddListener(OpenTaskBook);

        private void OnDestroy() =>
            _button.onClick.RemoveListener(OpenTaskBook);

        private void OpenTaskBook() =>
            _gameUIFactory.CreateTaskBookWindow(WindowId.TaskBookWindow);
    }
}