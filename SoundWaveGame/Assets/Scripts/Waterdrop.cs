using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Waterdrop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AudioSource waterdropSESource = SoundSource.Generate(transform.position, 5).gameObject.AddComponent<AudioSource>();
        waterdropSESource.clip = Resources.Load<AudioClip>("dropping");
        waterdropSESource.Play();
        Destroy(gameObject);
    }
}
