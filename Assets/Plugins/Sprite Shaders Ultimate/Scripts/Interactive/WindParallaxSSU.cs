using UnityEngine;

namespace Plugins.Sprite_Shaders_Ultimate.Scripts.Interactive
{
    [AddComponentMenu("Sprite Shaders Ultimate/Wind/Wind Parallax")]
    public class WindParallaxSSU : MonoBehaviour
    {
        float originalXPosition;

        void Awake()
        {
            originalXPosition = transform.position.x;
        }

        void Start()
        {
            GetComponent<Renderer>().material.SetFloat("_WindXPosition", originalXPosition);
        }
    }
}