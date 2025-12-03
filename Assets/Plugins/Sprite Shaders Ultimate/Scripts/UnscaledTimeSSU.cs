using UnityEngine;

namespace Plugins.Sprite_Shaders_Ultimate.Scripts
{
    [AddComponentMenu("Sprite Shaders Ultimate/Utility/Unscaled Time SSU")]
    public class UnscaledTimeSSU : MonoBehaviour
    {
        public bool dontDestroyOnLoad;

        void Awake()
        {
            if(dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        void Update()
        {
            Shader.SetGlobalFloat("UnscaledTime", Time.unscaledTime);
        }
    }
}
