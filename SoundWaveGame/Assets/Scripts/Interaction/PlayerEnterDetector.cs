using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class PlayerEnterDetector : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onPlayerEnter;

    public UnityEvent OnPlayerEnter => _onPlayerEnter;
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        _onPlayerEnter.Invoke();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;
        _onPlayerEnter.Invoke();
    }

    // For inspector events
    public void KillPlayer()
    {
        Player.Instance.Dead();
    }
    public void WarpPlayerTo(Transform anchor)
    {
        Player.Instance.SetPositionAndRotation(anchor);
    }
}
