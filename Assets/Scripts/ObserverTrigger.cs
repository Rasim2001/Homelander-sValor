using System;
using UnityEngine;

public class ObserverTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    public Action OnTriggerEnter;
    public Action OnTriggerExit;

    [field: SerializeField] public Collider2D CurrentCollider { get; private set; }

    private bool _isTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
            return;

        if (_isTriggered)
            return;

        _isTriggered = true;
        CurrentCollider = other;
        OnTriggerEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
            return;

        if (!_isTriggered)
            return;

        _isTriggered = false;

        OnTriggerExit?.Invoke();
        CurrentCollider = null;
    }
}