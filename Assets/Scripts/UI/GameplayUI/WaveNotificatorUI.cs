using System.Collections;
using UnityEngine;

namespace UI.GameplayUI
{
    public class WaveNotificatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] _arrows;

        public void Notify(int direction) =>
            StartCoroutine(StartNotifyWave(direction));

        private IEnumerator StartNotifyWave(int direction)
        {
            _arrows[direction == -1 ? 0 : 1].SetActive(true);

            yield return new WaitForSeconds(3);

            _arrows[direction == -1 ? 0 : 1].SetActive(false);
        }
    }
}