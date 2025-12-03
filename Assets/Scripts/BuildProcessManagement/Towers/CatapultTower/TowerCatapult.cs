using UnityEngine;

namespace BuildProcessManagement.Towers.CatapultTower
{
    public class TowerCatapult : MonoBehaviour
    {
        private void Start()
        {
            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.flipX = transform.position.x > 0;

            IBowl bowl = GetComponentInChildren<IBowl>();
            bowl.SetFlipX(transform.position.x > 0);
        }
    }
}