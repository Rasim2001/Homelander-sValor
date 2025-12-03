using UnityEngine;
using UnityEngine.UI;

namespace Fog
{
    public class FogUI : MonoBehaviour
    {
        [SerializeField] private Image _fogImage;

        public void ChangeIntensity(float value)
        {
            Color color = _fogImage.color;
            color.a = value;

            _fogImage.color = color;
        }

        public float GetAlpha() =>
            _fogImage.color.a;
    }
}