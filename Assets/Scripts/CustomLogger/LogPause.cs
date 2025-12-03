using UnityEngine;
using UnityEngine.UI;

namespace CustomLogger
{
    public class LogPause : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private bool _isPaused;

        private void Start() =>
            _button.onClick.AddListener(Pause);

        private void OnDestroy() =>
            _button.onClick.RemoveListener(Pause);

        private void Pause()
        {
            _isPaused = !_isPaused;

            Time.timeScale = _isPaused ? 0 : 1;
        }
    }
}