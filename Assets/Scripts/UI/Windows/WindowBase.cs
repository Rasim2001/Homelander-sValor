using UnityEngine;

namespace UI.Windows
{
    public class WindowBase : MonoBehaviour
    {
        private OpenWindowButton[] openWindowButtons;

        private void Awake() =>
            OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() =>
            Cleanup();

        protected virtual void OnAwake() =>
            openWindowButtons = GetComponentsInChildren<OpenWindowButton>();

        protected virtual void Initialize()
        {
        }

        protected virtual void SubscribeUpdates()
        {
            foreach (OpenWindowButton openWindowButton in openWindowButtons)
                openWindowButton.Button.onClick.AddListener(DestroyObject);
        }

        protected virtual void Cleanup()
        {
            foreach (OpenWindowButton openWindowButton in openWindowButtons)
                openWindowButton.Button.onClick.RemoveListener(DestroyObject);
        }

        private void DestroyObject() =>
            Destroy(gameObject);
    }
}