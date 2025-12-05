using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class UnknownTutorialState : ITutorialState
    {
        
        
        public void Enter()
        {
            Debug.Log("Unknown Tutorial State");
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}