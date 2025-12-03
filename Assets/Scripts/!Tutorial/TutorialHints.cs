using UnityEngine;

namespace _Tutorial
{
    public class TutorialHints : MonoBehaviour
    {
        [SerializeField] private GameObject _uiRoot;

        public void Show() =>
            _uiRoot.SetActive(true);

        public void Hide() =>
            _uiRoot.SetActive(false);

        public bool HintIsActive() =>
            _uiRoot.activeInHierarchy;
    }
}