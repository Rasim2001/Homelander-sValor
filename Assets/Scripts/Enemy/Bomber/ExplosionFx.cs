using System.Collections;
using UnityEngine;

namespace Enemy.Bomber
{
    public class ExplosionFx : MonoBehaviour
    {
        private void Start() =>
            StartCoroutine(DestroyCoroutine());

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(5f);

            Destroy(gameObject);
        }
    }
}