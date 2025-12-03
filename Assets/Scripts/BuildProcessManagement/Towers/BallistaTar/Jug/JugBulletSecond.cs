using System.Collections;
using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class JugBulletSecond : JugBulletBase
    {
        [SerializeField] private GameObject _tarPrefab;
        [SerializeField] private Animator _animator;

        protected override void TriggerEnter()
        {
            transform.SetParent(null);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(transform.position.x, -2.684f, 0);

            //SlowEffectService.CastEffect(transform.position);

            StartCoroutine(DestoyJug());
        }

        private IEnumerator DestoyJug()
        {
            TargetIsActive = false;

            _animator.SetTrigger(Break);
            Instantiate(_tarPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
        }
    }
}