using UnityEngine;

namespace BuildProcessManagement
{
    public class DecorOnBuild : MonoBehaviour
    {
        [SerializeField] private GameObject[] _decorations;

        public void Show()
        {
            foreach (GameObject decor in _decorations)
                decor.SetActive(true);
        }

        public void Hide()
        {
            foreach (GameObject decor in _decorations)
                decor.SetActive(false);
        }
    }
}