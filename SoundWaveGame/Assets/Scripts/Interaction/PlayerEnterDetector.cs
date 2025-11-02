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
}
