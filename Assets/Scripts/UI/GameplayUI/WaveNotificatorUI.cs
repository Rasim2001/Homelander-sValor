using System.Collections;
using UnityEngine;

namespace UI.GameplayUI
{
    public class WaveNotificatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] _arrows;
        [SerializeField] private float _showTime = 3f;

        private Coroutine[] _arrowCoroutines;

        private void Awake() =>
            _arrowCoroutines = new Coroutine[_arrows.Length];

        public void Notify(int direction)
        {
            int index = direction == -1 ? 0 : 1;

            if (_arrowCoroutines[index] != null)
                StopCoroutine(_arrowCoroutines[index]);

            _arrowCoroutines[index] = StartCoroutine(NotifyArrowRoutine(index));
        }


        private IEnumerator NotifyArrowRoutine(int index)
        {
            GameObject arrow = _arrows[index];

            arrow.SetActive(true);

            yield return new WaitForSeconds(_showTime);

            arrow.SetActive(false);
            _arrowCoroutines[index] = null;
        }
    }
}