using Cinemachine;
using UnityEngine;

public class CameraZoneHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _nearCamera;
    [SerializeField] private ObserverTrigger _observerTrigger;

    private void Start()
    {
        _observerTrigger.OnTriggerEnter += Enter;
        _observerTrigger.OnTriggerExit += Exit;
    }
    

    private void OnDestroy()
    {
        _observerTrigger.OnTriggerEnter -= Enter;
        _observerTrigger.OnTriggerExit -= Exit;
    }

    private void Enter() =>
        _nearCamera.gameObject.SetActive(false);

    private void Exit() =>
        _nearCamera.gameObject.SetActive(true);
}