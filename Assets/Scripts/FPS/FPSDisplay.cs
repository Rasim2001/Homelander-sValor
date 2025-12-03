using System;
using TMPro;
using UnityEngine;

namespace FPS
{
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _displayText;

        private int _avgFrameRate;

        private readonly float _timeToCheckDefault = 0.5f;
        private float _timeToCheck;

        private void Start() =>
            _timeToCheck = _timeToCheckDefault;

        public void Update()
        {
            if (_timeToCheck > 0)
                _timeToCheck -= Time.unscaledDeltaTime;
            else
            {
                float current = 0;
                current = (int)(1f / Time.unscaledDeltaTime);
                _avgFrameRate = (int)current;
                _displayText.text = _avgFrameRate + " FPS";

                _timeToCheck = _timeToCheckDefault;
            }
        }
    }
}