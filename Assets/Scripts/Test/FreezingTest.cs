using UnityEngine;

namespace Test
{
    public class FreezingTest : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _animator.speed = 0;
                Debug.Log("F");
            }


            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("U");
                _animator.speed = 1;
            }
        }
    }
}