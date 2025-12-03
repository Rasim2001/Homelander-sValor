using UnityEngine;

namespace BuildProcessManagement.Towers.FearTower.Samovar
{
    public class Samovar : MonoBehaviour, ISamovar
    {
        private static readonly int ShootHash = Animator.StringToHash("Shoot");

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;

        private void Start() =>
            SetCharge();

        public void SetCharge() =>
            _animator.SetBool(ShootHash, false);

        public void PlayShootAnimation() =>
            _animator.SetBool(ShootHash, true);
    }
}