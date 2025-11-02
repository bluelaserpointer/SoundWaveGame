using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KnifeThrow : Ability
{
    [SerializeField]
    Transform _throwingAnchor;
    [SerializeField]
    Knife knifePrefab;
    [SerializeField]
    float knifeThrowingForce = 100;
    [SerializeField]
    Cooldown knifeThrowCD = new Cooldown(1);
    [SerializeField]
    AudioClip knifeThrowSE;

    [Header("Debug")]
    [SerializeField]
    int _initialCount;

    int _knifeCount;
    public int KnifeCount
    {
        get => _knifeCount;
        set
        {
            _knifeCount = value;
            PlayerStatsUI.Instance.UpdateKnifeCount(value);
        }
    }

    public override string Name => "探测刀";

    public override bool IsUssable => _knifeCount > 0 && knifeThrowCD.IsReady;

    // Start is called before the first frame update
    void Start()
    {
        KnifeCount = _initialCount;
    }

    // Update is called once per frame
    void Update()
    {
        knifeThrowCD.Charge();
    }
    public void Throw()
    {
        knifeThrowCD.Reset();
        Vector3 throwingPosition = _throwingAnchor.position;
        Vector3 throwingDirection = _throwingAnchor.forward;
        Quaternion throwingRotation = _throwingAnchor.rotation;
        Knife knife = Instantiate(knifePrefab);
        knife.transform.SetPositionAndRotation(throwingPosition + throwingDirection * 0.5f, throwingRotation);
        knife.Rigidbody.AddForce(knifeThrowingForce * throwingDirection);
        AudioSource.PlayClipAtPoint(knifeThrowSE, throwingPosition);
    }

    public override void ActivateAbility()
    {
        --KnifeCount;
        Throw();
    }
}
