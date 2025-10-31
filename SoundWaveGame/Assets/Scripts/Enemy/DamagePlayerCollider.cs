using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class DamagePlayerCollider : MonoBehaviour
{
    [SerializeField]
    float _damage;
    [SerializeField]
    bool _isOneTime;
    [SerializeField]
    bool _isActive;
    void OnTriggerEnter(Collider other)
    {
        if (!_isActive || !other.CompareTag("Player"))
            return;
        Player.Instance.Damage(_damage);
        if (_isOneTime)
            _isActive = false;
    }
}
