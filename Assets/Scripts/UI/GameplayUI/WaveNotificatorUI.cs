using System.Collections;
using UnityEngine;

namespace UI.GameplayUI
{
    public class WaveNotificatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] _arrows;

        private Coroutine _currentCoroutine;

        public void Notify(int direction)
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(StartNotifyWave(direction));
        }

        private IEnumerator StartNotifyWave(int direction)
        {
            _arrows[direction == -1 ? 0 : 1].SetActive(true);

            yield return new WaitForSeconds(3);

            _arrows[direction == -1 ? 0 : 1].SetActive(false);

            _currentCoroutine = null;
        }
    }
}