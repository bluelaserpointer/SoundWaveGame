using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class vPickupItem : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onPickup = new();
    public GameObject particle;

    [SerializeField]
    AudioSource _audioSource;

    public UnityEvent OnPickup => _onPickup;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)            
            r.enabled = false;
        _onPickup.Invoke();
        if (_audioSource != null)
        {
            _audioSource.Play();
            Destroy(gameObject, _audioSource.clip.length);
        }
        else
        {
            Destroy(gameObject);
        }
        if (particle != null)
        {
            Instantiate(particle, transform.position, transform.rotation);
        }
    }
}