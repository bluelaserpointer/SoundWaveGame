using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class Knife : Item
{
    public bool stickToParent;

    [SerializeField]
    AudioClip _hitSE;
    public Rigidbody Rigidbody { get; private set; }
    public Timestamp SpawnedTime  { get; private set; }
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        SpawnedTime = new Timestamp();
        SpawnedTime.Stamp();
        // 应用面板初始值
        if (stickToParent)
        {
            StickTo(transform.parent);
        }
        else
        {
            Unstick();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (stickToParent)
            return;
        float speed = Rigidbody.velocity.magnitude;
        if(speed > 0.1)
        {
            SoundSource.Generate(transform.position, speed, Color.yellow);
            AudioSource.PlayClipAtPoint(_hitSE, transform.position);
            if (collision.collider.TryGetComponent(out KnifeCollideModifier collideModifier))
            {
                collideModifier.onKnifeCollision.Invoke();
                StickTo(collideModifier.transform);
            }
        }
    }
    public void StickTo(Transform target)
    {
        stickToParent = true;
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.isKinematic = true;
        transform.SetParent(target);
    }
    public void Unstick()
    {
        stickToParent = false;
        Rigidbody.isKinematic = false;
        transform.SetParent(null);
    }
    public override void Interact()
    {
        if(SpawnedTime.PassedTime > 3)
            base.Interact(); //preventing pickuping thrown knives too early
    }
}
