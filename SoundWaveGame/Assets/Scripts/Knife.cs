using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class Knife : Item
{
    public Rigidbody Rigidbody { get; private set; }
    public Timestamp SpawnedTime  { get; private set; }
    static AudioClip hitSE;
    static AudioClip HitSE => hitSE ?? (hitSE = Resources.Load<AudioClip>("sword_attack2"));
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        SpawnedTime = new Timestamp();
        SpawnedTime.Stamp();
    }
    private void OnCollisionEnter(Collision collision)
    {
        float speed = Rigidbody.velocity.magnitude;
        if(speed > 0.1)
        {
            SoundSource.Generate(transform.position, speed, Color.yellow);
            AudioSource.PlayClipAtPoint(HitSE, transform.position);
        }
    }
    public override void Interact()
    {
        if(SpawnedTime.PassedTime > 3)
            base.Interact(); //preventing pickuping thrown knives too early
    }
}
