using UnityEngine;

namespace UI.GameplayUI.CristalUI
{
    public interface ICristalMove
    {
        void Update();
        void Update(Vector2 targetPosition);
    }
}