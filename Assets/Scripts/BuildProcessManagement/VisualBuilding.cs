using UnityEngine;

namespace BuildProcessManagement
{
    public class VisualBuilding : MonoBehaviour
    {
        public SpriteRenderer BuildSpriteRender { get; private set; }

        private void Awake() =>
            BuildSpriteRender = GetComponent<SpriteRenderer>();
    }
}