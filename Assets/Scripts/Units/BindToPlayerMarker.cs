using UnityEngine;

namespace Units
{
    public class BindToPlayerMarker : MonoBehaviour
    {
        [SerializeField] private GameObject _bindMarker;
        private bool IsShowed => _bindMarker.activeInHierarchy;

        public void Show()
        {
            if (IsShowed)
                return;

            _bindMarker.SetActive(true);
        }

        public void Hide()
        {
            if (!IsShowed)
                return;

            _bindMarker.SetActive(false);
        }
    }
}