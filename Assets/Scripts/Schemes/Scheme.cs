using System.Collections;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace Schemes
{
    public class Scheme : MonoBehaviour
    {
        public BuildingTypeId BuildingTypeId;
        public bool CanCollect { get; private set; }

        private BoxCollider2D _boxCollider2D;
        private bool _isTriggered;

        private void Start() =>
            _boxCollider2D = GetComponent<BoxCollider2D>();

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isTriggered)
                return;

            _isTriggered = true;

            StartCoroutine(Delay());
        }


        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.35f);

            _boxCollider2D.enabled = false;

            CanCollect = true;

            _boxCollider2D.enabled = true;
        }
    }
}