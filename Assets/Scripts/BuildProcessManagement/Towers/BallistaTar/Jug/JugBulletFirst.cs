using System.Collections;
using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class JugBulletFirst : JugBulletBase
    {
        [SerializeField] private GameObject _tarPrefab;
        [SerializeField] private Animator _animator;

        private readonly float _deltaSpeed = 0.1f;

        protected override void Update()
        {
            base.Update();

            Speed += _deltaSpeed;

            _animator.speed += _deltaSpeed;
        }

        protected override void TriggerEnter()
        {
            transform.SetParent(null);
            transform.position = new Vector3(transform.position.x, -2.771f, 0);

            //SlowEffectService.CastEffect(transform.position);

            StartCoroutine(DestroyJug());
        }

        private IEnumerator DestroyJug()
        {
            TargetIsActive = false;

            _animator.SetTrigger(Break);
            Instantiate(_tarPrefab, transform.position, Quaternion.identity, transform);

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
        }
    }
}